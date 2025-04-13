using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowShoot : MonoBehaviour
{
    public float LaunchForce;   // Fixed: added semicolon
    public GameObject arrowPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))  // Fixed: use GetKeyDown with correct KeyCode
        {
            ShootArrow();
        }
    }

    void ShootArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        rb.AddForce(direction.normalized * LaunchForce, ForceMode2D.Impulse);
    }
}
