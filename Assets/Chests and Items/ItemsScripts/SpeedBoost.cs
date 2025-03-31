using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/SpeedBoost")]
public class SpeedBoost : ScriptableObject, ItemEffects
{
    public float SpeedPerStack = 10f; // Speed increase per stack

    public void ApplyEffect(GameObject player, int stackCount)
    {
        PlayerStatsEffects.Instance.moveSpeed += SpeedPerStack * stackCount;
    }
}


