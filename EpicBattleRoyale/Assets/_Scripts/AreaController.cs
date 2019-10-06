using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaController : MonoBehaviour
{
    public static AreaController Ins;
    public Area[,] areas;
    public AreaLevel[] areaLevels;
    public int curAreaLevel;

    public enum AreasState
    {
        Waiting,
        Decreasing,
    }

    public AreasState curAreasState;

    public static event Action<Area[,]> OnChangeState;
    public static event Action<int> OnStartDecresingArea;
    public static event Action<int> OnNextDecreasingArea;

    int curWaitingTicks;

    void Awake()
    {
        Ins = this;
        areas = new Area[MapsController.mapSize, MapsController.mapSize];

        for (int i = 0; i < MapsController.mapSize; i++)
        {
            for (int j = 0; j < MapsController.mapSize; j++)
            {
                areas[i, j] = new Area(Area.AreaState.Normal);
            }
        }
        curAreaSize = MapsController.mapSize - 1;
        SetAreaState(AreasState.Waiting);
    }

    public Area GetArea(int x, int y)
    {
        return areas[x, y];
    }

    public int GetDamageAmount()
    {
        return curAreaLevel;
    }

    public float GetHitCoudown()
    {
        return 3f / curAreaLevel;
    }

    void Update() { }

    void StartTimer(int timer, Action<int> onTimerTick, Action OnTimerEnd = null)
    {
        curWaitingTicks = timer;
        StartCoroutine(Timer(onTimerTick, OnTimerEnd));
    }

    IEnumerator Timer(Action<int> onTimerTick, Action OnTimerEnd)
    {
        while (curWaitingTicks > 0)
        {

            if (onTimerTick != null)
                onTimerTick(curWaitingTicks);

            yield return new WaitForSeconds(1);
            curWaitingTicks--;
        }
        if (OnTimerEnd != null)
            OnTimerEnd();
    }

    void SetAreaState(AreasState state)
    {
        curAreasState = state;

        if (curAreaLevel < areaLevels.Length)
            switch (state)
            {
                case AreasState.Waiting:
                    SetAreaStateWaiting();
                    break;
                case AreasState.Decreasing:
                    SetAreaStateDecreasing();
                    break;
                default:
                    break;
            }
        else
        {
            OnAllDamageableAreas();
        }

        if (OnChangeState != null)
            OnChangeState(areas);
    }

    void OnAllDamageableAreas()
    {
        DamageabeAreaState();

        ScreenUI.Instance.areaTimer.color = Color.white;
        ScreenUI.Instance.areaTimer.text = "--:--";
    }

    void SetAreaStateWaiting()
    {
        DamageabeAreaState();

        DecreasingAreaState();

        StartTimer(areaLevels[curAreaLevel].waitingTime, (int time) =>
        {
            if (time == 30)
            {
                if (OnNextDecreasingArea != null)
                    OnNextDecreasingArea(time);
            }

            ScreenUI.Instance.areaTimer.color = Color.white;
            ScreenUI.Instance.areaTimer.text = string.Format("{0}:{1}", (curWaitingTicks / 60).ToString("00"), (curWaitingTicks % 60).ToString("00"));

        }, () =>
        {
            SetAreaState(AreasState.Decreasing);
        });


        if (OnNextDecreasingArea != null)
            OnNextDecreasingArea(areaLevels[curAreaLevel].waitingTime);
    }

    void SetAreaStateDecreasing()
    {

        StartTimer(areaLevels[curAreaLevel].decreasingTime,
        (int time) =>
        {
            ScreenUI.Instance.areaTimer.color = Color.red;
            ScreenUI.Instance.areaTimer.text = TicksToTime(curWaitingTicks);
        }, () =>
        {
            curAreaLevel++;
            SetAreaState(AreasState.Waiting);
        });

        if (OnStartDecresingArea != null)
            OnStartDecresingArea(areaLevels[curAreaLevel].decreasingTime);
    }

    int curAreaSize;
    Vector2Int curAreaStartOffset;

    void DecreasingAreaState()
    {
        Vector2Int randomPoint = new Vector2Int(UnityEngine.Random.Range(curAreaStartOffset.x, curAreaStartOffset.x + curAreaSize), UnityEngine.Random.Range(curAreaStartOffset.y, curAreaStartOffset.y + curAreaSize));
        bool top = UnityEngine.Random.Range(0, 2) == 0;
        bool left = UnityEngine.Random.Range(0, 2) == 0;

        if (curAreaSize > 0)
            if (left)
            {

                for (int i = curAreaStartOffset.x; i <= curAreaStartOffset.x + curAreaSize; i++)
                {
                    areas[i, curAreaStartOffset.y].ChangeState(Area.AreaState.Decreasing);
                }

            }
            else
            {
                for (int i = curAreaStartOffset.x; i <= curAreaStartOffset.x + curAreaSize; i++)
                {
                    areas[i, curAreaStartOffset.y + curAreaSize].ChangeState(Area.AreaState.Decreasing);
                }
            }

        if (top)
        {
            for (int i = curAreaStartOffset.y; i <= curAreaStartOffset.y + curAreaSize; i++)
            {
                areas[curAreaStartOffset.x, i].ChangeState(Area.AreaState.Decreasing);
            }

        }
        else
        {
            for (int i = curAreaStartOffset.y; i <= curAreaStartOffset.y + curAreaSize; i++)
            {
                areas[curAreaStartOffset.x + curAreaSize, i].ChangeState(Area.AreaState.Decreasing);
            }
        }

        if (top) curAreaStartOffset.x++;
        if (left) curAreaStartOffset.y++;

        curAreaSize--;
    }

    public bool IsOnArea(Vector2Int coords)
    {
        return areas[coords.x, coords.y].curAreaState != Area.AreaState.Damageable;
    }

    void DamageabeAreaState()
    {
        for (int i = 0; i < MapsController.mapSize; i++)
        {
            for (int j = 0; j < MapsController.mapSize; j++)
            {
                if (areas[i, j].curAreaState == Area.AreaState.Decreasing)
                    areas[i, j].ChangeState(Area.AreaState.Damageable);

            }
        }
    }
    public static string TicksToTime(int ticks)
    {
        return string.Format("{0}:{1}", (ticks / 60).ToString("00"), (ticks % 60).ToString("00"));
    }

    public class Area
    {

        public enum AreaState
        {
            Normal,
            Decreasing,
            Damageable
        }

        public AreaState curAreaState;

        public void ChangeState(AreaState state)
        {
            if (curAreaState == AreaState.Damageable)
            {

            }
            else
            {
                curAreaState = state;
            }
        }


        public Area() { }

        public Area(AreaState curAreaState)
        {
            this.curAreaState = curAreaState;
        }
    }

    [System.Serializable]
    public class AreaLevel
    {
        public int waitingTime;
        public int decreasingTime;
    }
}