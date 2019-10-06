using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CharacterBase : LivingEntity
{
    public bool isPlayer;
    public GameAssets.CharacterList characterName;
    public Vector2 maxSpeed;
    public Vector2 firingSpeed;
    public Vector2 healingSpeed;
    Vector2 curSpeed;
    public float jumpForce = 500;
    public bool airControl = true;
    public LayerMask whatIsGround;
    [HideInInspector]
    public bool isFacingRight = true;
    Transform groundCheck;
    bool isGrounded;
    bool isJumping;
    public Animator animator;
    Rigidbody2D rb;

    [HideInInspector]
    public bool isFiring;
    [HideInInspector]
    public bool isHealing;
    [HideInInspector]
    public bool shootingSideRight;
    [HideInInspector]
    public Vector2 moveInput;
    [HideInInspector]
    public bool isOnArea;

    #region Components
    public CharacterInventory characterInventory;
    public CharacterMapNavigate characterMapNavigate;
    public CharacterInteractable characterInteractable;
    public CharacterAudio characterAudio;
    public WeaponController weaponController;
    #endregion

    float curJumpPos;
    float curJumpVelocity;

    #region Stats
    public int killsCount;
    #endregion

    #region Events
    public event Action<CharacterBase, Weapon, int, HitBox.HitBoxType> OnHittedEvent;
    public event Action<CharacterBase, CharacterBase, Weapon, HitBox.HitBoxType> OnKill;
    public static event Action<CharacterBase, CharacterBase, Weapon, HitBox.HitBoxType> OnKillStatic;

    public event Action<CharacterBase> OnIsOnArea;
    public event Action<CharacterBase> OnOutOfArea;

    #endregion

    protected override void Awake()
    {
        if (!isInit)
            Setup();
        base.Awake();
    }

    protected override void Start()
    {
        Init();
        StartCoroutine(InitRenederersCoroutine());
    }

    protected IEnumerator InitRenederersCoroutine()
    {
        yield return new WaitForSeconds(1);
        InitRenederers();
    }

    public void Setup()
    {
        if (isInit)
            return;

        healthSystem = new HealthSystem(100, 0);

        groundCheck = transform.Find("GroundCheck");
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        Material mat = Resources.Load<Material>("Materials/FillableMaterial");

        characterInventory = GetComponent<CharacterInventory>();
        characterMapNavigate = GetComponent<CharacterMapNavigate>();
        characterInteractable = GetComponentInChildren<CharacterInteractable>();
        characterAudio = GetComponent<CharacterAudio>();
        weaponController = GetComponent<WeaponController>();

        foreach (var item in renderers)
        {
            item.sharedMaterial = mat;
        }

        isInit = true;
    }
    float lerpSpeed = .1f;

    public void LerpCharacter()
    {
        foreach (Renderer item in renderers)
        {
            if (item != null)
                if (item.material.shader.name == "Spine/SkeletonFill")
                {
                    item.material.SetColor("_FillColor", Color.white);
                    item.material.DOFloat(0, "_FillAlpha", .1f).ChangeStartValue(1); //.SetFloat("_FillAlpha", t);
                }
        }
    }

    public void FadeCharacter(Action OnEndFade)
    {
        bool endFade = true;

        foreach (Renderer item in renderers)
        {
            if (item.material.shader.name == "Spine/SkeletonFill")
            {
                if (endFade)
                {
                    endFade = false;
                    item.material.DOColor(Color.clear, "_FillColor", 2f).SetDelay(1f).ChangeStartValue(Color.white).OnComplete(() => { OnEndFade(); });
                }
                else
                {
                    item.material.DOColor(Color.clear, "_FillColor", 2f).SetDelay(1f).ChangeStartValue(Color.white);
                }
                item.material.SetFloat("FillAlpha", 0);
            }
        }
    }

    private void FixedUpdate()
    {
        if (animator.GetInteger("Die") != -1 && isDead)
            return;

        if (curJumpPos > 0)
            isGrounded = false;
        else
            isGrounded = true;

        //если приземлились
        if (curJumpPos < 0)
        {
            curJumpVelocity = 0;
            curJumpPos = 0;
            isGrounded = true;
            isJumping = false;
        }

        //Если прыгнули
        if (curJumpVelocity != 0)
        {
            curJumpPos += Time.fixedDeltaTime * curJumpVelocity;
            curJumpVelocity -= Time.fixedDeltaTime * 20f;
            isGrounded = false;
            isJumping = true;
        }

        // if (isGrounded && animator.GetInteger("Die") == -1 && isDead)
        //     animator.SetInteger("Die", true);

        animator.SetBool("Jump", !isGrounded);
        Move(moveInput);
    }

    float nextAreaHitCooldown;

    void Update()
    {
        if (IsDead())
            return;

        if (isOnArea)
        {
            if (!AreaController.Ins.IsOnArea(mapCoords))
            {
                nextAreaHitCooldown = AreaController.Ins.GetHitCoudown();

                if (OnIsOnArea != null)
                    OnIsOnArea(this);
            }
        }
        else
        {
            if (AreaController.Ins.IsOnArea(mapCoords))
            {
                if (OnOutOfArea != null)
                    OnOutOfArea(this);
            }

            if (nextAreaHitCooldown > 0)
            {
                nextAreaHitCooldown -= Time.deltaTime;
            }
            else if (nextAreaHitCooldown < 0)
            {
                nextAreaHitCooldown += AreaController.Ins.GetHitCoudown();
                TakeHit(AreaController.Ins.GetDamageAmount(), false);
                // healthSystem.DamageHealth(AreaController.Ins.GetDamageAmount());
            }
        }

        isOnArea = AreaController.Ins.IsOnArea(mapCoords);
    }

    public override void OnEntityDie()
    {
        CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
        gameObject.layer = LayerMask.NameToLayer("IgnoreCharacter");
        World.Ins.allCharacters.Remove(this);

        showShadow = false;
        UpdateShadow(showShadow);

        animator.SetInteger("Die", shootEnemyDirection);

        FadeCharacter(delegate
        {
            Destroy(gameObject);
        });
    }

    // IEnumerator FadeCharacter(float delay = 2, float fadeTime = 2, Color fadeColor = default, Action OnEndFade = null)
    // {
    //     yield return new WaitForSeconds(delay);
    //     float curFadeTime = fadeTime;

    //     while (curFadeTime > 0)
    //     {
    //         curFadeTime -= Time.deltaTime;

    //         foreach (SkinnedMeshRenderer item in srs)
    //         {
    //             Color color = Color.Lerp(Color.white, Color.clear, curFadeTime / fadeTime);

    //             if (curFadeTime <= 0.05f)
    //             {
    //                 color = Color.clear;
    //                 curFadeTime = 0;
    //             }
    //             else
    //             {
    //                 color.a = curFadeTime / fadeTime;
    //             }

    //             item.material.SetColor("_FillColor", color);

    //         }

    //         yield return null;
    //     }
    //     if (OnEndFade != null)
    //         OnEndFade();

    // }

    public void Move(Vector2 moveDir)
    {
        if (isGrounded || airControl)
        {
            moveDir = moveDir.normalized;
            float animationSpeed = Mathf.Abs(Mathf.Clamp01(moveDir.sqrMagnitude));

            if (isFiring)
            {
                if (moveDir.x < 0 && shootingSideRight || moveDir.x > 0 && !shootingSideRight)
                {
                    animationSpeed = -.7f;
                }
                else
                {

                    animationSpeed = .7f;
                }

                curSpeed = firingSpeed;

                Flip(shootingSideRight);
                if (isHealing)
                    EndHeal();
            }
            else if (isHealing)
            {

                if (moveDir.x != 0)
                    Flip(moveDir.x > 0);

                animationSpeed = .5f;

                curSpeed = healingSpeed;
            }
            else if (hitCooldown > 0)
            {
                hitCooldown -= Time.fixedDeltaTime;
                Debug.Log(hitCooldown);
                animationSpeed = .8f;

                if (moveDir.x != 0)
                    Flip(moveDir.x > 0);

                curSpeed = firingSpeed;
            }
            else
            {
                animationSpeed = 1;
                if (moveDir.x != 0)
                    Flip(moveDir.x > 0);

                curSpeed = maxSpeed;
            }

            if (Mathf.Abs(Mathf.Clamp01(moveDir.sqrMagnitude)) == 0)
                animationSpeed = 0;

            animator.SetFloat("Speed", animationSpeed);

            float xPos = worldPosition.x + moveDir.x * curSpeed.x * Time.fixedDeltaTime;
            float yPos = worldPosition.y + moveDir.y * curSpeed.y * Time.fixedDeltaTime;

            MoveTo(new Vector2(xPos, yPos), curJumpPos);
        }
    }

    public void Jump()
    {
        if (isGrounded && !animator.GetBool("Jump"))
        {
            isGrounded = false;
            isJumping = true;
            curJumpVelocity = jumpForce;
            EndHeal();
        }
    }

    public void Flip(bool right)
    {
        isFacingRight = right;
        Vector3 theScale = transform.localScale;
        theScale.x = (right) ? 1 : -1;
        transform.localScale = theScale;
    }

    public Vector3 GetCharacterCenter()
    {
        return transform.position;
    }

    public bool CanPickUp()
    {
        return !IsDead();
    }

    public bool CanHit()
    {
        return !IsDead();
    }

    public void TakeHit(int damage, bool armorHit = true)
    {
        TakeHit(null, null, damage, HitBox.HitBoxType.Body, armorHit);
    }

    float hitCooldown;

    int shootEnemyDirection;

    public void TakeHit(CharacterBase givedHitCharacter, Weapon hitWeapon, int damage, HitBox.HitBoxType hitBoxType, bool armorHit = true)
    {
        if (givedHitCharacter != null)
        {
            if (isFacingRight)
                shootEnemyDirection = ((worldPosition.x - givedHitCharacter.worldPosition.x) < 0) ? 0 : 1;
            else
                shootEnemyDirection = ((worldPosition.x - givedHitCharacter.worldPosition.x) < 0) ? 1 : 0;
        }

        damage = Mathf.RoundToInt((float)damage * HitBox.GetDamagePersent(hitBoxType));

        if (!armorHit)
        {
            healthSystem.DamageHealth(damage);
        }
        else
            healthSystem.Damage(damage);

        LerpCharacter();

        hitCooldown = .3f;

        SpawnDamagePopUpText(damage);

        if (hitBoxType == HitBox.HitBoxType.Head)
            characterAudio.PlaySound(characterAudio.headshotHitSound);
        else
            characterAudio.PlaySound(characterAudio.hitSound);

        if (OnHittedEvent != null)
            OnHittedEvent(givedHitCharacter, hitWeapon, damage, hitBoxType);

        if (IsDead())
        {
            if (givedHitCharacter != null)
                givedHitCharacter.OnKillCharacter(this, hitWeapon, hitBoxType);

            if (OnKill != null)
            {
                OnKill(givedHitCharacter, this, hitWeapon, hitBoxType);
            }

            if (OnKillStatic != null)
            {
                OnKillStatic(givedHitCharacter, this, hitWeapon, hitBoxType);
            }
        }
    }

    public void GiveHit(CharacterBase takeHitCharacter, Weapon weapon, int damage, HitBox.HitBoxType hitBoxType)
    {
        if (takeHitCharacter.IsDead())
        {
            OnKillCharacter(takeHitCharacter, weapon, hitBoxType);
        }
    }

    public void OnKillCharacter(CharacterBase killedCharacter, Weapon weapon, HitBox.HitBoxType hitBoxType)
    {
        killsCount++;

        // if (OnKill != null)
        // {
        //     OnKill(this, killedCharacter, weapon, hitBoxType);
        // }

        // if (OnKillStatic != null)
        // {
        //     OnKillStatic(this, killedCharacter, weapon, hitBoxType);
        // }
    }

    public void Heal()
    {
        isHealing = true;
        animator.SetBool("isHealing", true);
        animator.SetLayerWeight(1, 1);
        weaponController.EnableWeaponGraphicsInHand(false);
    }

    public void EndHeal()
    {
        if (!isHealing)
            return;

        animator.SetBool("isHealing", false);
        isHealing = false;
        weaponController.EnableWeaponGraphicsInHand(true);
    }

    void SpawnDamagePopUpText(int damage)
    {
        GameObject damagePopUp = Instantiate(GameAssets.Get.pfPopUpDamage.gameObject);
        TMPro.TextMeshPro tmp = damagePopUp.GetComponent<TMPro.TextMeshPro>();
        tmp.text = "-" + damage.ToString();
        tmp.fontSize = Mathf.Clamp(6 + damage / 10, 6, 10);
        tmp.transform.position = transform.position;
        tmp.sortingOrder = 500;

        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(tmp.transform.DOMove(transform.position + Vector3.up + (Vector3)UnityEngine.Random.insideUnitCircle, .3f));
        mySequence.PrependInterval(.2f);
        mySequence.Append(tmp.transform.DOScale(Vector3.zero, .3f));

        // iTween.MoveTo(tmp.gameObject, transform.position + Vector3.up + (Vector3)UnityEngine.Random.insideUnitCircle, .3f);
        //Utility.Invoke(this, .2f, delegate { iTween.ScaleTo(tmp.gameObject, Vector3.zero, .3f); });

        Destroy(tmp.gameObject, .5f);
    }

    public void MoveToPosition(Vector3 position, bool isFacingRight)
    {
        MoveTo(position);
        isJumping = false;
        curJumpVelocity = 0;
        curJumpPos = 0;
        Flip(isFacingRight);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}