using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MobileInputsUI : MonoBehaviour {
	
	public Button pickUpBtn;
	public Button reloadRtn;
	public Image reloadImageProgress;
	WeaponController wc;

	public void ShowPickUpBtn (Action onClick)
	{
		pickUpBtn.gameObject.SetActive (true);
		pickUpBtn.onClick.RemoveAllListeners ();
		pickUpBtn.onClick.AddListener (() => onClick ());
	}

	public void HidePickUpBtn ()
	{
		pickUpBtn.gameObject.SetActive (false);
	}

	public void ShowReloadBtn (Action onClick)
	{
		if (reloadRtn.gameObject.activeSelf)
			return;
		reloadImageProgress.gameObject.SetActive (false);
		reloadRtn.gameObject.SetActive (true);
		reloadRtn.onClick.RemoveAllListeners ();
		reloadRtn.onClick.AddListener (() => onClick ());
	}

	public void HideReloadBtn ()
	{
		if (!reloadRtn.gameObject.activeSelf)
			return;
		reloadRtn.gameObject.SetActive (false);
	}


	public void SetReloadTimeProgress ()
	{
		reloadImageProgress.gameObject.SetActive (true);
		reloadImageProgress.fillAmount = reloadProgress / reloadTime;
	}

	public void HideReloadTimeProgress ()
	{
		reloadImageProgress.gameObject.SetActive (false);
	}

	public void Setup (WeaponController wc)
	{
		this.wc = wc;
		HidePickUpBtn ();
		HideReloadBtn ();
		HideReloadTimeProgress ();
		wc.OnWeaponSwitch += Wc_OnWeaponSwitch;

	}

	AutomaticWeapon aw;
	float reloadProgress;
	float reloadTime;

	void Update ()
	{
		if (aw != null) {
			if (aw.CanReload ()) {
				ShowReloadBtn (() => aw.Reload ());
			} else {
				HideReloadBtn ();
			}

			if (reloadProgress > 0) {
				Debug.Log (reloadProgress);
				reloadProgress -= Time.deltaTime;
				SetReloadTimeProgress ();
			} else if (reloadProgress < 0) {
					reloadProgress = 0;
					HideReloadTimeProgress ();
				}
		}
	}

	void Wc_OnWeaponSwitch (object sender, EventArgs e)
	{
		if (wc.GetCurrentWeapon ().GetType () == typeof(AutomaticWeapon)) {
			aw = (AutomaticWeapon)wc.GetCurrentWeapon ();
			aw.OnReload -= Aw_OnReload;
			aw.OnReload += Aw_OnReload;
		} else {
			aw = null;
		}

		reloadProgress = -1;
	}

	void Aw_OnReload (float reloadTime)
	{
		reloadImageProgress.gameObject.SetActive (true);
		reloadProgress = reloadTime;
		this.reloadTime = reloadTime;
	}

}
