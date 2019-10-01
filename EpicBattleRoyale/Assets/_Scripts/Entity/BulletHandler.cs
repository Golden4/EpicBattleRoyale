using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHandler : EntityBase
{
    public int damage;
    public float bulletSize = .5f;
    public float speed = 5;
    float destroyDistance = 3;
    Vector2 direction;
    float zPosition;
    public LayerMask hitLayers;
    public CharacterBase cb;
    public Weapon weapon;

    void FixedUpdate()
    {
        destroyDistance -= Time.fixedDeltaTime * speed;

        if (destroyDistance <= 0)
        {
            DestroyBullet();
        }

        MoveTo((Vector2)worldPosition + direction * Time.fixedDeltaTime * speed, zPosition, false);

        RaycastHit2D[] hitArray = Physics2D.RaycastAll((Vector2)transform.position - (direction * bulletSize), direction, bulletSize * 2, hitLayers);
        //RaycastHit2D hit = Physics2D.Raycast ((Vector2) transform.position - (direction * bulletSize), direction, bulletSize * 2, hitLayers);

        for (int i = 0; i < hitArray.Length; i++)
        {
            if (hitArray[i].collider != null)
            {
                HitBox damagable = hitArray[i].collider.GetComponent<HitBox>();

                if (damagable != null)
                {
                    OnHit(hitArray[i], damagable);
                    break;
                }
                else if (!hitArray[i].collider.isTrigger)
                {
                    DestroyBullet();
                    break;
                }
            }
        }
    }

    public void Setup(CharacterBase cb, Vector2 dir, float zPosition, int damage, float destroyDistance, Weapon weapon)
    {
        this.cb = cb;
        direction = (Vector3)dir.normalized;
        this.damage = damage;
        this.destroyDistance = destroyDistance;
        this.weapon = weapon;
        this.zPosition = zPosition;
        transform.localEulerAngles = new Vector3(0, 0, Vector3.Angle(Vector3.right, dir));
    }

    public void DestroyBullet()
    {
        Destroy(gameObject);
    }

    public void OnHit(RaycastHit2D hit, HitBox damagable)
    {
        if (damagable != cb)
        {
            if (damagable.CanHit())
            {
                damagable.TakeHit(cb, weapon, damage);

                ParticlesController.Ins.PlayBloodSplashParticle(hit.point, direction);

                //                Debug.Log("Hitted with Bullet " + damagable.characterBase.name + " | damage = " + damage + " | hitBoxType = " + damagable.hitBoxType + " | weapon = " + weapon.weaponName);

                DestroyBullet();
            }
        }
    }

}