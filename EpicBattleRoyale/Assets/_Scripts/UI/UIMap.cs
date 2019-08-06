using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMap : MonoBehaviour
{

    public Image playerPointImage;

    void Start()
    {
        MapsController.Ins.OnChangingMap += OnChangingMap;
    }

    void OnChangingMap(MapsController.MapInfo arg1, Direction arg2)
    {
        playerPointImage.rectTransform.anchoredPosition = new Vector3(12 + arg1.coord.x * 25, -12 - arg1.coord.y * 25);
    }

    void OnDestroy()
    {
        MapsController.Ins.OnChangingMap -= OnChangingMap;
    }
}
