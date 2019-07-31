using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

	public static World Ins;
	public List<ItemPickUp> itemsPickUp = new List<ItemPickUp> ();
	public Player player;

	void Awake ()
	{
		Ins = this;
	}

	void Start ()
	{
		SpawnItemPickUpWeapon (GameAssets.WeaponsList.MP5, new Vector3 (-5, -2));
		SpawnItemPickUpWeapon (GameAssets.WeaponsList.Beretta, new Vector3 (5, -2));
		SpawnItemPickUpWeapon (GameAssets.WeaponsList.SniperRiffle, new Vector3 (3, -2));
		SpawnItemPickUpHealth (GameAssets.PickUpItemsData.HealthPickUpList.Big, new Vector3 (-3, -2));
		//SpawnItemPickUp (ItemPickUp.ItemPickUpType.Weapon, );
	}

	public void SpawnCharacter ()
	{
		
	}

	public Player GetPlayer ()
	{
		return player;
	}

	public ArmorItemPickUp SpawnItemPickUpArmor (GameAssets.PickUpItemsData.ArmorPickUpList item, Vector3 position)
	{
		GameObject go = Instantiate (GameAssets.Get.pickUpItems.GetPickUpItem (item).gameObject);
		ArmorItemPickUp armorItemPickUp = go.GetComponent<ArmorItemPickUp> ();
		armorItemPickUp.Setup (position);

		return armorItemPickUp;
	}

	public HealthItemPickUp SpawnItemPickUpHealth (GameAssets.PickUpItemsData.HealthPickUpList item, Vector3 position)
	{
		GameObject go = Instantiate (GameAssets.Get.pickUpItems.GetPickUpItem (item).gameObject);
		HealthItemPickUp healthItemPickUp = go.GetComponent<HealthItemPickUp> ();
		healthItemPickUp.Setup (position);
		return healthItemPickUp;
	}

	public WeaponItemPickUp SpawnItemPickUpWeapon (GameAssets.WeaponsList item, Vector3 position)
	{
		GameObject go = Instantiate (GameAssets.Get.pickUpItems.GetPickUpItem (item).gameObject);
		WeaponItemPickUp weaponItemPickUp = go.GetComponent<WeaponItemPickUp> ();
		weaponItemPickUp.Setup (position);
		return weaponItemPickUp;
	}

	public AmmoItemPickUp SpawnItemPickUpAmmo (GameAssets.PickUpItemsData.AmmoPickUpList item, Vector3 position)
	{
		GameObject go = Instantiate (GameAssets.Get.pickUpItems.GetPickUpItem (item).gameObject);
		AmmoItemPickUp ammoItemPickUp = go.GetComponent<AmmoItemPickUp> ();
		ammoItemPickUp.Setup (position);
		return ammoItemPickUp;
	}

}
