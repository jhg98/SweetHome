using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DamagingObstacle : MonoBehaviour
{
    [SerializeField]
    float damageValue = 10;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.transform.parent != null && collision.transform.parent.CompareTag("Player"))
        {
            collision.transform.parent.SendMessage("ReceiveDamage", damageValue);
        }
    }
}