using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityStandardAssets.CrossPlatformInput;

public class AutomaticWeapon : Weapon {
	
	public int BulletsStock {
		get {
			return bulletSystem.GetCurBulletsStock ();
		}
	}

	public int Bullets {
		get {
			return bulletSystem.GetCurrentBullets ();
		}
	}

	public enum ShootMode {
		One,
		Burst,
		Automatic
	}

	public enum State {
		Normal,
		Shooting,
		ShotCooldown,
		Reloading,
	}

	public BulletSystem bulletSystem;

	public ShootMode mode;

	public State curState;

	public float reloadTime = 2;
	public float shootAnimationTime = .2f;

	public Transform muzzlePoint;

	public event Action<float> OnReload;
	public event Action OnReloadComplete;
	public event Action<int> OnShot;

	public override void Setup (WeaponController wc)
	{
		base.Setup (wc);
		bulletSystem.GiveBullets (10);
		bulletSystem.GiveBulletsStock (20);
	}

	public bool isFiring;

	public void Update ()
	{
		if (!isActive || wc.curState == WeaponController.State.Switching)
			return;
		
		shootingSide = CrossPlatformInputManager.GetAxisRaw ("Shoot");
		switch (curState) {
		case State.Normal:
			
			/*if (Input.GetKeyDown (KeyCode.R))
				Reload ();*/


			if (Mathf.Abs (shootingSide) > 0) {
				isFiring = true;
				if (Bullets <= 0) {
					Reload ();
				} else if (CanShoot ()) {
						curState = State.Shooting;					
						StartCoroutine ("ShootCoroutine");
					} 
			}

			break;
		case State.Shooting:
			
			if (Bullets <= 0) {
				Reload ();
			}

			if (Mathf.Abs (shootingSide) == 0) {
				isFiring = false;
			}

			break;
		default:
			break;
		}
	}

	public bool Reload ()
	{
		if (CanReload ()) {
			curState = State.Reloading;
			StartCoroutine ("ReloadCoroutine");
			return true;
		} else {
			return false;
		}
	}

	public bool CanReload ()
	{
		return curState != State.Reloading && bulletSystem.CanReload ();
	}

	public override bool isShooting ()
	{
		return curState == State.Shooting;
	}

	public bool CanShoot ()
	{
		return Bullets > 0 && curState == State.Normal;
	}

	void Shot (bool isFacingRight)
	{
		/*	if (!bulletTracerPS.isPlaying) {
			bulletTracerPS.Play ();//Воспроизводим партикл
		}*/

		/*	if (GetCurrentWeapon ().muzzleFlash != null)
			muzzleFlash.Activate (GetCurrentWeapon ().fireRate / 3);*/
		GameObject bullet = Instantiate (GameAssets.Get.pfBullet.gameObject);
		bullet.transform.position = muzzlePoint.transform.position;
		bullet.GetComponent<BulletHandler> ().Setup (wc.cb, Vector3.right * (isFacingRight ? 1 : -1), damage);
		bulletSystem.ShotBullet (1);

		if (OnShot != null)
			OnShot (Bullets);
	}

	IEnumerator ShootCoroutine ()
	{
		while (curState == State.Shooting && isActive && isFiring && Bullets > 0) {
			bool side = shootingSide < 0;
			wc.cb.PlayShootAnimation (shootAnimationTime, side);
			yield return new WaitForSeconds (shootAnimationTime / 2f);
			Shot (side);
			yield return new WaitForSeconds (shootAnimationTime / 2f);
			wc.cb.StopShootAnimation ();
			yield return new WaitForSeconds (fireRate - shootAnimationTime);
		}
		curState = State.Normal;
	}

	IEnumerator ReloadCoroutine ()
	{
		wc.cb.PlayReloadAnimation (reloadTime);

		if (OnReload != null)
			OnReload (reloadTime);
		
		Debug.Log ("Reloading " + reloadTime + "s.");

		yield return new WaitForSeconds (reloadTime);

		if (curState == State.Reloading && isActive) {
			bulletSystem.ReloadBullets ();
			curState = State.Normal;
			wc.cb.StopReloadAnimation ();
			if (OnReloadComplete != null)
				OnReloadComplete ();
			Debug.Log ("Reload was done");
		}
	}

	public override void OnWeaponSwitch (object sender, System.EventArgs e)
	{
		base.OnWeaponSwitch (sender, e);
		if (curState == State.Reloading) {
			StopCoroutine ("ReloadCoroutine");
		}
		if (curState == State.Shooting) {
			StopCoroutine ("ShootCoroutine");
		}
		curState = State.Normal;
		wc.cb.StopReloadAnimation ();
		wc.cb.StopShootAnimation ();
		isFiring = false;
		shootingSide = 0;
	}
}
