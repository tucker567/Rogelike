using UnityEngine;

[CreateAssetMenu(fileName = "WishBoneEffect", menuName = "Items/Effects/Wish Bone")]
public class WishBoneEffect : ScriptableObject, ItemEffects
{
    public float baseExtraDropChance = 0.2f;
    public float dropDecayRate = 0.5f;

    public void ApplyEffect(GameObject player, int stackCount)
    {
        // Nothing here for passive effect
    }

    public static float CalculateDropChance(int index, int stackCount, float baseChance, float decay)
    {
        if (stackCount <= 0) return 0;
        float scaledBaseChance = baseChance * (1 + Mathf.Log(stackCount + 1)); // log(1) = 0, log(2) = 0.69, etc.
        float finalChance = scaledBaseChance * Mathf.Pow(decay, index - 1);
        return finalChance;
    }
}
