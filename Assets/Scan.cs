using UnityEngine;
using Pathfinding; // Import A* Pathfinding namespace
using System.Collections;


public class AstarScanner : MonoBehaviour
{
    
    IEnumerator Start()
    {
        // Wait for 1 second before continuing
        yield return new WaitForSeconds(1f);
        
       
       AstarPath.active.Scan();
    }
}