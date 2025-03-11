using UnityEngine;

public class MeleeCombat : MonoBehaviour
{
   
    private float attackRate = 2f; 
    private float nextAttackTime = 0f; // Time when the player can attack again

    // To check if the player is attacking
    private bool isAttacking = false;

    // Update is called once per frame
    void Update()
    {
     
        if (Input.GetKeyDown(KeyCode.E) && Time.time >= nextAttackTime)
        {
            Attack();
        }
    }

    void Attack()
    {
        // Set the time for the next attack
        nextAttackTime = Time.time + 1f / attackRate;

        isAttacking = true;

        Debug.Log("Attacking!");
       
	
			transform.Find("defaultStab").GetComponent<Slash>();
		
	

        isAttacking = false;
    }
}
