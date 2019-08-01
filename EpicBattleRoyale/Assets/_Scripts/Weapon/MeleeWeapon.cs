using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class MeleeWeapon : Weapon {

	public enum State {
		Normal,
		Beating,
	}

	State curState;

	void Update ()
	{
		if (!isActive || wc.curState == WeaponController.State.Switching)
			return;
		
		shootingSide = CrossPlatformInputManager.GetAxisRaw ("Shoot");
		switch (curState) {
		case State.Normal:
			if (Mathf.Abs (shootingSide) > 0) {
				wc.cb.PlayShootAnimation (fireRate, shootingSide < 0);
				curState = State.Beating;
			}
			break;
		case State.Beating:
			if (Mathf.Abs (shootingSide) == 0) {
				wc.cb.StopShootAnimation ();
				curState = State.Normal;
			}
			break;

		default:
			break;
		}
	}

	public override bool isShooting ()
	{
		return curState == State.Beating;
	}

	public override void OnWeaponSwitch (object sender, System.EventArgs e)
	{
		base.OnWeaponSwitch (sender, e);
		wc.cb.StopReloadAnimation ();
		wc.cb.StopShootAnimation ();
		curState = State.Normal;
		shootingSide = 0;
	}
}
