using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJoiningState : StateMachineBehaviour
{
    Enemy enemy;
    PlayableCharacter target;
    private bool join;
    private Vector3 destination;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();
        join = enemy.Join;
        destination = enemy.JoinDestination;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        target = enemy.Target;

        //distance btw enemy and destination
        float distance = Vector3.Distance(enemy.transform.position, destination);
        //round distance, because the enemy can't reach the exact point
        distance = Mathf.Round(distance);

        if (target == null && distance > 1 && join)
        {
            enemy.LimitZ();
            enemy.FlipHandler();

            Vector3 targetDirection = (new Vector3(destination.x - enemy.FacingDirection, destination.y, destination.z) - enemy.transform.position).normalized;
            enemy.rb.velocity = new Vector3(targetDirection.x * enemy.MoveSpeed, enemy.rb.velocity.y, targetDirection.z * enemy.MoveSpeed);
        }
        else
        {
            animator.SetBool("joined", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
