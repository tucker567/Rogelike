using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;

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

    [Header("Health")]
    public int maxHealth = 100; // Maximum health of the player
    public int finalHealth; // Final health after item effects

    public event Action OnStatsReset;

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
        maxHealth = (int)def.baseHealth; // Explicitly cast baseHealth to int 

        

        // If you do some "final stats" logic:
        ResetStatsToBase();
    }

    public void ResetStatsToBase()
    {
        finalMoveSpeed = moveSpeed;
        finalJumpHeight = jumpHeight;
        finalWallJumpingPower = wallJumpingPower;
        finalMaxWallJumps = maxWallJumps; // Reset to base value
        finnalGravityScale = gravityScale; // Reset to base value
        finalHealth = maxHealth; // Reset to base value
        OnStatsReset?.Invoke(); // Notify listeners that stats have been reset
    }

    public void ApplyJumpBoost(float multiplier)
    {
        finalJumpHeight = jumpHeight * multiplier;
        // Removed scaling of wall jump strength here
    }
}