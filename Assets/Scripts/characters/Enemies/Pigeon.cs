using System.Collections;
using UnityEngine;

public class Pigeon : Enemy
{
    void Awake()
    {
        GetComponentsOnCharacter(); 
    }

    void Start()
    {
        players = FindObjectsOfType<PlayableCharacter>();
        SetStats();
    }

    void Update()
    {
        if (currentHealth <= 0 && !isReceivingCombo && !isDead)
        {
            isDead = true;
            animator.SetTrigger("deathTrigger");
        }

        target = FindTargetOnRange(players, targetRange);
    }

    override protected void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
}
