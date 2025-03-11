using UnityEngine;
using System.Collections;


public class Slash : MonoBehaviour
{
public IEnumerator Cut() {
    gameObject.GetComponent<Renderer>().enabled = true;           
    yield return new WaitForSeconds(0.25f);  
    gameObject.GetComponent<Renderer>().enabled = false;          
}
}