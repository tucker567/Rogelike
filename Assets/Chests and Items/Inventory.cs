using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<Item, int> items = new();

    public void AddItem(Item item)
    {
        if (items.ContainsKey(item))
            items[item]++;
        else
            items[item] = 1;

        ItemUIManager.Instance.UpdateUI(items);
    }

    public bool HasItem(Item item) => items.ContainsKey(item);
    public int GetItemCount(Item item) => items.TryGetValue(item, out var count) ? count : 0;
}
