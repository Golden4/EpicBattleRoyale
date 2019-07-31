using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(CharacterBase))]
public class Player : MonoBehaviour {
	public static Player Ins;
	public CharacterBase characterBase;
	public WeaponController weaponController;

	void Awake ()
	{
		Ins = this;
		characterBase = GetComponent<CharacterBase> ();
		weaponController = GetComponent<WeaponController> ();
	}

	Vector3[] positionsToMove = new Vector3[3] {
		Vector3.right * -22,
		Vector3.right * 22,
		Vector3.zero
	};

	void Start ()
	{
		MapsController.OnChangingMap += OnChangingMap;
	}

	void OnChangingMap (MapsController.MapInfo arg1, Direction arg2)
	{
		int index = -1;

		Direction[] dir1 = new Direction[] {
			Direction.Bottom, Direction.Left, Direction.Right, Direction.Top
		};

		Direction[] dir2 = new Direction[] {
			Direction.Top, Direction.Right, Direction.Left, Direction.Bottom
		};

		for (int i = 0; i < dir1.Length; i++) {
			if (arg2 == dir1 [i]) {
				for (int j = 0; j < arg1.roads.Length; j++) {
					if (arg1.roads [j] == dir2 [i]) {
						index = j;
						break;
					}
				}
			}
		}

		if (index != -1)
			transform.position = positionsToMove [index];

	}



	void OnDestroy ()
	{
		MapsController.OnChangingMap -= OnChangingMap;
	}
}
