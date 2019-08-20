using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGunWeapon : AutomaticWeapon
{

    public int shootingBulletCount = 5;

    public override void Shot(bool isFacingRight)
    {

        bool hittedWithRaycast = HitWithRaycast(muzzlePoint.transform.position - wc.cb.GetCharacterCenter(), Vector3.Distance(wc.cb.GetCharacterCenter(), muzzlePoint.transform.position));

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
