using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStatController : MonoBehaviour
{
    [SerializeField] private PlayerStatData playerStatData;

    private PlayerState state;

    private SpriteRenderer[] playerRenderers;
    private int health;

    private bool isTakingDamage = false;
    private float waitTime = 0.1f;

    private float restartDelayTime = 1.0f;

    private HealthTextUI healthTextUI;

    private Animator animator;

    private void Awake()
    {
        state = GetComponent<PlayerState>();
        playerRenderers = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        health = playerStatData.health;

        healthTextUI = FindObjectOfType<HealthTextUI>();
    }
    private void Update()
    {
        animator.SetBool("Dead", state.isDead);
    }

    void ReceiveDamage(float damageValue)
    {
        if (!isTakingDamage && !state.isDead)
        {
            health -= (int)damageValue;
            healthTextUI.UpdateHealthText(health);
            if (health <= 0)
            {
                state.isDead = true;

                StartCoroutine(RestartGame());
            }
            else
            {
                StartCoroutine(DamageCoroutine());
            }
        }
    }

    IEnumerator DamageCoroutine()
    {
        isTakingDamage = true;

        // waitTime마다 깜박거리게
        int iteration = 0;
        while (iteration <= 10)
        {
            iteration++;

            if (iteration % 2 == 0)
            {
                SetPlayerColor(Color.red);
            }
            else
            {
                SetPlayerColor(Color.white);
            }

            yield return new WaitForSeconds(waitTime);
        }

        yield return new WaitForSeconds(0.5f);

        isTakingDamage = false;
        SetPlayerColor(Color.white);
        yield return null;
    }
    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(restartDelayTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void SetPlayerColor(Color color)
    {
        foreach (SpriteRenderer renderer in playerRenderers)
        {
            renderer.color = color;
        }
    }
}
