using UnityEngine;

/// <summary>
/// Attach this to your Player. It uses 'PlayerStatsEffects' for max HP,
/// and updates a UI slider via 'HealthBarUI'.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerHealth : MonoBehaviour
{
    /// <summary>
    /// Current HP cannot be set from the Inspector, but you can read it at runtime.
    /// </summary>
    public int currentHealth { get; private set; }

    /// <summary>
    /// This reads the final max HP from PlayerStatsEffects each time you access it.
    /// So if items or character changes alter finalMaxHealth, this property sees it.
    /// </summary>
    private int maxHealth
    {
        get { return PlayerStatsEffects.Instance.finalMaxHealth; }
    }

    [Header("Invulnerability Settings")]
    [Tooltip("Time (in seconds) during which further damage is ignored after a hit.")]
    public float iFrameTime = 0.2f;
    private float lastHitTime = -999f;

    // Reference to your UI script
    private HealthBarUI healthUI;

    private void Start()
    {
        // Initialize current HP to the final max HP
        currentHealth = maxHealth;

        // Find the UI script in your scene (or you can drag it via Inspector if you prefer)
        healthUI = FindObjectOfType<HealthBarUI>();
        if (healthUI != null)
        {
            // Set the slider's maximum value and current value
            healthUI.SetMax(maxHealth);
            healthUI.SetValue(currentHealth);
        }

        // Subscribe to the event so we get notified if finalMaxHealth changes
        PlayerStatsEffects.Instance.OnStatsReset += RefreshMaxHealth;
    }

    /// <summary>
    /// Called when the player is destroyed (like on scene change or player death).
    /// We must unsubscribe or we risk errors if the event fires without us.
    /// </summary>
    private void OnDestroy()
    {
        // Defensive check, in case the instance was destroyed in some weird order
        if (PlayerStatsEffects.Instance != null)
        {
            PlayerStatsEffects.Instance.OnStatsReset -= RefreshMaxHealth;
        }
    }

    /// <summary>
    /// Refresh max HP from the Stats script if it changes.
    /// We clamp current HP to new max HP, and update the UI accordingly.
    /// </summary>
    private void RefreshMaxHealth()
    {
        int newMax = maxHealth;                     // e.g. finalMaxHealth might have changed
        currentHealth = Mathf.Min(currentHealth, newMax);

        // Update the slider
        if (healthUI != null)
        {
            healthUI.SetMax(newMax);
            healthUI.SetValue(currentHealth);
        }
    }

    /// <summary>
    /// The main method for enemies or hazards to damage the player.
    /// Enforces i-frames so the player won't get hammered every frame.
    /// </summary>
    public void TakeDamage(int amount)
    {
        // Invulnerability check
        if (Time.time - lastHitTime < iFrameTime)
        {
            // Still in grace period; ignore this hit
            return;
        }

        // Apply damage
        currentHealth = Mathf.Max(currentHealth - amount, 0);

        // Update the health bar
        if (healthUI != null)
        {
            healthUI.SetValue(currentHealth);
        }

        // Reset i-frame timer
        lastHitTime = Time.time;

        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Heals the player by 'amount', capped at finalMaxHealth.
    /// Call this from potions, item usage, etc.
    /// </summary>
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        // Reflect changes on the UI
        if (healthUI != null)
        {
            healthUI.SetValue(currentHealth);
        }
    }

    /// <summary>
    /// Called when the player's HP reaches zero.
    /// Insert your respawn, game-over, or fade-out code here.
    /// </summary>
    private void Die()
    {
        Debug.Log("Player died!");
        // For example, reload the scene:
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
