using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<Item, int> items = new();
    public GameObject poppedItemPrefab; // Assign in Inspector or dynamically


    public void AddItem(Item item)
    {
        if (items.ContainsKey(item))
            items[item]++;
        else
            items[item] = 1;

        ItemUIManager.Instance.UpdateUI(items);

        RecalculateAllItemStats();
        
        if (poppedItemPrefab != null)
        {
            GameObject popped = Instantiate(poppedItemPrefab, transform.position, Quaternion.identity);
            SpriteRenderer renderer = popped.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sprite = item.icon; // Set the icon to match the item
            }
        }
        
        PlayerStatsEffects.Instance.InvokeOnStatsReset(); // Call the new method

    }

    private void RecalculateAllItemStats()
    {
        PlayerStatsEffects.Instance.ResetStatsToBase();
        
        foreach (var kvp in items)
        {
            if (kvp.Key.Effect is ItemEffects Effect)
            { 
                Effect.ApplyEffect(gameObject, kvp.Value);
            }
        }
    }

    public bool HasItem(Item item) => items.ContainsKey(item);
    public int GetItemCount(Item item) => items.TryGetValue(item, out var count) ? count : 0;
}
