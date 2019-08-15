using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ItemPickUp : Interactable
{
    /*	public enum ItemPickUpType {
		Armor,
		Health,
		Weapon,
		Ammo
	}

	public ItemPickUpType type;*/

    /*	public void Setup (ItemPickUpType itemPickUpType, int amount)
	{
		type = itemPickUpType;
		this.amount = amount;
		switch (itemPickUpType) {
		case ItemPickUpType.Ammo:
			
			break;
		case ItemPickUpType.Armor:
			
			break;
		case ItemPickUpType.Health:
			
			break;
		case ItemPickUpType.Weapon:
			
			break;
		default:
			break;
		}

	}*/

    /*	public void SetupHealth (int healthAmount)
	{
		type = ItemPickUpType.Health;
		amount = healthAmount;
	}

	public void SetupWeapon (GameAssets.WeaponsList weapon)
	{
		type = ItemPickUpType.Weapon;
		amount = (int)weapon;
	}

	public void SetupAmmo (int ammoAmount)
	{
		type = ItemPickUpType.Ammo;
		amount = ammoAmount;
	}*/

    /*	public static ItemPickUp Create (ItemPickUp itemPickUpGO, Vector2 position, Vector2 throwForce)
	{
		GameObject go = Instantiate (itemPickUpGO.gameObject);
		go.transform.position = position;
		return go.GetComponent<ItemPickUp> ();
	}*/

    public int chanceForSpawn = 5;
    bool autoPickUp = false;
    float cantPickUpDelay = 1;

    public virtual void Setup(Vector3 position)
    {
        Init();
        InitRenederers();
        MoveTo(position, 0);
        //AddForce((Vector3.up + Vector3.right) * 150);
    }

    protected override void Start()
    {
        Utility.Invoke(this, cantPickUpDelay, delegate
        {
            cantPickUpDelay = 0;
        });
    }

    public override bool CanInteract(CharacterBase cb)
    {
        if (!CompareEntities(cb.worldPosition))
            return false;

        if (cantPickUpDelay == 0)
        {
            TryPickUp(cb);
            return true;
        }

        return false;
    }

    public override void AwayInteract(CharacterBase cb)
    {
        cb.inventorySystem.CanT_PickUpItem(this);
    }

    public override bool Interact(CharacterBase cb)
    {
        cb.inventorySystem.PickUp();
        return true;
    }

    public override InteractableType GetInteractableType()
    {
        return InteractableType.ItemPickUp;
    }

    public void AddForce(Vector3 vec)
    {
        GetComponent<Rigidbody2D>().AddForce((Vector2)vec);
    }

    public virtual bool PickUp(CharacterBase cb, bool clickedPickUp = false)
    {
        return false;
    }

    public void DestroyItem()
    {
        World.Ins.itemsPickUp.Remove(this);
        Destroy(gameObject);
    }

    public virtual void ShowPopUp(string info = "", Sprite sprite = null)
    {
        GameObject textMesh = Instantiate(GameAssets.Get.pfPopUpInfo.gameObject);

        if (textMesh != null)
        {
            textMesh.gameObject.SetActive(true);
            textMesh.transform.SetParent(null, false);
            TextMeshPro textMeshPro = textMesh.GetComponentInChildren<TextMeshPro>();

            textMeshPro.text = info;

            if (sprite != null)
            {
                textMesh.gameObject.SetActive(true);
                textMesh.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
            }

            textMesh.transform.position = transform.position;
            textMesh.GetComponentInChildren<MeshRenderer>().sortingOrder = 100;

            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(textMesh.transform.DOMove(textMesh.transform.position + Vector3.up * 2, .8f));
            mySequence.Insert(.5f, textMeshPro.DOFade(0, .3f));


            // iTween.MoveTo(textMesh.gameObject, , .8f);
            // iTween.FadeTo(textMesh.gameObject, 0f, .8f);
            Destroy(textMesh.gameObject, 1f);
        }
    }

    void TryPickUp(CharacterBase cb)
    {
        if (cb != null && cb.CanPickUp())
        {
            if (!autoPickUp || !PickUp(cb))
            {
                cb.inventorySystem.CanPickUpItem(this);
            }
            else
            {
                DestroyItem();
            }
        }
    }


}