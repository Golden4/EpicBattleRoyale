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
        MapsController.OnChangingMap += OnChangingMap;
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
        isInit = true;
    }

    Vector3[] positionsToMove = new Vector3[3] {
        Vector3.right * -22,
        Vector3.right * 22,
        Vector3.zero
    };

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

    void OnChangingMap(MapsController.MapInfo arg1, Direction arg2)
    {
        int index = -1;

        Direction[] dir1 = new Direction[] {
            Direction.Bottom, Direction.Left, Direction.Right, Direction.Top
        };

        Direction[] dir2 = new Direction[] {
            Direction.Top, Direction.Right, Direction.Left, Direction.Bottom
        };

        for (int i = 0; i < dir1.Length; i++)
        {
            if (arg2 == dir1[i])
            {
                for (int j = 0; j < arg1.roads.Length; j++)
                {
                    if (arg1.roads[j] == dir2[i])
                    {
                        index = j;
                        break;
                    }
                }
            }
        }
        if (index != -1)
            transform.position = positionsToMove[index];

    }
    void OnDestroy()
    {
        MapsController.OnChangingMap -= OnChangingMap;
    }
}