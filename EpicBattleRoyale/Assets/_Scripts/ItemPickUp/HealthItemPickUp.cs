using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItemPickUp : ItemPickUp {
	
	public GameAssets.PickUpItemsData.HealthPickUpList healthPickUpType;
	public int healthAmount;

	public override bool PickUp (CharacterBase cb, bool clickedPickUp = false)
	{
		cb.healthSystem.HealHealth (healthAmount);
		return true;
	}

}
