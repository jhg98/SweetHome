using UnityEngine;
using UnityEngine.UI;

public class HealthTextUI : MonoBehaviour
{
    [SerializeField] private PlayerStatData playerStatData;

    [SerializeField] private Text hpText;

    private int maxHealth;

    private void Start()
    {
        maxHealth = playerStatData.health;
        hpText.text = $"HP: {maxHealth} / {maxHealth}";
    }
    public void UpdateHealthText(float health)
    {
        hpText.text = $"HP: {health} / {maxHealth}";
    }
}
