using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItemPickUp : ItemPickUp
{
    public GameAssets.PickUpItemsData.HealthList healthPickUpType;

    public override bool PickUp(CharacterBase cb, bool clickedPickUp = false)
    {
        // cb.characterInventory.AddItem(item);
        //cb.healthSystem.HealHealth(healthAmount);
        return true;
    }

}
