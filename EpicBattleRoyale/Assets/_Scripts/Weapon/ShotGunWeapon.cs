using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGunWeapon : AutomaticWeapon
{
    public int shootingBulletCount = 5;

    public override void Shot(bool isFacingRight)
    {
        int curBulletDamage = Mathf.RoundToInt((float)damage / shootingBulletCount);

        for (int i = 0; i < shootingBulletCount; i++)
        {
            Vector2 dir = Vector2.right + Vector2.up * Random.Range(-.1f, .1f);
            if (!HitWithRaycast(((isFacingRight) ? 1 : -1) * Vector2.right + Vector2.up * Random.Range(-.1f, .1f), Vector3.Distance(wc.cb.GetCharacterCenter(), muzzlePoint.transform.position), curBulletDamage))
                SpawnBullet(isFacingRight, dir, curBulletDamage);
        }

        bulletSystem.ShotBullet(1);
        Shell.SpawnShell(shellPoint.position, shellPoint.localEulerAngles, weaponType);
        muzzleFlash.Play(true);
        wc.cb.characterAudio.PlaySound(fireSound);
        //AudioManager.PlaySoundAtObject(fireSound, gameObject);
        //AudioManager.PlaySound(fireSound);
    }
}
