﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class GameAssets : ScriptableObject
{

    static GameAssets data;

    static bool Loaded;

    public static GameAssets Get
    {
        get
        {
            if (data == null && !Loaded)
            {
                Loaded = true;
                GameAssets pi = Resources.Load<GameAssets>("Data/Data");

                data = pi;
            }

            return data;
        }
    }

    public List<Weapon> weapons;

    public enum WeaponsList
    {
        Fists,
        MP5,
        Beretta,
        SniperRiffle,
        ShotGun,
        AK12
    }

    public Weapon GetWeapon(WeaponsList weapon)
    {
        return weapons.Find(x =>
        {
            return x.weaponName == weapon;
        });
    }

    public List<CharacterBase> characters;

    public enum CharacterList
    {
        Swat,
        Soldier
    }

    public CharacterBase GetCharacter(CharacterList character)
    {
        return characters.Find(x =>
         {
             return x.characterName == character;
         });
    }

    public PickUpItemsData pickUpItems;

    [System.Serializable]
    public class PickUpItemsData
    {
        public List<WeaponItemPickUp> weaponsPickUpItems;
        public List<ArmorItemPickUp> armorPickUpItems;
        public List<HealthItemPickUp> healthPickUpItems;
        public List<AmmoItemPickUp> ammoPickUpItems;

        public enum ArmorPickUpList
        {
            Small,
            Big
        }

        public enum HealthPickUpList
        {
            Small,
            Big
        }

        public enum AmmoPickUpList
        {
            AutomaticWeapon,
            SniperWeapon,
            PistolWeapon,
            ShotGunWeapon,
        }

        public ItemPickUp GetPickUpItem(WeaponsList item)
        {
            return weaponsPickUpItems.Find(x =>
            {
                return x.weaponName == item;
            });
        }

        public ItemPickUp GetPickUpItem(ArmorPickUpList item)
        {
            return armorPickUpItems.Find(x =>
            {
                return x.armorPickUpType == item;
            });
        }

        public ItemPickUp GetPickUpItem(HealthPickUpList item)
        {
            return healthPickUpItems.Find(x =>
            {
                return x.healthPickUpType == item;
            });
        }

        public ItemPickUp GetPickUpItem(AmmoPickUpList item)
        {
            return ammoPickUpItems.Find(x =>
            {
                return x.ammoPickUpType == item;
            });
        }
    }

    public BulletHandler pfBullet;
    public Shell pfShell;
    public ParticleSystem pfMuzzleFlash;
    public TMPro.TextMeshPro pfPopUpDamage;

    public GameObject pfMap;

    public enum HouseType
    {
        highRiseBuilding,
    }

    [System.Serializable]
    public class HouseData
    {
        public HouseType houseType;
        public GameObject pfOuterHouse;
        public GameObject pfInnerHouse;
    }

    public List<HouseData> houses;

    public HouseData GetHouseData(HouseType type)
    {
        return houses.Find(x =>
        {
            return x.houseType == type;
        });
    }

    /*	void OnValidate ()
	{
		for (int i = 0; i < weapons.Length; i++) {
			weapons [i].weaponName = (WeaponsList)i;
		}
	}*/

}
