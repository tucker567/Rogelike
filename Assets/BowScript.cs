using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BowScript : MonoBehaviour
{
    public Vector2 direction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 bowPos = transform.position;
        direction = mousePos - bowPos;
        FaceMouse();
    }

    void FaceMouse()
    {
        transform.right = direction;
    }
}
