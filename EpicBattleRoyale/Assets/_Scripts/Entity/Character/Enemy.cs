﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public CharacterBase characterBase;
    public WeaponController weaponController;

    CharacterBase targetCharacter;
    ItemPickUp targetItem;
    Vector3 targetPosition;

    public enum EnemyState
    {
        Waiting,
        Moving,
        Attacking,
    }

    public enum TargetState
    {
        Finding,
        TargetCharacter,
        TargetItem,
    }

    public TargetState curTargetState;
    public EnemyState curEnemyState;

    void Start()
    {
        Reset();
    }

    public void Setup(Vector2 position)
    {
        Setup(GetComponent<CharacterBase>(), GetComponent<WeaponController>(), position);
    }

    public void Setup(CharacterBase characterBase, WeaponController weaponController, Vector2 position)
    {
        this.characterBase = characterBase;
        this.weaponController = weaponController;
        characterBase.MoveToPosition(position, true);
        characterBase.maxSpeed -= Vector2.one * .5f;
        characterBase.OnDie += OnDie;
        characterBase.OnHittedEvent += OnHitted;
    }

    void OnHitted(CharacterBase hitCharacter, Weapon hitWeapon, int damage, HitBox.HitBoxType hitBoxType)
    {
        //  if (curTargetState != TargetState.TargetCharacter) {
        if (hittedRageTime < 0)
        {
            targetCharacter = hitCharacter;
            curTargetState = TargetState.TargetCharacter;
            hittedRageTime = 2;
        }
    }

    void OnDie(LivingEntity characterBase)
    {
        World.Ins.RemoveEntity(characterBase.mapCoords, this);
    }

    public bool FindClosestTargetItem()
    {
        ItemPickUp closeItemPickUp = World.Ins.GetClosestItem(characterBase.mapCoords, characterBase.worldPosition);

        if (closeItemPickUp == null)
            return false;

        float closeItemDistance = Vector2.Distance(closeItemPickUp.worldPosition, characterBase.worldPosition);

        if (closeItemDistance < targetInRangeDistance)
        {
            targetItem = closeItemPickUp;
            return true;
        }

        return false;
    }

    public bool FindClosestTargetCharacter()
    {
        CharacterBase closeCharacter = World.Ins.GetClosestCharacter(characterBase.mapCoords, characterBase.worldPosition, characterBase);

        if (closeCharacter == null)
            return false;

        float closeCharacterDistance = Vector2.Distance(closeCharacter.worldPosition, characterBase.worldPosition);

        if (closeCharacterDistance < targetInRangeDistance)
        {
            targetCharacter = closeCharacter;
            return true;
        }

        return false;
    }

    public float findingTargetDelay = .5f;
    float findingTargetDelayCur;

    public float awayFromTargetTime = 3f;
    float awayFromTargetTimeCur;

    public float targetInRangeDistance = 6f;
    float curWaitingTime;
    float hittedRageTime;

    bool IsInRange(EntityBase target, float range)
    {
        float distanceToTarget = Vector2.Distance(characterBase.worldPosition, target.worldPosition);
        return (distanceToTarget < range);
    }

    void FixedUpdate()
    {
        if (characterBase.IsDead())
            return;

        switch (curTargetState)
        {
            case TargetState.Finding:

                findingTargetDelayCur -= Time.fixedDeltaTime;

                if (findingTargetDelayCur < 0)
                {
                    findingTargetDelayCur += findingTargetDelay;

                    if (FindClosestTargetCharacter())
                    {
                        curTargetState = TargetState.TargetCharacter;
                        findingTargetDelayCur = 0;
                    }
                    // else
                    // {
                    //     if (FindClosestTargetItem())
                    //     {
                    //         curTargetState = TargetState.TargetItem;
                    //     }
                    //     else
                    //     {
                    //         findingTargetDelayCur = 0;
                    //     }
                    // }
                }
                break;
            case TargetState.TargetCharacter:

                if (targetCharacter == null || targetCharacter.IsDead())
                {
                    curTargetState = TargetState.Finding;
                    targetCharacter = null;
                }
                else
                {
                    if (IsInRange(targetCharacter, targetInRangeDistance) || hittedRageTime > 0)
                    {
                        hittedRageTime -= Time.fixedDeltaTime;
                        awayFromTargetTimeCur = awayFromTargetTime;
                    }
                    else
                    {
                        awayFromTargetTimeCur -= Time.fixedDeltaTime;

                        if (awayFromTargetTimeCur < 0)
                        {
                            curTargetState = TargetState.Finding;
                            targetCharacter = null;
                        }
                    }
                }
                break;
            case TargetState.TargetItem:

                if (targetItem == null)
                {
                    curTargetState = TargetState.Finding;
                }
                else
                {
                    if (IsInRange(targetItem, targetInRangeDistance))
                    {
                        curTargetState = TargetState.TargetItem;
                    }
                    else
                    {
                        curTargetState = TargetState.Finding;
                        targetItem = null;
                    }
                }
                break;
            default:
                break;
        }

        switch (curEnemyState)
        {
            case EnemyState.Waiting:

                HandleEnemyWaiting();
                break;

            case EnemyState.Moving:

                HandleEnemyMoving();

                break;

            case EnemyState.Attacking:

                HandleEnemyAttacking();

                break;

            default:
                break;
        }
    }

    void HandleEnemyWaiting()
    {
        curWaitingTime -= Time.fixedDeltaTime;

        if (curWaitingTime < 0 || curTargetState == TargetState.TargetCharacter)
        {
            curWaitingTime = .5f;
            targetPosition = (Vector2)characterBase.worldPosition + UnityEngine.Random.insideUnitCircle * UnityEngine.Random.Range(-20f, 10f);
            targetPosition.x = Mathf.Clamp(targetPosition.x, MapsController.Ins.GetCurrentWorldEndPoints().x, MapsController.Ins.GetCurrentWorldEndPoints().y);
            targetPosition.y = Mathf.Clamp(targetPosition.y, MapsController.Ins.GetCurrentWorldUpDownEndPoints().x, MapsController.Ins.GetCurrentWorldUpDownEndPoints().y);
            curEnemyState = EnemyState.Moving;
        }

        characterBase.moveInput.x = 0;
        weaponController.GetCurrentWeapon().firingSideInput = 0;
    }

    float jumpDelay;

    float yAttackingDistance = .1f;

    void HandleEnemyMoving()
    {
        if (curTargetState == TargetState.Finding || curTargetState == TargetState.TargetItem)
        {
            HandleMovingToTargetPosition(targetPosition, new Vector2(.5f, yAttackingDistance), delegate
            {
                curEnemyState = EnemyState.Waiting;
            });
        }
        else if (curTargetState == TargetState.TargetCharacter)
        {
            if (weaponController.GetCurrentWeapon().GetType() == typeof(MeleeWeapon))
            {
                jumpDelay -= Time.fixedDeltaTime;
                if (jumpDelay < 0)
                {
                    if (UnityEngine.Random.Range(0, 3) == 0)
                    {
                        characterBase.Jump();
                    }

                    jumpDelay = UnityEngine.Random.Range(3, 10f);
                }
            }

            HandleMovingToTargetPosition(targetCharacter.worldPosition, new Vector2(GetFiringDistanceForCurrentWeapon(), yAttackingDistance), delegate
            {
                curEnemyState = EnemyState.Attacking;
            });
        }

        weaponController.GetCurrentWeapon().firingSideInput = 0;
    }

    float curAttackingTime;
    void HandleEnemyAttacking()
    {
        if (curTargetState == TargetState.TargetCharacter)
        {
            Vector3 dirT = ((Vector2)characterBase.worldPosition - (Vector2)targetCharacter.worldPosition).normalized;
            float distanceX = Mathf.Abs(characterBase.worldPosition.x - targetCharacter.worldPosition.x);
            float distanceY = Mathf.Abs(characterBase.worldPosition.y - targetCharacter.worldPosition.y);

            weaponController.GetCurrentWeapon().firingSideInput = Mathf.RoundToInt(dirT.x);

            if (weaponController.GetCurrentWeapon().WeaponIs(typeof(AutomaticWeapon)))
            {
                AutomaticWeapon aw = (AutomaticWeapon)weaponController.GetCurrentWeapon();

                if (aw.bulletSystem.NoBullets())
                {
                    ChangeWeapon();
                }
            }

            if (distanceX > GetFiringDistanceForCurrentWeapon() || distanceY > yAttackingDistance)
            {
                curAttackingTime -= Time.fixedDeltaTime;

                if (curAttackingTime < 0)
                {
                    curAttackingTime = .5f;
                    curEnemyState = EnemyState.Moving;
                }
            }
        }
        else
        {
            curEnemyState = EnemyState.Waiting;
        }

        characterBase.moveInput = Vector2.zero;
    }

    void OnPickUp(ItemPickUp item)
    {
        if (item.GetType() == typeof(WeaponItemPickUp) || item.GetType() == typeof(AmmoItemPickUp))
        {
            ChangeWeapon();
        }
    }

    void ChangeWeapon()
    {
        int index = weaponController.GetWeaponIndexWithBullets();

        if (index != -1)
            weaponController.SwitchWeapon(index, true);
        else
        {
            characterBase.characterInventory.OnPickUp -= OnPickUp;
            characterBase.characterInventory.OnPickUp += OnPickUp;
            weaponController.SwitchWeapon(3, true);
        }
    }

    float GetFiringDistanceForCurrentWeapon()
    {
        float distanceToAttack = 8;

        if (hittedRageTime > 0)
        {
            distanceToAttack = 9;
        }

        if (weaponController.GetCurrentWeapon().GetWeaponType() == Weapon.WeaponType.Melee)
        {
            distanceToAttack = 1;
        }

        return distanceToAttack;
    }

    void HandleMovingToTargetPosition(Vector2 position, Vector2 stopDistance, Action OnReachedTargetPosition = null)
    {
        Vector2 dir = (-(Vector2)characterBase.worldPosition + position).normalized;
        float distanceX = Mathf.Abs(characterBase.worldPosition.x - position.x);
        float distanceY = Mathf.Abs(characterBase.worldPosition.y - position.y);

        Vector2 moveInput = default;

        if (distanceX > stopDistance.x)
            moveInput.x = (dir.x > 0) ? 1 : -1;

        if (distanceY > stopDistance.y)
            moveInput.y = (dir.y > 0) ? 1 : -1;

        characterBase.Move(moveInput);

        if (distanceY <= stopDistance.y && distanceX <= stopDistance.x)
        {
            if (OnReachedTargetPosition != null)
            {
                OnReachedTargetPosition();
            }
        }

    }

    void Reset()
    {
        curTargetState = TargetState.Finding;
        curEnemyState = EnemyState.Waiting;
        findingTargetDelayCur = 0;
        awayFromTargetTimeCur = 0;
        curWaitingTime = 0;
        hittedRageTime = 0;
        curAttackingTime = 0;
        targetCharacter = null;
        targetItem = null;
    }

    void OnDisable()
    {
        Reset();
    }

    void OnEnable()
    {
        Reset();
    }
}