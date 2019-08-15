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

    List<CharacterBase> cbs = new List<CharacterBase>();

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

    void ChangeDirection(Direction direction)
    {
        colided = true;
        MapsController.Ins.GoToMap(direction);
    }

    public override bool Interact(CharacterBase cb)
    {
        if (!colided)
        {
            colided = true;
            ChangeDirection(direction);
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
