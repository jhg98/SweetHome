using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerActionController : MonoBehaviour
{
    public Transform startPoint;
    public Transform midPoint;

    // 이동방식 테스트
    [SerializeField] private enum MovementType { Lerp, MoveTowards }
    [SerializeField] private MovementType movementType = MovementType.Lerp;

    [SerializeField] private PlayerStatData playerStatData;
    [SerializeField] private float damageFallSpeed = -10f;
    [SerializeField] private float maxFallSpeed = -20f;
    [SerializeField] private float maxFallDamage = 5f;

    [SerializeField] private CapsuleCollider2D standingCollider;
    [SerializeField] private CapsuleCollider2D crouchingCollider;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private float ceilingRadius = 1.0f;
    [SerializeField] private LayerMask ceilingLayer;

    private float normalSpeed;
    private float crouchSpeed;
    private float acceleration;
    private float jumpForce;

    private PlayerState state;
    private Rigidbody2D rb;
    private Animator animator;

    private bool isWalking = false;
    private bool isGrounded = false;
    private bool isCrouching = false;

    private float fallSpeed;

    void Awake()
    {
        state = GetComponent<PlayerState>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        if (startPoint != null)
        {
            transform.position = startPoint.position;
            transform.rotation = startPoint.rotation;
        }
        else
        {
            transform.position = new Vector3(132, 89, 0);
            transform.rotation = Quaternion.identity;
        }

        normalSpeed = playerStatData.normalSpeed;
        crouchSpeed = playerStatData.crouchSpeed;
        acceleration = playerStatData.acceleration;
        jumpForce = playerStatData.jumpForce;

        standingCollider.gameObject.SetActive(true);
        crouchingCollider.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (state.isInteracting || state.isDead) return;

        Jump();
        Crouch();

        animator.SetBool("Walking", isWalking);
        animator.SetBool("Crouching", isCrouching);
        animator.SetBool("Grounded", isGrounded);
        animator.SetFloat("ySpeed", rb.velocity.y);
    }
    void FixedUpdate()
    {
        if (state.isInteracting || state.isDead) return;

        Move();

        // 공중에 있을 때만 속도 추적
        if (!isGrounded)
        {
            fallSpeed = Mathf.Min(fallSpeed, rb.velocity.y);
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    void Move()
    {
        float moveSpeed = isCrouching ? crouchSpeed : normalSpeed;

        float inputX = Input.GetAxisRaw("Horizontal");

        if (inputX > 0)
        {
            state.lastMoveDirection = Vector2.right;
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        else if (inputX < 0)
        {
            state.lastMoveDirection = Vector2.left;
            transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
        }

        Vector2 targetVelocity = new Vector2(inputX * moveSpeed, rb.velocity.y);

        switch (movementType)
        {
            case MovementType.Lerp:
                rb.velocity = new Vector2(
                    Mathf.Lerp(rb.velocity.x, targetVelocity.x, acceleration * Time.fixedDeltaTime),
                    rb.velocity.y
                );
                break;
            case MovementType.MoveTowards:
                rb.velocity = new Vector2(
                    Mathf.MoveTowards(rb.velocity.x, targetVelocity.x, acceleration * Time.fixedDeltaTime),
                    rb.velocity.y
                );
                break;
        }

        if(inputX != 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
        }
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.contacts[0].normal.y > 0.5f)
        {
            Debug.Log("fallSpeed: " + fallSpeed);
            if (fallSpeed < damageFallSpeed)
            {
                float t = Mathf.InverseLerp(damageFallSpeed, maxFallSpeed, fallSpeed);
                float damage = Mathf.Lerp(1f, maxFallDamage, t);
                Debug.Log("damage: " + damage);

                gameObject.SendMessage("ReceiveDamage", damage);
            }

            // 속도 초기화
            fallSpeed = 0f;
        }
    }
    void Crouch()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            EnterCrouch();
        }
        else
        {
            TryStandUp();
        }
    }
    void EnterCrouch()
    {
        if (isCrouching) return;

        isCrouching = true;

        standingCollider.gameObject.SetActive(false);
        crouchingCollider.gameObject.SetActive(true);
    }
    void TryStandUp()
    {
        if (!isCrouching) return;
        if (IsBlocked()) return;

        isCrouching = false;

        standingCollider.gameObject.SetActive(true);
        crouchingCollider.gameObject.SetActive(false);
    }
    bool IsBlocked()
    {
        // 일어서기 위해 필요한 공간만큼 검사
        bool isBlocked = Physics2D.OverlapCircle(ceilingCheck.position, ceilingRadius, ceilingLayer);

        return isBlocked;
    }
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }

        if (ceilingCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(ceilingCheck.position, ceilingRadius);
        }
    }
}
