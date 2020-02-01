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

    public Image areaOutImage;

    public override void OnInit()
    {
        World.OnPlayerSpawn += World_OnPlayerSpawn;
        CharacterBase.OnKillStatic += OnKillStatic;
        CharacterBase.OnDieStatic += OnDieStatic;

        AreaController.OnStartDecresingArea += OnStartDecresingArea;
        AreaController.OnNextDecreasingArea += OnNextDecreasingArea;
    }


    public override void OnShow()
    {
        base.OnShow();
        UpdateAlifeAmount(GameController.CHARACTERS_COUNT_MAX);
        UpdateKillsAmount(0);
    }

    void World_OnPlayerSpawn(Player player)
    {
        healthArmorUI.Setup(player.characterBase.healthSystem);
        weaponsUI.Setup(player.weaponController);
        mobileInputsUI.Setup(player.weaponController, player.characterBase);
        player.characterBase.OnKill += OnKill;
        player.characterBase.OnIsOnArea += OnIsOnArea;
        player.characterBase.OnOutOfArea += OnOutOfArea;
    }

    private void OnNextDecreasingArea(int time)
    {
        ShowAreaInfoText("RESTRICTING THE PLAY AREA IN " + AreaController.TicksToTime(time));
    }

    private void OnStartDecresingArea(int time)
    {
        ShowAreaInfoText("WARNING! RESTRICTING THE PLAY AREA");
    }

    private void OnOutOfArea(CharacterBase characterBase)
    {
        if (areaOutImage.gameObject.activeInHierarchy)
            areaOutImage.DOFade(0, .3f).OnComplete(delegate
            {
                areaOutImage.gameObject.SetActive(false);
            });
    }

    private void OnIsOnArea(CharacterBase characterBase)
    {
        areaOutImage.gameObject.SetActive(true);
        areaOutImage.DOFade(.2f, .3f);
    }

    void OnKill(CharacterBase characterBase, CharacterBase characterBaseKilled, Weapon weapon, HitBox.HitBoxType hitBoxType)
    {
        if (characterBase == null || characterBaseKilled == null)
            return;

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
            Player.Ins.characterBase.OnIsOnArea -= OnIsOnArea;
            Player.Ins.characterBase.OnOutOfArea -= OnOutOfArea;
        }
    }

    void OnKillStatic(CharacterBase characterBase, CharacterBase characterBaseKilled, Weapon weapon, HitBox.HitBoxType hitBoxType)
    {
        killsListUI.AddKillToList(Player.Ins.characterBase, characterBase, characterBaseKilled, weapon, hitBoxType);
    }

    void OnDieStatic(LivingEntity characterBase)
    {
        if ((characterBase as CharacterBase) != null)
            UpdateAlifeAmount(World.Ins.GetCurrentEntityCount());
    }

    void UpdateAlifeAmount(int count)
    {
        alifeAmount.text = string.Format("<color=white>{0}</color> <size=14>ALIFE</size>", count);
    }

    void UpdateKillsAmount(int count)
    {
        killsAmount.text = string.Format("<color=white>{0}</color> <size=14>KILLS</size>", count);
    }

}