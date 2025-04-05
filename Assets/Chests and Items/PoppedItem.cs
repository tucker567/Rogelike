using UnityEngine;
using System.Collections.Generic;

public class PoppedItem : MonoBehaviour
{
    public float gravity = -9.8f;
    public float lifetime = 2.5f;

    private Vector2 velocity;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector3(0.7f, 0.7f, 1f); // or tweak as needed
        float direction = Random.value < 0.5f ? -1f : 1f;
        float randomX = Random.Range(2f, 5f);
        float randomY = Random.Range(4f, 7f);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(new Vector2(direction * randomX, randomY), ForceMode2D.Impulse);
        }

        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Collided with: " + col.gameObject.name);
    }

}
