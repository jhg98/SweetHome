using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FallObstacle : MonoBehaviour, IInteractable
{
    [SerializeField] private float targetAngle = 90f;
    [SerializeField] private float fallDuration = 0.5f;

    private BoxCollider2D boxCollider;
    private bool isFalling = false;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;
    }
    public void StartInteract(GameObject caller, RaycastHit2D raycastHit)
    {
        if(isFalling) return;

        StartCoroutine(FallCoroutine());
    }

    public void StopInteract(GameObject caller)
    {
    }

    private IEnumerator FallCoroutine()
    {
        isFalling = true;

        float t = 0f;

        Quaternion initialRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

        while (t < fallDuration)
        {
            t += Time.deltaTime / fallDuration;
            transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);
            yield return null;
        }

        transform.rotation = targetRotation;

        boxCollider.enabled = true;
    }
}
