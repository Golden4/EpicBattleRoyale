﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class World : MonoBehaviour
{
    public static World Ins;
    public List<ItemPickUp> itemsPickUp = new List<ItemPickUp>();
    public Player player;
    public List<CharacterBase> allCharacters = new List<CharacterBase>();
    public Dictionary<Vector2Int, EntityBase> entities = new Dictionary<Vector2Int, EntityBase>();
    GameObject itemsHolder;
    public static event Action<Player> OnPlayerSpawn;
    public static event Action<Enemy> OnEnemySpawn;

    void Awake()
    {
        Ins = this;
    }

    void Start()
    {
        if (itemsHolder == null)
            itemsHolder = new GameObject("ItemsHolder");

        // SpawnItemPickUpWeapon(GameAssets.WeaponsList.MP5, new Vector3(-5, -3f));
        // SpawnItemPickUpWeapon(GameAssets.WeaponsList.Beretta, new Vector3(5, -3f));
        // SpawnItemPickUpWeapon(GameAssets.WeaponsList.ShotGun, new Vector3(3, -3f));
        // SpawnItemPickUpWeapon(GameAssets.WeaponsList.ShotGun, new Vector3(6, -3f));
        // SpawnItemPickUpWeapon(GameAssets.WeaponsList.SniperRiffle, new Vector3(-6, -3f));
        // SpawnItemPickUpHealth(GameAssets.PickUpItemsData.HealthPickUpList.Big, new Vector3(-3, -3f));
        // SpawnItemPickUpWeapon(GameAssets.WeaponsList.AK12, new Vector3(-12, -3f));
        // SpawnItemPickUpArmor(GameAssets.PickUpItemsData.ArmorPickUpList.Big, new Vector3(8, -3f));
        SpawnCharacterPlayer(GameAssets.CharacterList.Soldier, new Vector3(0, -3f));
        SpawnCharacterEnemy(GameAssets.CharacterList.Soldier, new Vector3(-10, -5f));

        // for (int i = -2; i <= 2; i++)
        // {
        //     if (i != 0)
        //         SpawnCharacterEnemy(GameAssets.CharacterList.Soldier, new Vector3(i * 5, -3));
        // }

        // SpawnItemPickUpAmmo(GameAssets.PickUpItemsData.AmmoPickUpList.AutomaticWeapon, new Vector3(1 * 3, -3f));
        // SpawnItemPickUpAmmo(GameAssets.PickUpItemsData.AmmoPickUpList.PistolWeapon, new Vector3(2 * 3, -3f));
        // SpawnItemPickUpAmmo(GameAssets.PickUpItemsData.AmmoPickUpList.ShotGunWeapon, new Vector3(3 * 3, -3f));
        // SpawnItemPickUpAmmo(GameAssets.PickUpItemsData.AmmoPickUpList.SniperWeapon, new Vector3(4 * 3, -3f));


        // enemy.weaponController.GiveWeapon(GameAssets.WeaponsList.SniperRiffle);
        //   enemy.weaponController.GiveWeapon(GameAssets.WeaponsList.MP5);
        //SpawnItemPickUp (ItemPickUp.ItemPickUpType.Weapon, );
    }

    public Enemy SpawnCharacterEnemy(GameAssets.CharacterList characterName, Vector3 position)
    {
        GameObject character = Instantiate<GameObject>(GameAssets.Get.GetCharacter(characterName).gameObject);
        character.transform.name = "CharacterEnemy" + allCharacters.Count;

        foreach (Component item in character.GetComponents<Component>())
        {
            if (item.GetType() == typeof(Player))
            {
                Destroy(item);
            }
        }

        Enemy enemy = character.AddComponent<Enemy>();
        enemy.Setup(position);
        allCharacters.Add(enemy.characterBase);

        if (OnEnemySpawn != null)
        {
            OnEnemySpawn(enemy);
        }

        return enemy;
    }

    public Player SpawnCharacterPlayer(GameAssets.CharacterList characterName, Vector3 position)
    {
        GameObject character = Instantiate<GameObject>(GameAssets.Get.GetCharacter(characterName).gameObject);
        character.transform.name = "CharacterPlayer";
        foreach (Component item in character.GetComponents<Component>())
        {
            if (item.GetType() == typeof(Player) || item.GetType() == typeof(Enemy))
            {
                Destroy(item);
            }
        }

        Player player = character.AddComponent<Player>();
        player.Setup(position);
        this.player = player;
        allCharacters.Add(player.characterBase);

        if (OnPlayerSpawn != null)
        {
            OnPlayerSpawn(player);
        }
        return player;
    }

    public Player GetPlayer()
    {
        return player;
    }

    public ArmorItemPickUp SpawnItemPickUpArmor(GameAssets.PickUpItemsData.ArmorList item, Vector3 position, bool random = false)
    {
        GameObject goPf = null;
        if (!random)
            goPf = GameAssets.Get.pickUpItems.GetPickUpItem(item).gameObject;
        else
            goPf = GameAssets.Get.pickUpItems.GetRandomPickUpItemArmor().gameObject;


        GameObject go = Instantiate(goPf);
        ArmorItemPickUp armorItemPickUp = go.GetComponent<ArmorItemPickUp>();
        armorItemPickUp.Setup(position);
        itemsPickUp.Add(armorItemPickUp);

        return armorItemPickUp;
    }

    public HealthItemPickUp SpawnItemPickUpHealth(GameAssets.PickUpItemsData.HealthList item, Vector3 position, bool random = false)
    {
        GameObject goPf = null;

        if (!random)
            goPf = GameAssets.Get.pickUpItems.GetPickUpItem(item).gameObject;
        else
            goPf = GameAssets.Get.pickUpItems.GetRandomPickUpItemHealth().gameObject;


        GameObject go = Instantiate(goPf);

        HealthItemPickUp healthItemPickUp = go.GetComponent<HealthItemPickUp>();
        healthItemPickUp.Setup(position);
        itemsPickUp.Add(healthItemPickUp);

        return healthItemPickUp;

    }

    public WeaponItemPickUp SpawnItemPickUpWeapon(GameAssets.WeaponsList item, Vector3 position, bool random = false)
    {

        GameObject goPf = null;

        if (!random)
            goPf = GameAssets.Get.pickUpItems.GetPickUpItem(item).gameObject;
        else
            goPf = GameAssets.Get.pickUpItems.GetRandomPickUpItemWeapon().gameObject;


        GameObject go = Instantiate(goPf);

        WeaponItemPickUp weaponItemPickUp = go.GetComponent<WeaponItemPickUp>();
        weaponItemPickUp.Setup(position);
        itemsPickUp.Add(weaponItemPickUp);
        return weaponItemPickUp;
    }

    public AmmoItemPickUp SpawnItemPickUpAmmo(GameAssets.PickUpItemsData.AmmoList item, Vector3 position, bool random = false)
    {
        GameObject goPf = null;

        if (!random)
            goPf = GameAssets.Get.pickUpItems.GetPickUpItem(item).gameObject;
        else
            goPf = GameAssets.Get.pickUpItems.GetRandomPickUpItemAmmo().gameObject;

        GameObject go = Instantiate(goPf);

        AmmoItemPickUp ammoItemPickUp = go.GetComponent<AmmoItemPickUp>();
        ammoItemPickUp.Setup(position);
        itemsPickUp.Add(ammoItemPickUp);

        return ammoItemPickUp;
    }

    public ItemPickUp GetClosestItem(Vector3 position)
    {
        int closeItemIndex = -1;
        float closeDistance = Mathf.Infinity;
        for (int i = 0; i < itemsPickUp.Count; i++)
        {
            float distance = Vector3.Distance(itemsPickUp[i].transform.position, position);
            if (distance < closeDistance)
            {
                closeDistance = distance;
                closeItemIndex = i;
            }
        }

        if (closeItemIndex > -1)
            return itemsPickUp[closeItemIndex];
        else
        {
            return null;
        }
    }

    public CharacterBase GetClosestCharacter(Vector2 position, CharacterBase cbExclude)
    {
        int closeCharacterIndex = -1;
        float closeDistance = Mathf.Infinity;

        for (int i = 0; i < allCharacters.Count; i++)
        {
            if (cbExclude == allCharacters[i] || allCharacters[i].IsDead())
                continue;

            float distance = Vector2.Distance((Vector2)allCharacters[i].worldPosition, position);

            if (distance < closeDistance)
            {
                closeDistance = distance;
                closeCharacterIndex = i;
            }
        }

        if (closeCharacterIndex > -1)
            return allCharacters[closeCharacterIndex];
        else
        {
            return null;
        }
    }

}