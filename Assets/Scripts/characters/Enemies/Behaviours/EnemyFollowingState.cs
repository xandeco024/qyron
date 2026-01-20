using UnityEngine;

public class EnemyFollowingState : StateMachineBehaviour
{
    Enemy enemy;
    PlayableCharacter target;

    // Configurações de comportamento orgânico
    [Header("Approach Behavior")]
    [SerializeField] float safeDistance = 3f;           // Distância para recuar se player atacando
    [SerializeField] float retreatSpeed = 0.6f;         // Multiplicador de velocidade ao recuar
    [SerializeField] float cautiousDistance = 5f;       // Distância para começar a ser cauteloso

    [Header("Strafe Variation")]
    [SerializeField] float strafeAmount = 0.3f;         // Quanto movimento lateral adicionar
    [SerializeField] float strafeChangeInterval = 0.8f; // Tempo para mudar direção do strafe

    float strafeDirection;
    float strafeTimer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();

        // Direção inicial aleatória do strafe (esquerda ou direita no eixo Z)
        strafeDirection = Random.Range(0, 2) == 0 ? 1f : -1f;
        strafeTimer = 0f;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        target = enemy.Target;

        if (target == null || !enemy.IsMovingAllowed)
        {
            animator.SetBool("following", false);
            return;
        }

        enemy.LimitZ();
        enemy.FlipHandler();

        // Chegou no range de ataque
        if (enemy.PlayerOnAttackRange())
        {
            animator.SetBool("following", false);
            animator.SetTrigger("attack");
            animator.SetBool("combat", true);
            return;
        }

        float distanceToTarget = Vector3.Distance(enemy.transform.position, target.transform.position);

        // RECUO: Se player está atacando e inimigo está perto demais
        if (target.IsAttacking && distanceToTarget < safeDistance)
        {
            // Recua na direção oposta ao player
            Vector3 retreatDirection = (enemy.transform.position - target.transform.position).normalized;
            enemy.rb.linearVelocity = new Vector3(
                retreatDirection.x * enemy.MoveSpeed * retreatSpeed,
                enemy.rb.linearVelocity.y,
                retreatDirection.z * enemy.MoveSpeed * retreatSpeed
            );
            return;
        }

        // APPROACH com variação
        Vector3 targetDirection = (target.transform.position - enemy.transform.position).normalized;

        // Atualiza direção do strafe periodicamente
        strafeTimer += Time.deltaTime;
        if (strafeTimer >= strafeChangeInterval)
        {
            strafeTimer = 0f;
            // Chance de mudar direção ou manter
            if (Random.Range(0f, 1f) > 0.5f)
            {
                strafeDirection *= -1f;
            }
        }

        // Calcula movimento base em direção ao player
        float moveX = targetDirection.x * enemy.MoveSpeed;
        float moveZ = targetDirection.z * enemy.MoveSpeed;

        // Adiciona strafe lateral quando está em distância cautelosa
        // (não vai em linha reta, faz um arco)
        if (distanceToTarget < cautiousDistance)
        {
            // Adiciona movimento perpendicular (strafe)
            // Perpendicular ao eixo X é o Z
            moveZ += strafeDirection * strafeAmount * enemy.MoveSpeed;
        }

        enemy.rb.linearVelocity = new Vector3(moveX, enemy.rb.linearVelocity.y, moveZ);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        strafeTimer = 0f;
    }
}
