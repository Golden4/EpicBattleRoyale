using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponController : MonoBehaviour
{
    bool isInit;
    public readonly int WEAPON_COUNT = 4;
    int currentWeaponInHandIndex = 0;

    Weapon[] weaponsInInventory = new Weapon[4];

    public enum SlotType
    {
        Melee,
        Pistol,
        Automatic,
    }

    public enum State
    {
        Normal,
        Switching,
    }

    public Slot[] slotsVisual;

    public State curState;
    // public Transform weaponHandHolder;
    // public int orderInLayerHandWeapon;
    // public Transform weaponBackHolder;
    // public int orderInLayerBackWeapon;
    // public Transform pistolBackHolder;
    // public int orderInLayerPistolWeapon;
    public float switchingWeaponTime = .5f;
    float switchingWeapon;

    public event EventHandler OnWeaponSwitch;
    public event EventHandler OnGiveWeapon;
    [HideInInspector]
    public CharacterBase cb;

    void Awake()
    {
        if (!isInit)
            Setup();
    }

    public void Setup()
    {
        if (isInit)
            return;
        isInit = true;
        cb = GetComponent<CharacterBase>();
        GiveWeapon(GameAssets.WeaponsList.Fists);
        cb.OnDie += OnDie;
    }

    public Weapon GiveWeapon(GameAssets.WeaponsList weapon)
    {
        Weapon newWeapon = null;
        SlotType slotType = GameAssets.Get.GetWeapon(weapon).GetSlotType();
        int slotIndex = GetSlotIndex(slotType);
        DropWeaponFromInventory(slotIndex);
        newWeapon = GiveWeapon(weapon, slotIndex);
        return newWeapon;
    }


    public Weapon GiveWeapon(GameAssets.WeaponsList weapon, int indexInInventory)
    {
        GameObject go = Instantiate(GameAssets.Get.GetWeapon(weapon).gameObject);
        weaponsInInventory[indexInInventory] = go.GetComponent<Weapon>();
        weaponsInInventory[indexInInventory].Setup(this);

        int indexWeapon = indexInInventory;

        if (GetCurrentWeapon() != null)
        {
            // if (GetCurrentWeapon().WeaponIs(typeof(AutomaticWeapon)))
            // {
            //     AutomaticWeapon automaticWeapon = GetCurrentWeapon().GetWeapon<AutomaticWeapon>();

            //     if (automaticWeapon.curState == AutomaticWeapon.State.Reloading)
            //     {
            //         indexWeapon = currentWeaponInHandIndex;
            //     }
            // }

            if (GetCurrentWeapon().isFiring())
                indexWeapon = currentWeaponInHandIndex;
        }


        Renderer weaponRenderer = go.GetComponentInChildren<Renderer>();
        if (weaponRenderer != null)
            cb.AddRenderer(weaponRenderer);

        SwitchWeapon(indexWeapon);


        if (OnGiveWeapon != null)
            OnGiveWeapon(weapon, EventArgs.Empty);

        return weaponsInInventory[indexInInventory];
    }


    public Weapon FindWeaponInInventory(GameAssets.WeaponsList weapon)
    {
        for (int i = 0; i < WEAPON_COUNT; i++)
        {
            if (weaponsInInventory[i] != null && weaponsInInventory[i].weaponName == weapon)
                return weaponsInInventory[i];
        }
        return null;
    }

    public void DropWeaponFromInventory(int index)
    {
        if (weaponsInInventory[index] != null)
        {
            if (weaponsInInventory[index].weaponName != GameAssets.WeaponsList.Fists)
            {
                WeaponItemPickUp weaponItemPickUp = World.Ins.SpawnItemPickUpWeapon(weaponsInInventory[index].weaponName,
                                                        cb.worldPosition + ((cb.isFacingRight) ? 2 : -2) * Vector3.right);

                if (weaponsInInventory[index].WeaponIs(typeof(AutomaticWeapon)))
                {
                    AutomaticWeapon automaticWeapon = (AutomaticWeapon)weaponsInInventory[index];
                    weaponItemPickUp.AddWeaponData(automaticWeapon);
                }
                //weaponItemPickUp.AddForce (new Vector3 (((cb.isFacingRight) ? 50 : -50), -50));


                /*if (weaponsInInventory [index].GetType () == typeof(AutomaticWeapon)) {
					AutomaticWeapon automaticWeapon = (AutomaticWeapon)weaponsInInventory [index];

					weaponItemPickUp.AddBulletInfo (automaticWeapon.bulletSystem);
				}*/
            }

            Renderer weaponRenderer = weaponsInInventory[index].GetComponentInChildren<Renderer>();
            if (weaponRenderer != null)
                cb.DeleteRederer(weaponRenderer);

            Destroy(weaponsInInventory[index].gameObject);
        }
    }

    int GetSlotIndex(SlotType type)
    {
        int index = -1;
        switch (type)
        {
            case SlotType.Automatic:

                for (int i = 0; i < 2; i++)
                {
                    if (weaponsInInventory[i] == null)
                    {
                        index = i;
                        break;
                    }
                }
                if (index == -1)
                {
                    if (currentWeaponInHandIndex < 2)
                        index = currentWeaponInHandIndex;
                    else
                    {
                        index = 1;
                    }
                }


                break;
            case SlotType.Pistol:
                index = 2;
                break;
            case SlotType.Melee:
                index = 3;
                break;
            default:
                break;
        }

        return index;
    }

    void Update()
    {
        if (cb.IsDead())
            return;

        switch (curState)
        {
            case State.Switching:
                switchingWeapon -= Time.deltaTime;
                if (switchingWeapon < 0)
                {
                    curState = State.Normal;
                }
                break;
            case State.Normal:
                if (Input.GetKeyDown(KeyCode.E))
                    SwitchWeapon(0, true);

                if (Input.GetKeyDown(KeyCode.Q))
                    SwitchWeapon(1, true);

                if (weaponsInInventory[currentWeaponInHandIndex] != null)
                    weaponsInInventory[currentWeaponInHandIndex].OnUpdate();
                break;
            default:
                break;
        }
    }

    void FixedUpdate()
    {
        if (cb.IsDead())
            return;


        switch (curState)
        {
            case State.Normal:
                if (weaponsInInventory[currentWeaponInHandIndex] != null)
                    weaponsInInventory[currentWeaponInHandIndex].OnFixedUpdate();
                break;
            default:
                break;
        }

    }

    public void SwitchWeapon(int weaponIndex, bool checkCanSwitch = false)
    {
        if (checkCanSwitch)
        {
            if (currentWeaponInHandIndex == weaponIndex || weaponsInInventory[weaponIndex] == null || curState == State.Switching)
                return;
        }

        currentWeaponInHandIndex = weaponIndex;

        SetWeaponLocations(weaponIndex);

        curState = State.Switching;

        if (!GetCurrentWeapon().isFiring())
            switchingWeapon = switchingWeaponTime;

        cb.SetWeaponAnimationType(weaponsInInventory[weaponIndex].slotType);

        /*if (GetCurrentWeapon () == null)
			cb.isWeapon = false;
		else
			cb.isWeapon = GetCurrentWeapon ().GetWeaponType () != Weapon.WeaponType.Melee;*/

        if (OnWeaponSwitch != null)
            OnWeaponSwitch(weaponsInInventory[weaponIndex], EventArgs.Empty);
    }

    void SetWeaponLocations(int activeIndex)
    {

        for (int i = 0; i < slotsVisual.Length; i++)
        {
            slotsVisual[i].isEmpty = true;
        }

        for (int i = 0; i < WEAPON_COUNT; i++)
        {
            if (weaponsInInventory[i] == null)
                continue;

            SetWeaponLocation(weaponsInInventory[i], activeIndex == i, i);
        }
    }

    void SetWeaponLocation(Weapon weapon, bool isActive, int slotIndex)
    {
        weapon.gameObject.SetActive(true);
        SpriteRenderer spriteRenderer = weapon.GetComponentInChildren<SpriteRenderer>();

        weapon.isActive = isActive;

        if (isActive)
        {
            weapon.transform.SetParent(slotsVisual[0].weaponHolder, false);
            if (spriteRenderer != null)
                cb.ChangeSortingOrder((Renderer)spriteRenderer, slotsVisual[0].orderInLayerWeapon);
            slotsVisual[0].isEmpty = false;
        }
        else if ((slotIndex == 0 || slotIndex == 1) && slotsVisual[1].isEmpty)
        {
            weapon.transform.SetParent(slotsVisual[1].weaponHolder, false);
            if (spriteRenderer != null)
                cb.ChangeSortingOrder((Renderer)spriteRenderer, slotsVisual[1].orderInLayerWeapon);
            slotsVisual[1].isEmpty = false;
        }
        else if (slotIndex == 2)
        {
            weapon.transform.SetParent(slotsVisual[2].weaponHolder, false);
            if (spriteRenderer != null)
                cb.ChangeSortingOrder((Renderer)spriteRenderer, slotsVisual[2].orderInLayerWeapon);
            slotsVisual[2].isEmpty = false;
        }
        else
        {
            weapon.gameObject.SetActive(false);
        }


        weapon.transform.localPosition = Vector3.zero;

    }

    /*	void ShowWeaponGraphics (int index)
	{
		for (int i = 0; i < weaponsInInventory.Count; i++) {
	
			if (weaponsInInventory [i].gameObject.activeSelf && i != index) {
				weaponsInInventory [i].gameObject.SetActive (false);
			} else if (i == index) {
					weaponsInInventory [i].gameObject.SetActive (true);
				}
		}
	}*/


    public Weapon GetCurrentWeapon()
    {
        return weaponsInInventory[currentWeaponInHandIndex];
    }

    public Weapon GetWeapon(int index)
    {
        return weaponsInInventory[index];
    }

    public Weapon[] GetWeaponsInInventory()
    {
        return weaponsInInventory;
    }

    public int GetCurrentActiveWeaponIndex()
    {
        return currentWeaponInHandIndex;
    }

    public bool InventoryFull(WeaponController.SlotType slotType)
    {
        switch (slotType)
        {
            case WeaponController.SlotType.Automatic:
                for (int i = 0; i < 2; i++)
                {
                    if (weaponsInInventory[i] == null)
                    {
                        return false;
                    }
                }
                break;
            case WeaponController.SlotType.Pistol:
                if (weaponsInInventory[2] == null)
                {
                    return false;
                }
                break;
            case WeaponController.SlotType.Melee:
                if (weaponsInInventory[3] == null)
                {
                    return false;
                }
                break;
            default:
                break;
        }

        return true;
    }

    public int GetWeaponIndexWithBullets()
    {
        for (int i = 0; i < weaponsInInventory.Length; i++)
        {
            if (weaponsInInventory[i] != null && weaponsInInventory[i].WeaponIs(typeof(AutomaticWeapon)))
            {
                AutomaticWeapon automaticWeapon = (AutomaticWeapon)weaponsInInventory[i];

                if (!automaticWeapon.bulletSystem.NoBullets())
                {
                    return i;
                }
            }
        }

        return -1;
    }

    void OnDie(object obj, EventArgs args)
    {
        for (int i = 0; i < weaponsInInventory.Length; i++)
        {
            if (weaponsInInventory[i] != null)
                DropWeaponFromInventory(i);
        }
    }

    [System.Serializable]
    public class Slot
    {
        public Transform weaponHolder;
        public int orderInLayerWeapon;
        public bool isEmpty;
    }

    /*	void OnEnable ()
	{
		EventManager.OnClickSelectWeaponEvent += (int arg1, GameObject arg2) => switchWeapon (arg1);
	}

	void OnDisable ()
	{
		EventManager.OnClickSelectWeaponEvent -= (int arg1, GameObject arg2) => switchWeapon (arg1);
	}*/
}
