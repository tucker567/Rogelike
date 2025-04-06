using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class PoppedItem : MonoBehaviour
{
    public float minLifetime = 2f;
    public float maxLifetime = 5f;
    public float fadeDuration = 1f;
    public Vector3 visualScale = new Vector3(0.7f, 0.7f, 1f);

    private SpriteRenderer spriteRenderer;
    private float spawnTime;
    private float totalLifetime;
    private float fadeStartTime;

    void Start()
    {
        transform.localScale = visualScale;

        // Random arc movement
        float direction = Random.value < 0.5f ? -1f : 1f;
        float randomX = Random.Range(2f, 7f);
        float randomY = Random.Range(4f, 9f);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(direction * randomX, randomY), ForceMode2D.Impulse);

        // Timing
        spawnTime = Time.time;
        totalLifetime = Random.Range(minLifetime + fadeDuration, maxLifetime + fadeDuration);
        fadeStartTime = totalLifetime - fadeDuration;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float timeAlive = Time.time - spawnTime;

        if (timeAlive >= totalLifetime)
        {
            Destroy(gameObject);
        }
        else if (timeAlive >= fadeStartTime)
        {
            float t = Mathf.InverseLerp(fadeStartTime, totalLifetime, timeAlive);
            Color c = spriteRenderer.color;
            c.a = 1 - t;
            spriteRenderer.color = c;
        }
    }
}
