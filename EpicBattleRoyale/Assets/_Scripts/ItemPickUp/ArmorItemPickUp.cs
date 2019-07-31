using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorItemPickUp : ItemPickUp {
	public GameAssets.PickUpItemsData.ArmorPickUpList armorPickUpType;
	public int armorAmount;

	public override bool PickUp (CharacterBase cb, bool clickedPickUp = false)
	{
		cb.healthSystem.HealArmor (armorAmount);
		DestroyItem ();
		return true;
	}

}
