using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MobileInputsUI : MonoBehaviour
{
    public Button pickUpBtn;
    public Button reloadRtn;
    public Button enterDoorBtn;
    public Image reloadImageProgress;
    AutomaticWeapon aw;
    WeaponController wc;
    CharacterBase characterBase;

    public void Setup(WeaponController wc, CharacterBase cb)
    {
        this.wc = wc;
        characterBase = cb;
        HidePickUpBtn();
        HideReloadBtn();
        HideReloadTimeProgress();
        wc.OnWeaponSwitch += Wc_OnWeaponSwitch;
        cb.inventorySystem.OnCanPickUp += CharacterBase_CanPickUpItem;
        cb.inventorySystem.OnCantPickUp += CharacterBase_CantPickUpItem;
    }

    void Update()
    {
        HandlerReloadProgressAndBtn();
    }

    public void ShowPickUpBtn(Action onClick)
    {
        pickUpBtn.gameObject.SetActive(true);
        pickUpBtn.onClick.RemoveAllListeners();
        pickUpBtn.onClick.AddListener(() => onClick());
    }

    public void HidePickUpBtn()
    {
        pickUpBtn.gameObject.SetActive(false);
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
        reloadImageProgress.fillAmount = progress / aw.reloadTime;
    }

    public void HideReloadTimeProgress()
    {
        reloadImageProgress.gameObject.SetActive(false);
    }

    public void ShowEnterDoorBtn()
    {
        enterDoorBtn.gameObject.SetActive(true);
    }

    public void HideEnterDoorBtn()
    {
        enterDoorBtn.gameObject.SetActive(false);
    }

    void CharacterBase_CanPickUpItem(object obj, EventArgs args)
    {
        ShowPickUpBtn(() =>
        {
            Debug.Log("ShowPickUpBtn");
            characterBase.inventorySystem.PickUp();
        });
    }

    void CharacterBase_CantPickUpItem(object obj, EventArgs args)
    {
        HidePickUpBtn();
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
