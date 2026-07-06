using UnityEngine;

public interface IInteractable
{
    void StartInteract(GameObject caller, RaycastHit2D raycastHit);
    void StopInteract(GameObject caller);
}
