using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameAssets.WeaponsList weaponName;
    public WeaponType weaponType;
    public WeaponController.SlotType slotType;

    public bool isActive;
    public Vector2 m_damage = new Vector2(30, 50);
    public float firingRange = 2;
    public Sprite sprite;
    protected WeaponController wc;
    public int damage
    {
        get
        {
            return (int)Random.Range(m_damage.x, m_damage.y);
        }
    }

    public enum WeaponType
    {
        Melee,
        Pistol,
        Automatic,
        Sniper,
        ShotGun
    }

    public float firingSideInput;

    public float fireRate = 0.05f;

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

}