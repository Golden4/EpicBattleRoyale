using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNavigate : MonoBehaviour {

	public Direction direction;

	/*	void OnTriggerEnter (Collider2D col)
	{
		if (col.CompareTag ("Player"))
			ChangeDirection ();
	}*/

	void OnTriggerStay2D (Collider2D col)
	{
		if (col.CompareTag ("Player") && Input.GetKey (KeyCode.Space)&& !colided){
			colided = true;
			ChangeDirection (direction);
		}
	}
	bool colided;
	void OnCollisionEnter2D (Collision2D col)
	{
		if (col.collider.CompareTag ("Player") && !colided){
			colided = true;
			ChangeDirection (direction);
		}
	}

	void ChangeDirection (Direction direction)
	{
		MapsController.Ins.GoToMap (direction);
	}

}
