using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenUI : ScreenBase {

	public HealthArmorUI healthArmorUI;
	public WeaponsUI weaponsUI;

	public override void OnInit ()
	{
		healthArmorUI.Setup (Player.Ins.characterBase.healthSystem);
		weaponsUI.Setup (Player.Ins.weaponController);
	}
}
