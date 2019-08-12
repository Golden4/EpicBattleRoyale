using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class ScreenUI : SimpleMenu<ScreenUI>
{
    public static ScreenUI Ins;
    public HealthArmorUI healthArmorUI;
    public WeaponsUI weaponsUI;
    public MobileInputsUI mobileInputsUI;

    protected override void Awake()
    {
        base.Awake();
        Ins = this;
        World.OnPlayerSpawn += World_OnPlayerSpawn;

    }

    void World_OnPlayerSpawn(Player player)
    {
        healthArmorUI.Setup(player.characterBase.healthSystem);
        weaponsUI.Setup(player.weaponController);
        mobileInputsUI.Setup(player.weaponController, player.characterBase);
    }
}
