using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class PoppedItem : MonoBehaviour
{
    public float maxrotationSpeed = 400f; // 🔀 maximum rotation speed
    public float minrotationSpeed = 200f; // 🔀 minimum rotation speed
    private bool isRotating = true;
    public float minFlyToPlayerDelay = 1f;   // 🔀 minimum delay
    public float maxFlyToPlayerDelay = 2.5f; // 🔀 maximum delay
    public float flySpeed = 10f;
    public float fadeSpeed = 5f;
    public Vector3 visualScale = new Vector3(0.75f, 0.75f, 1f);

    public Item itemData; // Assigned from ChestInteraction

    private SpriteRenderer spriteRenderer;
    private float rotationSpeed;
    private float fixedRotationAngle; // Store the fixed rotation angle

    public void Initialize(Item data)
    {
        itemData       = data;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = itemData.icon;
    }

    private Rigidbody2D rb;
    private Transform player;
    private float spawnTime;
    private float flyDelay;
    private bool flyingToPlayer = false;

    void Start()
    {
        rotationSpeed = Random.Range(minrotationSpeed, maxrotationSpeed);

        spriteRenderer ??= GetComponent<SpriteRenderer>();

        // Fallback in case Initialize wasn’t called early enough
        if (itemData != null && spriteRenderer.sprite == null)
        {
            spriteRenderer.sprite = itemData.icon;
        }
        if (spriteRenderer.sprite == null)
        {
            Debug.LogError("SpriteRenderer sprite is null! Make sure to call Initialize() before Start() or assign a sprite.");
            return;
        }
       
        transform.localScale = visualScale;

        // Initial bounce
        float dir = Random.value < 0.5f ? -1f : 1f;
        float x = Random.Range(2f, 5f);
        float y = Random.Range(4f, 7f);

        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(dir * x, y), ForceMode2D.Impulse);

        isRotating = true;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnTime = Time.time;

        // Get player
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // 🔀 Pick a random delay between min and max
        flyDelay = Random.Range(minFlyToPlayerDelay, maxFlyToPlayerDelay);
    }

    void Update()
    {
        float timeAlive = Time.time - spawnTime;
        // If still in physics-based arc, rotate
        if (isRotating && rb != null)
        {
            float direction = Mathf.Sign(rb.linearVelocity.x);
            transform.Rotate(0, 0, -rotationSpeed * direction * Time.deltaTime);
        }

        if (!flyingToPlayer && timeAlive >= flyDelay && player != null)
        {
            // Start following
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
            rb.simulated = false;
            flyingToPlayer = true;

            isRotating = false;
            fixedRotationAngle = transform.rotation.eulerAngles.z; // Store the current rotation angle
        }

        if (flyingToPlayer && player != null)
        {
            Vector3 target = player.position;
            target.z = 0;

            // Maintain the fixed rotation angle
            transform.rotation = Quaternion.Euler(0, 0, fixedRotationAngle);

            transform.position = Vector3.MoveTowards(transform.position, target, flySpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.2f)
            {
                PickupItem();
            }
        }
    }

    void PickupItem()
    {
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
