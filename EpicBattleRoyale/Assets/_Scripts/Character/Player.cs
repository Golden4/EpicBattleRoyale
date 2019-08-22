using System;
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

    public void Setup(Vector2 position)
    {
        if (isInit)
            return;

        Setup(GetComponent<CharacterBase>(), GetComponent<WeaponController>(), position);
    }

    public void Setup(CharacterBase characterBase, WeaponController weaponController, Vector2 position)
    {
        if (isInit)
            return;

        Ins = this;

        this.characterBase = characterBase;
        characterBase.Setup();

        this.weaponController = weaponController;
        weaponController.Setup();

        characterBase.MoveToPosition(position, true);
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

        characterBase.moveInput.x = Mathf.RoundToInt(CrossPlatformInputManager.GetAxisRaw("Horizontal"));

        characterBase.moveInput.y = Mathf.RoundToInt(CrossPlatformInputManager.GetAxisRaw("Vertical"));

        if (characterBase.moveInput.x == 0)
            characterBase.moveInput.x = Input.GetAxisRaw("Horizontal");

        if (characterBase.moveInput.y == 0)
            characterBase.moveInput.y = Input.GetAxisRaw("Vertical");

    }


}