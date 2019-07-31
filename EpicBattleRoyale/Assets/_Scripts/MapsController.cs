using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapsController : MonoBehaviour {

	public static MapsController Ins;

	public static event Action<MapInfo, Direction> OnChangingMap;

	public MapInfo[] mapInfo = new MapInfo[16];

	public Dictionary<Vector2Int, MapInfo> mapInfoDic = new Dictionary<Vector2Int, MapInfo> ();

	Vector2Int curMapCoords = Vector2Int.zero;

	public GameObject mapPrefab;

	public GameObject curMapGO;

	void Awake ()
	{
		Ins = this;
	}

	void Start ()
	{
		for (int i = 0; i < mapInfo.Length; i++) {
			mapInfoDic [mapInfo [i].coord] = mapInfo [i];
		}

		ChangeMap (GetMapInfo (Vector2Int.zero), Direction.None);
	}

	void ChangeMap (MapInfo map, Direction direction)
	{
		if (curMapGO != null)
			Destroy (curMapGO);
		
		curMapCoords = map.coord;
		curMapGO = Instantiate (mapPrefab);
		curMapGO.gameObject.SetActive (true);
		curMapGO.transform.name = "Map " + map.coord.ToString ();
		
		MapNavigate[] mapsNav = new MapNavigate[] {
			curMapGO.transform.Find ("MapToLeft").GetComponent <MapNavigate> (),
			curMapGO.transform.Find ("MapToRight").GetComponent <MapNavigate> (),	
			curMapGO.transform.Find ("MapToCenter").GetComponent <MapNavigate> ()
		};

		for (int i = 0; i < mapsNav.Length; i++) {
			if (map.roads.Length > i) {
				mapsNav [i].gameObject.SetActive (true);
				mapsNav [i].direction = map.roads [i];
			} else
				mapsNav [i].gameObject.SetActive (false);
		}

		if (map.houseCount > 0)
			curMapGO.transform.Find ("Home").gameObject.SetActive (true);
		else
			curMapGO.transform.Find ("Home").gameObject.SetActive (false);
		
		if (OnChangingMap != null) {
			OnChangingMap (map, direction);
		}

		Debug.Log (map.coord.ToString ());
	}

	public void GoToMap (Direction direction)
	{
		MapInfo map = GetMapInfo (curMapCoords + directions [(int)direction]);
		ChangeMap (map, direction);
	}

	public MapInfo GetMapInfo (int x, int y)
	{
		MapInfo mapInf;
		mapInfoDic.TryGetValue (new Vector2Int (x, y), out mapInf);
		return mapInf;
	}

	public MapInfo GetMapInfo (Vector2Int xy)
	{
		return GetMapInfo (xy.x, xy.y);
		/*MapInfo mapInf;
		mapInfoDic.TryGetValue (xy, out mapInf);
		return mapInf;*/
	}

	#if UNITY_EDITOR
	void OnValidate ()
	{
		for (int i = 0; i < mapInfo.Length; i++) {
			mapInfo [i].coord = new Vector2Int (i % 4, i / 4);
		}
	}
	#endif

	[System.Serializable]
	public class MapInfo {
		public Vector2Int coord;
		public Direction[] roads;
		public int houseCount;

	}

	Vector2Int[] directions = new Vector2Int[] {
		Vector2Int.down,
		Vector2Int.up,
		Vector2Int.left,
		Vector2Int.right,
		Vector2Int.zero
	};

}

public enum Direction {
	Top,
	Bottom,
	Left,
	Right,
	None
}
