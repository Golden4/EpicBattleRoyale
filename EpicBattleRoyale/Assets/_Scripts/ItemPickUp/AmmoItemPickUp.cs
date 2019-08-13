using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoItemPickUp : ItemPickUp
{
    public GameAssets.PickUpItemsData.AmmoList ammoType;
    public int ammoAmount;

    public override bool PickUp(CharacterBase cb, bool clickedPickUp = false)
    {
        WeaponController wc = cb.GetComponent<WeaponController>();
        if (wc != null)
        {
            for (int i = 0; i < wc.WEAPON_COUNT; i++)
            {
                Weapon weapon = wc.GetWeapon(i);

                if (weapon == null)
                {

                }
                else
                {
                    if (weapon.WeaponIs(typeof(AutomaticWeapon)))
                    {
                        AutomaticWeapon automaticWeapon = (AutomaticWeapon)weapon;
                        if (automaticWeapon.bulletSystem.ammoType == ammoType)
                        {
                            automaticWeapon.bulletSystem.GiveBulletsStock(ammoAmount);
                            ShowPopUp("+" + ammoAmount);
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

}
