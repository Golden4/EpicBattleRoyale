using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInteractable : MonoBehaviour
{
    CharacterBase characterBase;
    public event Action<Interactable> OnCanInteractEvent;
    public event Action<Interactable> OnCantInteractEvent;
    public event Action<Interactable> OnInteractEvent;

    List<InteractableObject> interactableObjects = new List<InteractableObject>();

    void Awake()
    {
        characterBase = GetComponentInParent<CharacterBase>();
    }

    void Update()
    {
        for (int i = 0; i < interactableObjects.Count; i++)
        {
            if (!interactableObjects[i].canInteract && interactableObjects[i].interactable.CanInteract(characterBase))
            {
                if (OnCanInteractEvent != null)
                    OnCanInteractEvent(interactableObjects[i].interactable);
            }

            if (interactableObjects[i].canInteract && !interactableObjects[i].interactable.CanInteract(characterBase))
            {
                interactableObjects[i].interactable.AwayInteract(characterBase);
                if (OnCantInteractEvent != null)
                    OnCantInteractEvent(interactableObjects[i].interactable);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Interactable interactable = col.GetComponent<Interactable>();

        if (interactable != null)
        {
            InteractableObject obj = interactableObjects.Find(x => (x.interactable == interactable));

            if (obj == null)
            {
                // bool canInteract = interactable.CanInteract(this);
                interactableObjects.Add(new InteractableObject(interactable, false));
            }
        }
    }


    void OnTriggerExit2D(Collider2D col)
    {
        Interactable interactable = col.GetComponent<Interactable>();

        if (interactable != null)
        {
            InteractableObject obj = interactableObjects.Find(x => (x.interactable == interactable));

            if (obj != null)
            {
                interactable.AwayInteract(characterBase);
                interactableObjects.Remove(obj);

                if (OnCantInteractEvent != null)
                    OnCantInteractEvent(interactable);
            }
        }
    }

    public void ClearInteractableObjects()
    {
        for (int i = 0; i < interactableObjects.Count; i++)
        {
            interactableObjects[i].interactable.AwayInteract(characterBase);

            if (OnCantInteractEvent != null)
                OnCantInteractEvent(interactableObjects[i].interactable);
        }
        interactableObjects.Clear();
    }

    [System.Serializable]
    public class InteractableObject
    {
        public Interactable interactable;
        public bool canInteract;

        public InteractableObject(Interactable interactable, bool canInteract)
        {
            this.interactable = interactable;
            this.canInteract = canInteract;
        }
    }
}
