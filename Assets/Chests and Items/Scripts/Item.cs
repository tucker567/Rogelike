using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/New_Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public string description;
    public int maxStacks = 999;
    public ScriptableObject Effect; // This should be a ScriptableObject that implements ItemEffects
}
