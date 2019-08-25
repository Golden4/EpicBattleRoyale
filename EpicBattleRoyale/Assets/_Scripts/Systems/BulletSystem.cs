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

    public event EventHandler OnBulletsChange;
    AmmoItem ammoItem;

    public void Setup(CharacterInventory characterInventory)
    {
        this.characterInventory = characterInventory;
        FindAmmoItem();
        characterInventory.OnAddItem += OnAddItem;
    }

    void OnAddItem(Item item)
    {
        if (item.GetType() == typeof(AmmoItem))
            FindAmmoItem();
    }

    void FindAmmoItem()
    {
        if (ammoItem == null)
            ammoItem = characterInventory.GetAmmoItem(ammoType);

        if (ammoItem != null)
        {
            ammoItem.OnChangeAmount += OnAddAmount;
        }

    }

    void OnAddAmount(int amount)
    {
        if (OnBulletsChange != null)
            OnBulletsChange(this, EventArgs.Empty);
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
        // if (curBulletsStock == maxBulletsStock)
        //     return false;

        curBullets += bulletsAmount;

        if (curBullets > maxBullets)
            curBullets = maxBullets;
        FindAmmoItem();

        if (OnBulletsChange != null)
            OnBulletsChange(this, EventArgs.Empty);

        return true;
    }

    // public bool GiveBulletsStock(int stockBulletsAmount)
    // {
    //     // if (curBulletsStock == maxBulletsStock)
    //     //     return false;

    //     curBulletsStock += stockBulletsAmount;

    //     // if (curBulletsStock > maxBulletsStock)
    //     //     curBulletsStock = maxBulletsStock;

    //     if (OnBulletsChange != null)
    //         OnBulletsChange(this, EventArgs.Empty);

    //     return true;
    // }

    public void SetBullets(int bulletsAmount)
    {
        curBullets = bulletsAmount;

        curBullets = Mathf.Clamp(curBullets, 0, maxBullets);
        FindAmmoItem();

        if (OnBulletsChange != null)
            OnBulletsChange(this, EventArgs.Empty);
    }

    // public void SetBulletsStock(int stockBulletsAmount)
    // {

    //     curBulletsStock = stockBulletsAmount;

    //     // curBulletsStock = Mathf.Clamp(curBulletsStock, 0, maxBulletsStock);

    //     if (OnBulletsChange != null)
    //         OnBulletsChange(this, EventArgs.Empty);
    // }

    public bool ShotBullet(int bulletCount)
    {
        if (curBullets > 0)
        {

            curBullets -= bulletCount;

            if (curBullets < 0)
                curBullets = 0;

            if (OnBulletsChange != null)
                OnBulletsChange(this, EventArgs.Empty);
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

        AmmoItem ammoItem = characterInventory.GetAmmoItem(ammoType);

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
        // curBulletsStock = Mathf.Clamp(curBulletsStock, 0, maxBulletsStock);

        if (OnBulletsChange != null)
            OnBulletsChange(this, EventArgs.Empty);
        return true;
    }

    public bool NoBullets()
    {
        return GetCurrentBullets() == 0 && GetCurBulletsStock() == 0;
    }

    /*	public enum BulletType {
		Automatic,
		Sniper,
		Pistol
	}

	public BulletType bulletType;

	public BulletType GetBulletType ()
	{
		return bulletType;
	}*/
}