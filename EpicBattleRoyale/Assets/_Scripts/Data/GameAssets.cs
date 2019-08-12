using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class GameAssets : ScriptableObject
{
    #region Singleton
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
    #endregion

    #region WeaponsData
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
    #endregion

    #region CharactersData
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
    #endregion

    public PickUpItemsData pickUpItems;

    [System.Serializable]
    public class PickUpItemsData
    {
        public List<WeaponItemPickUp> weaponsPickUpItems;
        public List<ArmorItemPickUp> armorPickUpItems;
        public List<HealthItemPickUp> healthPickUpItems;
        public List<AmmoItemPickUp> ammoPickUpItems;

        public enum ArmorList
        {
            Small,
            Big
        }

        public enum HealthList
        {
            Small,
            Big
        }

        public enum AmmoList
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

        public ItemPickUp GetPickUpItem(ArmorList item)
        {
            return armorPickUpItems.Find(x =>
            {
                return x.armorPickUpType == item;
            });
        }

        public ItemPickUp GetPickUpItem(HealthList item)
        {
            return healthPickUpItems.Find(x =>
            {
                return x.healthPickUpType == item;
            });
        }

        public ItemPickUp GetPickUpItem(AmmoList item)
        {
            return ammoPickUpItems.Find(x =>
            {
                return x.ammoType == item;
            });
        }

        public ItemPickUp GetRandomPickUpItemWeapon()
        {
            List<ItemPickUp> pickUpItems = new List<ItemPickUp>(weaponsPickUpItems.ToArray());
            return weaponsPickUpItems[GetRandomIndex(pickUpItems)];
        }

        public ItemPickUp GetRandomPickUpItemHealth()
        {
            List<ItemPickUp> pickUpItems = new List<ItemPickUp>(healthPickUpItems.ToArray());
            return weaponsPickUpItems[GetRandomIndex(pickUpItems)];
        }

        public ItemPickUp GetRandomPickUpItemArmor()
        {
            List<ItemPickUp> pickUpItems = new List<ItemPickUp>(armorPickUpItems.ToArray());
            return weaponsPickUpItems[GetRandomIndex(pickUpItems)];
        }

        int GetRandomIndex(List<ItemPickUp> pickUpItems)
        {
            int RandomNum = Random.Range(0, GetSummChance(pickUpItems));

            int i = 0;
            int sum = 0;

            while (sum <= RandomNum)
            {
                sum += pickUpItems[i].chanceForSpawn;
                i++;
            }

            return i - 1;
        }

        int GetSummChance(List<ItemPickUp> pickUpItems)
        {
            int summ = 0;

            for (int i = 0; i < pickUpItems.Count; i++)
            {
                summ += pickUpItems[i].chanceForSpawn;
            }

            return summ;
        }
    }

    public BulletHandler pfBullet;
    public Shell pfShell;
    public ParticleSystem pfMuzzleFlash;
    public TMPro.TextMeshPro pfPopUpDamage;

    /*	void OnValidate ()
    {
        for (int i = 0; i < weapons.Length; i++) {
            weapons [i].weaponName = (WeaponsList)i;
        }
    }*/

}
