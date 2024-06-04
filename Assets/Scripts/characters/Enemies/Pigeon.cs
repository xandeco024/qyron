using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pigeon : Enemy
{
    void Awake()
    {
        GetComponentsOnCharacter(); 
    }

    void Start()
    {
        SetStats();
    }

    void Update()
    {
        if (target == null) target = FindTargetOnRange();

        if (target != null)
        {
            if (Vector3.Distance(transform.position, target.transform.position) > loseTargetAtRange || target.IsDowned)
            {
                target = null;
            }
        }

        if (currentHealth <= 0 && !isReceivingCombo && !isDead)
        {
            isDead = true;
            animator.SetTrigger("deathTrigger");
            animator.SetBool("dead", true);
        }

        animator.SetBool("stunned", stunned);

        if (target != null && lastFrameTarget == null)
        {
            firstAttackOnTarget = true;
        }

        lastFrameTarget = target;

        StepAssist();
        StunHandler();
    }

    public override IEnumerator LightAttack()
    {
        attackAnimationIndex = (attackAnimationIndex == 1) ? 0 : 1;
        animator.SetFloat("attackAnimationIndex", attackAnimationIndex);

        StartCoroutine(base.LightAttack());
        yield return null;
    }

    public override IEnumerator HeavyAttack()
    {
        attackAnimationIndex = 2;
        animator.SetFloat("attackAnimationIndex", attackAnimationIndex);
        canHeavyAttack = false;

        yield return new WaitForSeconds(heavyAttackDelay);

        bool critical = Random.Range(0, 100) < criticalChance;
        //Debug.Log(critical);
        float damage = attackDamage * 2 * (critical? 2f : 1f);


        Collider[] colliders = Physics.OverlapBox(transform.position + new Vector3(combatBoxOffset.x * facingDirection, combatBoxOffset.y, combatBoxOffset.y), combatBoxSize / 2, transform.rotation);
        
        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<PlayableCharacter>() != null)
            {
                collider.GetComponent<PlayableCharacter>().TakeDamage(damage / 2, 0.2f);
            }
        }

        yield return new WaitForSeconds(heavyAttackDelay / 2);

        colliders = Physics.OverlapBox(transform.position + new Vector3(combatBoxOffset.x * facingDirection, combatBoxOffset.y, combatBoxOffset.y), combatBoxSize / 2, transform.rotation);
        

        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<PlayableCharacter>() != null)
            {
                collider.GetComponent<PlayableCharacter>().TakeDamage(damage / 2, 0.2f);
            }
        }

        yield return new WaitForSeconds(heavyAttackCD);
        canHeavyAttack = true;

    }

    override protected void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, targetSearchBoxSize);

        if (debug && rb != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + new Vector3(stepAssistLimit.x * facingDirection, stepAssistLimit.y, stepAssistLimit.z), new Vector3(stepAssistDistance.x * facingDirection, 0 , 0));
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position + new Vector3(stepAssistRay.x * facingDirection, stepAssistRay.y, stepAssistRay.z), new Vector3(stepAssistDistance.x * facingDirection, 0 , 0));

            //z ray
            float zDirection = 1;
            
            if (rb.velocity.z < 0) zDirection = -1;

            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + new Vector3(stepAssistLimit.x, stepAssistLimit.y, stepAssistLimit.z * zDirection), new Vector3(0, 0, stepAssistDistance.z * zDirection));
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position + new Vector3(stepAssistRay.x, stepAssistRay.y, stepAssistRay.z * zDirection), new Vector3(0, 0, stepAssistDistance.z * zDirection));
        }
    }
}
