using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Roguelike/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public string description;
    public int maxStacks = 999;
}
