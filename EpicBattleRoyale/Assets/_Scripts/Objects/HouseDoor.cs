using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseDoor : MonoBehaviour
{
    public Vector2Int mapCoords;
    public int houseIndex;
    public MapsController.HouseType houseType;

    public void Setup(Vector2Int mapCoords, MapsController.HouseType houseType, int houseIndex)
    {
        this.mapCoords = mapCoords;
        this.houseIndex = houseIndex;
        this.houseType = houseType;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        CharacterBase cb = col.transform.GetComponent<CharacterBase>();
        if (cb != null)
        {
            cbs.Add(cb);
            cb.CanEnterDoor(this);
        }
    }

    List<CharacterBase> cbs = new List<CharacterBase>();

    void OnTriggerExit2D(Collider2D col)
    {
        CharacterBase cb = col.transform.GetComponent<CharacterBase>();
        if (cb != null)
        {
            cbs.Remove(cb);
            cb.AwayDoor();
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < cbs.Count; i++)
        {
            cbs[i].AwayDoor();
        }
        cbs.Clear();
    }
}
