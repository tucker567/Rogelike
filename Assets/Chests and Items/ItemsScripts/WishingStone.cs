using UnityEngine;

[CreateAssetMenu(fileName = "WishingBoneEffect", menuName = "Items/Effects/Wishing Bone")]
public class WishingBoneEffect: ScriptableObject, ItemEffects
{
    public float baseExtraDropChance = 0.2f; // 20% base chance to drop an extra item
    public float dropDecayRate = 0.05f; // exponential decay rate for drop chance
    
    public static float CalculateDropChance(int extraItemIndex, int stackCount, float baseChance, float decay)
    {
        float scaledBaseChance = baseChance * (1 + Mathf.Log(stackCount + 1)); // Scale base chance with stack count
        return scaledBaseChance * Mathf.Pow(1 - decay, extraItemIndex - 1); // Apply exponen
    }

    public void ApplyEffect(GameObject player, int stackCount)
    {

    }
}
