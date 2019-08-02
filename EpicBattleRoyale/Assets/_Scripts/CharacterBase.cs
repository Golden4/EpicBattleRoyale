using System;
using System.Collections;
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
    public event EventHandler OnPickUp;
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
        Debug.Log(anim);
        rb = GetComponent<Rigidbody2D>();
        isInit = true;
    }

    float jumpDelay;

    void Update()
    {
        jumpDelay -= Time.deltaTime;
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

        StartCoroutine(FadeOutCharacter(2, 3, delegate
        {
            World.Ins.allCharacters.Remove(this);
            Destroy(gameObject);
        }));

        if (OnDie != null)
            OnDie(this, EventArgs.Empty);
    }

    IEnumerator FadeOutCharacter(float delay = 2, float fadeOutTime = 2, Action OnEndFadeOut = null)
    {
        yield return new WaitForSeconds(delay);

        float curFadeTime = fadeOutTime;
        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();

        while (curFadeTime > 0)
        {
            curFadeTime -= Time.deltaTime;
            foreach (SpriteRenderer item in srs)
            {
                Color color = item.color;

                if (curFadeTime <= 0.05f)
                {
                    color.a = 0;
                    curFadeTime = 0;
                }
                else
                {
                    color.a = curFadeTime / fadeOutTime;
                }

                item.color = color;
            }

            yield return null;
        }
        if (OnEndFadeOut != null)
            OnEndFadeOut();

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

    public void OnCharacterPickUp(ItemPickUp item)
    {
        if (OnPickUp != null)
            OnPickUp(item, EventArgs.Empty);
    }

    public bool CanPickUp()
    {
        return !IsDead();
    }

    public bool CanHit()
    {
        return !IsDead();
    }

    public void OnCharacterHitted(CharacterBase hitCharacter, Weapon hitWeapon, int damage)
    {
        healthSystem.Damage(damage);

        if (OnHitted != null)
            OnHitted(hitCharacter, hitWeapon, damage);
    }
}