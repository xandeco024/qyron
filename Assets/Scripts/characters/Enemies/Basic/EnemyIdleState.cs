using UnityEngine;

public class EnemyIdleState : StateMachineBehaviour
{
    Enemy enemy;
    PlayableCharacter target;

    float idleTime;
    float currentIdleTime;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();

        target = enemy.Target;
        enemy.rb.velocity = Vector3.zero;
        idleTime = Random.Range(enemy.IdleTimeRange.x, enemy.IdleTimeRange.y);

        //start idle animation on random frame

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentIdleTime += Time.deltaTime;

        target = enemy.Target;

        if(currentIdleTime >= idleTime && enemy.FreeMovement && !enemy.Stunned)
        {
            animator.SetBool("freeWalk", true);
        }

        if (target != null && !enemy.Stunned)
        {
            animator.SetBool("following", true);
            animator.SetBool("freeWalk", false);
        }

        enemy.FlipHandler();
        enemy.LimitZ();
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentIdleTime = 0;
    }
}
