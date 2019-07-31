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
		if (!isActive)
			return;
		
		switch (curState) {
		case State.Normal:
			
			shootingSide = CrossPlatformInputManager.GetAxisRaw ("Shoot");
			if (Mathf.Abs (shootingSide) > 0) {
				wc.cb.PlayShootAnimation (fireRate, shootingSide < 0);
				curState = State.Beating;
			}
			break;
		case State.Beating:
			if (Mathf.Abs (shootingSide) < 0) {
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
}
