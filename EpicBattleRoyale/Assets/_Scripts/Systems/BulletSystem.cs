using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BulletSystem
{
    public int curBullets = 0;
    // public int curBulletsStock = 0;

    public int maxBullets;
    // public int maxBulletsStock;

    public GameAssets.PickUpItemsData.AmmoList ammoType;
    public CharacterInventory characterInventory;

    public event Action<int> OnBulletsChange;
    CharacterInventory.InventoryItem ammoItem;

    public void Setup(CharacterInventory characterInventory)
    {
        this.characterInventory = characterInventory;

        if (!FindAmmoItem())
        {
            characterInventory.OnPickUp += OnPickUp;
        }
    }

    void OnPickUp(ItemPickUp item)
    {
        if (FindAmmoItem())
        {
            characterInventory.OnPickUp -= OnPickUp;
        }
    }

    bool FindAmmoItem()
    {
        if (ammoItem == null)
        {
            ammoItem = characterInventory.GetAmmoItem(ammoType);

            if (ammoItem != null)
            {
                ammoItem.OnChangeAmount += OnChangeAmount;
                OnChangeAmount(curBullets);
                return true;
            }
        }

        return false;
    }

    void OnChangeAmount(int amount)
    {
        if (OnBulletsChange != null)
            OnBulletsChange(curBullets);
    }

    public int GetCurrentBullets()
    {
        return curBullets;
    }

    public int GetCurBulletsStock()
    {
        if (ammoItem != null)
            return ammoItem.CurCount;

        return 0;
    }

    public bool GiveBullets(int bulletsAmount)
    {
        curBullets += bulletsAmount;

        if (curBullets > maxBullets)
            curBullets = maxBullets;
        FindAmmoItem();

        if (OnBulletsChange != null)
            OnBulletsChange(curBullets);

        return true;
    }

    public void SetBullets(int bulletsAmount)
    {
        curBullets = bulletsAmount;

        curBullets = Mathf.Clamp(curBullets, 0, maxBullets);
        FindAmmoItem();

        if (OnBulletsChange != null)
            OnBulletsChange(curBullets);
    }

    public bool ShotBullet(int bulletCount)
    {
        if (curBullets > 0)
        {

            curBullets -= bulletCount;

            if (curBullets < 0)
                curBullets = 0;

            if (OnBulletsChange != null)
                OnBulletsChange(curBullets);
            return true;
        }
        else
        {

            return false;
        }
    }

    public bool CanReload()
    {
        return curBullets >= 0 && GetCurBulletsStock() > 0 && curBullets != maxBullets;
    }

    public bool ReloadBullets()
    {

        if (GetCurBulletsStock() == 0)
            return false;

        CharacterInventory.InventoryItem ammoItem = characterInventory.GetAmmoItem(ammoType);

        if (GetCurBulletsStock() < maxBullets - curBullets)
        {
            curBullets += GetCurBulletsStock();
            if (ammoItem != null)
                ammoItem.CurCount = 0;
        }
        else
        {
            if (ammoItem != null)
                ammoItem.CurCount -= maxBullets - curBullets;

            curBullets += maxBullets - curBullets;
        }

        curBullets = Mathf.Clamp(curBullets, 0, maxBullets);

        if (OnBulletsChange != null)
            OnBulletsChange(curBullets);
        return true;
    }

    public bool NoBullets()
    {
        return GetCurrentBullets() == 0 && GetCurBulletsStock() == 0;
    }
}