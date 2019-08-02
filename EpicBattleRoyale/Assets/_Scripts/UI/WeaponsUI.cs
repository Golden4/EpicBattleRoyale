using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponsUI : MonoBehaviour
{

    //public WeaponSlotUI weaponSlotBtnPrefab;

    public WeaponSlotUI[] weaponSlots;
    public int curActiveSlot;
    public Image selectedImage;
    WeaponController weaponController;

    public void Setup(WeaponController wc)
    {
        this.weaponController = wc;
        WeaponController_OnWeaponSwitch(null, EventArgs
        .Empty);
        weaponController.OnWeaponSwitch += WeaponController_OnWeaponSwitch;
    }

    void WeaponController_OnWeaponSwitch(object sender, System.EventArgs e)
    {
        for (int i = 0; i < weaponController.WEAPON_COUNT; i++)
        {
            Weapon weapon = (Weapon)weaponController.GetWeaponsInInventory()[i];

            if (weapon == null)
            {
                Color color = weaponSlots[i].GetComponent<Image>().color;
                color.a = .2f;
                weaponSlots[i].GetComponent<Image>().color = color;

                weaponSlots[i].Setup(false);
                continue;
            }

            if (i == weaponController.GetCurrentActiveWeaponIndex())
            {
                Color color = weaponSlots[i].GetComponent<Image>().color;
                color.a = .7f;
                weaponSlots[i].GetComponent<Image>().color = color;

                selectedImage.transform.SetParent(weaponSlots[i].transform, false);
                selectedImage.rectTransform.anchoredPosition = Vector3.zero;
            }
            else
            {
                Color color = weaponSlots[i].GetComponent<Image>().color;
                color.a = .6f;
                weaponSlots[i].GetComponent<Image>().color = color;
            }

            int index = i;

            if (weapon.WeaponIs(typeof(AutomaticWeapon)))
            {
                AutomaticWeapon automaticWeapon = (AutomaticWeapon)weapon;
                weaponSlots[i].Setup(
                    true,
                    i == weaponController.GetCurrentActiveWeaponIndex(),
                    delegate
                    {
                        weaponController.SwitchWeapon(index, true);
                    },
                    automaticWeapon.GetWeaponSprite(),
                    automaticWeapon.bulletSystem
                );
            }

            if (weapon.GetType() == typeof(MeleeWeapon))
            {
                MeleeWeapon meleeWeapon = (MeleeWeapon)weapon;
                weaponSlots[i].Setup(
                    true,
                    i == weaponController.GetCurrentActiveWeaponIndex(),
                    delegate
                    {
                        weaponController.SwitchWeapon(index, true);
                    },
                    meleeWeapon.GetWeaponSprite()
                );
            }

        }
    }

}
