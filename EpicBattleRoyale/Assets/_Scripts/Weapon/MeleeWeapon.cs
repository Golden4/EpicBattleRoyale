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

        while (curState == State.Beating && isActive && Mathf.Abs(firingSideInput) > 0)
        {
            wc.cb.PlayFireAnimation(-1, side);
            side = firingSideInput < 0;
            wc.cb.shootingSideRight = side;
            yield return new WaitForSeconds(fireRate / 2f);
            Beat(side);
            yield return new WaitForSeconds(fireRate / 2f);
        }
        wc.cb.StopFireAnimation();
        curState = State.Normal;
    }

    public void Beat(bool isFacingRight)
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(wc.cb.GetCharacterCenter(), Vector3.right * firingRange * ((isFacingRight) ? 1 : -1), firingRange);
        Debug.DrawRay(wc.cb.GetCharacterCenter(), Vector3.right * firingRange * ((isFacingRight) ? 1 : -1), Color.red, firingRange);

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
                        break;
                    }
                }
            }
        }
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

        if (curState == State.Beating)
        {
            StopCoroutine("BeatCoroutine");
        }

        wc.cb.StopReloadAnimation();
        wc.cb.StopFireAnimation();
        curState = State.Normal;
        firingSideInput = 0;
    }
}