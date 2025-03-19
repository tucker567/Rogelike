using UnityEngine;
using Pathfinding; // Import A* Pathfinding namespace

public class AstarScanner : MonoBehaviour
{
    void Start()
    {
        AstarPath.active.Scan(); // Runs a scan when the game starts
        Debug.Log("A* Graph scanned at game start!");
    }
}
