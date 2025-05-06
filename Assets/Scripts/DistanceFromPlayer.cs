using UnityEngine;
using Pathfinding; 
using System.Collections.Generic; 

public class DistanceCalculator : MonoBehaviour
{
    public Transform target;
    private GameObject enemyObject;
    private AIPath enemyPathfinder;  

    void Update()
    {   
        if (target == null)
        {
            FindPlayer();
        }
        
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);

            

            if (enemyObject == null)
            {
                enemyObject = GameObject.FindGameObjectWithTag("Enemy");
                if (enemyObject != null)
                {
                    enemyPathfinder = enemyObject.GetComponent<AIPath>(); // Replace with the correct component name
                }
            }

            if (enemyObject != null && enemyPathfinder != null)
            {
                // If the enemy is more than 10 units away, disable the Pathfinder component.
                if (distance > 33f)
                {
                    if (enemyPathfinder.enabled)
                    {
                        enemyPathfinder.enabled = false;
                        Debug.Log("Disabled the enemy's Pathfinder component.");
                    }
                }
                else
                {
                    // Re-enable the component if the enemy is within 10 units.
                    if (!enemyPathfinder.enabled)
                    {
                        enemyPathfinder.enabled = true;
                        Debug.Log("Enabled the enemy's Pathfinder component.");
                    }
                }
            }
            else
            {
                Debug.LogWarning("Enemy or its Pathfinder component not found.");
            }
        }
    }

    void FindPlayer() 
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.transform;
        }
        else
        {
            Debug.LogError("No GameObject found with the tag Player");
        }
    }
}
