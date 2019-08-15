using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapsController : MonoBehaviour
{
    public static MapsController Ins;
    public List<HouseData> pfHouses;
    public List<MapData> pfMaps;

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

    public int mapSize = 4;

    public MapInfo[] mapInfo = new MapInfo[16];

    public MapInfo[,] maps;

    Vector2Int curMapCoords = Vector2Int.zero;
    public int curHouseIndex;

    public Dictionary<Vector2Int, GameObject> curMaps = new Dictionary<Vector2Int, GameObject>();

    public List<Tuple<Vector2Int, int, GameObject>> curInnerHouses = new List<Tuple<Vector2Int, int, GameObject>>();

    public event Action<MapInfo, Direction> OnChangingMap;
    public event Action OnGenerateMap;

    void Awake()
    {
        Ins = this;
    }

    void Start()
    {
        GenerateMap();

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                SpawnMap(maps[i, j]);
            }

        }

        ChangeMap(GetMapInfo(Vector2Int.zero), Direction.None);
    }

    void GenerateMap()
    {
        MapsGenerator.GenerateMaps(mapSize, ref maps);

        if (OnGenerateMap != null)
            OnGenerateMap();
    }

    void SpawnMap(MapInfo map)
    {
        GameObject mapGo = Instantiate(GetMapData(map.mapType).pfMap.gameObject);

        mapGo.gameObject.SetActive(true);
        mapGo.transform.name = "Map " + map.coord.ToString();
        curMaps[map.coord] = mapGo;

        MapNavigate[] mapsNav = new MapNavigate[] {
            mapGo.transform.Find ("Left").GetComponent <MapNavigate> (),
            mapGo.transform.Find ("Center").GetComponent <MapNavigate> (),
            mapGo.transform.Find ("Right").GetComponent <MapNavigate> ()
        };

        Vector3 centerPos = mapsNav[1].transform.position;
        centerPos.x = map.centerRoadPositionX;
        mapsNav[1].transform.position = centerPos;



        if (map.centerRoad != Direction.None)
        {
            mapsNav[0].gameObject.SetActive(true);
            mapsNav[0].direction = map.roads[0];
            mapsNav[2].gameObject.SetActive(true);
            mapsNav[2].direction = map.roads[1];
            mapsNav[1].gameObject.SetActive(true);
            mapsNav[1].direction = map.centerRoad;
        }
        else
        {
            mapsNav[0].gameObject.SetActive(true);
            mapsNav[0].direction = map.roads[0];
            mapsNav[2].gameObject.SetActive(true);
            mapsNav[2].direction = map.roads[1];
            mapsNav[1].gameObject.SetActive(false);
        }

        // for (int i = 0; i < mapsNav.Length; i++)
        // {
        //     if (map.roads.Count > i)
        //     {
        //         mapsNav[i].gameObject.SetActive(true);
        //         mapsNav[i].direction = map.roads[i];
        //     }
        //     else
        //         mapsNav[i].gameObject.SetActive(false);
        // }

        if (map.houses.Count > 0)
            for (int i = 0; i < map.houses.Count; i++)
            {
                SpawnOuterHouse(map, map.houses[i].houseType, i, mapGo.transform);
            }

        List<Vector3> itemsSpawnPoints = new List<Vector3>();

        for (int i = (int)GetMapData(GetCurrentMapInfo().mapType).worldEndPoints.x + 10; i < (int)GetMapData(GetCurrentMapInfo().mapType).worldEndPoints.y - 10; i = i + 3)
        {
            itemsSpawnPoints.Add(new Vector3(i, -4));
        }

        SpawnItems(itemsSpawnPoints, mapGo.transform);
    }

    void SpawnOuterHouse(MapInfo map, HouseType houseType, int houseIndex, Transform parent)
    {
        GameObject outerHouse = Instantiate(GetHouseData(houseType).pfOuterHouse.gameObject);
        outerHouse.transform.SetParent(parent, false);
        HouseDoor outsideHouse = outerHouse.AddComponent<HouseDoor>();
        outsideHouse.MoveTo(map.houses[houseIndex].GetHouseSpawnPosition(map));
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
        innerHouse.transform.name = "Inner House" + houseIndex + ": " + map.houses[houseIndex].houseType + " | Map " + map.coord.ToString();

        SpawnItems(itemsSpawnPoints, innerHouse.transform);

        curInnerHouses.Add(new Tuple<Vector2Int, int, GameObject>(map.coord, houseIndex, innerHouse));
    }

    void SpawnItems(List<Vector3> itemsSpawnPoints, Transform parent)
    {
        //check spawn point
        for (int i = 0; i < itemsSpawnPoints.Count; i++)
        {
            if (UnityEngine.Random.Range(0, 1) == 0)
            {
                int randNum = UnityEngine.Random.Range(0, 101);

                //spawn weapon with ammo
                if (randNum < 30)
                {
                    WeaponItemPickUp wa = World.Ins.SpawnItemPickUpWeapon(GameAssets.WeaponsList.Fists, itemsSpawnPoints[i], true);
                    wa.transform.SetParent(parent, false);
                    AutomaticWeapon aw = GameAssets.Get.GetWeapon(wa.weaponName) as AutomaticWeapon;

                    if (aw != null)
                    {
                        int itemPickUpAmmoAmount = UnityEngine.Random.Range(1, 5);
                        for (int k = 0; k < itemPickUpAmmoAmount; k++)
                        {
                            AmmoItemPickUp ammo = World.Ins.SpawnItemPickUpAmmo(aw.bulletSystem.ammoType, itemsSpawnPoints[i] + (Vector3)UnityEngine.Random.insideUnitCircle * .5f);
                            ammo.transform.SetParent(parent, false);
                        }

                    }
                }
                else
                //random Armor spawn
                if (randNum - 30 < 20)
                {
                    ArmorItemPickUp newItem = World.Ins.SpawnItemPickUpArmor(GameAssets.PickUpItemsData.ArmorList.Big, itemsSpawnPoints[i], true);
                    newItem.transform.SetParent(parent, false);
                }
                else
                //random Health spawn
                if (randNum - 50 < 20)
                {
                    HealthItemPickUp newItem = World.Ins.SpawnItemPickUpHealth(GameAssets.PickUpItemsData.HealthList.Big, itemsSpawnPoints[i], true);
                    newItem.transform.SetParent(parent, false);
                }
                else
                //random Ammo spawn
                if (randNum - 70 < 20)
                {
                    AmmoItemPickUp newItem = World.Ins.SpawnItemPickUpAmmo(GameAssets.PickUpItemsData.AmmoList.AutomaticWeapon, itemsSpawnPoints[i], true);
                    newItem.transform.SetParent(parent, false);
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
        UpdateAllWorldEndPoints();

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
        return maps[x, y];
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

    Vector2 worldUpDownEndPoints;
    Vector2 worldEndPoints;

    public Vector2 GetCurrentWorldEndPoints()
    {
        if (worldEndPoints == default)
            UpdateAllWorldEndPoints();

        return worldEndPoints;
    }

    public Vector2 GetCurrentWorldUpDownEndPoints()
    {
        if (worldUpDownEndPoints == default)
            UpdateAllWorldEndPoints();
        return worldUpDownEndPoints;
    }

    public float GetCurrentVerticalCenterPoint()
    {
        return GetCurrentWorldUpDownEndPoints().x + (GetCurrentWorldUpDownEndPoints().y - GetCurrentWorldUpDownEndPoints().x) / 2f;
    }

    void UpdateAllWorldEndPoints()
    {
        if (mapState == State.House)
        {
            worldEndPoints = GetHouseData(GetCurrentMapInfo().houses[curHouseIndex].houseType).worldEndPoints;
            worldUpDownEndPoints = GetHouseData(GetCurrentMapInfo().houses[curHouseIndex].houseType).worldUpDownEndPoints;
        }
        else
        {
            worldEndPoints = GetMapData(GetCurrentMapInfo().mapType).worldEndPoints;
            worldUpDownEndPoints = GetMapData(GetCurrentMapInfo().mapType).worldUpDownEndPoints;
        }

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
        UpdateAllWorldEndPoints();

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

    public int GetSpawnDirection(MapsController.MapInfo mapInfo, Direction dir)
    {
        //0 left 1 right 2 center
        int spawnDirection = -1;

        Direction[] dir1 = new Direction[] {
            Direction.Bottom, Direction.Left, Direction.Right, Direction.Top
        };

        Direction[] dir2 = new Direction[] {
            Direction.Top, Direction.Right, Direction.Left, Direction.Bottom
        };

        for (int i = 0; i < dir1.Length; i++)
        {
            if (dir == dir1[i])
            {
                for (int j = 0; j < mapInfo.roads.Count; j++)
                {
                    if (mapInfo.roads[j] == dir2[i])
                    {
                        spawnDirection = j;
                        break;
                    }
                }

                if (mapInfo.centerRoad == dir2[i])
                {
                    spawnDirection = 2;
                    break;
                }
            }
        }

        return spawnDirection;
    }

    public enum HouseType
    {
        House1,
        House2,
        House3,
        House4,
        House5,
    }

    public enum MapType
    {
        Normal
    }

    [System.Serializable]
    public class HouseData
    {
        public HouseType houseType;
        public GameObject pfOuterHouse;
        public GameObject pfInnerHouse;
        public Vector2 worldEndPoints;
        public Vector2 worldUpDownEndPoints;
    }

    [System.Serializable]
    public class MapData
    {
        public MapType mapType;
        public GameObject pfMap;
        public Vector2 worldEndPoints;
        public Vector2 worldUpDownEndPoints;
    }

    public HouseData GetHouseData(HouseType type)
    {
        return pfHouses.Find(x =>
        {
            return x.houseType == type;
        });
    }

    public MapData GetMapData(MapType type)
    {
        return pfMaps.Find(x =>
        {
            return x.mapType == type;
        });
    }


    // #if UNITY_EDITOR
    //     void OnValidate()
    //     {
    //         for (int i = 0; i < mapInfo.Length; i++)
    //         {
    //             mapInfo[i].coord = new Vector2Int(i % 4, i / 4);
    //         }
    //     }
    // #endif

    [System.Serializable]
    public class MapInfo
    {
        public MapType mapType;
        public bool isFirstEntry;
        public Vector2Int coord;
        public List<Direction> roads = new List<Direction>();
        public Direction centerRoad = Direction.None;
        public float centerRoadPositionX;
        public List<HouseInfo> houses = new List<HouseInfo>();

        public void ValidateRoads()
        {
            if (roads.Contains(Direction.Bottom) && roads.Contains(Direction.Top))
            {
                if (roads.Contains(Direction.Left))
                    centerRoad = Direction.Left;
                if (roads.Contains(Direction.Right))
                    centerRoad = Direction.Right;
            }

            if (roads.Contains(Direction.Left) && roads.Contains(Direction.Right))
            {
                if (roads.Contains(Direction.Bottom))
                    centerRoad = Direction.Bottom;
                if (roads.Contains(Direction.Top))
                    centerRoad = Direction.Top;
            }


            if (centerRoad != Direction.None)
                roads.Remove(centerRoad);
            SortRoads();
        }

        public void SortRoads()
        {
            List<Direction> dir = new List<Direction>() { Direction.Top, Direction.Left, Direction.Bottom, Direction.Right };
            //List<Direction> dir = new List<Direction>() { Direction.Top, Direction.Bottom, Direction.Right, Direction.Left };
            roads.Sort((x, y) =>
            {
                return (dir.IndexOf(x)).CompareTo(dir.IndexOf(y));
            });
        }

        public int GetRoadsCount()
        {
            return roads.Count;
        }
    }

    [System.Serializable]
    public class HouseInfo
    {
        public float houseSpawnPositionsX;
        public HouseType houseType;
        BoxCollider2D doorCollider;

        public HouseInfo(float houseSpawnPositionsX, HouseType houseType)
        {
            this.houseSpawnPositionsX = houseSpawnPositionsX;
            this.houseType = houseType;
        }

        public Vector3 GetHouseSpawnPosition(MapInfo mapsInfo)
        {
            Vector3 pos = Vector3.zero;
            pos.x = houseSpawnPositionsX;
            pos.y = MapsController.Ins.GetMapData(mapsInfo.mapType).worldUpDownEndPoints.y;
            return pos;
        }

        public Vector3 GetDoorPosition(MapInfo mapsInfo)
        {
            if (doorCollider == null)
                doorCollider = MapsController.Ins.GetHouseData(houseType).pfOuterHouse.GetComponent<BoxCollider2D>();

            Vector3 pos = Vector3.zero;
            pos.x = houseSpawnPositionsX + doorCollider.offset.x;
            pos.y = MapsController.Ins.GetMapData(mapsInfo.mapType).worldUpDownEndPoints.y;
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
