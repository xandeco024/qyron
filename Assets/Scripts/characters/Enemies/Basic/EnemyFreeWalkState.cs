using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFreeWalkState : StateMachineBehaviour
{   
    Enemy enemy;
    float moveTime;
    float currentMoveTime;
    Vector3 moveDirection;
    PlayableCharacter target;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();
        moveTime = Random.Range(enemy.MoveTimeRange.x, enemy.MoveTimeRange.y);
        moveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentMoveTime += Time.deltaTime;
        target = enemy.Target;

        if(currentMoveTime >= moveTime)
        {
            animator.SetBool("freeWalk", false);
            animator.SetBool("following", false);
        }
        else
        {
            enemy.rb.velocity = new Vector3(moveDirection.x * enemy.MoveSpeed, enemy.rb.velocity.y, moveDirection.z * enemy.MoveSpeed);
        }

        if (target != null)
        {
            animator.SetBool("following", true);
            animator.SetBool("freeWalk", false);
        }

        enemy.LimitZ();
        enemy.Flip();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentMoveTime = 0;
    }
}
