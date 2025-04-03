using UnityEngine;

[CreateAssetMenu(fileName = "MoreChestDrop", menuName = "Items/Effects/More Chest Drop")]
public class MoreChestDrop : ScriptableObject, ItemEffects
{
    public float maxBonus = 1.0f;       // +100% max bonus
    public float falloffRate = 0.5f;    // Higher = faster falloff

    public void ApplyEffect(GameObject player, int stackCount)
    {

    }
}