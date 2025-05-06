using UnityEngine;

[CreateAssetMenu(fileName = "GodSandal", menuName = "Items/Effects/God Sandal")]
public class GodSandal : ScriptableObject, ItemEffects
{
    public int jumpbonus = 1; // +1 jump bonus
    
    public void ApplyEffect(GameObject player, int stackCount)
    {
        PlayerStatsEffects.Instance.finalMaxWallJumps += jumpbonus * stackCount;
    }
}