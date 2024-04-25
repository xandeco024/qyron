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

        if (target != null && Vector3.Distance(enemy.transform.position, target.transform.position) <= enemy.AttackRange)
        {
            animator.SetBool("following", false);
            //animator.SetBool("Attacking", true);
            Debug.Log("Deve atacar!");
        }
        else if (target != null)
        {
            Vector3 targetDirection = (target.transform.position - enemy.transform.position).normalized;
            enemy.rb.velocity = new Vector3(targetDirection.x * enemy.MoveSpeed, enemy.rb.velocity.y, targetDirection.z * enemy.MoveSpeed);
        }
        else
        {
            animator.SetBool("following", false);
        }

        enemy.LimitZ();
        enemy.FlipSprite();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
