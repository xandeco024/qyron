using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EnemyWaitingCDState : StateMachineBehaviour
{
    Enemy enemy;

    // Hesitation: tempo de "pensar" antes de atacar novamente
    [SerializeField] Vector2 hesitationRange = new Vector2(0.15f, 0.5f);
    float hesitationTime;
    float hesitationTimer;
    bool canAttackAfterHesitation;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();

        // Define um tempo aleatório de hesitação
        hesitationTime = Random.Range(hesitationRange.x, hesitationRange.y);
        hesitationTimer = 0f;
        canAttackAfterHesitation = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Player saiu do range - perseguir
        if (!enemy.PlayerOnAttackRange())
        {
            animator.SetBool("following", true);
            animator.SetBool("combat", false);
            return;
        }

        // Conta o tempo de hesitação
        hesitationTimer += Time.deltaTime;
        if (hesitationTimer >= hesitationTime)
        {
            canAttackAfterHesitation = true;
        }

        // Só ataca depois de "pensar" um pouco
        if (canAttackAfterHesitation && (enemy.CanHeavyAttack || enemy.CanLightAttack))
        {
            animator.SetTrigger("attack");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hesitationTimer = 0f;
        canAttackAfterHesitation = false;
    }
}
