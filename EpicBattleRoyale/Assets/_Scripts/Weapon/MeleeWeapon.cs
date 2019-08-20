using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class MeleeWeapon : Weapon
{
    public enum State
    {
        Normal,
        Beating,
    }

    State curState;

    public override void OnUpdate()
    {
        switch (curState)
        {
            case State.Normal:
                if (Mathf.Abs(firingSideInput) > 0)
                {
                    curState = State.Beating;
                    StartCoroutine("BeatCoroutine");
                }
                break;
            default:
                break;
        }
    }

    IEnumerator BeatCoroutine()
    {
        bool side = firingSideInput < 0;
        wc.weaponAnimator.enabled = true;
        while (curState == State.Beating && isActive && Mathf.Abs(firingSideInput) > 0)
        {
            wc.PlayFireAnimation(-1, side);
            side = firingSideInput < 0;
            wc.cb.shootingSideRight = side;
            yield return new WaitForSeconds(fireRate / 2f);
            Beat(side);
            yield return new WaitForSeconds(fireRate / 2f);
        }
        wc.StopFireAnimation();
        wc.weaponAnimator.enabled = false;
        curState = State.Normal;
    }

    public void Beat(bool isFacingRight)
    {
        HitWithRaycast(Vector3.right * firingRange * ((isFacingRight) ? 1 : -1), firingRange);
    }

    bool CanBeat()
    {
        return curState == State.Normal;
    }

    public override bool isFiring()
    {
        return curState == State.Beating;
    }

    public override void OnWeaponSwitch(object sender, System.EventArgs e)
    {
        base.OnWeaponSwitch(sender, e);

        if (isActive)
        {
            wc.weaponAnimator.enabled = false;
        }
        else
        {
            wc.weaponAnimator.enabled = true;
        }

        if (curState == State.Beating)
        {
            StopCoroutine("BeatCoroutine");
        }

        wc.StopReloadAnimation();
        wc.StopFireAnimation();
        curState = State.Normal;
        firingSideInput = 0;
    }
}