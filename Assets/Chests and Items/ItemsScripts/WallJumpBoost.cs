using UnityEngine;

[CreateAssetMenu(fileName = "WallJumpBoostEffect", menuName = "Items/Effects/Wall Jump Boost")]
public class WallJumpBoostEffect : ScriptableObject, ItemEffects
{
    public void ApplyEffect(GameObject player, int stackCount)
    {
        if (stackCount <= 0) return;

        var stats = PlayerStatsEffects.Instance;
        Vector2 baseWallJumpPower = stats.wallJumpingPower;
        float bonusMultiplier = 1 + 0.25f * Mathf.Log(stackCount + 1, 2); // gentle curve

        stats.finalWallJumpingPower = baseWallJumpPower * bonusMultiplier; // Scale only wall jump strength
    }
}