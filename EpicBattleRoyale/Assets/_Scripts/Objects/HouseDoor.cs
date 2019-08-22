using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseDoor : Interactable
{
    public enum HouseDoorType
    {
        Inner,
        Outer
    }

    public HouseDoorType doorType;
    public MapsController.HouseType houseType;
    public int houseIndex;

    public void Setup(Vector2Int mapCoords, MapsController.HouseType houseType, int houseIndex)
    {
        this.mapCoords = mapCoords;
        this.houseIndex = houseIndex;
        this.houseType = houseType;
    }

    public override bool CanInteract(CharacterBase cb)
    {
        if (doorType != HouseDoorType.Inner && !CompareEntitiesPositions(cb.worldPosition))
            return false;

        return true;
    }

    public override bool Interact(CharacterBase cb)
    {
        if (doorType == HouseDoorType.Inner)
        {
            MapsController.Ins.ExitHouse(cb, this);
        }
        else
        if (doorType == HouseDoorType.Outer)
        {
            MapsController.Ins.EnterHouse(cb, this);
        }
        return true;
    }

    public override InteractableType GetInteractableType()
    {
        return InteractableType.HouseDoor;
    }
}
