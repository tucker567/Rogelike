using System.Collections.Generic;
using UnityEngine;

public class EnhancedWalkingEnemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    private List<Vector2Int> currentPath = new List<Vector2Int>();
    private int pathIndex = 0;
    private Transform player;
    private Vector2 targetPos;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        InvokeRepeating(nameof(UpdatePath), 0f, 1f);
    }

    void UpdatePath()
    {
        if (player == null) return;

        Vector2Int start = Vector2Int.RoundToInt(transform.position);
        Vector2Int end = Vector2Int.RoundToInt(player.position);
        currentPath = AStarPathfinder.Instance.FindPath(start, end, AStarPathfinder.Instance.map.IsWalkable);
        pathIndex = 0;
    }

    private void Update()
    {
        if (currentPath == null || pathIndex >= currentPath.Count) return;

        TryJumpIfGap();

        Vector2 nextTile = currentPath[pathIndex];
        Vector2 direction = (nextTile - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, nextTile) < 0.1f)
        {
            pathIndex++;
        }
    }

    void TryJumpIfGap()
    {
        if (pathIndex >= currentPath.Count - 1) return;

        Vector2Int current = Vector2Int.RoundToInt(transform.position);
        Vector2Int next = currentPath[pathIndex];

        if (next.y == current.y && Mathf.Abs(next.x - current.x) == 2)
        {
            pathIndex++; // Simulate a jump over a 1-tile gap
        }
    }
}
