using System.Collections.Generic;
using UnityEngine;

public class ItemUIManager : MonoBehaviour
{
    public static ItemUIManager Instance;

    public GameObject itemSlotPrefab; // assign your prefab here
    public Transform itemBarParent;   // assign your UI panel here

    private Dictionary<Item, ItemSlotUI> slotDictionary = new();

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateUI(Dictionary<Item, int> inventory)
    {
        foreach (var kvp in inventory)
        {
            if (!slotDictionary.ContainsKey(kvp.Key))
            {
                GameObject slotGO = Instantiate(itemSlotPrefab, itemBarParent);
                ItemSlotUI slotUI = slotGO.GetComponent<ItemSlotUI>();
                slotDictionary[kvp.Key] = slotUI;
            }

            slotDictionary[kvp.Key].SetItem(kvp.Key, kvp.Value);
        }
    }
}
