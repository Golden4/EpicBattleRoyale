using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour {
	
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

	public virtual void Setup (Vector3 position)
	{
		transform.position = position;
	}

	public void AddForce (Vector3 vec)
	{
		GetComponent <Rigidbody2D> ().AddForce ((Vector2)vec);
	}

	public virtual bool PickUp (CharacterBase cb, bool clickedPickUp = false)
	{
		return false;
	}

	public void DestroyItem ()
	{
		Destroy (gameObject);
	}

	CharacterBase cb;

	void Update ()
	{
		if (cb != null) {
			if (Input.GetKeyDown (KeyCode.Z)) {
				PickUp (cb, true);
			}
		}
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		cb = col.transform.GetComponent <CharacterBase> ();

		if (cb != null) {
			PickUp (cb);
		}
		Debug.Log ("OnTriggerEnter2D " + col.name);
	}

	void OnTriggerExit2D (Collider2D col)
	{
		cb = col.transform.GetComponent <CharacterBase> ();

		if (cb != null) {
			cb = null;
		}

		Debug.Log ("OnTriggerExit2D " + col.name);
	}

}
