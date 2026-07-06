using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public Vector2 lastMoveDirection { get; set; } = Vector2.right;
    public bool isInteracting { get; set; } = false;
    public bool isDead { get; set; } = false;
}