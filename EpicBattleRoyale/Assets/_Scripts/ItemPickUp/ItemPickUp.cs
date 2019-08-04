using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
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
    public static event EventHandler OnPickUp;
    bool autoPickUp = false;
    float cantPickUpDelay = 1;

    public virtual void Setup(Vector3 position)
    {
        transform.position = position;
        //AddForce((Vector3.up + Vector3.right) * 150);

        Utility.Invoke(this, cantPickUpDelay, delegate
        {
            cantPickUpDelay = 0;
            TryPickUp();
        });
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
        ScreenUI.Ins.mobileInputsUI.HidePickUpBtn();
        World.Ins.itemsPickUp.Remove(this);
        cb.OnCharacterPickUp(this);
        Destroy(gameObject);
    }

    public virtual void ShowPopUp(string info = "", Sprite sprite = null)
    {
        GameObject textMesh = Instantiate(Resources.Load<GameObject>("Prefabs/PopUpInfo"));

        if (textMesh != null)
        {
            textMesh.gameObject.SetActive(true);
            textMesh.transform.SetParent(null, false);
            textMesh.GetComponentInChildren<TextMesh>().text = info;

            if (sprite != null)
            {
                textMesh.gameObject.SetActive(true);
                textMesh.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
            }

            textMesh.transform.position = transform.position;
            textMesh.GetComponentInChildren<MeshRenderer>().sortingOrder = 100;
            iTween.MoveTo(textMesh.gameObject, textMesh.transform.position + Vector3.up * 2, .8f);
            iTween.FadeTo(textMesh.gameObject, 0f, .8f);
            Destroy(textMesh.gameObject, 1f);
        }
    }

    CharacterBase cb;

    void OnTriggerEnter2D(Collider2D col)
    {
        cb = col.transform.GetComponent<CharacterBase>();

        if (cantPickUpDelay == 0)
            TryPickUp();

    }

    void TryPickUp()
    {
        if (cb != null && cb.CanPickUp())
        {
            if (!autoPickUp || !PickUp(cb))
            {
                if (cb == World.Ins.player.characterBase)
                {
                    ScreenUI.Ins.mobileInputsUI.ShowPickUpBtn(() =>
                    {
                        Debug.Log("ShowPickUpBtn");
                        if (cb.CanPickUp() && PickUp(cb, true))
                        {
                            DestroyItem();
                        }
                    });
                }
            }
            else
            {
                DestroyItem();
            }
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (cb == null)
            cb = col.transform.GetComponent<CharacterBase>();
    }

    void OnTriggerExit2D(Collider2D col)
    {
        cb = col.transform.GetComponent<CharacterBase>();

        if (cb != null)
        {
            ScreenUI.Ins.mobileInputsUI.HidePickUpBtn();
            cb = null;
        }
    }

}