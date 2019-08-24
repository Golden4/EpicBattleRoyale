using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMapNavigate : MonoBehaviour
{
    public MapsController.State state;
    public int houseIndex = -1;
    public CharacterBase characterBase;

    void Awake()
    {
        characterBase = GetComponent<CharacterBase>();
        MapsController.OnChangeMap += OnChangeMap;
        MapsController.OnEnterHouse += OnEnterHouse;
        MapsController.OnExitHouse += OnExitHouse;
    }

    public void ChangeMap(Direction direction)
    {
        ChangeMap(characterBase.mapCoords + MapsController.directions[(int)direction], direction);
    }

    public void ChangeMap(Vector2Int targetMapCoords, Direction direction = Direction.None, Vector2 postition = default)
    {
        int index = -1;

        if (direction != Direction.None)
            index = MapsController.Ins.GetSpawnDirection(targetMapCoords, direction);

        //0 left 1 right 2 center
        if (index != -1)
        {
            bool isFacingRight = true;

            Vector3 pos = default;

            if (index == 1)
            {
                isFacingRight = false;
                pos = new Vector3(MapsController.Ins.GetCurrentWorldEndPoints().y - 1, characterBase.worldPosition.y);
            }

            if (index == 0)
            {
                pos = new Vector3(MapsController.Ins.GetCurrentWorldEndPoints().x + 1, characterBase.worldPosition.y);
            }

            if (index == 2)
            {
                pos = new Vector3(0, MapsController.Ins.GetCurrentWorldUpDownEndPoints().y);
            }

            characterBase.MoveToPosition(pos, isFacingRight);
        }
        else
        {
            //road center
            if (postition == default)
                characterBase.MoveToPosition(Vector2.up * MapsController.Ins.GetCurrentVerticalCenterPoint(), true);

            else characterBase.MoveToPosition(postition, true);
        }


        characterBase.mapCoords = targetMapCoords;
        houseIndex = -1;
        state = MapsController.State.Map;
        characterBase.characterInteractable.ClearInteractableObjects();


        if (MapsController.Ins.curMapCoords != this.characterBase.mapCoords)
        {
            characterBase.Disable();
        }
        else
        {
            characterBase.Enable();
        }

    }

    public void EnterDoor(HouseDoor houseInfo)
    {
        if (state == MapsController.State.Map)
        {
            if (houseInfo != null)
            {
                Vector3 pos = new Vector3(MapsController.Ins.GetHouseData(houseInfo.houseType).worldEndPoints.x + 1, -4);

                characterBase.MoveToPosition(pos, true);
                characterBase.characterInteractable.ClearInteractableObjects();

                houseIndex = houseInfo.houseIndex;
                state = MapsController.State.House;

                if (MapsController.Ins.curMapCoords != this.characterBase.mapCoords)
                {
                    characterBase.Disable();
                }
                else
                {
                    if (houseIndex == MapsController.Ins.curHouseIndex)
                        characterBase.Enable();
                    else characterBase.Disable();
                }
            }
        }
    }

    public void ExitDoor(HouseDoor houseInfo)
    {
        if (state == MapsController.State.House)
        {
            if (houseInfo != null)
            {
                Vector3 pos = MapsController.Ins.GetCurrentMapInfo().houses[MapsController.Ins.curHouseIndex].GetDoorPosition(MapsController.Ins.GetCurrentMapInfo()) + Vector3.up;

                characterBase.MoveToPosition(pos, false);

                characterBase.characterInteractable.ClearInteractableObjects();
                houseIndex = -1;
                state = MapsController.State.Map;


                if (MapsController.Ins.curMapCoords != this.characterBase.mapCoords)
                {
                    characterBase.Disable();
                }
                else
                {
                    characterBase.Enable();
                }
            }
        }
    }

    void OnChangeMap(CharacterBase cb, Vector2Int mapCoords, Direction direction)
    {
        if (MapsController.Ins.curMapCoords != this.characterBase.mapCoords)
        {
            this.characterBase.Disable();
        }
        else
        {
            this.characterBase.Enable();
        }
    }

    void OnEnterHouse(CharacterBase cb, HouseDoor house)
    {
        if (MapsController.Ins.curMapCoords != this.characterBase.mapCoords)
        {
            this.characterBase.Disable();
        }
        else
        {
            if (houseIndex == MapsController.Ins.curHouseIndex)
                this.characterBase.Enable();
            else this.characterBase.Disable();
        }
    }

    void OnExitHouse(CharacterBase cb, HouseDoor house)
    {
        if (MapsController.Ins.curMapCoords != this.characterBase.mapCoords)
        {
            this.characterBase.Disable();
        }
        else
        {
            this.characterBase.Enable();
        }

    }

    void OnDestroy()
    {
        MapsController.OnChangeMap -= OnChangeMap;
        MapsController.OnEnterHouse -= OnEnterHouse;
        MapsController.OnExitHouse -= OnExitHouse;
    }
}
