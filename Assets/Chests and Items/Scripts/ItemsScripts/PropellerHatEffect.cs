using UnityEngine;

[CreateAssetMenu(fileName = "PropellerHatEffect", menuName = "Items/Effects/Propeller Hat")]
public class PropellerHatEffect : ScriptableObject, ItemEffects
{
    public float gravityReductionPerStack = 0.8f;
    public float minGravityScale = 0.5f;

    public void ApplyEffect(GameObject player, int stackCount)
    {
        if (stackCount <= 0) return;

        var stats = PlayerStatsEffects.Instance;
        float reducedGravity = stats.gravityScale - gravityReductionPerStack * stackCount;

        // Clamp so gravity doesn't go negative or break physics
        stats.finnalGravityScale = Mathf.Max(minGravityScale, reducedGravity);
    }
}
