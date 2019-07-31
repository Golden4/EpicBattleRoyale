using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponController : MonoBehaviour {
	
	public readonly int WEAPON_COUNT = 2;
	int currentWeaponInHandIndex = 0;

	Weapon[] weaponsInInventory = new Weapon[2];

	public enum State {
		Normal,
		Switching,
	}

	State curState;
	public Transform weaponHandHolder;
	public int orderInLayerHandWeapon;
	public Transform weaponBackHolder;
	public int orderInLayerBackWeapon;
	public Transform pistolBackHolder;
	public int orderInLayerPistolWeapon;
	public float switchingWeaponTime = .5f;
	float switchingWeapon;

	public event EventHandler OnWeaponSwitch;
	public event EventHandler OnGiveWeapon;

	public CharacterBase cb;

	void Start ()
	{
		cb = GetComponent <CharacterBase> ();
		GiveWeapon (GameAssets.WeaponsList.Fists);
		//GiveWeapon (GameAssets.WeaponsList.MP5, 1);
	}

	public Weapon GiveWeapon (GameAssets.WeaponsList weapon)
	{
		Weapon newWeapon;
		int emptySlot = GetEmptySlot ();
		Debug.Log ("emptySlot " + emptySlot);
		if (emptySlot >= 0) {
			newWeapon = GiveWeapon (weapon, emptySlot);
		} else {
			DropWeaponFromInventory (currentWeaponInHandIndex);
			newWeapon = GiveWeapon (weapon, currentWeaponInHandIndex);
		}
		return newWeapon;
	}

	public Weapon FindWeaponInInventory (GameAssets.WeaponsList weapon)
	{
		for (int i = 0; i < WEAPON_COUNT; i++) {
			if (weaponsInInventory [i] != null && weaponsInInventory [i].weaponName == weapon)
				return weaponsInInventory [i];
		}
		return null;
	}

	public Weapon GiveWeapon (GameAssets.WeaponsList weapon, int indexInInventory)
	{
		GameObject go = Instantiate (GameAssets.Get.GetWeapon (weapon).gameObject);
		weaponsInInventory [indexInInventory] = go.GetComponent<Weapon> ();
		weaponsInInventory [indexInInventory].Setup (this);

		if (OnGiveWeapon != null)
			OnGiveWeapon (weapon, EventArgs.Empty);
		
		SwitchWeapon (indexInInventory);
		return weaponsInInventory [indexInInventory];
	}


	public void DropWeaponFromInventory (int index)
	{
		if (weaponsInInventory [index] != null) {

			if (weaponsInInventory [index].weaponName != GameAssets.WeaponsList.Fists) {
				WeaponItemPickUp weaponItemPickUp = World.Ins.SpawnItemPickUpWeapon (weaponsInInventory [index].weaponName,
					                                    transform.position + ((cb.isFacingRight) ? 2 : -2) * Vector3.right);
				//weaponItemPickUp.AddForce (new Vector3 (((cb.isFacingRight) ? 50 : -50), -50));


				/*if (weaponsInInventory [index].GetType () == typeof(AutomaticWeapon)) {
					AutomaticWeapon automaticWeapon = (AutomaticWeapon)weaponsInInventory [index];

					weaponItemPickUp.AddBulletInfo (automaticWeapon.bulletSystem);
				}*/

			}
			Destroy (weaponsInInventory [index].gameObject);
		}
	}

	int GetEmptySlot ()
	{
		
		for (int i = 0; i < WEAPON_COUNT; i++) {
			if (weaponsInInventory [i] == null) {
				return i;
			}
		}

		for (int i = 0; i < WEAPON_COUNT; i++) {
			if (weaponsInInventory [i].weaponName == GameAssets.WeaponsList.Fists) {
				return i;
			}
		}

		return -1;
	}

	void Update ()
	{
		switch (curState) {
		case State.Switching:
			switchingWeapon -= Time.deltaTime;
			if (switchingWeapon < 0) {
				curState = State.Normal;
			}
			break;
		case State.Normal:
			if (Input.GetKeyDown (KeyCode.E))
				if (currentWeaponInHandIndex != 0 && weaponsInInventory [0] != null)
					SwitchWeapon (0);
			if (Input.GetKeyDown (KeyCode.Q))
				if (currentWeaponInHandIndex != 1 && weaponsInInventory [1] != null)
					SwitchWeapon (1);
			break;
		default:
			break;
		}

		if (GetCurrentWeapon () != null)
			cb.isShooting = GetCurrentWeapon ().isShooting ();
	}

	public void SwitchWeapon (int weaponIndex)
	{
		currentWeaponInHandIndex = weaponIndex;

		SetWeaponLocations (weaponIndex);

		curState = State.Switching;

		switchingWeapon = switchingWeaponTime;

		cb.SetWeaponAnimationType (weaponsInInventory [weaponIndex].weaponType);

		/*if (GetCurrentWeapon () == null)
			cb.isWeapon = false;
		else
			cb.isWeapon = GetCurrentWeapon ().GetWeaponType () != Weapon.WeaponType.Melee;*/

		if (OnWeaponSwitch != null)
			OnWeaponSwitch (weaponsInInventory [weaponIndex], EventArgs.Empty);
	}

	void SetWeaponLocations (int activeIndex)
	{
		for (int i = 0; i < WEAPON_COUNT; i++) {

			if (weaponsInInventory [i] == null)
				continue;

			SetWeaponLocation (weaponsInInventory [i], activeIndex == i, weaponsInInventory [i].weaponType);
		}
	}

	void SetWeaponLocation (Weapon weapon, bool isActive, Weapon.WeaponType type)
	{
		if (isActive) {
			weapon.isActive = true;
			weapon.transform.SetParent (weaponHandHolder, false);
			if (weapon.GetComponentInChildren <SpriteRenderer> () != null)
				weapon.GetComponentInChildren <SpriteRenderer> ().sortingOrder = orderInLayerHandWeapon;
		} else if (type == Weapon.WeaponType.Pistol) {
				weapon.isActive = false;
				weapon.transform.SetParent (pistolBackHolder, false);
				if (weapon.GetComponentInChildren <SpriteRenderer> () != null)
					weapon.GetComponentInChildren <SpriteRenderer> ().sortingOrder = orderInLayerPistolWeapon;
			} else {
				weapon.isActive = false;
				weapon.transform.SetParent (weaponBackHolder, false);
				if (weapon.GetComponentInChildren <SpriteRenderer> () != null)
					weapon.GetComponentInChildren <SpriteRenderer> ().sortingOrder = orderInLayerBackWeapon;
			}

		weapon.transform.localPosition = Vector3.zero;
		
	}

	/*	void ShowWeaponGraphics (int index)
	{
		for (int i = 0; i < weaponsInInventory.Count; i++) {
	
			if (weaponsInInventory [i].gameObject.activeSelf && i != index) {
				weaponsInInventory [i].gameObject.SetActive (false);
			} else if (i == index) {
					weaponsInInventory [i].gameObject.SetActive (true);
				}
		}
	}*/


	public Weapon GetCurrentWeapon ()
	{
		return weaponsInInventory [currentWeaponInHandIndex];
	}

	public Weapon GetWeapon (int index)
	{
		return weaponsInInventory [index];
	}

	public Weapon[] GetWeaponsInInventory ()
	{
		return weaponsInInventory;
	}

	public int GetCurrentActiveWeaponIndex ()
	{
		return currentWeaponInHandIndex;
	}

	public bool InventoryFull ()
	{

		for (int i = 0; i < WEAPON_COUNT; i++) {
			if (weaponsInInventory [i] == null || weaponsInInventory [i].weaponName == GameAssets.WeaponsList.Fists) {
				return false;
			}
		}

		return true;
	}

	/*	void OnEnable ()
	{
		EventManager.OnClickSelectWeaponEvent += (int arg1, GameObject arg2) => switchWeapon (arg1);
	}

	void OnDisable ()
	{
		EventManager.OnClickSelectWeaponEvent -= (int arg1, GameObject arg2) => switchWeapon (arg1);
	}*/
}
