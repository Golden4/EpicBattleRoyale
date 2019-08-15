using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterBase : EntityBase
{
    public GameAssets.CharacterList characterName;
    bool isInit;
    public Vector2 maxSpeed;
    public Vector2 firingSpeed;
    Vector2 curSpeed;
    public float jumpForce = 500;
    public bool airControl = true;
    public LayerMask whatIsGround;
    [HideInInspector]
    public bool isFacingRight = true;
    Transform groundCheck;
    bool isGrounded;
    bool isJumping;
    Animator anim;
    Rigidbody2D rb;

    [HideInInspector]
    public WeaponController.SlotType weaponType;
    [HideInInspector]
    public bool isFiring;
    [HideInInspector]
    public bool shootingSideRight;
    [HideInInspector]
    public Vector2 moveInput;
    bool isDead;
    public HealthSystem healthSystem;
    public InventorySystem inventorySystem;

    float curJumpPos;
    float curJumpVelocity;

    public event Action<CharacterBase, Weapon, int> OnHitted;
    public event EventHandler OnDie;

    List<Interactable> interactableObjects = new List<Interactable>();
    public event Action<Interactable> OnCanInteractEvent;
    public event Action<Interactable> OnCantInteractEvent;
    public event Action<Interactable> OnInteractEvent;

    void Awake()
    {
        if (!isInit)
            Setup();
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

        healthSystem.OnHealthZero += HealthSystem_OnHealthZero;
        MapsController.Ins.OnChangingMap += OnChangingMap;
        MapsController.Ins.OnEnterHouseEvent += OnEnterHouse;

        groundCheck = transform.Find("GroundCheck");
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        srs = GetComponentsInChildren<SkinnedMeshRenderer>();
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
        foreach (SkinnedMeshRenderer item in srs)
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

        if (curJumpPos > 0)
            isGrounded = false;
        else
            isGrounded = true;

        // RaycastHit2D[] hit = Physics2D.RaycastAll(groundCheck.position, Vector3.down, .2f);
        // for (int i = 0; i < hit.Length; i++)
        // {
        //     if (hit[i].collider != null && hit[i].collider.gameObject != gameObject && !hit[i].collider.isTrigger)
        //     {
        //         isGrounded = true;
        //         isJumping = false;
        //         break;
        //     }
        // }

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

        if (isGrounded && !anim.GetBool("Die") && isDead)
            anim.SetBool("Die", true);

        anim.SetBool("Jump", !isGrounded);
        Move(moveInput);
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
        // collider.offset = new Vector2(.89f, .34f);
        // collider.size = new Vector2(1.8f, .8f);
        // collider.direction = CapsuleDirection2D.Horizontal;
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

    SkinnedMeshRenderer[] srs;
    IEnumerator FadeCharacter(float delay = 2, float fadeTime = 2, Color fadeColor = default, Action OnEndFade = null)
    {
        yield return new WaitForSeconds(delay);
        float curFadeTime = fadeTime;

        while (curFadeTime > 0)
        {
            curFadeTime -= Time.deltaTime;

            foreach (SkinnedMeshRenderer item in srs)
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
    public void Move(Vector2 moveDir)
    {
        if (isGrounded || airControl)
        {
            float maginitude = Mathf.Abs(Mathf.Clamp01(moveDir.sqrMagnitude));

            anim.SetFloat("Speed", maginitude);

            if (isFiring)
            {
                if (moveDir.x < 0 && shootingSideRight || moveDir.x > 0 && !shootingSideRight)
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

                if (moveDir.x != 0)
                    Flip(moveDir.x > 0);

                curSpeed = maxSpeed;
            }

            float xPos = worldPosition.x + moveDir.x * curSpeed.x * Time.fixedDeltaTime;
            float yPos = worldPosition.y + moveDir.y * curSpeed.y * Time.fixedDeltaTime;

            MoveTo(new Vector2(xPos, yPos), curJumpPos);

            // rb.velocity = new Vector2(moveDir * curSpeed, rb.velocity.y);

        }
    }

    public void Jump()
    {
        if (isGrounded && !anim.GetBool("Jump") && (jumpDelay <= 0))
        {
            isGrounded = false;
            jumpDelay = .2f;
            isJumping = true;
            curJumpVelocity = jumpForce;
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
        return transform.position;
    }

    public bool CanPickUp()
    {
        return !IsDead();
    }

    public void EnterOrExitDoor(HouseDoor houseInfo)
    {
        if (MapsController.Ins.mapState == MapsController.State.Map)
        {
            if (houseInfo != null)
            {
                MapsController.Ins.EnterHouse(houseInfo);
            }
        }
        else MapsController.Ins.ExitHouse();
    }

    void OnEnterHouse(HouseDoor house)
    {

        MoveToPosition(new Vector3(MapsController.Ins.GetHouseData(house.houseType).worldEndPoints.x + 2, -4), true);
        ClearInteractableObjects();
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

        SpawnDamagePopUpText(hitCharacter, hitWeapon, damage);

        if (OnHitted != null)
            OnHitted(hitCharacter, hitWeapon, damage);
    }

    void SpawnDamagePopUpText(CharacterBase hitCharacter, Weapon hitWeapon, int damage)
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
        //rb.velocity = Vector3.zero;
        curJumpVelocity = 0;
        curJumpPos = 0;
        Flip(isFacingRight);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Interactable interactable = col.GetComponent<Interactable>();
        if (interactable != null)
            if (!interactableObjects.Contains(interactable) && interactable.CanInteract(this))
            {
                //Debug.Log("OnTriggerEnter2D" + interactable.name);
                interactableObjects.Add(interactable);

                if (OnCanInteractEvent != null)
                    OnCanInteractEvent(interactable);
            }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        Interactable interactable = col.GetComponent<Interactable>();

        if (interactable != null)
        {
            if (!interactableObjects.Contains(interactable) && interactable.CanInteract(this))
            {
                //Debug.Log("OnTriggerEnter2D" + interactable.name);
                interactableObjects.Add(interactable);

                if (OnCanInteractEvent != null)
                    OnCanInteractEvent(interactable);
            }

            if (interactableObjects.Contains(interactable) && !interactable.CanInteract(this))
            {
                //Debug.Log("OnTriggerExit2D" + interactable.name);
                interactable.AwayInteract(this);
                interactableObjects.Remove(interactable);

                if (OnCantInteractEvent != null)
                    OnCantInteractEvent(interactable);
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Interactable interactable = col.GetComponent<Interactable>();

        if (interactable != null)
            if (interactableObjects.Contains(interactable))
            {
                //Debug.Log("OnTriggerExit2D" + interactable.name);
                interactable.AwayInteract(this);
                interactableObjects.Remove(interactable);

                if (OnCantInteractEvent != null)
                    OnCantInteractEvent(interactable);
            }
    }

    void OnChangingMap(MapsController.MapInfo mapInfo, Direction dir)
    {
        //если вышли из дома
        if (dir == Direction.None)
        {
            Vector3 pos = MapsController.Ins.GetCurrentMapInfo().houses[MapsController.Ins.curHouseIndex].GetDoorPosition(mapInfo) + Vector3.up;
            MoveToPosition(pos, false);
        }
        else
        {
            int index = -1;

            index = MapsController.Ins.GetSpawnDirection(mapInfo, dir);

            if (index != -1)
            {
                bool isFacingRight = true;

                Vector3 pos = default;

                if (index == 1)
                {
                    isFacingRight = false;
                    pos = new Vector3(MapsController.Ins.GetCurrentWorldEndPoints().y - 2, worldPosition.y);
                }

                if (index == 0)
                {
                    pos = new Vector3(MapsController.Ins.GetCurrentWorldEndPoints().x + 2, worldPosition.y);
                }

                if (index == 2)
                {
                    pos = new Vector3(0, MapsController.Ins.GetCurrentWorldUpDownEndPoints().y);
                }

                MoveToPosition(pos, isFacingRight);

                Debug.Log(index + "   " + pos + "  " + MapsController.Ins.GetCurrentWorldEndPoints().x);
            }
        }


        ClearInteractableObjects();
    }

    void ClearInteractableObjects()
    {
        for (int i = 0; i < interactableObjects.Count; i++)
        {
            interactableObjects[i].AwayInteract(this);

            if (OnCantInteractEvent != null)
                OnCantInteractEvent(interactableObjects[i]);
        }
        interactableObjects.Clear();
    }


    void OnDestroy()
    {
        MapsController.Ins.OnChangingMap -= OnChangingMap;
    }
}