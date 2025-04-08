using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class PoppedItem : MonoBehaviour
{
    [Header("Item Settings")]
    public float flyToPlayerDelay = 1.25f;
    public float flySpeed = 10f;
    public float fadeSpeed = 5f;
    public Vector3 visualScale = new Vector3(0.7f, 0.7f, 1f);

    public Item itemData; // Assign this when the item is spawned!

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private float spawnTime;
    private bool flyingToPlayer = false;
    private Transform player;


    void Start()
    {
        transform.localScale = visualScale;

        float dir = Random.value < 0.5f ? -1f : 1f;
        float x = Random.Range(2f, 5f);
        float y = Random.Range(4f, 7f);

        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(dir * x, y), ForceMode2D.Impulse);

        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnTime = Time.time;

        // Randomize the delay before the item starts following the player
        flyToPlayerDelay = Random.Range(1f, 3f); // Adjust the range as needed

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        
        float timeAlive = Time.time - spawnTime;

        if (!flyingToPlayer && timeAlive >= flyToPlayerDelay && player != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
            rb.simulated = false;
            flyingToPlayer = true;
        }

        if (flyingToPlayer && player != null)
        {
            Vector3 target = player.position;
            target.z = 0;

            transform.position = Vector3.MoveTowards(transform.position, target, flySpeed * Time.deltaTime);

            // Check for pickup
            if (Vector3.Distance(transform.position, target) < 0.2f)
            {
                PickupItem();
            }
        }
    }

    void PickupItem()
    {
        // Add to inventory
        if (itemData != null)
        {
            var inventory = GameObject.FindWithTag("Player")?.GetComponent<Inventory>();
            if (inventory != null)
            {
                inventory.AddItem(itemData);
            }
        }
        
        Destroy(gameObject);
    }
    

}
