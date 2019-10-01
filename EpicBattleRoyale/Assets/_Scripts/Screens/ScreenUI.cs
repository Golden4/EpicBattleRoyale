using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class ScreenUI : SimpleMenu<ScreenUI>
{
    public HealthArmorUI healthArmorUI;
    public WeaponsUI weaponsUI;
    public MobileInputsUI mobileInputsUI;
    public KillsListUI killsListUI;
    public Text killText;
    public Text killsInfoText;
    public Text areaInfoText;
    public Text alifeAmount;
    public Text killsAmount;
    public Text areaTimer;

    public override void OnInit()
    {
        World.OnPlayerSpawn += World_OnPlayerSpawn;
        CharacterBase.OnKillStatic += OnKillStatic;
        CharacterBase.OnDieStatic += OnDieStatic;

        AreaController.OnStartDecresingArea += OnStartDecresingArea;
        AreaController.OnNextDecreasingArea += OnNextDecreasingArea;
        UpdateAlifeAmount(GameController.CHARACTERS_COUNT_MAX);
        UpdateKillsAmount(0);
    }

    private void OnNextDecreasingArea(int time)
    {
        ShowAreaInfoText("RESTRICTING THE PLAY AREA IN " + AreaController.TicksToTime(time));
    }

    private void OnStartDecresingArea(int time)
    {
        ShowAreaInfoText("WARNING! RESTRICTING THE PLAY AREA");
    }

    void World_OnPlayerSpawn(Player player)
    {
        healthArmorUI.Setup(player.characterBase.healthSystem);
        weaponsUI.Setup(player.weaponController);
        mobileInputsUI.Setup(player.weaponController, player.characterBase);
        Player.Ins.characterBase.OnKill += OnKill;
    }

    void OnKill(CharacterBase characterBase, CharacterBase characterBaseKilled, Weapon weapon, HitBox.HitBoxType hitBoxType)
    {
        ShowKilledText("Killed: " + characterBase.killsCount);
        ShowKillsInfoText("YOU kills " + characterBaseKilled.name + " using " + weapon.weaponName);
        UpdateKillsAmount(characterBase.killsCount);
    }

    void ShowAreaInfoText(string text)
    {
        areaInfoText.gameObject.SetActive(true);
        areaInfoText.DOFade(1, .1f);
        areaInfoText.DOFade(0, .5f).SetDelay(5f);
        areaInfoText.text = text;
    }

    void ShowKillsInfoText(string text)
    {
        killsInfoText.gameObject.SetActive(true);
        killsInfoText.DOFade(1, .1f);
        killsInfoText.DOFade(0, .5f).SetDelay(5f);
        killsInfoText.text = text;
    }

    void ShowKilledText(string text)
    {
        killText.gameObject.SetActive(true);
        killText.DOFade(1, .1f);
        killText.DOFade(0, .5f).SetDelay(5f);
        killText.text = text;
    }

    public override void OnCleanUp()
    {
        World.OnPlayerSpawn -= World_OnPlayerSpawn;
        CharacterBase.OnKillStatic -= OnKillStatic;
        CharacterBase.OnDieStatic -= OnDieStatic;
        AreaController.OnStartDecresingArea -= OnStartDecresingArea;
        AreaController.OnNextDecreasingArea -= OnNextDecreasingArea;

        if (Player.Ins != null)
        {
            Player.Ins.characterBase.OnKill -= OnKill;
        }
    }

    void OnKillStatic(CharacterBase characterBase, CharacterBase characterBaseKilled, Weapon weapon, HitBox.HitBoxType hitBoxType)
    {
        killsListUI.AddKillToList(Player.Ins.characterBase, characterBase, characterBaseKilled, weapon.weaponName, hitBoxType);
    }

    void OnDieStatic(CharacterBase characterBase)
    {
        UpdateAlifeAmount(World.Ins.allCharacters.Count);
    }

    void UpdateAlifeAmount(int count)
    {
        alifeAmount.text = string.Format("<color=white>{0}</color> <size=14>ALIFE</size>", count);
    }

    void UpdateKillsAmount(int count)
    {
        killsAmount.text = string.Format("<color=white>{0}</color> <size=14>KILLS</size>", count);
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     OnKillStatic(World.Ins.allCharacters[Random.Range(0, 3)], World.Ins.allCharacters[Random.Range(0, 3)], GameAssets.Get.GetWeapon((GameAssets.WeaponsList)Random.Range(0, 6)));
        // }
    }
}