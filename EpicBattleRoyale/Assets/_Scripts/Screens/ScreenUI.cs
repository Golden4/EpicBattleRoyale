using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using DG.Tweening;

public class ScreenUI : SimpleMenu<ScreenUI>
{
    public HealthArmorUI healthArmorUI;
    public WeaponsUI weaponsUI;
    public MobileInputsUI mobileInputsUI;
    public KillsListUI killsListUI;
    public Text killText;
    public Text infoText;

    public override void OnInit()
    {
        World.OnPlayerSpawn += World_OnPlayerSpawn;
        CharacterBase.OnKillStatic += OnKillStatic;
    }

    void World_OnPlayerSpawn(Player player)
    {
        healthArmorUI.Setup(player.characterBase.healthSystem);
        weaponsUI.Setup(player.weaponController);
        mobileInputsUI.Setup(player.weaponController, player.characterBase);

        Player.Ins.characterBase.OnKill += OnKill;
    }


    void OnKill(CharacterBase characterBase, CharacterBase characterBaseKilled, Weapon weapon)
    {
        killText.gameObject.SetActive(true);

        killText.DOFade(1, .1f);

        DOVirtual.DelayedCall(2f, delegate
        {
            killText.DOFade(0, .5f);
        });

        killText.text = "Killed: " + characterBase.killsCount;
    }

    void ShowInfoText(string text)
    {
        infoText.gameObject.SetActive(true);

        infoText.DOFade(1, .1f);

        DOVirtual.DelayedCall(2f, delegate
        {
            infoText.DOFade(0, .5f);
        });

        infoText.text = text;
    }

    public override void OnCleanUp()
    {
        World.OnPlayerSpawn -= World_OnPlayerSpawn;

        if (Player.Ins != null)
        {
            Player.Ins.characterBase.OnKill -= OnKill;
        }
    }

    void OnKillStatic(CharacterBase characterBase, CharacterBase characterBaseKilled, Weapon weapon)
    {
        killsListUI.AddKillToList(Player.Ins.characterBase, characterBase, characterBaseKilled, weapon.weaponName);
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     OnKillStatic(World.Ins.allCharacters[Random.Range(0, 3)], World.Ins.allCharacters[Random.Range(0, 3)], GameAssets.Get.GetWeapon((GameAssets.WeaponsList)Random.Range(0, 6)));
        // }
    }
}
