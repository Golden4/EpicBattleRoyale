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

}