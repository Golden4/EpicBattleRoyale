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
    public Text alifeAmount;

    public override void OnInit()
    {
        World.OnPlayerSpawn += World_OnPlayerSpawn;
        CharacterBase.OnKillStatic += OnKillStatic;
        CharacterBase.OnDieStatic += OnDieStatic;
        UpdateAlifeAmount(GameController.CHARACTERS_COUNT_MAX);
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
        ShowKilledText("Killed: " + characterBase.killsCount);
        ShowInfoText("YOU kills " + characterBaseKilled.name + " using " + weapon.weaponName);
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

    void ShowKilledText(string text)
    {
        killText.gameObject.SetActive(true);

        killText.DOFade(1, .1f);

        DOVirtual.DelayedCall(2f, delegate
        {
            killText.DOFade(0, .5f);
        });

        killText.text = text;
    }

    public override void OnCleanUp()
    {
        World.OnPlayerSpawn -= World_OnPlayerSpawn;
        CharacterBase.OnKillStatic -= OnKillStatic;
        CharacterBase.OnDieStatic -= OnDieStatic;

        if (Player.Ins != null)
        {
            Player.Ins.characterBase.OnKill -= OnKill;
        }
    }

    void OnKillStatic(CharacterBase characterBase, CharacterBase characterBaseKilled, Weapon weapon)
    {
        killsListUI.AddKillToList(Player.Ins.characterBase, characterBase, characterBaseKilled, weapon.weaponName);
    }

    void OnDieStatic(CharacterBase characterBase)
    {
        Debug.Log(characterBase.name + "   " + World.Ins.allCharacters.Count);

        UpdateAlifeAmount(World.Ins.allCharacters.Count);
    }

    void UpdateAlifeAmount(int count)
    {
        alifeAmount.text = string.Format("<color=white>{0}</color> <size=14>ALIFE</size>", count);
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     OnKillStatic(World.Ins.allCharacters[Random.Range(0, 3)], World.Ins.allCharacters[Random.Range(0, 3)], GameAssets.Get.GetWeapon((GameAssets.WeaponsList)Random.Range(0, 6)));
        // }
    }
}
