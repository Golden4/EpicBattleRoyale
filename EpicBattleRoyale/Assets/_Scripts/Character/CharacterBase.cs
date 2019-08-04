using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    public GameAssets.CharacterList characterName;
    bool isInit;
    public float maxSpeed = 3.5f;
    public float firingSpeed = 2;
    float curSpeed;
    public float jumpForce = 500;
    public bool airControl = true;
    public LayerMask whatIsGround;
    public bool isFacingRight = true;
    Transform groundCheck;
    bool isGrounded;
    bool isJumping;
    Animator anim;
    Rigidbody2D rb;
    public WeaponController.SlotType weaponType;
    public bool isFiring;
    public bool shootingSideRight;
    public float move;
    bool isDead;
    public HealthSystem healthSystem;
    public InventorySystem inventorySystem;

    public event EventHandler OnCanEnterDoor;
    public event Action<CharacterBase, Weapon, int> OnHitted;
    public event EventHandler OnDie;

    void Awake()
    {
        if (!isInit)
            Setup();
    }

    public void Setup()
    {
        if (isInit)
            return;

        healthSystem = new HealthSystem(100, 0);
        healthSystem.OnHealthZero += HealthSystem_OnHealthZero;
        groundCheck = transform.Find("GroundCheck");
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        srs = GetComponentsInChildren<SpriteRenderer>();
        Material mat = Resources.Load<Material>("Materials/FillableMaterial");
        inventorySystem = new InventorySystem(this);
        foreach (var item in srs)
        {
            item.sharedMaterial = mat;
        }

        isInit = true;
    }

    float jumpDelay;
    float t;
    float lerpSpeed = .1f;

    void Update()
    {
        jumpDelay -= Time.deltaTime;
        hitCooldown -= Time.deltaTime;
        if (t > 0)
        {
            t -= Time.deltaTime / lerpSpeed;

            Timer();

        }
        else if (t <= -0.01f)
        {
            t = 0;

            TimerEnd();
        }

    }

    public void Timer()
    {
        LerpCharacter(t, Color.red, Color.white);
    }

    public void TimerEnd()
    {
        LerpCharacter(1, Color.red, Color.white);
    }

    public void LerpCharacter(float persent, Color fadeColor = default, Color origColor = default)
    {
        foreach (SpriteRenderer item in srs)
        {
            //Color color = Color.Lerp(origColor, fadeColor, persent);
            item.material.SetFloat("_FillAlpha", t);
            // item.color = color;
        }
    }

    private void FixedUpdate()
    {
        if (anim.GetBool("Die") && isDead)
            return;

        isGrounded = false;

        Debug.DrawRay(groundCheck.position, Vector3.down * .2f);

        //Debug.Log (hit.collider.name);
        /*		Collider2D[] colliders = Physics2D.OverlapCircleAll (groundCheck.position, k_GroundedRadius, whatIsGround);
                for (int i = 0; i < colliders.Length; i++) {
                    if (colliders [i].gameObject != gameObject)
                        isGrounded = true;
                }*/

        RaycastHit2D[] hit = Physics2D.RaycastAll(groundCheck.position, Vector3.down, .2f);
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider != null && hit[i].collider.gameObject != gameObject)
            {
                isGrounded = true;
                isJumping = false;
                break;
            }
        }

        if (isGrounded && !anim.GetBool("Die") && isDead)
            anim.SetBool("Die", true);

        anim.SetBool("Jump", !isGrounded);
        Move(move);
    }

    void HealthSystem_OnHealthZero(object sender, EventArgs e)
    {
        if (!IsDead())
            Die();
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void Die()
    {
        isDead = true;
        CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
        //collider.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("IgnoreCharacter");
        collider.offset = new Vector2(.89f, .34f);
        collider.size = new Vector2(1.8f, .8f);
        collider.direction = CapsuleDirection2D.Horizontal;
        StartCoroutine(FadeCharacter(2, 3, Color.clear, delegate
                 {
                     World.Ins.allCharacters.Remove(this);
                     Destroy(gameObject);
                 }));
        // Utility.LerpSprite(this, srs, 2, 3, Color.clear, delegate
        //  {
        //      World.Ins.allCharacters.Remove(this);
        //      Destroy(gameObject);
        //  });

        if (OnDie != null)
            OnDie(this, EventArgs.Empty);
    }

    SpriteRenderer[] srs;
    IEnumerator FadeCharacter(float delay = 2, float fadeTime = 2, Color fadeColor = default, Action OnEndFade = null)
    {
        yield return new WaitForSeconds(delay);
        float curFadeTime = fadeTime;

        while (curFadeTime > 0)
        {
            curFadeTime -= Time.deltaTime;

            foreach (SpriteRenderer item in srs)
            {
                Color color = Color.Lerp(Color.white, Color.clear, curFadeTime / fadeTime);

                if (curFadeTime <= 0.05f)
                {
                    color = Color.clear;
                    curFadeTime = 0;
                }
                else
                {
                    color.a = curFadeTime / fadeTime;
                }

                item.material.SetColor("_FillColor", color);

            }

            yield return null;
        }
        if (OnEndFade != null)
            OnEndFade();

    }

    public void SetWeaponAnimationType(WeaponController.SlotType type)
    {
        weaponType = type;
        anim.SetInteger("WeaponType", (int)type);
        //anim.Play ("Hold" + weaponType.ToString (), 1, );
    }

    public void PlayFireAnimation(float fireRate = -1, bool shootingSide = false)
    {
        Flip(shootingSide);
        this.shootingSideRight = shootingSide;
        if (fireRate > -1)
            anim.SetFloat("ShootingTime", 1f / fireRate);
        anim.Play("Shoot" + weaponType.ToString(), 1);
        anim.SetBool("Fire", true);
    }

    public void StopFireAnimation()
    {
        anim.SetBool("Fire", false);
    }

    public void PlayReloadAnimation(float reloadTime)
    {
        anim.SetFloat("ReloadTime", 1 / reloadTime);
        anim.SetBool("isReloading", true);
        anim.Play("Reload" + weaponType.ToString());
    }

    public void StopReloadAnimation()
    {
        anim.SetBool("isReloading", false);
        //anim.Play ("Reload" + weaponType.ToString ());
    }
    public void Move(float moveDir)
    {
        if (isGrounded || airControl)
        {
            anim.SetFloat("Speed", Mathf.Abs(moveDir));
            if (isFiring)
            {
                if (moveDir < 0 && shootingSideRight || moveDir > 0 && !shootingSideRight)
                {
                    anim.SetFloat("SpeedMultiplayer", -.7f);
                    curSpeed = firingSpeed;
                }
                else
                {
                    anim.SetFloat("SpeedMultiplayer", .7f);
                    curSpeed = firingSpeed;
                }
                Flip(shootingSideRight);
            }
            else
            {
                anim.SetFloat("SpeedMultiplayer", 1);
                if (moveDir != 0)
                    Flip(moveDir > 0);
                curSpeed = maxSpeed;
            }

            //anim.SetBool ("isWeapon", isWeapon);

            rb.velocity = new Vector2(moveDir * curSpeed, rb.velocity.y);

        }
    }

    public void Jump()
    {
        if (isGrounded && !anim.GetBool("Jump") && (jumpDelay <= 0))
        {
            isGrounded = false;
            jumpDelay = .2f;
            rb.AddForce(new Vector2(0f, jumpForce));
            isJumping = true;
        }
    }

    private void Flip(bool right)
    {
        isFacingRight = right;
        Vector3 theScale = transform.localScale;
        theScale.x = (right) ? 1 : -1;
        transform.localScale = theScale;
    }

    public Vector3 GetCharacterCenter()
    {
        return transform.position + Vector3.up;
    }

    public bool CanPickUp()
    {
        return !IsDead();
    }

    public void CanEnterDoor()
    {

    }

    public void EnterDoor()
    {

    }

    public bool CanHit()
    {
        return !IsDead();
    }

    float hitCooldown;
    public void OnCharacterHitted(CharacterBase hitCharacter, Weapon hitWeapon, int damage)
    {
        healthSystem.Damage(damage);

        t = 1;

        hitCooldown = .3f;

        if (OnHitted != null)
            OnHitted(hitCharacter, hitWeapon, damage);
    }
}