using UnityEngine;

public interface IInteractable
{
    void Interact(GameObject player);

    void SetHighlight(bool isHighlighted);
}