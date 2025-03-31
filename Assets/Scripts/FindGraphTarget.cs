using UnityEngine;
using Pathfinding; // Import Pathfinding namespace

public class FindGraphTarget : MonoBehaviour 
{
    private ProceduralGridMover gridMover;  // Declared gridMover variable
    private GameObject target;

    void Start() 
    {
        // Automatically find the ProceduralGridMover on this GameObject
        gridMover = GetComponent<ProceduralGridMover>();

        if (gridMover == null)
        {
            Debug.LogError("Grid Mover component not found on this GameObject!");
            return;
        }

        TargetGrid();
    }
    void Update() 
    {
        // Check if the target is null and call TargetGrid() if it is
        if (target == null) 
        {
            TargetGrid();
        }
    }

    public void TargetGrid() 
    {
        target = GameObject.FindGameObjectWithTag("Player");

        if (target != null) 
        {
            gridMover.target = target.transform; // Assign the player's transform
        } 
        else 
        {
            Debug.LogError("No GameObject found with the tag 'Player'! Make sure your player has the correct tag.");
        }
    }
}
