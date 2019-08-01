using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenUI : ScreenBase {
	public static ScreenUI Ins;
	public HealthArmorUI healthArmorUI;
	public WeaponsUI weaponsUI;
	public MobileInputsUI mobileInputsUI;

	public override void OnInit ()
	{
		Ins = this;
		healthArmorUI.Setup (World.Ins.GetPlayer().characterBase.healthSystem);
		weaponsUI.Setup (World.Ins.GetPlayer().weaponController);
		mobileInputsUI.Setup (World.Ins.GetPlayer().weaponController);
		Debug.Log("InitScreenUI");
	}
}
