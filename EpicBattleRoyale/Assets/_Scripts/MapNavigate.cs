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


    List<CharacterBase> cbs = new List<CharacterBase>();

    void OnTriggerEnter2D(Collider2D col)
    {

        CharacterBase cb = col.transform.GetComponent<CharacterBase>();
        if (cb != null)
        {
            cbs.Add(cb);

            if (World.Ins.player.characterBase == cb)
                MobileInputsUI.Ins.ShowCanGoMapBtn(delegate
                {
                    colided = true;
                    ChangeDirection(direction);
                });
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        CharacterBase cb = col.transform.GetComponent<CharacterBase>();
        if (cb != null)
        {
            cbs.Remove(cb);
            if (World.Ins.player.characterBase == cb)
                MobileInputsUI.Ins.HideCanGoMapBtn();
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < cbs.Count; i++)
        {
            if (World.Ins.player.characterBase == cbs[i])
                MobileInputsUI.Ins.HideCanGoMapBtn();
        }
        cbs.Clear();
    }

    bool colided;

    void OnCollisionEnter2D(Collision2D col)
    {
        CharacterBase character = col.transform.GetComponent<CharacterBase>();

        if (character != null && World.Ins.player.characterBase == character)
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
