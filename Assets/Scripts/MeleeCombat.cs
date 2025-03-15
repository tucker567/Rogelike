using UnityEngine;


public class MeleeCombat : MonoBehaviour
{
   public Slash slash;

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
        nextAttackTime = Time.time + 1f / attackRate;

    isAttacking = true;

    Debug.Log("Attacking!");

    if (slash != null)
    {
        StartCoroutine(slash.Cut());
    }

    isAttacking = false;
    }
}