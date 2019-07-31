using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoItemPickUp : ItemPickUp {
	public GameAssets.PickUpItemsData.AmmoPickUpList ammoPickUpType;
	public Weapon.WeaponType bulletType;
	public int ammoAmount;

	public override bool PickUp (CharacterBase cb, bool clickedPickUp = false)
	{
		WeaponController wc = cb.GetComponent <WeaponController> ();
		if (wc != null) {
			for (int i = 0; i < wc.WEAPON_COUNT; i++) {
				Weapon weapon = wc.GetWeapon (i);

				if (weapon == null) {
					
				} else {
					if (weapon.GetType () == typeof(AutomaticWeapon)) {
						AutomaticWeapon automaticWeapon = (AutomaticWeapon)weapon;
						if (automaticWeapon.weaponType == bulletType) {
							automaticWeapon.bulletSystem.GiveBulletsStock (20);
							DestroyItem ();
							return true;
						}
					}
				} 
			}
		}
		return false;
	}

}
