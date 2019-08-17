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
    [SerializeField] RectTransform gameMapPanel;
    [SerializeField] Image pfHouse;

    Dictionary<Vector2Int, UILineRenderer> lineRenderers = new Dictionary<Vector2Int, UILineRenderer>();

    public RawImage playerPointImage;
    public bool increasedMap = false;

    IEnumerator Start()
    {
        gameMapPanel.gameObject.AddComponent<Button>().onClick.AddListener(delegate
        {
            if (increasedMap)
            {
                ShowDecreaseMapBtn();
            }
            else
            {
                ShowIncreaseMapBtn();
            }
        });
        yield return new WaitForEndOfFrame();
        CreateLineRenderers();
        yield return new WaitForEndOfFrame();
        CreateAllHouses();
    }

    void CreateLineRenderers()
    {
        for (int i = 0; i < MapsController.Ins.mapSize; i++)
        {
            for (int j = 0; j < MapsController.Ins.mapSize; j++)
            {
                CreateLineRenderer(MapsController.Ins.maps[i, j], new Vector2Int(i, j));
            }
        }
    }

    void CreateLineRenderer(MapsController.MapInfo data, Vector2Int coords)
    {
        GameObject newLr = Instantiate(pfLineRenderer.gameObject);
        newLr.SetActive(true);
        newLr.transform.SetParent(lineRendererHolder, false);

        newLr.GetComponent<RectTransform>().anchoredPosition = new Vector2(coords.x * 100, coords.y * -100) + new Vector2(25, -25);

        UILineRenderer lr = newLr.GetComponent<UILineRenderer>();

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
        for (int i = 0; i < MapsController.Ins.mapSize; i++)
        {
            for (int j = 0; j < MapsController.Ins.mapSize; j++)
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

            float persent = WorldPositionToPersent(data.houses[i].houseSpawnPositionsX);
            persent = 1 - persent;
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

        //если игрок не в доме
        if (MapsController.Ins.mapState == MapsController.State.Map)
        {
            playerPointImage.rectTransform.localPosition = new Vector2(point.x * 100, point.y * 100) + Vector2.up * -100;

            playerPointImage.rectTransform.localRotation = Quaternion.FromToRotation(Vector3.down, direction);
        }

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

        return new Vector2(point.x * 100, point.y * 100) + Vector2.up * -100;
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

            Evaluate(persent, MapsController.Ins.GetCurrentMapInfo().coord);
        }
    }

    float WorldPositionToPersent(float position)
    {
        float size = (Mathf.Abs(MapsController.Ins.GetCurrentWorldEndPoints().y) + Mathf.Abs(MapsController.Ins.GetCurrentWorldEndPoints().x));

        float persent = (Mathf.Abs(MapsController.Ins.GetCurrentWorldEndPoints().x - position)) / size;

        return persent;
    }

    public void ShowDecreaseMapBtn()
    {
        gameMapPanel.anchorMin = Vector2.one;
        gameMapPanel.anchorMax = Vector2.one;

        gameMapPanel.localScale = Vector3.one * .25f;
        gameMapPanel.pivot = Vector2.one;
        gameMapPanel.anchoredPosition = Vector3.zero;
        increasedMap = false;
    }

    public void ShowIncreaseMapBtn()
    {
        gameMapPanel.anchorMin = Vector2.one * .5f;
        gameMapPanel.anchorMax = Vector2.one * .5f;

        gameMapPanel.localScale = Vector3.one * .82f;
        gameMapPanel.pivot = Vector2.one * .5f;
        gameMapPanel.anchoredPosition = Vector3.zero;
        increasedMap = true;
    }



}
