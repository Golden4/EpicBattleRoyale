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
    public int houseIndex;
    public MapsController.HouseType houseType;

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
        cb.EnterOrExitDoor(this);
        return true;
    }

    public override InteractableType GetInteractableType()
    {
        return InteractableType.HouseDoor;
    }

    // void OnDisable()
    // {
    //     for (int i = 0; i < cbs.Count; i++)
    //     {
    //         cbs[i].AwayDoor();
    //     }
    //     cbs.Clear();
    // }
}
