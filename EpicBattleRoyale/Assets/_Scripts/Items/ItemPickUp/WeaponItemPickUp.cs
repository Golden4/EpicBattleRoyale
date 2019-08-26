using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItemPickUp : ItemPickUp
{
    public GameAssets.WeaponsList weaponName;

    WeaponItemPickUpData data = null;
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
                        SetPickUpData(wc.GiveWeapon(weaponName)); ;
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
                    SetPickUpData(wc.GiveWeapon(weaponName));
                    return true;
                }
            }
            else
            {
                // if (weapon.WeaponIs(typeof(AutomaticWeapon)))
                // {
                //     AutomaticWeapon automaticWeapon = (AutomaticWeapon)weapon;
                //     /*if (bulletInfo != null)
                // 		automaticWeapon.bulletSystem = bulletInfo;
                // 	else*/
                //     // automaticWeapon.bulletSystem.GiveBulletsStock(5);
                //     ShowPopUp("+" + 5 + "bullets");
                //     /*Debug.Log (bulletInfo.ToString ());*/
                //     return true;
                // }
            }
        }
        return false;
    }

    public void AddWeaponData(AutomaticWeapon weapon)
    {
        data = new WeaponItemPickUpData(weapon.bulletSystem);
    }

    public void SetPickUpData(Weapon weapon)
    {
        if (data != null)
        {
            if (weapon.WeaponIs(typeof(AutomaticWeapon)))
            {
                AutomaticWeapon automaticWeapon = (AutomaticWeapon)weapon;
                automaticWeapon.bulletSystem.SetBullets(data.bulletSystem.curBullets);
                // automaticWeapon.bulletSystem.SetBulletsStock(data.bulletSystem.curBulletsStock);
            }
        }
    }

    [System.Serializable]
    public class WeaponItemPickUpData
    {
        public BulletSystem bulletSystem;
        public WeaponItemPickUpData(BulletSystem bulletSystem)
        {
            this.bulletSystem = bulletSystem;
        }
    }

    /*	public void AddBulletInfo (BulletSystem bulletInfo)
	{
		this.bulletInfo = new BulletSystem (bulletInfo);
	}*/
}
