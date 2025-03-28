using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChestInteraction : MonoBehaviour
{
    public Canvas chestCanvas; // Assign in the prefab
    public GameObject floatingText;
    private bool playerInRange = false;
    public Vector3 textOffset = new Vector3(0, 1.5f, 0); // Adjust this if needed
    public float floatingTextYOffset = -0.5f; // Public Y offset for floating text

    private void Start()
    {
        InitializeChest(); // Initialize chest on start
    }

    private void InitializeChest()
    {
        Debug.Log("Initializing Chest...");

        if (chestCanvas != null)
        {
            chestCanvas.worldCamera = Camera.main; // Fix Camera issue
        }

        floatingText = chestCanvas.transform.Find("FloatingText")?.gameObject;
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
            // Move the floating text to the chest's position with the Y offset
            floatingText.transform.position = transform.position + textOffset + new Vector3(0, floatingTextYOffset, 0);
            floatingText.SetActive(true);
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
        if (playerInRange)
        {
            // Update text position dynamically
            chestCanvas.transform.position = transform.position + textOffset;

            if (Input.GetKeyDown(KeyCode.E)) // Player presses E
            {
                Debug.Log("Chest Opened!");
                floatingText.SetActive(false); // Hide text after opening
            }
        }
    }

    public class Buttons : MonoBehaviour
    {
        public void openChest()
        {
            Debug.Log("Chest Opened!");
        }
    }
}
