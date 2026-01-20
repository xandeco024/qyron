using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : StateMachineBehaviour
{
    Enemy enemy;

    // Chance de usar heavy attack quando disponível (0-100)
    [SerializeField, Range(0, 100)] float heavyAttackChance = 30f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();

        // Primeiro ataque no target é sempre light (mais justo pro jogador)
        if (enemy.FirstAttackOnTarget)
        {
            if (enemy.CanLightAttack)
            {
                enemy.StartCoroutine(enemy.LightAttack());
            }
            return;
        }

        // Escolha aleatória de ataque
        bool wantsHeavy = Random.Range(0f, 100f) < heavyAttackChance;

        if (wantsHeavy && enemy.CanHeavyAttack)
        {
            enemy.StartCoroutine(enemy.HeavyAttack());
        }
        else if (enemy.CanLightAttack)
        {
            enemy.StartCoroutine(enemy.LightAttack());
        }
        else if (enemy.CanHeavyAttack)
        {
            // Fallback: se light não disponível mas heavy sim
            enemy.StartCoroutine(enemy.HeavyAttack());
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
