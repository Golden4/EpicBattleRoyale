using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class HealthItem : Item
{
    public int health = 50;
    public GameAssets.PickUpItemsData.HealthList healthType;

    public override void OnAddInventory()
    {

    }

    public override void OnRemoveInventory()
    {
    }

    public override void Use(CharacterBase characterBase)
    {

    }
}
