using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatData", menuName = "Scriptable/PlayerStatData")]
public class PlayerStatData : ScriptableObject
{
    public string characterName;
    public int health = 20;
    public float normalSpeed = 10.0f; // 평소 속도
    public float crouchSpeed = 7.0f; // 앉았을 때 속도
    public float acceleration = 5.0f;
    public float jumpForce = 7.0f;
}
