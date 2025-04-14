using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PlayerStatsEffects : MonoBehaviour
{
    public static PlayerStatsEffects Instance;

    [Header("All possible characters")]
    public List<CharacterDefinition> allCharacters;
    
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

    [Header("Final Health")]
    public CharacterDefinition chosenCharacter;
    public int finalMaxHealth;
    public int Maxhealth => chosenCharacter.baseMaxHealth; // Base max health from character definition


    public delegate void StatsResetHandler();
    // --- ② NEW: the actual event anyone can subscribe to --
    public event StatsResetHandler OnStatsReset;

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

    private void Start()
    {
        // Grab the chosen index from PlayerPrefs
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);
        if (selectedIndex < 0 || selectedIndex >= allCharacters.Count)
            selectedIndex = 0;

        // Fetch that definition
        CharacterDefinition def = allCharacters[selectedIndex];

        // Set our stats
        moveSpeed  = def.baseMoveSpeed;
        jumpHeight = def.baseJumpHeight;
        wallJumpingPower = def.baseWallJumpingPower;
        maxWallJumps = def.baseMaxWallJumps; // Set the base max wall jumps
        gravityScale = def.baseGravityScale; // Set the base gravity scale
        finalMaxHealth = def.baseMaxHealth; // Set the base max health

        

        // If you do some "final stats" logic:
        ResetStatsToBase();
    }

    public void ResetStatsToBase()
    {
        // Reset all final stats to base values
        finnalGravityScale = gravityScale; // Reset to base value
        finalMoveSpeed = moveSpeed;
        finalJumpHeight = jumpHeight;
        finalWallJumpingPower = wallJumpingPower;
        finalMaxWallJumps = maxWallJumps; // Reset to base value
        finalMaxHealth = Maxhealth; // Reset to base value

    // --- ③ NEW: notify listeners that numbers are final ----
    OnStatsReset?.Invoke();

    }

    public void ApplyJumpBoost(float multiplier)
    {
        finalJumpHeight = jumpHeight * multiplier;
        // Removed scaling of wall jump strength here
    }

    public void InvokeOnStatsReset()
    {
        // Logic for handling stats reset, if any
        Debug.Log("Player stats have been reset.");
    }
}