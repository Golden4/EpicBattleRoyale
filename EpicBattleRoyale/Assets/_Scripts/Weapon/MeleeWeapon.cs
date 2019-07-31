using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

			if (Input.GetButton ("Fire1")) {

				curState = State.Beating;
			}
			break;
		case State.Beating:
			if (!Input.GetButton ("Fire1")) {
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
