using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNavigate : MonoBehaviour
{

    public Direction direction;

    /*	void OnTriggerEnter (Collider2D col)
	{
		if (col.CompareTag ("Player"))
			ChangeDirection ();
	}*/


    void Awake()
    {
        MapsController.Ins.OnChangingMap += OnChangingMap;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        CharacterBase character = col.transform.GetComponent<CharacterBase>();

        if (character != null)
            if (Input.GetKey(KeyCode.Space) && !colided)
            {
                colided = true;
                ChangeDirection(direction);
            }
    }
    bool colided;
    void OnCollisionEnter2D(Collision2D col)
    {
        CharacterBase character = col.transform.GetComponent<CharacterBase>();

        if (character != null)
            if (!colided)
            {
                colided = true;
                ChangeDirection(direction);
            }
    }

    void ChangeDirection(Direction direction)
    {
        MapsController.Ins.GoToMap(direction);
    }

    void OnChangingMap(MapsController.MapInfo mapInfo, Direction direction)
    {
        colided = false;
    }

}
