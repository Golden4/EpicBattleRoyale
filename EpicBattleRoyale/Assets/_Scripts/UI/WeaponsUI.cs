using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponsUI : MonoBehaviour {
	
	//public WeaponSlotUI weaponSlotBtnPrefab;

	public WeaponSlotUI[] weaponSlots;
	public int curActiveSlot;

	WeaponController weaponController;

	public void Setup (WeaponController wc)
	{
		this.weaponController = wc;
		weaponController.OnWeaponSwitch += WeaponController_OnWeaponSwitch;
	}

	void WeaponController_OnWeaponSwitch (object sender, System.EventArgs e)
	{
		for (int i = 0; i < weaponController.WEAPON_COUNT; i++) {
			Weapon weapon = (Weapon)weaponController.GetWeaponsInInventory () [i];
			if (weapon == null) {
				weaponSlots [i].Setup (false);
				continue;
			}
			int index = i;

			if (weapon.GetType () == typeof(AutomaticWeapon)) {
				AutomaticWeapon automaticWeapon = (AutomaticWeapon)weapon;
				weaponSlots [i].Setup (
					true,
					i == weaponController.GetCurrentActiveWeaponIndex (),
					delegate {
						weaponController.SwitchWeapon (index, true);
					},
					automaticWeapon.GetWeaponSprite (),
					automaticWeapon.bulletSystem
				);
			}

			if (weapon.GetType () == typeof(MeleeWeapon)) {
				MeleeWeapon meleeWeapon = (MeleeWeapon)weapon;
				weaponSlots [i].Setup (
					true,
					i == weaponController.GetCurrentActiveWeaponIndex (),
					delegate {
						weaponController.SwitchWeapon (index, true);
					},
					meleeWeapon.GetWeaponSprite ()
				);

			}
		}
	}

}
