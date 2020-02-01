using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public static CameraController Ins;

    Camera camera;

    void Awake()
    {
        Ins = this;
        camera = GetComponent<Camera>();
    }

    public void ShakeCamera(float duration)
    {
        camera.DOShakePosition(duration, .05f, 1);
    }
}
