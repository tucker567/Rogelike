using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/JumpBoost")]
public class JumpBoost : ScriptableObject, ItemEffects
{
    public float JumpHeightPerStack = .1f;

    public void ApplyEffect(GameObject player, int stackCount)
    {
        PlayerStatsEffects.Instance.jumpHeight += JumpHeightPerStack * stackCount;
    }
}
