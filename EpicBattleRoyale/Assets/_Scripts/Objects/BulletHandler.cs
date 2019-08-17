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
    public CharacterBase cb;
    public Weapon weapon;
    public LayerMask hitLayers;

    void FixedUpdate()
    {
        destroyDistance -= Time.fixedDeltaTime * speed;

        if (destroyDistance <= 0)
        {
            DestroyBullet();
        }

        MoveTo((Vector2)worldPosition + direction * Time.fixedDeltaTime * speed, zPosition);

        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position - (direction * bulletSize), direction, bulletSize * 2, hitLayers);

        if (hit.collider != null)
        {
            HitBox damagable = hit.collider.GetComponent<HitBox>();
            if (damagable != null)
                OnHit(hit, damagable);

            else if (!hit.collider.isTrigger) DestroyBullet();
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
                damagable.OnHitted(cb, weapon, damage);

                Debug.Log("Hitted" + hit.collider.name + " damage =" + damage + "  hitBoxType = " + damagable.hitBoxType);
                DestroyBullet();
            }
        }
    }

}