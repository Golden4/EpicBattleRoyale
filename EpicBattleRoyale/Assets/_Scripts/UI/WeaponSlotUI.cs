﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    public Sprite defaultSprite;
    public Image weaponSpriteImage;
    public Text bulletsText;
    public bool isActive;
    public Button btn;
    BulletSystem bulletSystem;

    void BulletSystem_OnBulletsChange(object sender, System.EventArgs e)
    {
        UpdateBullets(bulletSystem.GetCurrentBullets(), bulletSystem.GetCurBulletsStock());
    }

    public void Setup(bool enabled, bool isActive = false, System.Action onClick = null, Sprite sprite = null, BulletSystem bulletSystem = null)
    {
        if (!enabled)
        {
            weaponSpriteImage.sprite = defaultSprite;

            SetSpritesColor(.1f);

            if (bulletsText != null)
            {
                bulletsText.gameObject.SetActive(false);
            }

            return;
        }
        else
        {
            gameObject.SetActive(true);
        }

        if (bulletSystem != null)
        {
            this.bulletSystem = bulletSystem;
            UpdateBullets(bulletSystem.GetCurrentBullets(), bulletSystem.GetCurBulletsStock());
            bulletSystem.OnBulletsChange -= BulletSystem_OnBulletsChange;
            bulletSystem.OnBulletsChange += BulletSystem_OnBulletsChange;
        }
        else
        {
            if (bulletsText != null)
            {
                bulletsText.gameObject.SetActive(false);
            }
        }

        if (sprite != null && weaponSpriteImage != null)
        {
            weaponSpriteImage.sprite = sprite;
        }

        btn.onClick.RemoveAllListeners();

        if (!isActive)
        {
            SetSpritesColor(.5f);

            btn.onClick.AddListener(() => onClick());

        }
        else
        {
            SetSpritesColor(1);
        }
    }

    void SetSpritesColor(float alpha)
    {
        Color color = weaponSpriteImage.color;
        color.a = alpha;
        weaponSpriteImage.color = color;

        if (bulletsText != null)
        {
            color = bulletsText.color;
            color.a = alpha;
            bulletsText.color = color;
        }
    }

    public void UpdateBullets(int bullets = -1, int bulletsStock = -1)
    {
        bulletsText.gameObject.SetActive(true);
        if ((bullets + bulletsStock) == 0)
            bulletsText.color = Color.red;
        else
            bulletsText.color = Color.white;
        bulletsText.text = "<size=10>" + bullets + "</size>/" + bulletsStock;
    }

}
