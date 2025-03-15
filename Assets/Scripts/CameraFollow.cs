using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Declare the target variable
    public float smoothSpeed = 5f; // Speed of the camera follow
    public Vector3 offset = new Vector3(0f, 2f, -10f); // Offset for the camera position

    void LateUpdate()
    {
        // Search for the player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        else
        {
            // Optionally, search by name if needed
            GameObject namedPlayer = GameObject.Find("character_0(copy)");
            if (namedPlayer != null)
            {
                target = namedPlayer.transform;
            }
            else
            {
                return; // If no player found, exit the function
            }
        }

        // Smoothly follow the target once it's available
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }
}   