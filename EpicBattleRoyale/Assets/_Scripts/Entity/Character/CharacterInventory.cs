using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInventory : MonoBehaviour
{
    public CharacterBase characterBase;
    public List<ItemPickUp> canPickUpItems = new List<ItemPickUp>();
    //public Dictionary<Item, int> items = new Dictionary<Item, int>();
    public List<Item> items = new List<Item>();

    public event Action<ItemPickUp> OnPickUp;

    public event Action<Item> OnAddItem;

    void Awake()
    {
        characterBase = GetComponent<CharacterBase>();
    }

    public void OnCharacterPickUp(ItemPickUp item)
    {
        if (OnPickUp != null)
            OnPickUp(item);
    }

    public void CanPickUpItem(ItemPickUp item)
    {

        if (!canPickUpItems.Contains(item))
            canPickUpItems.Add(item);
    }

    public void CanT_PickUpItem(ItemPickUp item)
    {

        if (canPickUpItems.Contains(item))
            canPickUpItems.Remove(item);

    }

    public void PickUp()
    {
        for (int i = 0; i < canPickUpItems.Count; i++)
        {
            if (canPickUpItems[i] == null)
            {
                canPickUpItems.Remove(canPickUpItems[i]);
                i--;
                continue;
            }

            if (characterBase.CanPickUp() && canPickUpItems[i].PickUp(characterBase, true))
            {
                canPickUpItems[i].DestroyItem();
                break;
            }
        }
    }

    public void AddItem(Item item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);
            item.OnAddInventory();
        }
        else
        {
            item.OnAddInventory();
        }

        if (OnAddItem != null)
            OnAddItem(item);
    }

    // public int GetItemCount(Item item)
    // {
    //     return items[item];
    // }

    public void Remove(Item item)
    {
        items.Remove(item);
    }

    public AmmoItem GetAmmoItem(GameAssets.PickUpItemsData.AmmoList ammoType)
    {
        for (int i = 0; i < items.Count; i++)
        {
            AmmoItem ammoItem = items[i] as AmmoItem;

            if (ammoItem != null)
            {
                if (ammoItem.bulletType == ammoType)
                {
                    return ammoItem;
                }
            }
        }

        return null;
    }
}
