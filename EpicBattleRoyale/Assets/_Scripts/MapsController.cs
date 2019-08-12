﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapsController : MonoBehaviour
{
    public static MapsController Ins;
    public GameObject pfMap;
    public List<HouseData> pfHouses;

    public enum State
    {
        Map,
        House,
    }

    public State mapState;

    public Vector3[] characterSpawnPoints = new Vector3[3] {
        (Vector3.right * -36 + Vector3.up * -3),
        (Vector3.right * 36 + Vector3.up * -3),
        (Vector3.zero + Vector3.up * -3)
    };

    public MapInfo[] mapInfo = new MapInfo[16];

    public Dictionary<Vector2Int, MapInfo> mapInfoDic = new Dictionary<Vector2Int, MapInfo>();

    Vector2Int curMapCoords = Vector2Int.zero;
    public int curHouseIndex;

    public Dictionary<Vector2Int, GameObject> curMaps = new Dictionary<Vector2Int, GameObject>();

    public List<Tuple<Vector2Int, int, GameObject>> curInnerHouses = new List<Tuple<Vector2Int, int, GameObject>>();

    public event Action<MapInfo, Direction> OnChangingMap;

    void Awake()
    {
        Ins = this;
    }

    void Start()
    {
        for (int i = 0; i < mapInfo.Length; i++)
        {
            mapInfoDic[mapInfo[i].coord] = mapInfo[i];
        }

        for (int i = 0; i < mapInfo.Length; i++)
        {
            SpawnMap(mapInfo[i]);
        }

        ChangeMap(GetMapInfo(Vector2Int.zero), Direction.None);
    }

    void SpawnMap(MapInfo map)
    {
        GameObject mapGo = Instantiate(pfMap);

        mapGo.gameObject.SetActive(true);
        mapGo.transform.name = "Map " + map.coord.ToString();
        curMaps[map.coord] = mapGo;

        MapNavigate[] mapsNav = new MapNavigate[] {
            mapGo.transform.Find ("Left").GetComponent <MapNavigate> (),
            mapGo.transform.Find ("Right").GetComponent <MapNavigate> (),
            mapGo.transform.Find ("Center").GetComponent <MapNavigate> ()
        };

        List<Vector3> itemsSpawnPoints = new List<Vector3>();

        for (int i = (int)GetCurrentWorldEndPoints().x + 10; i < (int)GetCurrentWorldEndPoints().y - 10; i = i + 3)
        {
            itemsSpawnPoints.Add(new Vector3(i, -4));
        }

        SpawnItems(itemsSpawnPoints, mapGo.transform);

        Vector3 centerPos = mapsNav[2].transform.position;
        centerPos.x = map.centerRoadPositionX;
        mapsNav[2].transform.position = centerPos;

        for (int i = 0; i < mapsNav.Length; i++)
        {
            if (map.roads.Length > i)
            {
                mapsNav[i].gameObject.SetActive(true);
                mapsNav[i].direction = map.roads[i];
            }
            else
                mapsNav[i].gameObject.SetActive(false);
        }

        if (map.houses.Length > 0)
            for (int i = 0; i < map.houses.Length; i++)
            {
                SpawnOuterHouse(map, map.houses[i].houseType, i, mapGo.transform);
            }
    }

    void SpawnOuterHouse(MapInfo map, HouseType houseType, int houseIndex, Transform parent)
    {
        GameObject outerHouse = Instantiate(GetHouseData(houseType).pfOuterHouse.gameObject);
        outerHouse.transform.position = map.houses[houseIndex].GetHouseSpawnPosition();
        outerHouse.transform.SetParent(parent, false);
        HouseDoor outsideHouse = outerHouse.AddComponent<HouseDoor>();
        outsideHouse.Setup(map.coord, houseType, houseIndex);
    }

    void SpawnInnerHouse(MapInfo map, int houseIndex)
    {
        List<Vector3> itemsSpawnPoints = new List<Vector3>();

        for (int i = (int)GetCurrentWorldEndPoints().x + 10; i < (int)GetCurrentWorldEndPoints().y - 10; i = i + 3)
        {
            itemsSpawnPoints.Add(new Vector3(i, -3));
        }

        GameObject innerHouse = Instantiate(GetHouseData(map.houses[houseIndex].houseType).pfInnerHouse.gameObject);
        innerHouse.gameObject.SetActive(true);
        innerHouse.transform.name = "Inner House: " + map.houses[houseIndex].houseType + " | Map " + map.coord.ToString();

        SpawnItems(itemsSpawnPoints, innerHouse.transform);

        curInnerHouses.Add(new Tuple<Vector2Int, int, GameObject>(map.coord, houseIndex, innerHouse));
    }

    void SpawnItems(List<Vector3> itemsSpawnPoints, Transform parent)
    {
        for (int i = 0; i < itemsSpawnPoints.Count; i++)
        {
            if (UnityEngine.Random.Range(0, 1) == 0)
            {
                int randNum = UnityEngine.Random.Range(0, 1);

                //spawn weapon with ammo

                if (randNum == 0)
                {
                    WeaponItemPickUp wa = World.Ins.SpawnItemPickUpWeapon(GameAssets.WeaponsList.Fists, itemsSpawnPoints[i], true);
                    wa.transform.SetParent(parent, false);
                    AutomaticWeapon aw = GameAssets.Get.GetWeapon(wa.weaponName) as AutomaticWeapon;

                    if (aw != null)
                    {
                        int itemPickUpAmmoAmount = UnityEngine.Random.Range(1, 5);
                        for (int k = 0; k < itemPickUpAmmoAmount; k++)
                        {
                            AmmoItemPickUp ammo = World.Ins.SpawnItemPickUpAmmo(aw.bulletSystem.ammoType, itemsSpawnPoints[i] + Vector3.right * UnityEngine.Random.Range(-.5f, .5f));
                            ammo.transform.SetParent(parent, false);
                        }

                    }


                }
            }
        }
    }

    public void ChangeMap(MapInfo map, Direction direction)
    {

        for (int i = 0; i < curInnerHouses.Count; i++)
        {
            curInnerHouses[i].Item3.SetActive(false);
        }

        foreach (var item in curMaps)
        {
            if (map.coord == item.Key)
            {
                item.Value.SetActive(true);
            }
            else
            {
                item.Value.SetActive(false);
            }

        }

        curMapCoords = map.coord;
        mapState = State.Map;
        UpdateWorldEndPoints();

        if (OnChangingMap != null)
        {
            OnChangingMap(map, direction);
        }
    }

    public void GoToMap(Direction direction)
    {
        MapInfo map = GetMapInfo(curMapCoords + directions[(int)direction]);
        ChangeMap(map, direction);
    }

    public MapInfo GetMapInfo(int x, int y)
    {
        MapInfo mapInf;
        mapInfoDic.TryGetValue(new Vector2Int(x, y), out mapInf);
        return mapInf;
    }

    public MapInfo GetMapInfo(Vector2Int xy)
    {
        return GetMapInfo(xy.x, xy.y);
        /*MapInfo mapInf;
		mapInfoDic.TryGetValue (xy, out mapInf);
		return mapInf;*/
    }

    public MapInfo GetCurrentMapInfo()
    {
        return GetMapInfo(curMapCoords);
    }

    public Vector2 worldEndPoints;

    public Vector2 GetCurrentWorldEndPoints()
    {
        return worldEndPoints;
    }

    void UpdateWorldEndPoints()
    {
        if (mapState == State.House)
            worldEndPoints = GetHouseData(GetMapInfo(curMapCoords).houses[curHouseIndex].houseType).worldEndPoints;
        else
            worldEndPoints = GetMapInfo(curMapCoords).worldEndPoints;
    }

    public event Action<HouseDoor> OnEnterHouseEvent;

    public void EnterHouse(HouseDoor house)
    {
        foreach (var item in curMaps)
        {
            item.Value.SetActive(false);
        }

        mapState = State.House;
        curHouseIndex = house.houseIndex;
        UpdateWorldEndPoints();

        for (int i = 0; i < curInnerHouses.Count; i++)
        {
            curInnerHouses[i].Item3.SetActive(false);
        }

        Tuple<Vector2Int, int, GameObject> go = curInnerHouses.Find(x => { return ((x.Item1 == GetMapInfo(curMapCoords).coord) && (x.Item2 == house.houseIndex)); });

        if (go != null)
        {
            go.Item3.SetActive(true);
        }
        else
        {
            SpawnInnerHouse(GetMapInfo(curMapCoords), house.houseIndex);
        }

        if (OnEnterHouseEvent != null)
            OnEnterHouseEvent(house);
    }

    public void ExitHouse()
    {
        ChangeMap(GetCurrentMapInfo(), Direction.None);
    }

    public enum HouseType
    {
        House1,
        House2,
        House3,
        House4,
        House5,
    }

    [System.Serializable]
    public class HouseData
    {
        public HouseType houseType;
        public GameObject pfOuterHouse;
        public GameObject pfInnerHouse;
        public Vector2 worldEndPoints = new Vector2(-37, 37);
    }

    public HouseData GetHouseData(HouseType type)
    {
        return pfHouses.Find(x =>
        {
            return x.houseType == type;
        });
    }


#if UNITY_EDITOR
    void OnValidate()
    {
        for (int i = 0; i < mapInfo.Length; i++)
        {
            mapInfo[i].coord = new Vector2Int(i % 4, i / 4);
        }
    }
#endif

    [System.Serializable]
    public class MapInfo
    {
        public bool isFirstEntry;
        public Vector2Int coord;
        public Direction[] roads;
        public float centerRoadPositionX;
        public HouseInfo[] houses;
        public Vector2 worldEndPoints = new Vector2(-37, 37);
    }

    [System.Serializable]
    public class HouseInfo
    {
        public float houseSpawnPositionsX;
        public HouseType houseType;

        public Vector3 GetHouseSpawnPosition()
        {
            Vector3 pos = Vector3.zero;
            pos.x = houseSpawnPositionsX;
            pos.y = -5.25f;
            return pos;
        }
    }

    // [System.Serializable]
    // public class House
    // {
    //     


    // }

    public static Vector2Int[] directions = new Vector2Int[] {
        Vector2Int.down,
        Vector2Int.up,
        Vector2Int.left,
        Vector2Int.right,
        Vector2Int.zero
    };
}

public enum Direction
{
    Top,
    Bottom,
    Left,
    Right,
    None
}
