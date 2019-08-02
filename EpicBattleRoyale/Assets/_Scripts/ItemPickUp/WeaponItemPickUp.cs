using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItemPickUp : ItemPickUp
{
    public GameAssets.WeaponsList weaponName;
    //public BulletSystem bulletInfo;

    public override bool PickUp(CharacterBase cb, bool clickedPickUp = false)
    {
        WeaponController wc = cb.GetComponent<WeaponController>();
        if (wc != null)
        {
            Weapon weapon = wc.FindWeaponInInventory(weaponName);
            if (weapon == null)
            {
                if (wc.InventoryFull(GameAssets.Get.GetWeapon(weaponName).slotType))
                {
                    if (clickedPickUp)
                    {
                        wc.GiveWeapon(weaponName);
                        ShowPopUp(weaponName.ToString());
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    wc.GiveWeapon(weaponName);
                    return true;
                }
            }
            else
            {
                if (weapon.WeaponIs(typeof(AutomaticWeapon)))
                {
                    AutomaticWeapon automaticWeapon = (AutomaticWeapon)weapon;
                    /*if (bulletInfo != null)
						automaticWeapon.bulletSystem = bulletInfo;
					else*/
                    automaticWeapon.bulletSystem.GiveBulletsStock(5);
                    ShowPopUp("+" + 5 + "bullets");
                    /*Debug.Log (bulletInfo.ToString ());*/
                    return true;
                }
            }
        }
        return false;
    }

    /*	public void AddBulletInfo (BulletSystem bulletInfo)
	{
		this.bulletInfo = new BulletSystem (bulletInfo);
	}*/
}
