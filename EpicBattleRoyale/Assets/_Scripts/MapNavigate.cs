using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNavigate : Interactable
{
    public Direction direction;

    void OnEnable()
    {
        colided = false;
    }

    bool colided;

    void ChangeDirection(CharacterBase cb, Direction direction)
    {
        colided = true;
        MapsController.Ins.GoToMapWithFade(cb, direction);
    }

    public override bool Interact(CharacterBase cb)
    {
        if (!colided)
        {
            colided = true;
            ChangeDirection(cb, direction);
            return true;
        }

        return false;
    }

    public override bool CanInteract(CharacterBase cb)
    {
        return true;
    }

    public override void AwayInteract(CharacterBase cb)
    {
    }

    public override InteractableType GetInteractableType()
    {
        return InteractableType.MapChange;
    }
}
