﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    [SerializeField]
    int Count;

    public int CurCount
    {
        get
        {
            return curCount;
        }
        set
        {
            curCount = value;

            if (OnChangeAmount != null)
                OnChangeAmount(curCount);
        }
    }

    int curCount;

    public Action<int> OnChangeAmount;

    public void AddAmount(int amount)
    {
        curCount += amount;
    }

    public bool RemoveAmount(int amount)
    {
        if (amount > curCount)
            return false;

        curCount -= amount;

        return true;
    }

    public virtual void OnAddInventory()
    {
        AddAmount(Count);
    }

    public virtual void OnRemoveInventory()
    {
    }

    public virtual void Use(CharacterBase characterBase)
    {
    }
}