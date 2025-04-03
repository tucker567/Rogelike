using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class ChestInteraction : MonoBehaviour
{
    public Canvas chestCanvas; // Assign in the prefab
    public GameObject floatingText;
    private bool playerInRange = false;
    public Vector3 textOffset = new Vector3(0, 1.5f, 0);
    public float floatingTextYOffset = -0.5f;
    public List<Item> CommonItems;
    public GameObject openchest;
    public static Dictionary<Vector3, GameObject> openChestsDictionary = new Dictionary<Vector3, GameObject>();
    private bool chestOpened = false;

    private void Start()
    {
        chestOpened = false;
        InitializeChest();
    }

    private void InitializeChest()
    {
        Debug.Log("Initializing Chest...");

        if (chestCanvas != null)
        {
            chestCanvas.worldCamera = Camera.main;
            bool wasActive = chestCanvas.gameObject.activeSelf;
            if (!wasActive)
                chestCanvas.gameObject.SetActive(true);

            floatingText = chestCanvas.transform.Find("FloatingText")?.gameObject;

            if (!wasActive)
                chestCanvas.gameObject.SetActive(false);
        }

        if (floatingText == null)
        {
            Debug.LogError("FloatingText not found in Canvas!");
            return;
        }

        floatingText.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (floatingText == null)
            {
                floatingText = chestCanvas.transform.Find("FloatingText")?.gameObject;
                if (floatingText == null)
                {
                    Debug.LogError("FloatingText not found in Canvas!");
                    return;
                }
            }

            StartCoroutine(ShowFloatingText());
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            floatingText.SetActive(false);
            playerInRange = false;
        }
    }

    void Update()
    {
        if (playerInRange && !chestOpened)
        {
            chestCanvas.transform.position = transform.position + textOffset;

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Chest Interact Key Pressed");

                floatingText.SetActive(false);
                if (chestOpened) return;
                chestOpened = true;

                var player = GameObject.FindWithTag("Player");
                var inventory = player.GetComponent<Inventory>();

                if (inventory == null || CommonItems == null || CommonItems.Count == 0)
                {
                    Debug.LogError("Inventory or CommonItems not found.");
                    return;
                }

                int totalItemsDropped = 0;

                // Drop the guaranteed item
                Item baseDrop = CommonItems[Random.Range(0, CommonItems.Count)];
                inventory.AddItem(baseDrop);
                Debug.Log($"[Drop] Base item: {baseDrop.itemName}");
                totalItemsDropped++;

                // Check for Wish Bone
                Item wishBone = null;
        foreach (var item in inventory.items.Keys)
        {
            Debug.Log($"[Wish Bone] Checking inventory item: {item.itemName}");

            if (item.itemName == "WishingBone") // OR change this to match your item's name exactly
            {
                wishBone = item;
                Debug.Log($"[Wish Bone] FOUND in inventory: {wishBone.itemName}");

                if (wishBone.Effect != null)
                {
                    Debug.Log($"[Wish Bone] Effect assigned: {wishBone.Effect.GetType()}");
                }
                else
                {
                    Debug.LogWarning($"[Wish Bone] Effect is NULL! Make sure it's assigned in the item asset.");
                }
                break;
            }
        }


                if (wishBone != null && wishBone.Effect is WishBoneEffect effect)
                {
                    int stacks = inventory.GetItemCount(wishBone);
                    Debug.Log($"[Wish Bone] Found {stacks} stack(s)");

                    for (int i = 1; i <= 10; i++)
                    {
                        float chance = WishBoneEffect.CalculateDropChance(i, stacks, effect.baseExtraDropChance, effect.dropDecayRate);
                        Debug.Log($"[Wish Bone] Calculated chance for item {i}: {chance}");
                        float roll = Random.value;
                        Debug.Log($"Rolling... chance={chance}, roll={roll}");
                        Debug.Log($"[Wish Bone] Roll #{i}: Chance={chance:P2}, Rolled={roll:F2}");

                        if (roll < chance)
                        {
                            Item bonus = CommonItems[Random.Range(0, CommonItems.Count)];
                            inventory.AddItem(bonus);
                            Debug.Log($"[Drop] Extra item {i}: {bonus.itemName}");
                            totalItemsDropped++;
                        }
                        else
                        {
                            Debug.Log($"[Wish Bone] Roll #{i} failed. Stopping extra drops.");
                            break;
                        }
                    }
                }

                Debug.Log($"[Summary] Total items dropped from chest: {totalItemsDropped}");

                GameObject opened = Instantiate(openchest, transform.position, Quaternion.identity);
                openChestsDictionary.TryAdd(transform.position, opened);
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator ShowFloatingText()
    {
        floatingText.transform.position = transform.position + textOffset + new Vector3(0, floatingTextYOffset, 0);
        yield return null;
        floatingText.SetActive(true);
    }

    public class Buttons : MonoBehaviour
    {
        public void openChest()
        {
            Debug.Log("Chest Opened!");
        }
    }
}
