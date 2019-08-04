using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHandler : MonoBehaviour
{
    public int damage;
    public float bulletSize = .5f;
    public float speed = 5;
    float destroyDistance = 3;
    Vector3 direction;
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

        transform.position += direction * Time.fixedDeltaTime * speed;

        Debug.DrawRay(transform.position - direction * bulletSize, direction * (bulletSize * 2 + .02f));

        RaycastHit2D hit = Physics2D.Raycast(transform.position - (direction * bulletSize), direction, bulletSize * 2, hitLayers);

        if (hit.collider != null)
        {
            CharacterBase damagable = hit.transform.GetComponent<CharacterBase>();

            if (damagable != null)
                OnHit(hit, damagable);
            else if (!hit.collider.isTrigger) DestroyBullet();
        }
    }

    public void Setup(CharacterBase cb, Vector2 dir, int damage, float destroyDistance, Weapon weapon)
    {
        this.cb = cb;
        direction = (Vector3)dir.normalized;
        this.damage = damage;
        this.destroyDistance = destroyDistance;
        this.weapon = weapon;
        transform.localEulerAngles = new Vector3(0, 0, Vector3.Angle(Vector3.right, dir));
    }

    public void DestroyBullet()
    {
        Destroy(gameObject);
    }

    public void OnHit(RaycastHit2D hit, CharacterBase damagable)
    {
        if (damagable != cb)
        {
            if (damagable.CanHit())
            {
                damagable.OnCharacterHitted(cb, weapon, damage);
                Debug.Log("Hitted " + hit.collider.name + " damage =" + damage);
                DestroyBullet();
            }
        }
    }

}