using UnityEngine;

public class RopeGenerator : MonoBehaviour
{
    public GameObject ropePiecePrefab;
    public int pieceCount = 30;
    public float pieceSpacing = 0.5f;

    private void Start()
    {
        Rigidbody2D prevRb = null;

        for (int i = 0; i < pieceCount; i++)
        {
            Vector2 pos = (Vector2)transform.position - new Vector2(0f, i * pieceSpacing);
            GameObject piece = Instantiate(ropePiecePrefab, pos, Quaternion.identity, transform);

            Rigidbody2D rb = piece.GetComponent<Rigidbody2D>();
            HingeJoint2D joint = piece.GetComponent<HingeJoint2D>();

            if (i == 0)
            {
                rb.bodyType = RigidbodyType2D.Static; // 맨 위는 고정
                joint.autoConfigureConnectedAnchor = true; // 맨 위 앵커 이상한 곳에 설정되는 문제 수정
            }
            else
            {
                joint.connectedBody = prevRb;
            }

            prevRb = rb;
        }

        Rope rope = GetComponent<Rope>();
        if (rope != null)
        {
            rope.InitRope();
        }
    }
}
