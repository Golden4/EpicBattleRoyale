using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class InventorySystem
{
    public CharacterBase characterBase;
    public List<ItemPickUp> canPickUpItems = new List<ItemPickUp>();
    public List<ItemPickUp> pickedUpItems = new List<ItemPickUp>();
    public List<Bullets> bullets = new List<Bullets>();

    public event EventHandler OnPickUp;

    public void OnCharacterPickUp(ItemPickUp item)
    {
        if (OnPickUp != null)
            OnPickUp(item, EventArgs.Empty);
    }

    public void CanPickUpItem(ItemPickUp item)
    {

        if (!canPickUpItems.Contains(item))
            canPickUpItems.Add(item);

        // if (OnCanPickUp != null)
        // {
        //     OnCanPickUp(this, EventArgs.Empty);
        // }
    }

    public void CanT_PickUpItem(ItemPickUp item)
    {

        if (canPickUpItems.Contains(item))
            canPickUpItems.Remove(item);

        // if (canPickUpItems.Count == 0)
        // {
        //     if (OnCantPickUp != null)
        //     {
        //         OnCantPickUp(this, EventArgs.Empty);
        //     }
        // }
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

        // if (canPickUpItems.Count == 0)
        // {
        //     if (OnCantPickUp != null)
        //     {
        //         OnCantPickUp(this, EventArgs.Empty);
        //     }
        // }
    }

    public InventorySystem(CharacterBase characterBase)
    {
        this.characterBase = characterBase;

        for (int i = 0; i < Enum.GetNames(typeof(Weapon.WeaponType)).Length; i++)
        {

            bullets.Add(new Bullets((Weapon.WeaponType)i, 0));
        }

    }

    public class Bullets
    {
        public Weapon.WeaponType bulletType;
        public int count;

        public Bullets(Weapon.WeaponType bulletType, int count)
        {
            this.bulletType = bulletType;
            this.count = count;

        }
    }

}
