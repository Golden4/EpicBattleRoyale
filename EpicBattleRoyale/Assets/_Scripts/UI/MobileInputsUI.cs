using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MobileInputsUI : MonoBehaviour {
    public static MobileInputsUI Ins;

    public enum ButtonTypes {
        Reload,
        Health
    }

    public List<TimerButton> timerButtons = new List<TimerButton> ();
    public Text healthCountText;

    public Button pickUpBtn;
    public Button reloadRtn;
    public Button enterDoorBtn;
    public Button goMapBtn;
    AutomaticWeapon aw;
    WeaponController wc;
    CharacterBase characterBase;

    void Awake () {
        Ins = this;
    }

    public void Setup (WeaponController wc, CharacterBase cb) {
        this.wc = wc;
        characterBase = cb;
        GetTimerButton (ButtonTypes.Reload).Hide ();
        GetTimerButton (ButtonTypes.Health).Hide ();
        ShowPickUpBtn (false);
        ShowCanEnterDoorBtn (false);
        ShowCanGoMapBtn (false);
        wc.OnWeaponSwitch += Wc_OnWeaponSwitch;
        cb.characterInteractable.OnCanInteractEvent += OnCanInteract;
        cb.characterInteractable.OnCantInteractEvent += OnCantInteract;
        cb.characterInventory.OnPickUp += OnPickUp;
    }

    public TimerButton GetTimerButton (ButtonTypes types) {
        return timerButtons[(int) types];
    }

    void Update () {
        if (aw != null) {
            if (aw.CanReload ()) {
                if (!GetTimerButton (ButtonTypes.Reload).isShow)
                    GetTimerButton (ButtonTypes.Reload).Show (aw.reloadTime, () => aw.Reload (), null);
            } else if (aw.curState != AutomaticWeapon.State.Reloading) {
                if (GetTimerButton (ButtonTypes.Reload).isShow) {
                    GetTimerButton (ButtonTypes.Reload).Hide ();
                }
            } else if (GetTimerButton (ButtonTypes.Reload).isShow && !GetTimerButton (ButtonTypes.Reload).isUsing) {
                GetTimerButton (ButtonTypes.Reload).Hide ();
            }
        } else if (GetTimerButton (ButtonTypes.Reload).isShow) {
            GetTimerButton (ButtonTypes.Reload).Hide ();
        }

        if (!characterBase.isHealing && GetTimerButton (ButtonTypes.Health).isUsing) {
            GetTimerButton (ButtonTypes.Health).Cancel ();
        }

        for (int i = 0; i < timerButtons.Count; i++) {
            timerButtons[i].OnUpdate ();
        }
    }

    void FixedUpdate () {
        for (int i = 0; i < timerButtons.Count; i++) {
            timerButtons[i].OnFixedUpdate ();
        }
    }

    void OnPickUp (ItemPickUp item) {
        if (item.item == null)
            return;

        HealthItem healthItem = item.item as HealthItem;

        if (healthItem != null) {
            characterBase.characterInventory.items[item.item].OnChangeAmount -= OnChangeHealthCount;
            characterBase.characterInventory.items[item.item].OnChangeAmount += OnChangeHealthCount;
            if (!GetTimerButton (ButtonTypes.Health).isShow)
                GetTimerButton (ButtonTypes.Health).Show (healthItem.usingTime, () => characterBase.Heal (), delegate {
                    characterBase.EndHeal ();
                    healthItem.Use (characterBase);
                });

            OnChangeHealthCount (characterBase.characterInventory.items[item.item].CurCount);
        }
    }

    private void OnChangeHealthCount (int amount) {
        healthCountText.text = amount.ToString ();

        if (amount <= 0)
            if (GetTimerButton (ButtonTypes.Health).isShow)
                GetTimerButton (ButtonTypes.Health).Hide ();
    }

    void OnCanInteract (Interactable interactable) {
        switch (interactable.GetInteractableType ()) {
            case Interactable.InteractableType.ItemPickUp:
                ShowPickUpBtn (true, () => {
                    interactable.Interact (characterBase);
                });
                break;
            case Interactable.InteractableType.HouseDoor:

                ShowCanEnterDoorBtn (true, () => {
                    interactable.Interact (characterBase);
                });
                break;

            case Interactable.InteractableType.MapChange:
                ShowCanGoMapBtn (true, () => {
                    interactable.Interact (characterBase);
                });
                break;
        }
    }

    void OnCantInteract (Interactable interactable) {
        switch (interactable.GetInteractableType ()) {
            case Interactable.InteractableType.ItemPickUp:
                if (characterBase.characterInventory.canPickUpItems.Count == 0)
                    ShowPickUpBtn (false);
                break;
            case Interactable.InteractableType.HouseDoor:
                ShowCanEnterDoorBtn (false);
                break;

            case Interactable.InteractableType.MapChange:
                ShowCanGoMapBtn (false);
                break;
        }
    }

    public void ShowPickUpBtn (bool show, Action onClick = null) {
        if (show) {
            pickUpBtn.transform.DOScale (Vector3.one, .1f);
            //pickUpBtn.gameObject.SetActive(true);
            pickUpBtn.onClick.RemoveAllListeners ();
            pickUpBtn.onClick.AddListener (() => onClick ());
        } else {

            pickUpBtn.transform.DOScale (Vector3.zero, .21f);
        }
    }

    public void ShowCanEnterDoorBtn (bool show, Action onClick = null) {
        if (show) {
            enterDoorBtn.transform.DOScale (Vector3.one, .1f);
            enterDoorBtn.onClick.RemoveAllListeners ();
            enterDoorBtn.onClick.AddListener (() => onClick ());
        } else if (enterDoorBtn != null)
            enterDoorBtn.transform.DOScale (Vector3.zero, .1f);
    }

    public void ShowCanGoMapBtn (bool show, Action onClick = null) {
        if (show) {
            goMapBtn.transform.DOScale (Vector3.one, .1f);
            goMapBtn.onClick.RemoveAllListeners ();
            goMapBtn.onClick.AddListener (() => onClick ());
        } else {

            if (goMapBtn != null)
                goMapBtn.transform.DOScale (Vector3.zero, .1f);
        }
    }

    void HandlerReloadProgressAndBtn () {

        // if (aw != null)
        // {
        //     if (aw.CanReload())
        //     {
        //         ShowReloadBtn(() => aw.Reload());
        //         HideReloadTimeProgress();
        //     }
        //     else if (aw.reloadingProgress > 0)
        //     {
        //         if (aw.curState == AutomaticWeapon.State.Reloading)
        //         {
        //             SetReloadTimeProgress(aw.reloadingProgress);
        //             ShowReloadBtn(null);
        //         }
        //         else
        //         {
        //             SetReloadTimeProgress(0);
        //             HideReloadTimeProgress();
        //         }
        //     }
        //     else if (aw.reloadingProgress < 0)
        //     {
        //         HideReloadTimeProgress();
        //         HideReloadBtn();
        //     }
        //     else
        //     {
        //         HideReloadTimeProgress();
        //         HideReloadBtn();
        //     }
        // }
        // else
        // {
        //     HideReloadBtn();
        //     HideReloadTimeProgress();
        // }
    }

    void Wc_OnWeaponSwitch (object sender, EventArgs e) {
        if (wc.GetCurrentWeapon ().WeaponIs (typeof (AutomaticWeapon))) {
            aw = (AutomaticWeapon) wc.GetCurrentWeapon ();
        } else {
            aw = null;
        }
    }

    [System.Serializable]
    public class TimerButton {
        [SerializeField]
        Button button;
        [SerializeField]
        Image imageProgress;
        [SerializeField]
        Text textTimer;

        float usingTime;

        float curTimer;

        Action onEndUseEvent;
        Action onClickEvent;
        public bool isUsing;
        public bool isShow = false;

        public void Show (float usingTime, Action onClickUse, Action onEndUse) {
            button.transform.DOScale (Vector3.one, .21f);
            imageProgress.gameObject.SetActive (false);
            textTimer.gameObject.SetActive (false);
            button.onClick.RemoveAllListeners ();

            button.onClick.AddListener (delegate {
                Use (usingTime);
            });

            onEndUseEvent = onEndUse;
            onClickEvent = onClickUse;
            isShow = true;
        }

        public void Hide () {
            isShow = false;
            button.transform.DOScale (Vector3.zero, .21f);
            Cancel ();
        }

        public void Cancel () {
            imageProgress.gameObject.SetActive (false);
            textTimer.gameObject.SetActive (false);
            isUsing = false;
            curTimer = 0;
        }

        void Use (float time) {
            imageProgress.gameObject.SetActive (true);
            textTimer.gameObject.SetActive (true);
            curTimer = time;
            usingTime = time;
            SetTimeProgress (time);
            isUsing = true;

            if (onClickEvent != null)
                onClickEvent ();
        }

        void OnEndUse () {
            if (onEndUseEvent != null)
                onEndUseEvent ();
        }

        void SetTimeProgress (float progress) {
            textTimer.text = string.Format ("{0}s", progress.ToString ("0.0"));
            imageProgress.fillAmount = progress / usingTime;
        }

        public void OnFixedUpdate () {
            if (isUsing) {
                if (curTimer > 0) {
                    curTimer -= Time.deltaTime;
                } else if (curTimer <= 0) {
                    Cancel ();
                    OnEndUse ();
                }
            }
        }

        public void OnUpdate () {
            if (isUsing) {
                SetTimeProgress (curTimer);
            }
        }
    }

}