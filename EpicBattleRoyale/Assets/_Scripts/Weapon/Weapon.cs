using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WeaponType
    {
        Melee,
        Pistol,
        Automatic,
        Sniper,
        ShotGun
    }

    public GameAssets.WeaponsList weaponName;
    public WeaponType weaponType;
    public WeaponController.SlotType slotType;
    public Sprite sprite;
    public Vector2 m_damage = new Vector2(30, 50);
    public RuntimeAnimatorController animatorController;
    public AnimatorOverrideController overrideController;

    public float firingRange = 2;

    public int damage
    {
        get
        {
            return (int)Random.Range(m_damage.x, m_damage.y);
        }
    }

    public float fireRate = 0.05f;
    public Sound fireSound;
    [HideInInspector]
    public bool isActive;
    [HideInInspector]
    public float firingSideInput;
    protected WeaponController wc;

    /*	[Header ("Muzzle Flash and Tracer")]
	//эффект выстрела патронами
	public ParticleSystem bulletTracerPS;

	//Воспризвести звук по имени
	public void PlaySound (string _name)
	{
		for (int i = 0; i < sounds.Length; i++) {
			if (sounds [i].name == _name) {
				sounds [i].PlaySound ();
			}
		}

		Debug.LogWarning ("Error. Audio is not found!");
	}*/

    /*	bool canShoot ()
	{
		#if UNITY_EDITOR
		return (!IsInvoking ("Shot") && Time.time > lastTimeShot + GetCurrentWeapon ().fireRate && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ());
		#else
		return (!IsInvoking ("Shot") && Time.time > lastTimeShot + GetCurrentWeapon ().fireRate && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject (Input.touches [0].fingerId));
		#endif
	}*/

    public virtual bool CanFiring()
    {
        return false;
    }

    public virtual void Setup(WeaponController wc)
    {

        this.wc = wc;
        wc.OnWeaponSwitch += OnWeaponSwitch;
    }

    public virtual void OnWeaponSwitch(object sender, System.EventArgs e)
    {
    }

    public virtual void OnUpdate()
    {

    }

    public virtual void OnFixedUpdate()
    {

    }

    public virtual bool isFiring()
    {
        return false;
    }

    public virtual float GetFiringRange()
    {
        return firingRange;
    }

    public WeaponType GetWeaponType()
    {
        return weaponType;
    }

    public Sprite GetWeaponSprite()
    {
        return sprite;
    }

    public WeaponController.SlotType GetSlotType()
    {
        return slotType;
    }

    public bool WeaponIs(System.Type type)
    {
        return (type.IsAssignableFrom(this.GetType()));
    }

    public T GetWeaponWithType<T>() where T : Weapon
    {
        if (WeaponIs(typeof(T)))
            return (T)this;

        return null;
    }

    public void OnPickUp()
    {

    }

    public bool HitWithRaycast(Vector2 direction, float distance, int raycastDamage = -1)
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(wc.cb.GetCharacterCenter(), direction, distance);

        Debug.DrawRay(wc.cb.GetCharacterCenter(), direction, Color.red, distance);

        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider != null)
            {
                HitBox damagable = hit[i].collider.GetComponent<HitBox>();

                if (damagable != null && damagable.characterBase != wc.cb && wc.cb.CompareEntitiesPositions(damagable.characterBase.worldPosition))
                {
                    if (damagable.CanHit())
                    {
                        if (raycastDamage != -1)
                            damagable.TakeHit(wc.cb, this, raycastDamage);
                        else
                            damagable.TakeHit(wc.cb, this, damage);

                        ParticlesController.Ins.PlayBloodSplashParticle(hit[i].point, direction);

                        Debug.Log("Hitted with raycast " + damagable.characterBase.name + " damage = " + damage + "  hitBoxType = " + damagable.hitBoxType);
                        return true;
                    }
                }
            }
        }
        return false;
    }
}