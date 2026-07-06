using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ClimbableObstacle : MonoBehaviour, IMovableInteractable
{
    public float swingForce = 3f;
    public float climbSpeed = 3f;

    private Rigidbody2D hitChildRb;
    private Rigidbody2D playerRb;
    private HingeJoint2D playerJoint;
    private bool isConnected = false;

    private Rope rope;
    private float climbTimer = 0f;

    public void StartInteract(GameObject caller, RaycastHit2D raycastHit)
    {
        Collider2D hitChild = raycastHit.collider;

        hitChildRb = hitChild.GetComponent<Rigidbody2D>();
        if (hitChildRb == null) return;

        playerRb = caller.GetComponent<Rigidbody2D>();
        if (playerRb == null) return;

        playerJoint = caller.GetComponent<HingeJoint2D>();
        if (playerJoint == null)
        {
            playerJoint = caller.AddComponent<HingeJoint2D>();
            playerJoint.autoConfigureConnectedAnchor = false;
        }

        rope = GetComponent<Rope>();

        playerJoint.connectedBody = hitChildRb;
        playerJoint.anchor = caller.transform.InverseTransformPoint(caller.transform.Find("GrabPoint").position); // 플레이어 위치를 앵커로
        playerJoint.connectedAnchor = hitChildRb.transform.InverseTransformPoint(hitChildRb.transform.position); // 충돌 지점을 앵커로
        playerJoint.enabled = true;

        playerRb.freezeRotation = false;

        isConnected = true;
    }
    public void StopInteract(GameObject caller)
    {
        if (!isConnected) return;

        Quaternion originRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        playerRb.SetRotation(originRotation.eulerAngles.z);
        playerRb.freezeRotation = true;

        playerJoint.enabled = false;
        playerJoint.connectedBody = null;
        playerRb = null;

        isConnected = false;
    }
    public void HandleInput(float horizontal, float vertical)
    {
        if (!isConnected) return;

        Quaternion ropeRotation = playerJoint.connectedBody.transform.rotation;
        playerRb.MoveRotation(ropeRotation.eulerAngles.z);

        playerRb.AddForce(new Vector2(horizontal * swingForce, 0f), ForceMode2D.Force);

        if (rope == null) return; // 로프 없으면 좌우 움직임만

        climbTimer -= Time.deltaTime;
        if (climbTimer <= 0f)
        {
            if (vertical > 0f)
            {
                MoveToNextPiece(1);
                climbTimer = 1f / climbSpeed;
            }
            else if (vertical < 0f)
            {
                MoveToNextPiece(-1);
                climbTimer = 1f / climbSpeed;
            }
        }
    }
    private void MoveToNextPiece(int direction)
    {
        var curPiece = playerJoint.connectedBody.GetComponent<RopePiece>();
        int curIndex = curPiece.index;

        int nextIndex = Mathf.Clamp(curIndex - direction, 0, rope.pieces.Count - 1); // 최소: 0, 최대: 개수-1
        var nextPiece = rope.pieces[nextIndex];

        Vector2 targetPos = nextPiece.transform.position;

        // 연결 지점 변경
        playerJoint.connectedBody = nextPiece.GetComponent<Rigidbody2D>();
        playerJoint.connectedAnchor = nextPiece.transform.InverseTransformPoint(nextPiece.transform.position);
        playerJoint.enabled = true;
    }
    public float GetAnimationSpeed(float h, float v)
    {
        return Mathf.Abs(v) > 0.01f ? 1f : 0f;
    }
}
