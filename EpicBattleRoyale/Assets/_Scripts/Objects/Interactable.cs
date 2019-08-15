using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : EntityBase
{
    public enum InteractableType
    {
        ItemPickUp,
        HouseDoor,
        MapChange
    }

    public virtual bool Interact(CharacterBase cb)
    {
        return false;
    }

    public virtual bool CanInteract(CharacterBase cb)
    {
        return CompareEntities(cb.worldPosition);
    }

    public virtual void AwayInteract(CharacterBase cb)
    {
    }

    public virtual InteractableType GetInteractableType()
    {
        return InteractableType.HouseDoor;
    }
}
