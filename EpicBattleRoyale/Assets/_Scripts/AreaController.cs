using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaController : MonoBehaviour {
    public static AreaController Ins;
    public Area[, ] areas;
    public AreaLevel[] areaLevels;
    int curAreaLevel;

    public enum AreasState {
        Waiting,
        Decreasing,
    }

    public AreasState curAreasState;

    int curWaitingTicks;

    void Awake () {
        Ins = this;
    }

    void Start () {

        areas = new Area[MapsController.Ins.mapSize, MapsController.Ins.mapSize];

        for (int i = 0; i < MapsController.Ins.mapSize; i++) {
            for (int j = 0; j < MapsController.Ins.mapSize; j++) {
                areas[i, j] = new Area (Area.AreaState.Normal);
            }
        }
        SetAreaState (AreasState.Waiting);
    }

    public Area GetArea (int x, int y) {
        return areas[x, y];
    }

    void Update () { }

    void StartTimer (int timer, Action OnTimerEnd = null) {
        curWaitingTicks = timer;
        StartCoroutine (Timer (OnTimerEnd));
    }

    IEnumerator Timer (Action OnTimerEnd) {
        while (curWaitingTicks > 0) {

            if (curAreasState == AreasState.Waiting) {
                ScreenUI.Instance.areaTimer.color = Color.white;
            } else {
                ScreenUI.Instance.areaTimer.color = Color.red;
            }

            ScreenUI.Instance.areaTimer.text = string.Format ("{0}:{1}", (curWaitingTicks / 60).ToString ("00"), (curWaitingTicks % 60).ToString ("00"));

            Debug.Log (curWaitingTicks);
            yield return new WaitForSeconds (1);
            curWaitingTicks--;
        }
        if (OnTimerEnd != null)
            OnTimerEnd ();
    }

    void SetAreaState (AreasState state) {
        curAreasState = state;
        if (curAreaLevel >= areaLevels.Length)
            return;

        switch (state) {
            case AreasState.Waiting:
                StartTimer (areaLevels[curAreaLevel].waitingTime, () => SetAreaState (AreasState.Decreasing));
                break;
            case AreasState.Decreasing:
                StartTimer (areaLevels[curAreaLevel].decreasingTime, () => { curAreaLevel++; SetAreaState (AreasState.Waiting); });
                break;
            default:
                break;
        }

        Debug.Log (curAreaLevel);
    }

    public class Area {

        public enum AreaState {
            Normal,
            Decreasing,
            Damageable
        }

        public AreaState curAreaState;

        public Area () { }

        public Area (AreaState curAreaState) {
            this.curAreaState = curAreaState;
        }
    }

    [System.Serializable]
    public class AreaLevel {
        public int waitingTime;
        public int decreasingTime;
    }
}