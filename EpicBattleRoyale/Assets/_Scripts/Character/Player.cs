using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(CharacterBase))]
public class Player : MonoBehaviour
{
    public static Player Ins;
    bool isInit;
    public CharacterBase characterBase;
    public WeaponController weaponController;

    public void Setup(Vector3 position)
    {
        if (isInit)
            return;

        Setup(GetComponent<CharacterBase>(), GetComponent<WeaponController>(), position);
    }

    public void Setup(CharacterBase characterBase, WeaponController weaponController, Vector3 position)
    {
        if (isInit)
            return;

        Ins = this;

        this.characterBase = characterBase;
        characterBase.Setup();

        this.weaponController = weaponController;
        weaponController.Setup();

        transform.position = position;
        MapsController.Ins.OnChangingMap += OnChangingMap;
        MapsController.Ins.OnEnterHouseEvent += OnEnterHouse;
        isInit = true;
    }

    void Update()
    {
        if (characterBase.IsDead())
            return;

        float shootingSideInput = CrossPlatformInputManager.GetAxisRaw("Shoot");
        weaponController.GetCurrentWeapon().firingSideInput = shootingSideInput;

        if (weaponController.GetCurrentWeapon() != null)
            characterBase.isFiring = weaponController.GetCurrentWeapon().isFiring();

        bool isJumping = CrossPlatformInputManager.GetButtonDown("Jump");

        if (!isJumping)
            isJumping = Input.GetButtonDown("Jump");

        if (isJumping)
            characterBase.Jump();
    }

    void FixedUpdate()
    {
        if (characterBase.IsDead())
            return;

        characterBase.move = Mathf.RoundToInt(CrossPlatformInputManager.GetAxisRaw("Horizontal"));
        if (characterBase.move == 0)
            characterBase.move = Input.GetAxisRaw("Horizontal");

    }

    void OnDestroy()
    {
        MapsController.Ins.OnChangingMap -= OnChangingMap;
    }

    void OnChangingMap(MapsController.MapInfo mapInfo, Direction dir)
    {
        //если вышли из дома
        if (dir == Direction.None)
        {
            Vector3 pos = MapsController.Ins.GetCurrentMapInfo().houses[MapsController.Ins.curHouseIndex].GetHouseSpawnPosition() + Vector3.up;
            characterBase.MoveToPosition(pos, false);
            return;
        }

        int index = -1;

        Direction[] dir1 = new Direction[] {
            Direction.Bottom, Direction.Left, Direction.Right, Direction.Top
        };

        Direction[] dir2 = new Direction[] {
            Direction.Top, Direction.Right, Direction.Left, Direction.Bottom
        };

        for (int i = 0; i < dir1.Length; i++)
        {
            if (dir == dir1[i])
            {
                for (int j = 0; j < mapInfo.roads.Count; j++)
                {
                    if (mapInfo.roads[j] == dir2[i])
                    {
                        index = j;
                        break;
                    }
                }

                if (mapInfo.centerRoad == dir2[i])
                {
                    index = 2;
                    break;
                }
            }
        }

        if (index != -1)
        {
            bool isFacingRight = true;
            if (index == 1)
            {
                isFacingRight = false;
            }
            characterBase.MoveToPosition(MapsController.Ins.characterSpawnPoints[index], isFacingRight);
        }

    }

    void OnEnterHouse(HouseDoor house)
    {
        characterBase.MoveToPosition(new Vector3(MapsController.Ins.GetHouseData(house.houseType).worldEndPoints.x + 2, -4), true);
    }

}