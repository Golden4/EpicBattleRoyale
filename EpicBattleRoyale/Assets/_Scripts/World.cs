using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Ins;
    public List<ItemPickUp> itemsPickUp = new List<ItemPickUp>();
    public Player player;

    public List<CharacterBase> allCharacters = new List<CharacterBase>();
    public Dictionary<Vector2Int, List<EntityBase>> entities = new Dictionary<Vector2Int, List<EntityBase>>();

    public static event Action<Player> OnPlayerSpawn;
    public static event Action<Enemy> OnEnemySpawn;

    RandomNameGen.RandomName names;

    void Awake()
    {
        Ins = this;
        names = new RandomNameGen.RandomName(new System.Random());
    }

    void Start()
    {
        SpawnCharacterPlayer(Vector2Int.zero, GameAssets.CharacterList.Soldier, new Vector3(0, -3f));

        int spawnedEnemyCount = 0;

        while (spawnedEnemyCount < GameController.CHARACTERS_COUNT_MAX - 1)
        {
            Vector2Int mapCoords = new Vector2Int(UnityEngine.Random.Range(0, MapsController.mapSize), UnityEngine.Random.Range(0, MapsController.mapSize));

            Vector2 pos = MapsController.Ins.GetValidRandomSpawnPoint(mapCoords);

            SpawnCharacterEnemy(mapCoords, GameAssets.CharacterList.Soldier, pos).weaponController.GiveWeapon((GameAssets.WeaponsList)UnityEngine.Random.Range(0, 6));

            spawnedEnemyCount++;
        }

    }

    public Enemy SpawnCharacterEnemy(Vector2Int mapCoords, GameAssets.CharacterList characterName, Vector2 position)
    {
        GameObject character = Instantiate<GameObject>(GameAssets.Get.GetCharacter(characterName).gameObject);
        character.transform.name = names.Generate(RandomNameGen.Sex.Male, 0, true); // "CharacterEnemy" + allCharacters.Count;

        foreach (Component item in character.GetComponents<Component>())
        {
            if (item.GetType() == typeof(Player) || item.GetType() == typeof(Enemy))
            {
                Destroy(item);
            }
        }

        Enemy enemy = character.AddComponent<Enemy>();
        enemy.Setup(position);
        allCharacters.Add(enemy.characterBase);

        enemy.characterBase.characterMapNavigate.ChangeMap(mapCoords);
        enemy.characterBase.MoveToPosition(position, true);

        if (OnEnemySpawn != null)
        {
            OnEnemySpawn(enemy);
        }

        return enemy;
    }

    public Player SpawnCharacterPlayer(Vector2Int mapCoords, GameAssets.CharacterList characterName, Vector2 position)
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

        MapsController.Ins.GoToMap(player.characterBase, mapCoords, false);

        player.characterBase.MoveToPosition(position, true);

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

    public WorldEntity SpawnWorldEntity(GameAssets.WorldEntityList item, Vector3 position)
    {
        GameObject goPf = null;

        goPf = GameAssets.Get.GetWorldEntity(item).gameObject;

        GameObject go = Instantiate(goPf);

        WorldEntity worldEntity = go.GetComponent<WorldEntity>();
        worldEntity.Setup(position);

        return worldEntity;
    }

    public ItemPickUp GetClosestItem(Vector2Int mapCoords, Vector3 position)
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

    public CharacterBase GetClosestCharacter(Vector2Int mapCoords, Vector2 position, CharacterBase cbExclude)
    {
        int closeCharacterIndex = -1;
        float closeDistance = Mathf.Infinity;

        for (int i = 0; i < allCharacters.Count; i++)
        {
            if (cbExclude == allCharacters[i] || allCharacters[i].IsDead() || allCharacters[i].mapCoords != mapCoords)
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

    public void AddEntity(Vector2Int mapCoords, EntityBase entity)
    {
        entities[mapCoords].Add(entity);
    }

    public void RemoveEntity(Vector2Int mapCoords, EntityBase entity)
    {
        entities[mapCoords].Remove(entity);
    }
}