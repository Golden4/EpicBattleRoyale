using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponIndex : MonoBehaviour {
	
	public int indexWeapon;

	public static event System.Action<int,GameObject> OnClickSelectWeaponEvent;

	public void OnMouseDown ()
	{
		if (OnClickSelectWeaponEvent != null) {
			OnClickSelectWeaponEvent (indexWeapon, gameObject);
		}
	}
}
