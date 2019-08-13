using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class MobileInputsUI : MonoBehaviour
{
    public static MobileInputsUI Ins;
    public Button pickUpBtn;
    public Button reloadRtn;
    public Button enterDoorBtn;
    public Button goMapBtn;
    public Image reloadImageProgress;
    public Text reloadTextProgress;
    AutomaticWeapon aw;
    WeaponController wc;
    CharacterBase characterBase;

    void Awake()
    {
        Ins = this;
    }
    public void Setup(WeaponController wc, CharacterBase cb)
    {
        this.wc = wc;
        characterBase = cb;
        HidePickUpBtn();
        HideReloadBtn();
        HideReloadTimeProgress();
        HideCanEnterDoorBtn();
        HideCanGoMapBtn();
        wc.OnWeaponSwitch += Wc_OnWeaponSwitch;
        cb.inventorySystem.OnCanPickUp += CharacterBase_CanPickUpItem;
        cb.inventorySystem.OnCantPickUp += CharacterBase_CantPickUpItem;
        cb.CanEnterDoorEvent += CharacterBase_CanEnterDoor;
        cb.AwayDoorEvent += CharacterBase_CantEnterDoor;
    }

    void Update()
    {
        HandlerReloadProgressAndBtn();
    }

    public void ShowPickUpBtn(Action onClick)
    {
        pickUpBtn.transform.DOScale(Vector3.one, .1f);
        //pickUpBtn.gameObject.SetActive(true);
        pickUpBtn.onClick.RemoveAllListeners();
        pickUpBtn.onClick.AddListener(() => onClick());
    }

    public void HidePickUpBtn()
    {
        pickUpBtn.transform.DOScale(Vector3.zero, .21f);
        //pickUpBtn.gameObject.SetActive(false);
    }

    public void ShowReloadBtn(Action onClick)
    {
        if (reloadRtn.gameObject.activeSelf)
            return;
        reloadImageProgress.gameObject.SetActive(false);
        reloadRtn.gameObject.SetActive(true);
        reloadRtn.onClick.RemoveAllListeners();
        reloadRtn.onClick.AddListener(() => onClick());
    }

    public void HideReloadBtn()
    {
        if (!reloadRtn.gameObject.activeSelf)
            return;
        reloadRtn.gameObject.SetActive(false);
    }

    public void SetReloadTimeProgress(float progress)
    {
        reloadImageProgress.gameObject.SetActive(true);
        reloadTextProgress.text = string.Format("{0}s", progress.ToString("0.0"));
        reloadImageProgress.fillAmount = progress / aw.reloadTime;
    }

    public void HideReloadTimeProgress()
    {
        reloadImageProgress.gameObject.SetActive(false);
    }

    void CharacterBase_CanPickUpItem(object obj, EventArgs args)
    {
        ShowPickUpBtn(() =>
        {
            characterBase.inventorySystem.PickUp();
        });
    }

    void CharacterBase_CantPickUpItem(object obj, EventArgs args)
    {
        HidePickUpBtn();
    }

    public void ShowCanEnterDoorBtn(Action onClick)
    {
        enterDoorBtn.transform.DOScale(Vector3.one, .1f);
        enterDoorBtn.onClick.RemoveAllListeners();
        enterDoorBtn.onClick.AddListener(() => onClick());
    }

    public void HideCanEnterDoorBtn()
    {
        if (enterDoorBtn != null)
            enterDoorBtn.transform.DOScale(Vector3.zero, .1f);
    }

    public void ShowCanGoMapBtn(Action onClick)
    {
        goMapBtn.transform.DOScale(Vector3.one, .1f);
        goMapBtn.onClick.RemoveAllListeners();
        goMapBtn.onClick.AddListener(() => onClick());
    }

    public void HideCanGoMapBtn()
    {
        if (goMapBtn != null)
            goMapBtn.transform.DOScale(Vector3.zero, .1f);
    }

    void CharacterBase_CanEnterDoor(object obj, EventArgs args)
    {
        ShowCanEnterDoorBtn(() =>
        {
            characterBase.EnterOrExitDoor();
        });
    }

    void CharacterBase_CantEnterDoor(object obj, EventArgs args)
    {
        HideCanEnterDoorBtn();
    }

    void HandlerReloadProgressAndBtn()
    {
        if (aw != null)
        {
            if (aw.CanReload())
            {
                ShowReloadBtn(() => aw.Reload());
                HideReloadTimeProgress();
            }
            else if (aw.reloadingProgress > 0)
            {
                if (aw.curState == AutomaticWeapon.State.Reloading)
                {
                    SetReloadTimeProgress(aw.reloadingProgress);
                    ShowReloadBtn(null);
                }
                else
                {
                    SetReloadTimeProgress(0);
                    HideReloadTimeProgress();
                }
            }
            else if (aw.reloadingProgress < 0)
            {
                HideReloadTimeProgress();
                HideReloadBtn();
            }
            else
            {
                HideReloadTimeProgress();
                HideReloadBtn();
            }

        }
        else
        {
            HideReloadBtn();
            HideReloadTimeProgress();
        }
    }

    void Wc_OnWeaponSwitch(object sender, EventArgs e)
    {
        if (wc.GetCurrentWeapon().WeaponIs(typeof(AutomaticWeapon)))
        {
            aw = (AutomaticWeapon)wc.GetCurrentWeapon();
            aw.OnReload -= Aw_OnReload;
            aw.OnReload += Aw_OnReload;
        }
        else
        {
            aw = null;
        }

    }

    void Aw_OnReload(float reloadTime)
    {
        reloadImageProgress.gameObject.SetActive(true);
    }

}
