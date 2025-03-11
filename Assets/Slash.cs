using UnityEngine;

public class Slash : MonoBehaviour
{
public IEnumerator Slash() {
    spriteRenderer.enabled = true;           
    yield return new WaitForSeconds(0.25f);  
    spriteRenderer.enabled = false;          
}
}