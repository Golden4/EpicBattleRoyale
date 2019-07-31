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
		healthArmorUI.Setup (Player.Ins.characterBase.healthSystem);
		weaponsUI.Setup (Player.Ins.weaponController);
		mobileInputsUI.Setup (Player.Ins.weaponController);
	}
}
