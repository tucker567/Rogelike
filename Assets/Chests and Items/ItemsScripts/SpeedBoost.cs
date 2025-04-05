using UnityEngine;

[CreateAssetMenu(fileName = "SpeedBoostEffect", menuName = "Items/Effects/Speed Boost")]
public class SpeedBoostEffect : ScriptableObject, ItemEffects
{
    public float maxBonus = 1.0f;       // +100% max bonus
    public float falloffRate = 0.5f;    // Higher = faster falloff

    public void ApplyEffect(GameObject player, int stackCount)
    {
        if (stackCount <= 0) return;

        var stats = PlayerStatsEffects.Instance;
        float baseSpeed = stats.moveSpeed;

        float bonusMultiplier = 1 + (maxBonus * (1 - Mathf.Exp(-falloffRate * stackCount)));
        stats.finalMoveSpeed = baseSpeed * bonusMultiplier;
    }
}
