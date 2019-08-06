using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapsController : MonoBehaviour
{
    public static MapsController Ins;
    public GameObject pfMap;
    public List<HouseData> pfHouses;

    public Vector3[] characterSpawnPoints = new Vector3[3] {
        (Vector3.right * -36 + Vector3.up * -3),
        (Vector3.right * 36 + Vector3.up * -3),
        (Vector3.zero + Vector3.up * -3)
    };

    public MapInfo[] mapInfo = new MapInfo[16];

    public Dictionary<Vector2Int, MapInfo> mapInfoDic = new Dictionary<Vector2Int, MapInfo>();

    Vector2Int curMapCoords = Vector2Int.zero;

    public Dictionary<Vector2Int, GameObject> curMaps = new Dictionary<Vector2Int, GameObject>();

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

        MapNavigate[] mapsNav = new MapNavigate[] {
            mapGo.transform.Find ("Left").GetComponent <MapNavigate> (),
            mapGo.transform.Find ("Right").GetComponent <MapNavigate> (),
            mapGo.transform.Find ("Center").GetComponent <MapNavigate> ()
        };

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
                GameObject house = Instantiate(GetHouseData(map.houses[i].houseType).pfOuterHouse.gameObject);
                Vector3 pos = house.transform.position;
                pos.x = map.houses[i].houseSpawnPositionsX;
                pos.y = -5.25f;
                house.transform.position = pos;
                house.transform.SetParent(mapGo.transform, false);
            }

        curMaps[map.coord] = mapGo;
    }

    void ChangeMap(MapInfo map, Direction direction)
    {
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


        if (OnChangingMap != null)
        {
            OnChangingMap(map, direction);
        }

        Debug.Log(map.coord.ToString());
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
    }

    [System.Serializable]
    public class HouseInfo
    {
        public float houseSpawnPositionsX;
        public HouseType houseType;
    }

    // [System.Serializable]
    // public class House
    // {
    //     


    // }

    Vector2Int[] directions = new Vector2Int[] {
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
