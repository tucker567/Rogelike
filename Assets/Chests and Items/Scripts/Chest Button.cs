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
    public Vector3 textOffset = new Vector3(0, 1.5f, 0);
    public float floatingTextYOffset = -0.5f;
    public List<Item> CommonItems; // List of items to give
    public GameObject openchest; // Assign the open chest prefab here
    public GameObject poppedItemPrefab; // 🔥 The item animation prefab

    public static Dictionary<Vector3, GameObject> openChestsDictionary = new Dictionary<Vector3, GameObject>();

    private bool playerInRange = false;
    private bool chestOpened = false;

    private void Start()
    {
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

            if (Input.GetKeyDown(KeyCode.E)) // Open chest
            {
                Debug.Log("Chest Opened!");
                floatingText.SetActive(false);
                chestOpened = true;

                // Select a random item
                if (CommonItems != null && CommonItems.Count > 0)
                {
                    Item selectedItem = CommonItems[Random.Range(0, CommonItems.Count)];
                    Vector3 spawnPos = transform.position + Vector3.up * 0.3f;
                    GameObject go    = Instantiate(poppedItemPrefab, spawnPos, Quaternion.identity);

                    go.GetComponent<PoppedItem>().Initialize(selectedItem);

                    // Find inventory and add item
                    var player = GameObject.FindWithTag("Player");
                    var inventory = player.GetComponent<Inventory>();
                    if (inventory != null && selectedItem != null)
                    {
                        // 🔥 No longer directly add to inventory — let the popped item handle it
                        if (poppedItemPrefab != null)
                        {
                            GameObject popped = Instantiate(poppedItemPrefab, transform.position, Quaternion.identity);

                            // Assign the item data to the popped item
                            var poppedItemScript = popped.GetComponent<PoppedItem>();
                            if (poppedItemScript != null)
                            {
                                poppedItemScript.itemData = selectedItem;
                            }
                        }
                        else
                        {
                            Debug.LogWarning("PoppedItemPrefab not assigned!");
                        }

                        // Spawn open chest prefab
                        GameObject openChestInstance = Instantiate(openchest, transform.position, Quaternion.identity);
                        if (!openChestsDictionary.ContainsKey(transform.position))
                        {
                            openChestsDictionary.Add(transform.position, openChestInstance);
                        }

                        Destroy(gameObject); // Remove this chest
                    }
                    else
                    {
                        Debug.LogError("Inventory or selected item is missing!");
                    }
                }
                else
                {
                    Debug.LogError("No items available to give!");
                }
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
