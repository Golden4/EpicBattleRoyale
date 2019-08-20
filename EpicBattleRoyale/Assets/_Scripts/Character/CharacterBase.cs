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

    public event Action<CharacterBase, Weapon, int, HitBox.HitBoxType> OnHittedEvent;
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

        Material mat = Resources.Load<Material>("Materials/FillableMaterial");

        inventorySystem = new InventorySystem(this);

        foreach (var item in renderers)
        {
            item.sharedMaterial = mat;
        }

        MapsController.Ins.ChangeMap(this, Vector2Int.zero, Direction.Right);

        isInit = true;
    }

    float jumpDelay;
    float lerpSpeed = .1f;

    void Update()
    {
        jumpDelay -= Time.deltaTime;
        hitCooldown -= Time.deltaTime;
        // if (t > 0)
        // {
        //     t -= Time.deltaTime / lerpSpeed;

        //     Timer();

        // }
        // else if (t <= -0.01f)
        // {
        //     t = 0;

        //     TimerEnd();
        // }
    }

    // public void Timer()
    // {
    //     LerpCharacter(t, Color.red, Color.white);
    // }

    // public void TimerEnd()
    // {
    //     LerpCharacter(1, Color.red, Color.white);
    // }

    public void LerpCharacter()
    {
        foreach (Renderer item in renderers)
        {
            if (item.material.shader.name == "Spine/SkeletonFill")
            {
                item.material.SetColor("_FillColor", Color.white);
                item.material.DOFloat(0, "_FillAlpha", .1f).ChangeStartValue(1);//.SetFloat("_FillAlpha", t);
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
        if (anim.GetBool("Die") && isDead)
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
        // StartCoroutine(FadeCharacter(2, 3, Color.clear, delegate
        //          {
        //              World.Ins.allCharacters.Remove(this);
        //              Destroy(gameObject);
        //          }));
        // Utility.LerpSprite(this, srs, 2, 3, Color.clear, delegate
        //  {
        //      World.Ins.allCharacters.Remove(this);
        //      Destroy(gameObject);
        //  });

        FadeCharacter(delegate
         {
             World.Ins.allCharacters.Remove(this);
             Destroy(gameObject);
         });

        if (OnDie != null)
            OnDie(this, EventArgs.Empty);
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

    public void EnterOrExitDoor(HouseDoor houseInfo)
    {
        if (MapsController.Ins.mapState == MapsController.State.Map)
        {
            if (houseInfo != null)
            {
                MapsController.Ins.EnterHouseWithFade(houseInfo);
            }
        }
        else MapsController.Ins.ExitHouseWithFade(this);
    }

    void OnEnterHouse(HouseDoor house)
    {
        MoveToPosition(new Vector3(MapsController.Ins.GetHouseData(house.houseType).worldEndPoints.x + 2, -4), true);

        //transform.SetParent(MapsController.Ins.GetHouseGO(house).transform);

        ClearInteractableObjects();
    }

    public bool CanHit()
    {
        return !IsDead();
    }

    float hitCooldown;

    public void OnHitted(CharacterBase hitCharacter, Weapon hitWeapon, int damage, HitBox.HitBoxType hitBoxType)
    {
        damage = Mathf.RoundToInt((float)damage * HitBox.GetDamagePersent(hitBoxType));
        healthSystem.Damage(damage);

        LerpCharacter();

        hitCooldown = .3f;

        SpawnDamagePopUpText(hitCharacter, hitWeapon, damage);

        if (OnHittedEvent != null)
            OnHittedEvent(hitCharacter, hitWeapon, damage, hitBoxType);
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

    void OnChangingMap(CharacterBase characterBase, MapsController.MapInfo mapInfo, Direction dir)
    {
        if (characterBase == this)
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
                }
            }

            //transform.SetParent(MapsController.Ins.curMaps[mapInfo.coord].transform);

            ClearInteractableObjects();
        }
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
        MapsController.Ins.OnEnterHouseEvent -= OnEnterHouse;
    }
}