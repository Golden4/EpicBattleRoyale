using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapUI : MonoBehaviour
{
    Vector2 miniMapOffsets;
    [SerializeField] RectTransform gameMap;

    void Awake()
    {
        MapsController.OnChangeMap += OnChangeMap;
    }

    private void OnChangeMap(CharacterBase cb, Vector2Int coords, Direction direction)
    {
        gameMap.anchoredPosition = new Vector2(((coords.x > 0) ? (coords.x - 1) * -200 - 75 : 0), ((coords.y > 0) ? (coords.y - 1) * 200 + 75 : 0));
    }

    public void ShowMap()
    {
        MapScreen.Show();
    }

    void OnDestroy()
    {

        MapsController.OnChangeMap -= OnChangeMap;
    }

}
