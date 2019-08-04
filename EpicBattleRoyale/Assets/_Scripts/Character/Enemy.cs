using System;
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
        curTargetState = TargetState.Finding;
        curEnemyState = EnemyState.Waiting;
    }

    public void Setup(Vector3 position)
    {
        Setup(GetComponent<CharacterBase>(), GetComponent<WeaponController>(), position);
    }

    public void Setup(CharacterBase characterBase, WeaponController weaponController, Vector3 position)
    {
        this.characterBase = characterBase;
        this.weaponController = weaponController;
        transform.position = position;
        characterBase.maxSpeed -= .5f;
        characterBase.OnDie += OnDie;
        characterBase.OnHitted += OnHitted;
    }

    void OnHitted(CharacterBase hitCharacter, Weapon hitWeapon, int damage)
    {
        if (curTargetState != TargetState.TargetCharacter)
        {
            targetCharacter = hitCharacter;
            curTargetState = TargetState.TargetCharacter;
            hittedRageTime = 2;
            Debug.Log("SDADSADA");
        }
    }

    void OnDie(object obj, EventArgs args)
    {

    }

    public bool FindClosestTargetItem()
    {
        ItemPickUp closeItemPickUp = World.Ins.GetClosestItem(transform.position);

        if (closeItemPickUp == null)
            return false;

        float closeItemDistance = Vector3.Distance(closeItemPickUp.transform.position, transform.position);

        if (closeItemDistance < targetInRangeDistance)
        {
            targetItem = closeItemPickUp;
            return true;
        }

        return false;
    }

    public bool FindClosestTargetCharacter()
    {
        CharacterBase closeCharacter = World.Ins.GetClosestCharacter(transform.position, characterBase);

        if (closeCharacter == null)
            return false;

        float closeCharacterDistance = Vector3.Distance(closeCharacter.transform.position, transform.position);



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

    bool IsInRange(Transform target, float range)
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
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
                    if (IsInRange(targetCharacter.transform, targetInRangeDistance) || hittedRageTime > 0)
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
                    if (IsInRange(targetItem.transform, targetInRangeDistance))
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
            curWaitingTime = 2;
            targetPosition = transform.position + Vector3.right * UnityEngine.Random.Range(-10f, 10f);
            targetPosition.x = Mathf.Clamp(targetPosition.x, World.Ins.worldEndPoints.x, World.Ins.worldEndPoints.y);
            curEnemyState = EnemyState.Moving;
        }

        characterBase.move = 0;
        weaponController.GetCurrentWeapon().firingSideInput = 0;
    }
    float jumpDelay;
    void HandleEnemyMoving()
    {
        if (curTargetState == TargetState.Finding || curTargetState == TargetState.TargetItem)
        {
            HandleMovingToTargetPosition(targetPosition, .5f, delegate
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
                    if (UnityEngine.Random.Range(0, 10) == 0)
                    {
                        characterBase.Jump();
                    }
                    jumpDelay = .5f;
                }
            }

            HandleMovingToTargetPosition(targetCharacter.transform.position, GetFiringDistanceForCurrentWeapon(), delegate
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
            Vector3 dirT = (transform.position - targetCharacter.transform.position).normalized;
            float distance = Mathf.Abs(transform.position.x - targetCharacter.transform.position.x);

            weaponController.GetCurrentWeapon().firingSideInput = Mathf.RoundToInt(dirT.x);

            if (weaponController.GetCurrentWeapon().WeaponIs(typeof(AutomaticWeapon)))
            {
                AutomaticWeapon aw = (AutomaticWeapon)weaponController.GetCurrentWeapon();

                if (aw.bulletSystem.NoBullets())
                {
                    ChangeWeapon();
                }
            }

            if (distance > GetFiringDistanceForCurrentWeapon())
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
        characterBase.move = 0;
    }

    void OnPickUp(object obj, EventArgs arg)
    {
        if (obj.GetType() == typeof(WeaponItemPickUp) || obj.GetType() == typeof(AmmoItemPickUp))
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
            characterBase.inventorySystem.OnPickUp -= OnPickUp;
            characterBase.inventorySystem.OnPickUp += OnPickUp;
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

    void HandleMovingToTargetPosition(Vector3 position, float stopDistance = .5f, Action OnReachedTargetPosition = null)
    {
        Vector3 dir = (-transform.position + position).normalized;
        float distance = Mathf.Abs(transform.position.x - position.x);

        if (distance > stopDistance)
        {
            characterBase.Move(Mathf.RoundToInt(dir.x));
        }
        else
        {
            characterBase.Move(0);
            if (OnReachedTargetPosition != null)
            {
                OnReachedTargetPosition();
            }
        }
    }
}