using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class HealthItem : Item
{
    public int health = 50;
    public GameAssets.PickUpItemsData.HealthList healthType;
    public float usingTime = 5;

    public void Use(CharacterBase characterBase)
    {
        characterBase.healthSystem.HealHealth(health);
        characterBase.characterInventory.items[this].CurCount--;
    }
}
