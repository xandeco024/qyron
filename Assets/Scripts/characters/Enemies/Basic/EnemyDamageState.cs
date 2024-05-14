using UnityEngine;

public class EnemyDamageState : StateMachineBehaviour
{
    Enemy enemy;
    float damageTimeCounter = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        damageTimeCounter += Time.deltaTime;

        if (damageTimeCounter >= enemy.DamageTime)
        {
            damageTimeCounter = 0;
            animator.SetBool("takingDamage", false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
