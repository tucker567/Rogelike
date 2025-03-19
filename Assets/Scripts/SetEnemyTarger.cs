using UnityEngine;
using Pathfinding; // Import Pathfinding namespace

public class SetEnemyTarget : MonoBehaviour 
{
    private AIDestinationSetter aIDestinationSetter;
    private GameObject target;

    void Start() 
    {
        // Automatically find the AIDestinationSetter on this GameObject
        aIDestinationSetter = GetComponent<AIDestinationSetter>();

        if (aIDestinationSetter == null)
        {
            Debug.LogError("AIDestinationSetter component not found on this GameObject!");
            return;
        }

        ZombieTargetsPlayer();
    }

    public void ZombieTargetsPlayer() 
    {
        target = GameObject.FindGameObjectWithTag("Player");

        if (target != null) 
        {
            aIDestinationSetter.target = target.transform; // Assign the player's transform
        } 
        else 
        {
            Debug.LogError("No GameObject found with the tag 'Player'! Make sure your player has the correct tag.");
        }
    }
}
