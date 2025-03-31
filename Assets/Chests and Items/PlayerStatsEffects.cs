using UnityEngine;

public class PlayerStatsEffects : MonoBehaviour
{
    public static PlayerStatsEffects Instance;

    [Header("Base Stats")]
    public float moveSpeed = 5f;
    public float jumpHeight = 5f;
    
    [Header("Final Stats (With Item Effects)")]
    public float finalMoveSpeed;
    public float finalJumpHeight;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
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
    }
}