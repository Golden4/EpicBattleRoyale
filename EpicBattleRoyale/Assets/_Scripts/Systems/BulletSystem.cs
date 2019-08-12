using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BulletSystem
{
    public int curBullets = 0;
    public int curBulletsStock = 0;

    public int maxBullets;
    public int maxBulletsStock;

    public GameAssets.PickUpItemsData.AmmoList ammoType;

    public event EventHandler OnBulletsChange;

    public int GetCurrentBullets()
    {
        return curBullets;
    }

    public int GetCurBulletsStock()
    {
        return curBulletsStock;
    }

    public bool GiveBullets(int bulletsAmount)
    {
        if (curBulletsStock == maxBulletsStock)
            return false;

        curBullets += bulletsAmount;

        if (curBullets > maxBullets)
            curBullets = maxBullets;

        if (OnBulletsChange != null)
            OnBulletsChange(this, EventArgs.Empty);

        return true;
    }

    public bool GiveBulletsStock(int stockBulletsAmount)
    {
        if (curBulletsStock == maxBulletsStock)
            return false;

        curBulletsStock += stockBulletsAmount;

        if (curBulletsStock > maxBulletsStock)
            curBulletsStock = maxBulletsStock;

        if (OnBulletsChange != null)
            OnBulletsChange(this, EventArgs.Empty);

        return true;
    }

    public void SetBullets(int bulletsAmount)
    {
        curBullets = bulletsAmount;

        curBullets = Mathf.Clamp(curBullets, 0, maxBullets);

        if (OnBulletsChange != null)
            OnBulletsChange(this, EventArgs.Empty);
    }

    public void SetBulletsStock(int stockBulletsAmount)
    {

        curBulletsStock = stockBulletsAmount;

        curBulletsStock = Mathf.Clamp(curBulletsStock, 0, maxBulletsStock);

        if (OnBulletsChange != null)
            OnBulletsChange(this, EventArgs.Empty);
    }

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
        return curBullets >= 0 && curBulletsStock > 0 && curBullets != maxBullets;
    }

    public bool ReloadBullets()
    {

        if (curBulletsStock == 0)
            return false;

        if (curBulletsStock < maxBullets - curBullets)
        {
            curBullets += curBulletsStock;
            curBulletsStock = 0;
        }
        else
        {
            curBulletsStock -= maxBullets - curBullets;
            curBullets += maxBullets - curBullets;
        }

        curBullets = Mathf.Clamp(curBullets, 0, maxBullets);
        curBulletsStock = Mathf.Clamp(curBulletsStock, 0, maxBulletsStock);

        if (OnBulletsChange != null)
            OnBulletsChange(this, EventArgs.Empty);
        return true;
    }

    public BulletSystem(BulletSystem bs)
    {
        this.curBullets = bs.curBullets;
        this.curBulletsStock = bs.curBulletsStock;
        this.maxBullets = bs.maxBullets;
        this.maxBulletsStock = bs.maxBulletsStock;
    }

    public override string ToString()
    {
        return string.Format("[BulletSystem: curBullets={0}, curBulletsStock={1}, maxBullets={2}, maxBulletsStock={3}]", curBullets, curBulletsStock, maxBullets, maxBulletsStock);
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