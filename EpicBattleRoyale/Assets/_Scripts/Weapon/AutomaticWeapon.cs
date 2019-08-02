using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

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
    public State curState;
    public float reloadTime = 2;
    public float shootAnimationTime = .2f;
    public Transform muzzlePoint;
    public event Action<float> OnReload;
    public event Action OnReloadComplete;
    public event Action<int> OnShot;

    public override void Setup(WeaponController wc)
    {
        base.Setup(wc);
        bulletSystem.GiveBullets(10);
        bulletSystem.GiveBulletsStock(20);
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

    public bool Reload()
    {
        if (CanReload())
        {
            curState = State.Reloading;
            StartCoroutine("ReloadCoroutine");
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanReload()
    {
        return curState != State.Reloading && bulletSystem.CanReload();
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
        /*	if (!bulletTracerPS.isPlaying) {
			bulletTracerPS.Play ();//Воспроизводим партикл
		}*/

        /*	if (GetCurrentWeapon ().muzzleFlash != null)
			muzzleFlash.Activate (GetCurrentWeapon ().fireRate / 3);*/

        RaycastHit2D[] hit = Physics2D.RaycastAll(wc.cb.GetCharacterCenter(), muzzlePoint.transform.position - wc.cb.GetCharacterCenter(), Vector3.Distance(wc.cb.GetCharacterCenter(), muzzlePoint.transform.position));
        bool hittedWithRaycast = false;
        Debug.DrawLine(wc.cb.GetCharacterCenter(), muzzlePoint.transform.position, Color.red, Vector3.Distance(wc.cb.GetCharacterCenter(), muzzlePoint.transform.position));
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider != null)
            {
                CharacterBase damagable = hit[i].transform.GetComponent<CharacterBase>();

                if (damagable != null && damagable != wc.cb)
                {
                    if (damagable.CanHit())
                    {
                        damagable.OnCharacterHitted(wc.cb, this, damage);
                        Debug.Log("Hitted with raycast" + hit[i].collider.name + " damage =" + damage);
                        hittedWithRaycast = true;
                        break;
                    }
                }
            }
        }

        if (!hittedWithRaycast)
            SpawnBullet(isFacingRight);

        bulletSystem.ShotBullet(1);
    }

    protected void SpawnBullet(bool isFacingRight, Vector2 direction = default, int bulletDamage = -1)
    {
        GameObject bullet = Instantiate(GameAssets.Get.pfBullet.gameObject);
        bullet.transform.position = muzzlePoint.transform.position;
        int curBulletDamage = damage;

        if (bulletDamage != -1)
            curBulletDamage = bulletDamage;

        if (direction == default)
        {
            bullet.GetComponent<BulletHandler>().Setup(wc.cb, Vector3.right * (isFacingRight ? 1 : -1), curBulletDamage, firingRange, this);
        }
        else
        {
            bullet.GetComponent<BulletHandler>().Setup(wc.cb, direction * (isFacingRight ? 1 : -1), curBulletDamage, firingRange, this);
        }
    }

    IEnumerator ShootCoroutine()
    {
        while (curState == State.Shooting && isActive && Mathf.Abs(firingSideInput) > 0 && Bullets > 0)
        {
            bool side = firingSideInput < 0;
            wc.cb.PlayFireAnimation(shootAnimationTime, side);
            yield return new WaitForSeconds(shootAnimationTime / 2f);

            if (OnShot != null)
                OnShot(Bullets);

            Shot(side);
            yield return new WaitForSeconds(shootAnimationTime / 2f);
            wc.cb.StopFireAnimation();
            yield return new WaitForSeconds(fireRate - shootAnimationTime);
        }
        curState = State.Normal;
    }

    IEnumerator ReloadCoroutine()
    {
        wc.cb.PlayReloadAnimation(reloadTime);

        if (OnReload != null)
            OnReload(reloadTime);

        Debug.Log("Reloading " + reloadTime + "s.");

        yield return new WaitForSeconds(reloadTime);

        if (curState == State.Reloading && isActive)
        {
            bulletSystem.ReloadBullets();
            curState = State.Normal;
            wc.cb.StopReloadAnimation();
            if (OnReloadComplete != null)
                OnReloadComplete();
            Debug.Log("Reload was done");
        }
    }

    public override void OnWeaponSwitch(object sender, System.EventArgs e)
    {
        base.OnWeaponSwitch(sender, e);
        if (curState == State.Reloading)
        {
            StopCoroutine("ReloadCoroutine");
        }
        if (curState == State.Shooting)
        {
            StopCoroutine("ShootCoroutine");
        }
        curState = State.Normal;
        wc.cb.StopReloadAnimation();
        wc.cb.StopFireAnimation();
        firingSideInput = 0;
    }

}