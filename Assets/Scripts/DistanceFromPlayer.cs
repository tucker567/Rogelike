using UnityEngine;

public class DistanceCalculator : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        if (target == null)
        {
            FindPlayer();
        }
        
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            Debug.Log("Distance to target: " + distance);
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
