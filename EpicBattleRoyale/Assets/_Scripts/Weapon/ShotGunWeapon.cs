using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGunWeapon : AutomaticWeapon
{

    public int shootingBulletCount = 5;

    public override void Shot(bool isFacingRight)
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(wc.cb.GetCharacterCenter(), muzzlePoint.transform.position - wc.cb.GetCharacterCenter(), Vector3.Distance(wc.cb.GetCharacterCenter(), muzzlePoint.transform.position));

        bool hittedWithRaycast = false;
        Debug.DrawLine(wc.cb.GetCharacterCenter(), muzzlePoint.transform.position, Color.red, Vector3.Distance(wc.cb.GetCharacterCenter(), muzzlePoint.transform.position));

        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider != null)
            {
                HitBox damagable = hit[i].transform.GetComponent<HitBox>();

                if (damagable != null && damagable.characterBase != wc.cb)
                {
                    if (damagable.CanHit())
                    {
                        damagable.OnHitted(wc.cb, this, damage);
                        Debug.Log("Hitted with raycast" + hit[i].collider.name + " damage =" + damage);
                        hittedWithRaycast = true;
                        break;
                    }
                }
            }
        }

        int curBulletDamage = Mathf.RoundToInt((float)damage / shootingBulletCount);

        for (int i = 0; i < shootingBulletCount; i++)
        {
            if (!hittedWithRaycast)
                SpawnBullet(isFacingRight, Vector2.right + Vector2.up * Random.Range(-.1f, .1f), curBulletDamage);
        }


        bulletSystem.ShotBullet(1);
        Shell.SpawnShell(shellPoint.position, shellPoint.localEulerAngles, weaponType);
        muzzleFlash.Emit(1);
        AudioManager.PlaySound(fireSound);
    }
}
