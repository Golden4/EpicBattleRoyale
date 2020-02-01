using System.Linq.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AutomaticWeapon : Weapon
{
    public int BulletsStock
    {
        get
        {
            return bulletSystem.GetCurBulletsStock();
        }
    }

    public int Bullets
    {
        get
        {
            return bulletSystem.GetCurrentBullets();
        }
    }

    public enum ShootMode
    {
        One,
        Burst,
        Automatic
    }

    public enum State
    {
        Normal,
        Shooting,
        Reloading,
    }

    public BulletSystem bulletSystem;
    public ShootMode mode;
    [HideInInspector]
    public State curState;
    public float reloadTime = 2;
    public float shootAnimationTime = .2f;
    public Transform muzzlePoint;
    protected ParticleSystem muzzleFlash;
    protected Transform shellPoint;
    [HideInInspector]
    public float reloadingProgress;
    public Sound[] reloadSounds;

    public event Action<float> OnReload;
    public event Action OnReloadComplete;
    public event Action<int> OnShot;

    public override void Setup(WeaponController wc)
    {
        base.Setup(wc);
        shellPoint = transform.Find("ShellPoint");

        GameObject go = Instantiate(GameAssets.Get.pfMuzzleFlash.gameObject);
        muzzleFlash = go.GetComponent<ParticleSystem>();
        muzzleFlash.transform.SetParent(muzzlePoint, false);
        muzzleFlash.transform.localPosition = Vector3.right * .2f;
        bulletSystem.Setup(wc.cb.characterInventory);
        bulletSystem.GiveBullets(10);
    }

    public override void OnUpdate()
    {
        switch (curState)
        {
            case State.Normal:
                if (Mathf.Abs(firingSideInput) > 0)
                {
                    if (Bullets <= 0)
                    {
                        Reload();
                    }
                    else if (CanShoot())
                    {
                        curState = State.Shooting;
                        StartCoroutine("ShootCoroutine");
                    }
                }
                break;
            case State.Shooting:

                if (Bullets <= 0)
                {
                    Reload();
                }

                break;

            default:
                break;
        }
    }

    public override void OnFixedUpdate()
    {
        switch (curState)
        {
            case State.Reloading:
                reloadingProgress -= Time.fixedDeltaTime;

                if (reloadingProgress < 0)
                {
                    reloadingProgress = 0;
                    bulletSystem.ReloadBullets();
                    curState = State.Normal;
                    wc.StopReloadAnimation();
                    if (OnReloadComplete != null)
                        OnReloadComplete();
                }

                break;
            default:
                break;
        }
    }

    public bool Reload()
    {
        if (CanReload())
        {
            reloadingProgress = reloadTime;
            curState = State.Reloading;
            // StartCoroutine("ReloadCoroutine");
            wc.PlayReloadAnimation(reloadTime);
            wc.cb.EndHeal();

            if (OnReload != null)
                OnReload(reloadTime);

            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanReload()
    {
        return curState != State.Reloading && bulletSystem.CanReload() && reloadingProgress == 0;
    }

    public override bool isFiring()
    {
        return curState == State.Shooting;
    }

    public bool CanShoot()
    {
        return Bullets > 0 && curState == State.Normal;
    }

    public virtual void Shot(bool isFacingRight)
    {

        if (wc.cb.IsDead())
            return;

        /*	if (!bulletTracerPS.isPlaying) {
			bulletTracerPS.Play ();//Воспроизводим партикл
		}*/

        /*	if (GetCurrentWeapon ().muzzleFlash != null)
			muzzleFlash.Activate (GetCurrentWeapon ().fireRate / 3);*/

        bool hittedWithRaycast = HitWithRaycast(muzzlePoint.transform.position - wc.cb.GetCharacterCenter(), Vector3.Distance(wc.cb.GetCharacterCenter(), muzzlePoint.transform.position));

        if (!hittedWithRaycast)
            SpawnBullet(isFacingRight, Vector2.right + Vector2.up * UnityEngine.Random.Range(-.05f, .05f));

        bulletSystem.ShotBullet(1);

        Shell.SpawnShell(shellPoint.position, shellPoint.localEulerAngles, weaponType);

        if (wc.cb.isPlayer)
            CameraController.Ins.ShakeCamera(fireRate / 1.5f);

        // ParticlesController.Ins.PlayMuzzleFlashParticle(muzzlePoint.transform.position + Vector3.right * .2f, muzzlePoint.transform.localEulerAngles);

        muzzleFlash.Play(true);

        wc.cb.characterAudio.PlaySound(fireSound);
        //AudioManager.PlaySoundAtObject(fireSound, gameObject);
        //AudioManager.PlaySound(fireSound);
    }

    protected void SpawnBullet(bool isFacingRight, Vector2 direction = default, int bulletDamage = -1)
    {
        GameObject bullet = Instantiate(GameAssets.Get.pfBullet.gameObject);
        BulletHandler bh = bullet.GetComponent<BulletHandler>();

        Vector2 position = (Vector2)wc.cb.worldPosition + Vector2.right * (-wc.cb.worldPosition.x + muzzlePoint.transform.position.x) + Vector2.right * (isFacingRight ? 1 : -1) * .3f;
        float zPosition = /*-(wc.cb.worldPosition.y) */ Mathf.Abs(-wc.cb.worldPosition.y + muzzlePoint.transform.position.y);
        int curBulletDamage = damage;

        if (bulletDamage != -1)
            curBulletDamage = bulletDamage;

        if (direction == default)
        {
            bh.Setup(wc.cb, Vector3.right * (isFacingRight ? 1 : -1), zPosition, curBulletDamage, firingRange, this);
        }
        else
        {
            bh.Setup(wc.cb, direction * (isFacingRight ? 1 : -1), zPosition, curBulletDamage, firingRange, this);
        }
        bh.MoveTo(position, zPosition);
    }

    IEnumerator ShootCoroutine()
    {
        while (curState == State.Shooting && isActive && Mathf.Abs(firingSideInput) > 0 && Bullets > 0)
        {
            bool side = firingSideInput < 0;
            wc.PlayAimAnimation();
            wc.PlayFireAnimation(.167f, shootAnimationTime, side);
            yield return new WaitForSeconds(shootAnimationTime / 2f);

            if (OnShot != null)
                OnShot(Bullets);

            Shot(side);
            yield return new WaitForSeconds(shootAnimationTime / 2f);
            wc.StopFireAnimation();
            yield return new WaitForSeconds(fireRate - shootAnimationTime);
            wc.StopAimAnimation();
        }

        if (curState == State.Shooting)
            curState = State.Normal;
    }

    IEnumerator ReloadCoroutine()
    {
        wc.PlayReloadAnimation(reloadTime);

        if (OnReload != null)
            OnReload(reloadTime);

        yield return new WaitForSeconds(reloadTime);

        if (curState == State.Reloading && isActive)
        {
            bulletSystem.ReloadBullets();
            curState = State.Normal;
            wc.StopReloadAnimation();
            if (OnReloadComplete != null)
                OnReloadComplete();
        }
    }

    public override void OnWeaponSwitch(Weapon weapon)
    {
        base.OnWeaponSwitch(weapon);

        // if (curState == State.Reloading)
        // {
        //     StopCoroutine("ReloadCoroutine");
        // }

        if (curState == State.Shooting)
        {
            StopCoroutine("ShootCoroutine");
        }
        reloadingProgress = 0;
        curState = State.Normal;
        wc.StopReloadAnimation();
        wc.StopFireAnimation();
        firingSideInput = 0;
    }

}