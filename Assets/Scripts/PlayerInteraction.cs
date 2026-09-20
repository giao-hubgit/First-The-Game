using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
    private List<IInteractable> nearbyInteractables = new List<IInteractable>();

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed && nearbyInteractables.Count > 0)
        {
            IInteractable interactableToUse = nearbyInteractables[nearbyInteractables.Count - 1];

            RemoveInteractable(interactableToUse);
            interactableToUse.Interact(gameObject);
        }
    }

    public void AddInteractable(IInteractable interactable)
    {
        if (!nearbyInteractables.Contains(interactable))
        {
            nearbyInteractables.Add(interactable);
        }
        UpdateHighlights();
    }

    public void RemoveInteractable(IInteractable interactable)
    {
        if (nearbyInteractables.Contains(interactable))
        {
            interactable.SetHighlight(false);
            nearbyInteractables.Remove(interactable);
        }
        UpdateHighlights();
    }

    private void UpdateHighlights()
    {
        foreach (IInteractable interactable in nearbyInteractables)
        {
            interactable?.SetHighlight(false);
        }

        if (nearbyInteractables.Count > 0)
        {
            IInteractable topInteractable = nearbyInteractables[nearbyInteractables.Count - 1];
            topInteractable?.SetHighlight(true);
        }
    }
}