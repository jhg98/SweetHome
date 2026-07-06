using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
public class PushableObstacle : MonoBehaviour, IMovableInteractable
{
    public float pushSpeed = 3.0f;
    public float pushAcceleration = 5.0f;

    private Rigidbody2D playerRb;
    private FixedJoint2D joint;
    private bool isConnected = false;

    private void Awake()
    {
        joint = gameObject.AddComponent<FixedJoint2D>();
        joint.enabled = false;
        joint.autoConfigureConnectedAnchor = true;
    }

    public void StartInteract(GameObject caller, RaycastHit2D raycastHit)
    {
        if (isConnected) return;

        playerRb = caller.GetComponent<Rigidbody2D>();
        if (playerRb == null) return;

        joint.connectedBody = playerRb;
        joint.anchor = transform.InverseTransformPoint(raycastHit.point);
        joint.enabled = true;
        isConnected = true;
    }
    public void StopInteract(GameObject caller)
    {
        if (!isConnected) return;

        playerRb = null;
        joint.connectedBody = null;
        joint.enabled = false;
        isConnected = false;
    }
    public void HandleInput(float horizontal, float vertical)
    {
        Vector2 targetVelocity = new Vector2(horizontal * pushSpeed, playerRb.velocity.y);

        playerRb.velocity = new Vector2(
            Mathf.Lerp(playerRb.velocity.x, targetVelocity.x, pushAcceleration * Time.fixedDeltaTime),
            playerRb.velocity.y
        );
    }
    public float GetAnimationSpeed(float h, float v)
    {
        return Mathf.Abs(h) > 0.01f ? 1f : 0f;
    }
}