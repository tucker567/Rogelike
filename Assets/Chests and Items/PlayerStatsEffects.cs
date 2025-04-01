using UnityEngine;

public class PlayerStatsEffects : MonoBehaviour
{
    public static PlayerStatsEffects Instance;

    [Header("Base Stats")]
    public float moveSpeed = 5f;
    public float jumpHeight = 10f;
    public Vector2 wallJumpingPower = new Vector2(8f, 16f);
    public float maxWallJumps = 4f; // Maximum number of wall jumps

    [Header("Final Stats (With Item Effects)")]
    public float finalMoveSpeed;
    public float finalJumpHeight;
    public Vector2 finalWallJumpingPower;
    public float finalMaxWallJumps; // Final maximum number of wall jumps

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("PlayerStatsEffects instance initialized."); // Added debug log
        }
        else
        {
            Debug.LogWarning("Multiple PlayerStatsEffects instances detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ResetStatsToBase();
    }

    public void ResetStatsToBase()
    {
        finalMoveSpeed = moveSpeed;
        finalJumpHeight = jumpHeight;
        finalWallJumpingPower = wallJumpingPower;
        finalMaxWallJumps = maxWallJumps; // Reset to base value
    }

    public void ApplyJumpBoost(float multiplier)
    {
        finalJumpHeight = jumpHeight * multiplier;
        // Removed scaling of wall jump strength here
    }
}