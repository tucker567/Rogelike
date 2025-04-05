using UnityEngine;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TilemapCollider2D))]
[RequireComponent(typeof(CompositeCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class ForceTilemapComposite : MonoBehaviour
{
    void Reset()
    {
        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

        var tilemapCol = GetComponent<TilemapCollider2D>();
        tilemapCol.usedByComposite = true;

        Debug.Log("✅ Tilemap set to use Composite Collider via script");
    }
}
