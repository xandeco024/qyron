using UnityEngine;

public class EnemyFollowingState : StateMachineBehaviour
{
    Enemy enemy;
    PlayableCharacter target; 

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        target = enemy.Target;

        if (target != null && enemy.IsMovingAllowed)
        {
            enemy.LimitZ();
            enemy.FlipHandler();

            if (enemy.PlayerOnAttackRange())
            {
                animator.SetBool("following", false);
                animator.SetTrigger("attack");
                animator.SetBool("combat", true);
            }

            else if (!enemy.PlayerOnAttackRange())
            {
                Vector3 targetDirection = (new Vector3(target.transform.position.x - enemy.FacingDirection, target.transform.position.y, target.transform.position.z) - enemy.transform.position).normalized;
                enemy.rb.velocity = new Vector3(targetDirection.x * enemy.MoveSpeed, enemy.rb.velocity.y, targetDirection.z * enemy.MoveSpeed);
            }

        }
        else
        {
            animator.SetBool("following", false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
