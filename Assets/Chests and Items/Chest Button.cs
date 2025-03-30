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
    public Vector3 textOffset = new Vector3(0, 1.5f, 0); // Adjust this if needed
    public float floatingTextYOffset = -0.5f; // Public Y offset for floating text
    public List<Item> CommonItems; // List of items to give
    public GameObject openchest; // Assign the open chest prefab here
    public static Dictionary<Vector3, GameObject> openChestsDictionary = new Dictionary<Vector3, GameObject>(); // Dictionary to store open chests

    private bool chestOpened = false; // Ensure the chest is only opened once

    private void Start()
    {
        InitializeChest(); // Initialize chest on start
    }

private void InitializeChest()
{
    Debug.Log("Initializing Chest...");

    if (chestCanvas != null)
    {
        chestCanvas.worldCamera = Camera.main;

        // Enable canvas temporarily if it’s inactive to find children
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
            // Ensure floatingText is properly initialized
            if (floatingText == null)
            {
                floatingText = chestCanvas.transform.Find("FloatingText")?.gameObject;
                if (floatingText == null)
                {
                    Debug.LogError("FloatingText not found in Canvas!");
                    return;
                }
            }

            // Move the floating text to the chest's position with the Y offset
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
            // Update text position dynamically
            chestCanvas.transform.position = transform.position + textOffset;

            if (Input.GetKeyDown(KeyCode.E)) // Player presses E
            {
                Debug.Log("Chest Opened!");
                floatingText.SetActive(false); // Hide text after opening
                chestOpened = true; // Mark the chest as opened

                // Randomly select an item from the list
                if (CommonItems != null && CommonItems.Count > 0)
                {
                    Item selectedItem = CommonItems[Random.Range(0, CommonItems.Count)];

                    // Give the selected item to the player
                    var inventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();
                    if (inventory != null && selectedItem != null)
                    {
                        inventory.AddItem(selectedItem);

                        // Instantiate the open chest prefab
                        GameObject openChestInstance = Instantiate(openchest, transform.position, Quaternion.identity);

                        // Add the open chest to the dictionary
                        if (!openChestsDictionary.ContainsKey(transform.position))
                        {
                            openChestsDictionary.Add(transform.position, openChestInstance);
                        }

                        Destroy(gameObject); // Destroy the chest after giving the item
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
        yield return null; // Wait one frame so layout can rebuild
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
