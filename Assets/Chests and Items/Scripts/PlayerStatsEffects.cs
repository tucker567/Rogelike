using UnityEngine;

public class PlayerStatsEffects : MonoBehaviour
{
    public static PlayerStatsEffects Instance;

    [Header("Base Simple Movement Stats")]
    public float moveSpeed = 5f;
    public float jumpHeight = 10f;
    public Vector2 wallJumpingPower = new Vector2(4f, 8f);
    public float maxWallJumps = 4f; // Maximum number of wall jumps

    [Header("Gravity Settings")]
    public float gravityScale = 2f; // Gravity scale for the player
    public float finnalGravityScale ; // Final gravity scale after item effects

    [Header("Final Simple Movement Stats ")]
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
        finnalGravityScale = gravityScale; // Reset to base value
    }

    public void ApplyJumpBoost(float multiplier)
    {
        finalJumpHeight = jumpHeight * multiplier;
        // Removed scaling of wall jump strength here
    }
}