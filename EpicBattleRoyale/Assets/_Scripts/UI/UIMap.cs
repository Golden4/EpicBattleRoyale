using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UIMap : MonoBehaviour
{
    [SerializeField] GameObject pfLineRenderer;
    [SerializeField] Transform lineRendererHolder;
    [SerializeField] RectTransform gameMapPanel;
    [SerializeField] Image pfHouse;
    Image[,] areaGrids;

    Dictionary<Vector2Int, UILineRenderer> lineRenderers = new Dictionary<Vector2Int, UILineRenderer>();

    public RawImage playerPointImage;

    void Awake()
    {
        MapsController.OnChangeMap += OnChangeMap;
        AreaController.OnChangeState += OnChangeState;

        if (seed == 0)
            seed = (int)Random.Range(-Mathf.Infinity, Mathf.Infinity);
    }

    private void OnChangeState(AreaController.Area[,] areas)
    {
        if (areaGrids != null)
            for (int i = 0; i < MapsController.mapSize; i++)
            {
                for (int j = 0; j < MapsController.mapSize; j++)
                {
                    UpdateAreaGridColor(i, j, areas[i, j].curAreaState);
                }
            }
    }

    private void OnChangeMap(CharacterBase cb, Vector2Int mapCoords, Direction dir)
    {
        // if (grids != null)
        //     for (int i = 0; i < MapsController.Ins.mapSize; i++) {
        //         for (int j = 0; j < MapsController.Ins.mapSize; j++) {
        //             UpdateGridColor (i, j, mapCoords == new Vector2Int (i, j));
        //         }
        //     }
    }

    void UpdateAreaGridColor(int i, int j, AreaController.Area.AreaState state)
    {
        Color color = default;
        switch (state)
        {
            case AreaController.Area.AreaState.Normal:
                color = Color.black;
                color.a = 0;
                break;

            case AreaController.Area.AreaState.Decreasing:
                color = Color.blue;
                color.a = .3f;
                break;
            case AreaController.Area.AreaState.Damageable:
                color = Color.red;
                color.a = .6f;
                break;
            default:
                break;
        }
        areaGrids[i, j].color = color;
    }

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        Random.InitState(seed);
        CreateLineRenderers();
        yield return new WaitForEndOfFrame();
        Random.InitState(seed);
        CreateAllHouses();

        Canvas canvas = playerPointImage.gameObject.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder += 10;
    }

    static int seed = 0;

    void CreateLineRenderers()
    {
        areaGrids = new Image[MapsController.mapSize, MapsController.mapSize];
        for (int i = 0; i < MapsController.mapSize; i++)
        {
            for (int j = 0; j < MapsController.mapSize; j++)
            {
                CreateLineRenderer(MapsController.Ins.maps[i, j], new Vector2Int(i, j));
            }
        }
    }

    float mapWidth = -1;

    void CreateLineRenderer(MapsController.MapInfo data, Vector2Int coords)
    {
        if (mapWidth == -1)
        {
            mapWidth = pfLineRenderer.GetComponent<RectTransform>().sizeDelta.x;
        }
        GameObject newLr = Instantiate(pfLineRenderer);
        newLr.SetActive(true);
        newLr.transform.SetParent(lineRendererHolder, false);

        newLr.GetComponent<RectTransform>().anchoredPosition = new Vector2(coords.x * mapWidth, coords.y * -mapWidth);

        UILineRenderer lr = newLr.GetComponentInChildren<UILineRenderer>();
        areaGrids[coords.x, coords.y] = newLr.transform.GetChild(0).GetComponent<Image>();
        UpdateAreaGridColor(coords.x, coords.y, AreaController.Ins.areas[coords.x, coords.y].curAreaState);

        Vector2[] pointsList;

        if (data.centerRoad != Direction.None)
        {

            pointsList = new Vector2[7];

            if (data.roads.Contains(Direction.Bottom) && data.roads.Contains(Direction.Top))
            {
                pointsList[0] = (new Vector2(.5f, 1f));
                pointsList[6] = (new Vector2(.5f, 0));

                if (data.centerRoad == Direction.Left)
                    pointsList[3] = (new Vector2(0, .5f));
                if (data.centerRoad == Direction.Right)
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

                if (data.centerRoad == Direction.Bottom)
                    pointsList[3] = (new Vector2(.5f, 0));
                if (data.centerRoad == Direction.Top)
                    pointsList[3] = (new Vector2(.5f, 1f));

                pointsList[1] = (new Vector2(.1f, .5f)) + new Vector2(Random.Range(0, 1f), Random.Range(-1f, 1f)) * .3f;
                pointsList[2] = (new Vector2(.1f, .5f)) + new Vector2(Random.Range(0, 1f), Random.Range(-1f, 1f)) * .3f;

                pointsList[4] = (new Vector2(.5f, .5f)) + new Vector2(Random.Range(0, 1f), Random.Range(-1f, 1f)) * .3f;
                pointsList[5] = (new Vector2(.5f, .5f)) + new Vector2(Random.Range(0, 1f), Random.Range(-1f, 1f)) * .3f;
            }
        }
        else
        {
            pointsList = new Vector2[4];
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
        }

        lr.Points = pointsList;
        lineRenderers[coords] = lr;
    }

    void CreateAllHouses()
    {
        for (int i = 0; i < MapsController.mapSize; i++)
        {
            for (int j = 0; j < MapsController.mapSize; j++)
            {
                CreateHouses(MapsController.Ins.maps[i, j], new Vector2Int(i, j));
            }
        }
    }
    void CreateHouses(MapsController.MapInfo data, Vector2Int coords)
    {
        for (int i = 0; i < data.houses.Count; i++)
        {
            RectTransform newHouse = Instantiate(pfHouse.gameObject).GetComponent<RectTransform>();
            newHouse.gameObject.SetActive(true);

            float persent = WorldPositionToPersent(data, data.houses[i].houseSpawnPositionsX);
            persent = 1f - persent;
            if (newHouse.transform.parent != lineRenderers[coords].transform)
                newHouse.transform.SetParent(lineRenderers[coords].transform, false);


            newHouse.localPosition = CalculateRoadPoint(persent, coords);
            newHouse.localRotation = Quaternion.FromToRotation(Vector3.up, CalculateRoadDirection(persent, coords));
        }
    }

    public Vector2 Evaluate(float persent, Vector2Int coords)
    {
        Vector2 point = Vector2.zero;
        Vector2 direction = Vector2.zero;

        if (lineRenderers.Count == 0 || lineRenderers[coords] == null || lineRenderers[coords].bezierPath == null || lineRenderers[coords].Points == null || lineRenderers[coords].bezierPath.GetControlPoints() == null || lineRenderers[coords].bezierPath.GetControlPoints().Count == 0)
            return point;

        if (MapsController.Ins.maps[coords.x, coords.y].centerRoad != Direction.None)
        {
            if (persent < .5f)
            {
                point = lineRenderers[coords].bezierPath.CalculateBezierPoint(1, 1 - persent * 2f);
                direction = lineRenderers[coords].bezierPath.CalculateBezierDirection(1, 1 - persent * 2f);
            }
            else
            {
                point = lineRenderers[coords].bezierPath.CalculateBezierPoint(0, 1 - (persent - .5f) * 2f);
                direction = lineRenderers[coords].bezierPath.CalculateBezierDirection(0, 1 - (persent - .5f) * 2f);
            }
        }
        else
        {
            point = lineRenderers[coords].bezierPath.CalculateBezierPoint(0, 1f - persent);
            direction = lineRenderers[coords].bezierPath.CalculateBezierDirection(0, 1f - persent);
        }

        if (playerPointImage.transform.parent != lineRenderers[coords].transform)
        {
            playerPointImage.transform.SetParent(lineRenderers[coords].transform, false);
        }

        playerPointImage.transform.SetAsLastSibling();

        playerPointImage.rectTransform.localPosition = new Vector2(point.x * mapWidth, point.y * mapWidth) + Vector2.up * -mapWidth;

        playerPointImage.rectTransform.localRotation = Quaternion.FromToRotation(Vector3.down, direction);


        Vector3 scale = playerPointImage.rectTransform.localScale;
        scale.y = (!World.Ins.player.characterBase.isFacingRight) ? 4 : -4;
        playerPointImage.rectTransform.localScale = scale;

        return point;
    }

    public Vector2 CalculateRoadPoint(float persent, Vector2Int coords)
    {
        Vector2 point = Vector2.zero;

        if (lineRenderers[coords] == null || lineRenderers[coords].bezierPath == null || lineRenderers[coords].Points == null || lineRenderers[coords].bezierPath.GetControlPoints() == null || lineRenderers[coords].bezierPath.GetControlPoints().Count == 0)
            return point;

        if (MapsController.Ins.maps[coords.x, coords.y].centerRoad != Direction.None)
        {
            if (persent < .5f)
            {
                point = lineRenderers[coords].bezierPath.CalculateBezierPoint(1, 1 - persent * 2f);
            }
            else
            {
                point = lineRenderers[coords].bezierPath.CalculateBezierPoint(0, 1 - (persent - .5f) * 2f);
            }
        }
        else
        {
            point = lineRenderers[coords].bezierPath.CalculateBezierPoint(0, 1f - persent);
        }

        return new Vector2(point.x * mapWidth, point.y * mapWidth) + Vector2.up * -mapWidth;
    }

    public Vector2 CalculateRoadDirection(float persent, Vector2Int coords)
    {
        Vector2 direction = Vector2.zero;

        if (lineRenderers[coords] == null || lineRenderers[coords].bezierPath == null || lineRenderers[coords].Points == null || lineRenderers[coords].bezierPath.GetControlPoints() == null || lineRenderers[coords].bezierPath.GetControlPoints().Count == 0)
            return direction;

        if (MapsController.Ins.maps[coords.x, coords.y].centerRoad != Direction.None)
        {
            if (persent < .5f)
            {
                direction = lineRenderers[coords].bezierPath.CalculateBezierDirection(1, 1 - persent * 2f);
            }
            else
            {
                direction = lineRenderers[coords].bezierPath.CalculateBezierDirection(0, 1 - (persent - .5f) * 2f);
            }
        }
        else
        {
            direction = lineRenderers[coords].bezierPath.CalculateBezierDirection(0, 1f - persent);
        }
        return direction;
    }

    void Update()
    {
        if (World.Ins.player != null)
        {
            float persent = WorldPositionToPersent(World.Ins.player.transform.position.x);
            persent = 1f - persent;

            if (MapsController.Ins.mapState == MapsController.State.Map)
                Evaluate(persent, MapsController.Ins.GetCurrentMapInfo().coord);
        }
    }

    void OnDestroy()
    {

        MapsController.OnChangeMap -= OnChangeMap;
        AreaController.OnChangeState -= OnChangeState;
    }

    float WorldPositionToPersent(float position)
    {
        float size = (Mathf.Abs(MapsController.Ins.GetCurrentWorldEndPoints().y) + Mathf.Abs(MapsController.Ins.GetCurrentWorldEndPoints().x));

        float persent = (Mathf.Abs(MapsController.Ins.GetCurrentWorldEndPoints().x - position)) / size;

        return persent;
    }


    float WorldPositionToPersent(MapsController.MapInfo data, float position)
    {
        float size = (Mathf.Abs(MapsController.Ins.GetWorldEndPoints(data.mapType).y) + Mathf.Abs(MapsController.Ins.GetWorldEndPoints(data.mapType).x));

        float persent = (Mathf.Abs(MapsController.Ins.GetWorldEndPoints(data.mapType).x - position)) / size;

        return persent;
    }

    // public void ShowDecreaseMapBtn()
    // {
    //     gameMapPanel.anchorMin = Vector2.one;
    //     gameMapPanel.anchorMax = Vector2.one;

    //     gameMapPanel.localScale = Vector3.one * .25f;
    //     gameMapPanel.pivot = Vector2.one;
    //     gameMapPanel.anchoredPosition = Vector3.zero;
    //     increasedMap = false;
    // }

    // public void ShowIncreaseMapBtn()
    // {
    //     gameMapPanel.anchorMin = Vector2.one * .5f;
    //     gameMapPanel.anchorMax = Vector2.one * .5f;

    //     gameMapPanel.localScale = Vector3.one * .82f;
    //     gameMapPanel.pivot = Vector2.one * .5f;
    //     gameMapPanel.anchoredPosition = Vector3.zero;
    //     increasedMap = true;
    // }
}