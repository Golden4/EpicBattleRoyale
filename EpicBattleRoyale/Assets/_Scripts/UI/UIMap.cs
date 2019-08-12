using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UIMap : MonoBehaviour
{
    [SerializeField] UILineRenderer pfLineRenderer;
    [SerializeField] Transform lineRendererHolder;

    const int mapSize = 4;
    MapData[,] maps = new MapData[mapSize, mapSize];

    Dictionary<Vector2Int, UILineRenderer> lineRenderers = new Dictionary<Vector2Int, UILineRenderer>();

    public RawImage playerPointImage;

    void Start()
    {
        MapsController.Ins.OnChangingMap += OnChangingMap;
        GenerateMap();
        CreateLineRenderers();
    }

    void OnChangingMap(MapsController.MapInfo arg1, Direction arg2)
    {
        playerPointImage.rectTransform.anchoredPosition = new Vector3(12 + arg1.coord.x * 25, -12 - arg1.coord.y * 25);
    }

    void OnDestroy()
    {
        MapsController.Ins.OnChangingMap -= OnChangingMap;
    }

    void GenerateMap()
    {
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                maps[i, j] = new MapData();

            }
        }

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                GoToDirection(new Vector2Int(i, j), (Direction)Random.Range(0, 4));
            }
        }

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                int roadsCount = Random.Range(2, 4);

                if (maps[i, j].GetRoadsCount() < roadsCount)
                {
                    int randNum = Random.Range(0, 4);

                    for (int k = 0; k < 4; k++)
                    {
                        if (maps[i, j].roads.Contains((Direction)((k + randNum) % 4)))
                            continue;
                        if (GoToDirection(new Vector2Int(i, j), (Direction)((k + randNum) % 4)))

                            if (maps[i, j].GetRoadsCount() >= roadsCount)
                                break;
                    }

                }
            }
        }

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                maps[i, j].SortRoads();
            }
        }

    }

    bool GoToDirection(Vector2Int mapCoords, Direction direction)
    {
        if (mapCoords.x >= 0 && mapCoords.x < mapSize && mapCoords.y >= 0 && mapCoords.y < mapSize)
        {
            Vector2Int coords = mapCoords + MapsController.directions[(int)direction];
            if (coords.x >= 0 && coords.x < mapSize && coords.y >= 0 && coords.y < mapSize)
            {


                Direction[] dir = new Direction[] { Direction.Top, Direction.Bottom, Direction.Left, Direction.Right };
                Direction[] oppositeDir = new Direction[] { Direction.Bottom, Direction.Top, Direction.Right, Direction.Left };

                if (!maps[mapCoords.x, mapCoords.y].roads.Contains(direction) && !maps[coords.x, coords.y].roads.Contains(oppositeDir[(int)direction]) && maps[coords.x, coords.y].GetRoadsCount() < 3)
                {

                    maps[mapCoords.x, mapCoords.y].roads.Add(dir[(int)direction]);
                    maps[coords.x, coords.y].roads.Add(oppositeDir[(int)direction]);
                    return true;
                }
            }
        }

        return false;
    }

    void ShowDirectionsMapsInConsole()
    {
        for (int j = 0; j < mapSize; j++)
        {
            string roads = "";

            for (int i = 0; i < mapSize; i++)
            {
                roads += "   " + string.Format("[{0},{1}]", i, j);

                for (int k = 0; k < maps[i, j].roads.Count; k++)
                {
                    roads += " " + maps[i, j].roads[k].ToString();
                }
            }

            Debug.Log(roads + j);
        }
    }
    void CreateLineRenderers()
    {
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                CreateLineRenderer(maps[i, j], new Vector2Int(i, j));
            }
        }
    }
    void CreateLineRenderer(MapData data, Vector2Int coords)
    {
        GameObject newLr = Instantiate(pfLineRenderer.gameObject);
        newLr.SetActive(true);
        newLr.transform.SetParent(lineRendererHolder, false);

        newLr.GetComponent<RectTransform>().anchoredPosition = new Vector2(coords.x * 100, coords.y * -100);

        UILineRenderer lr = newLr.GetComponent<UILineRenderer>();

        Vector2[] pointsList = new Vector2[1 + (data.roads.Count - 1) * 3];

        if (data.roads.Count == 3)
        {
            if (data.roads.Contains(Direction.Bottom) && data.roads.Contains(Direction.Top))
            {
                pointsList[0] = (new Vector2(.5f, 1f));
                pointsList[6] = (new Vector2(.5f, 0));

                if (data.roads.Contains(Direction.Left))
                    pointsList[3] = (new Vector2(0, .5f));
                if (data.roads.Contains(Direction.Right))
                    pointsList[3] = (new Vector2(1f, .5f));

                pointsList[1] = (new Vector2(.5f, .5f)) + new Vector2(Random.Range(-1f, 1f), Random.Range(0, 1f)) * .3f;
                pointsList[2] = (new Vector2(.5f, .5f)) + new Vector2(Random.Range(-1f, 1f), Random.Range(0, 1f)) * .3f;

                pointsList[4] = (new Vector2(.5f, .1f)) + new Vector2(Random.Range(-1f, 1f), Random.Range(0, 1f)) * .3f;
                pointsList[5] = (new Vector2(.5f, .1f)) + new Vector2(Random.Range(-1f, 1f), Random.Range(0, 1f)) * .3f;
            }

            if (data.roads.Contains(Direction.Left) && data.roads.Contains(Direction.Right))
            {
                pointsList[0] = (new Vector2(0, .5f));
                pointsList[6] = (new Vector2(1f, .5f));

                if (data.roads.Contains(Direction.Bottom))
                    pointsList[3] = (new Vector2(.5f, 0));
                if (data.roads.Contains(Direction.Top))
                    pointsList[3] = (new Vector2(.5f, 1f));

                pointsList[1] = (new Vector2(.1f, .5f)) + new Vector2(Random.Range(0, 1f), Random.Range(-1f, 1f)) * .3f;
                pointsList[2] = (new Vector2(.1f, .5f)) + new Vector2(Random.Range(0, 1f), Random.Range(-1f, 1f)) * .3f;

                pointsList[4] = (new Vector2(.5f, .5f)) + new Vector2(Random.Range(0, 1f), Random.Range(-1f, 1f)) * .3f;
                pointsList[5] = (new Vector2(.5f, .5f)) + new Vector2(Random.Range(0, 1f), Random.Range(-1f, 1f)) * .3f;
            }
        }

        if (data.roads.Count == 2)
            for (int i = 0; i < pointsList.Length; i++)
            {
                if ((i % 3) == 0)

                    switch (data.roads[i / 3])
                    {
                        case Direction.Bottom:
                            pointsList[i] = (new Vector2(.5f, 0));
                            break;
                        case Direction.Top:
                            pointsList[i] = (new Vector2(.5f, 1f));
                            break;
                        case Direction.Left:
                            pointsList[i] = (new Vector2(0, .5f));
                            break;
                        case Direction.Right:
                            pointsList[i] = (new Vector2(1f, .5f));
                            break;
                        default:
                            break;
                    }
                else
                    pointsList[i] = (new Vector2(.5f, .5f)) + Random.insideUnitCircle * .3f;
            }

        lr.Points = pointsList;
        lineRenderers[coords] = lr;
    }

    public Vector2 Evaluate(float persent, Vector2Int coords)
    {
        Vector2 point = Vector2.zero;

        if (lineRenderers[coords] == null || lineRenderers[coords].bezierPath == null)
            return point;

        if (maps[coords.x, coords.y].roads.Count == 3)
        {
            if (persent < .5f)
                point = lineRenderers[coords].bezierPath.CalculateBezierPoint(0, persent * 2);
            else
                point = lineRenderers[coords].bezierPath.CalculateBezierPoint(1, .5f + persent * 2f);
        }
        else
        {
            point = lineRenderers[coords].bezierPath.CalculateBezierPoint(0, persent);
        }

        playerPointImage.rectTransform.localPosition = new Vector2(point.x * 100, point.y * -100);

        if (playerPointImage.transform.parent != lineRenderers[coords].transform)
            playerPointImage.transform.SetParent(lineRenderers[coords].transform, false);
        Debug.Log(point);
        return point;
    }

    void Update()
    {
        if (World.Ins.player != null)
        {
            float size = (Mathf.Abs(MapsController.Ins.GetCurrentWorldEndPoints().y) + Mathf.Abs(MapsController.Ins.GetCurrentWorldEndPoints().x));
            float persent = (Mathf.Abs(MapsController.Ins.GetCurrentWorldEndPoints().x) + Mathf.Abs(World.Ins.player.transform.position.x)) / size;
            Evaluate(persent, MapsController.Ins.GetCurrentMapInfo().coord);
        }
    }

    public class MapData
    {
        public List<Direction> roads = new List<Direction>();

        public void SortRoads()
        {
            List<Direction> dir = new List<Direction>() { Direction.Top, Direction.Right, Direction.Bottom, Direction.Left };

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

}
