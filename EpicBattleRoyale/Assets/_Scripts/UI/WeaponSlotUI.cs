﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour {

	public Image weaponSpriteImage;
	public Text bulletsText;
	public bool isActive;
	public Button btn;
	public int curSlotIndex;
	public Weapon curWeapon;
	BulletSystem bulletSystem;

	public void Init (int slotIndex)
	{
		curSlotIndex = slotIndex;
	}

	void BulletSystem_OnBulletsChange (object sender, System.EventArgs e)
	{
		UpdateBullets (bulletSystem.GetCurrentBullets (), bulletSystem.GetCurBulletsStock ());
	}

	public void Setup (bool enabled, bool isActive = false, System.Action onClick = null, Sprite sprite = null, BulletSystem bulletSystem = null)
	{
		if (!enabled) {
			gameObject.SetActive (false);
			return;
		} else {
			gameObject.SetActive (true);
		}

		if (bulletSystem != null) {
			this.bulletSystem = bulletSystem;
			UpdateBullets (bulletSystem.GetCurrentBullets (), bulletSystem.GetCurBulletsStock ());
			bulletSystem.OnBulletsChange -= BulletSystem_OnBulletsChange;
			bulletSystem.OnBulletsChange += BulletSystem_OnBulletsChange;
		} else {
			
			bulletsText.gameObject.SetActive (false);
		}

		if (sprite != null) {
			weaponSpriteImage.sprite = sprite;
		}

		btn.onClick.RemoveAllListeners ();
		if (!isActive) {
			Color color = weaponSpriteImage.color;
			color.a = .5f;
			weaponSpriteImage.color = color;

			color = bulletsText.color;
			color.a = .5f;
			bulletsText.color = color;

			btn.onClick.AddListener (() => onClick ());

		} else {
			Color color = weaponSpriteImage.color;
			color.a = 1;
			weaponSpriteImage.color = color;

			color = bulletsText.color;
			color.a = 1;
			bulletsText.color = color;
		}
	}

	public void UpdateBullets (int bullets = -1, int bulletsStock = -1)
	{
		bulletsText.gameObject.SetActive (true);
		bulletsText.text = bullets + "/" + bulletsStock;
	}

}