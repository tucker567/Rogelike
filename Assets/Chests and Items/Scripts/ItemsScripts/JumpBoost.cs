using UnityEngine;

[CreateAssetMenu(fileName = "JumpBoostEffect", menuName = "Items/Effects/Jump Boost")]
public class JumpBoostEffect : ScriptableObject, ItemEffects
{
    public void ApplyEffect(GameObject player, int stackCount)
    {
        if (stackCount <= 0) return;

        var stats = PlayerStatsEffects.Instance;
        float baseJump = stats.jumpHeight; // base value
        float bonusMultiplier = 1 + 0.25f * Mathf.Log(stackCount + 1, 2); // gentle curve

        stats.finalJumpHeight = baseJump * bonusMultiplier;
        stats.finalWallJumpingPower = stats.wallJumpingPower * bonusMultiplier; // Reflect scaled wall jump strength
    }
}
