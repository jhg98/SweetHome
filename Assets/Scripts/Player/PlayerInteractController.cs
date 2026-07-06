using UnityEngine;

public class PlayerInteractController : MonoBehaviour
{
    [SerializeField] private float interactDistance = 1.0f;
    [SerializeField] private LayerMask interactableLayer;

    private PlayerState state;
    private Animator animator;

    private IInteractable currentInteractable;

    private void Awake()
    {
        state = GetComponent<PlayerState>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (state.isDead)
        {
            Release();
            return;
        }
        TryInteract();

        animator.SetBool("Interacting", state.isInteracting);
    }
    private void FixedUpdate()
    {
        if (state.isDead)
        {
            Release();
            return;
        }
        MoveInteract();
    }
    void TryInteract()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            Release();
        }
    }
    void Interact()
    {
        Vector2 direction = state.lastMoveDirection;
        Vector2 origin = (Vector2)transform.position - new Vector2(0.3f, 0f) * direction;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, interactDistance, interactableLayer);

        if (hit.collider != null)
        {
            currentInteractable = hit.collider.GetComponentInParent<IInteractable>();
            currentInteractable?.StartInteract(gameObject, hit);

            state.isInteracting = true;
        }
    }
    void Release()
    {
        currentInteractable?.StopInteract(gameObject);
        currentInteractable = null;

        animator.SetFloat("AnimSpeed", 1f);

        state.isInteracting = false;
    }
    void MoveInteract()
    {
        if (currentInteractable is IMovableInteractable movable)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            movable.HandleInput(h, v);

            animator.SetFloat("AnimSpeed", movable.GetAnimationSpeed(h, v));
        }
    }
    private void OnDrawGizmos()
    {
        Vector2 direction = Vector2.right;
        if (state != null)
        {
            direction = state.lastMoveDirection;
        }
        Vector2 origin = (Vector2)transform.position - new Vector2(0.3f, 0f) * direction;

        Gizmos.color = Color.red;
        Debug.DrawRay(origin, direction * interactDistance, Color.red, 0f);
    }
}
