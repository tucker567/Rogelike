using UnityEngine;

/// <summary>
/// Keeps track of HP and drives the HealthBarUI.
/// Attach to the Player prefab / object.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int currentHealth { get; private set; }
    private int maxHealth => PlayerStatsEffects.Instance.finalHealth;

    [Header("Invulnerability (seconds)")]
    public float iFrameTime = 0.2f;
    private float lastHitTime = -999f;

    /* ---- References ---- */
    private HealthBarUI healthUI;

    void Start()
    {
        currentHealth = maxHealth;

        healthUI = FindObjectOfType<HealthBarUI>();
        if (healthUI != null)
        {
            healthUI.SetMax(maxHealth);
            healthUI.SetValue(currentHealth);
        }

        PlayerStatsEffects.Instance.OnStatsReset += RefreshMaxHealth;
    }

    void OnDestroy()
    {
        if (PlayerStatsEffects.Instance != null)
            PlayerStatsEffects.Instance.OnStatsReset -= RefreshMaxHealth;
    }

    /* ---------- Public API ---------- */
    public void TakeDamage(int amount)
    {
        if (Time.time - lastHitTime < iFrameTime) return;      // still invulnerable?

        currentHealth = Mathf.Max(currentHealth - amount, 0);
        healthUI?.SetValue(currentHealth);
        lastHitTime = Time.time;

        if (currentHealth <= 0) Die();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        healthUI?.SetValue(currentHealth);
    }

    /* ---------- Helpers ---------- */
    private void RefreshMaxHealth()
    {
        int newMax = maxHealth;
        currentHealth = Mathf.Min(currentHealth, newMax);
        healthUI?.SetMax(newMax);
        healthUI?.SetValue(currentHealth);
    }

    private void Die()
    {
        Debug.Log("Player died!");
        // TODO: trigger respawn / restart / animation
    }
}
