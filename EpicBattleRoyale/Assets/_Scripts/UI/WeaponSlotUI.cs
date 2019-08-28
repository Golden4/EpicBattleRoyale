using System.Collections;
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
    public Text bulletType;
    BulletSystem bulletSystem;
    Material fillableMaterial;

    void BulletSystem_OnBulletsChange(int bullets)
    {
        UpdateBullets(bulletSystem.GetCurrentBullets(), bulletSystem.GetCurBulletsStock());
    }

    public void Setup(bool enabled, bool isActive = false, System.Action onClick = null, Sprite sprite = null, BulletSystem bulletSystem = null)
    {
        if (fillableMaterial == null)
            fillableMaterial = Resources.Load<Material>("Materials/FillableSpriteMaterial");

        if (!enabled)
        {
            weaponSpriteImage.sprite = defaultSprite;

            SetSpritesAlpha(.1f);

            if (bulletsText != null)
            {
                bulletsText.gameObject.SetActive(false);
            }
            if (bulletType != null)
                bulletType.gameObject.SetActive(false);

            return;
        }
        else
        {
            gameObject.SetActive(true);
        }


        if (sprite != null && weaponSpriteImage != null)
        {
            weaponSpriteImage.sprite = sprite;
            Material mat = new Material(fillableMaterial);
            weaponSpriteImage.material = mat;
            weaponSpriteImage.material.SetFloat("_FillAlpha", 1);
        }

        if (bulletSystem != null)
        {
            this.bulletSystem = bulletSystem;
            UpdateBullets(bulletSystem.GetCurrentBullets(), bulletSystem.GetCurBulletsStock());

            bulletType.gameObject.SetActive(true);

            switch (bulletSystem.ammoType)
            {
                case GameAssets.PickUpItemsData.AmmoList.Ammo762mm:
                    bulletType.text = "7.62mm";
                    break;
                case GameAssets.PickUpItemsData.AmmoList.Ammo9mm:
                    bulletType.text = "9mm";
                    break;
                case GameAssets.PickUpItemsData.AmmoList.Ammo12Gauge:
                    bulletType.text = "12 Gauge";
                    break;
                case GameAssets.PickUpItemsData.AmmoList.Ammo300Magnum:
                    bulletType.text = ".300 Magnum";
                    break;
                case GameAssets.PickUpItemsData.AmmoList.Ammo556mm:
                    bulletType.text = "5.56mm";
                    break;
                default:
                    break;
            }

            bulletSystem.OnBulletsChange -= BulletSystem_OnBulletsChange;
            bulletSystem.OnBulletsChange += BulletSystem_OnBulletsChange;
        }
        else
        {
            if (bulletsText != null)
            {
                bulletsText.gameObject.SetActive(false);

            }
            if (bulletType != null)
                bulletType.gameObject.SetActive(false);
        }


        btn.onClick.RemoveAllListeners();

        if (!isActive)
        {
            SetSpritesAlpha(.5f);

            btn.onClick.AddListener(() => onClick());

        }
        else
        {
            SetSpritesAlpha(1);
        }
    }

    void SetSpritesAlpha(float alpha)
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
        {
            bulletsText.color = Color.red;
            weaponSpriteImage.material.SetColor("_FillColor", Color.red);
        }
        else
        {
            bulletsText.color = Color.white;
            weaponSpriteImage.material.SetColor("_FillColor", Color.white);
        }
        bulletsText.text = "<size=10>" + bullets + "</size>/" + bulletsStock;
    }

}
