using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    public float animationTime;
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
                else
                {
                    if (wc.cb.animator.GetLayerWeight(1) == 1)
                    {
                        wc.cb.animator.SetLayerWeight(1, 0);
                    }

                }
                break;
            case State.Beating:
                break;
            default:
                break;
        }
    }

    IEnumerator BeatCoroutine()
    {
        bool side = firingSideInput < 0;
        wc.cb.animator.SetLayerWeight(1, 1);
        while (curState == State.Beating && isActive && Mathf.Abs(firingSideInput) > 0)
        {
            wc.PlayFireAnimation(animationTime, animationTime, side);
            side = firingSideInput < 0;
            wc.cb.shootingSideRight = side;
            yield return new WaitForSeconds(fireRate / 2f);
            Beat(side);
            yield return new WaitForSeconds(fireRate / 2f);
        }
        wc.StopFireAnimation();
        wc.cb.animator.SetLayerWeight(1, 0);
        curState = State.Normal;
    }

    public void Beat(bool isFacingRight)
    {
        HitWithRaycast(Vector3.right * firingRange * ((isFacingRight) ? 1 : -1) + Vector3.up * Random.Range(-.5f, .5f), firingRange);
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

        if (weaponName == GameAssets.WeaponsList.Fists && isActive)
            wc.cb.animator.SetLayerWeight(1, 0);
        else
            wc.cb.animator.SetLayerWeight(1, 1);



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