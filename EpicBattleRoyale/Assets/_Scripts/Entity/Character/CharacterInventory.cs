using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInventory : MonoBehaviour {
    public CharacterBase characterBase;
    public List<ItemPickUp> canPickUpItems = new List<ItemPickUp> ();

    public Dictionary<Item, InventoryItem> items = new Dictionary<Item, InventoryItem> ();

    public event Action<ItemPickUp> OnPickUp;

    //  public event Action<InventoryItem> OnAddItem;

    void Awake () {
        characterBase = GetComponent<CharacterBase> ();
    }

    public void OnCharacterPickUp (ItemPickUp item) {
        if (item.item != null)
            AddItem (item.item);

        if (OnPickUp != null)
            OnPickUp (item);
    }

    public void CanPickUpItem (ItemPickUp item) {

        if (!canPickUpItems.Contains (item))
            canPickUpItems.Add (item);
    }

    public void CanT_PickUpItem (ItemPickUp item) {

        if (canPickUpItems.Contains (item))
            canPickUpItems.Remove (item);

    }

    public void PickUp () {
        for (int i = 0; i < canPickUpItems.Count; i++) {
            if (canPickUpItems[i] == null) {
                canPickUpItems.Remove (canPickUpItems[i]);
                i--;
                continue;
            }

            if (characterBase.CanPickUp () && canPickUpItems[i].PickUp (characterBase, true)) {
                characterBase.characterInventory.OnCharacterPickUp (canPickUpItems[i]);
                canPickUpItems[i].DestroyItem ();
                break;
            }
        }
    }

    public void AddItem (Item item) {
        if (!items.ContainsKey (item)) {
            items[item] = new InventoryItem (item, 0);
        }

        items[item].OnAddInventory ();
    }

    public void Remove (Item item) {
        items.Remove (item);
    }

    public InventoryItem GetAmmoItem (GameAssets.PickUpItemsData.AmmoList ammoType) {
        foreach (Item item in items.Keys) {
            AmmoItem ammoItem = item as AmmoItem;

            if (ammoItem != null) {
                if (ammoItem.bulletType == ammoType) {
                    return items[ammoItem];
                }
            }
        }

        return null;
    }

    [System.Serializable]
    public class InventoryItem {
        public Item item;

        public int CurCount {
            get {
                return curCount;
            }
            set {
                curCount = value;

                if (OnChangeAmount != null)
                    OnChangeAmount (curCount);
            }
        }

        public int curCount;

        public Action<int> OnChangeAmount;

        public void AddAmount (int amount) {
            CurCount += amount;
        }

        public bool RemoveAmount (int amount) {
            if (amount > CurCount)
                return false;

            CurCount -= amount;

            return true;
        }

        public virtual void OnAddInventory () {
            AddAmount (item.Count);
        }

        public virtual void OnRemoveInventory () { }

        public virtual void Use () { }

        public InventoryItem (Item item, int curCount) {
            this.item = item;
            this.CurCount = curCount;
        }
    }
}