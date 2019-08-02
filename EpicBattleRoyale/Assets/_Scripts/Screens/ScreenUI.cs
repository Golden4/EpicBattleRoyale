using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenUI : ScreenBase
{
    public static ScreenUI Ins;
    public HealthArmorUI healthArmorUI;
    public WeaponsUI weaponsUI;
    public MobileInputsUI mobileInputsUI;

    public override void OnInit()
    {
        Ins = this;
        World.OnPlayerSpawn += World_OnPlayerSpawn;

    }
    void World_OnPlayerSpawn(Player player)
    {
        healthArmorUI.Setup(player.characterBase.healthSystem);
        weaponsUI.Setup(player.weaponController);
        mobileInputsUI.Setup(player.weaponController);
    }
}
