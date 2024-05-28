using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy : Character
{
    protected List<PlayableCharacter> players;
    protected PlayableCharacter target;
    public PlayableCharacter Target { get => target; }
    protected PlayableCharacter lastFrameTarget;
    [SerializeField] protected float targetRange;
    public float TargetRange { get => targetRange; }
    [SerializeField] protected Vector3 rangeBoxSize;
    public Vector3 RangeBoxSize { get => rangeBoxSize; }
    [SerializeField] protected float loseTargetRange;
    public float LoseTargetRange { get => loseTargetRange; }


    [Header("Attack")]
    protected bool canLightAttack = true;
    public bool CanLightAttack { get => canLightAttack; }
    [SerializeField] protected float lightAttackDelay;
    public float LightAttackDelay { get => lightAttackDelay; }
    [SerializeField] protected float lightAttackCD;
    public float LightAttackCD { get => lightAttackCD; }
    [SerializeField] protected bool canHeavyAttack = true;
    public bool CanHeavyAttack { get => canHeavyAttack; }
    [SerializeField] protected float heavyAttackDelay;
    public float HeavyAttackDelay { get => heavyAttackDelay; }
    [SerializeField] protected float heavyAttackCD;
    public float HeavyAttackCD { get => heavyAttackCD; }
    [SerializeField] protected Vector3 combatBoxSize;
    public Vector3 CombatBoxSize { get => combatBoxSize; }
    [SerializeField] protected Vector3 combatBoxOffset;
    public Vector3 CombatBoxOffset { get => combatBoxOffset; }
    protected bool firstAttackOnTarget = true;
    public bool FirstAttackOnTarget { get => firstAttackOnTarget; }


    
    [Header("Fake Liberty")]
    [SerializeField] protected bool freeMovement;
    public bool FreeMovement { get => freeMovement; }
    [SerializeField] protected Vector2 moveTimeRange;
    public Vector2 MoveTimeRange { get => moveTimeRange; }
    [SerializeField] protected Vector2 idleTimeRange;
    public Vector2 IdleTimeRange { get => idleTimeRange; }



    [Header("Other")]
    [SerializeField] protected int deathTime;
    public int DeathTime { get => deathTime; }
    [SerializeField] protected int xpAmount;
    public int XpAmount { get => xpAmount; }
    [SerializeField] protected Vector3 exPBoxSize;
    public Vector3 XpBoxSize { get => exPBoxSize; }

    public override void TakeDamage(float damage, float stunDuration, bool critical = false, Vector3 knockbackDir = default, float knockbackForce = 0, float knockbackDuration = 0.2f)
    {
        if (isLightAttacking)
        {
            StartCoroutine(CancelLightAttack());
        }

        if (isHeavyAttacking)
        {
            StartCoroutine(CancelHeavyAttack());
        }

        base.TakeDamage(damage, stunDuration, critical, knockbackDir, knockbackForce, knockbackDuration);
        damageTime = knockbackDuration;
        animator.SetTrigger("damageTrigger");
        animator.SetBool("takingDamage", true);
        animator.SetFloat("knockbackY", Mathf.Abs(knockbackDir.y));
        animator.SetFloat("knockbackX", Mathf.Abs(knockbackDir.x));
    }

    public virtual IEnumerator LightAttack()
    {
        firstAttackOnTarget = false;
        isLightAttacking = true;
        canLightAttack = false;
        yield return new WaitForSeconds(lightAttackDelay);

        bool critical = Random.Range(0, 100) < criticalChance;
        //Debug.Log(critical);
        float damage = attackDamage * (critical? 2f : 1f);


        Collider[] colliders = Physics.OverlapBox(transform.position + new Vector3(combatBoxOffset.x * facingDirection, combatBoxOffset.y, combatBoxOffset.y), combatBoxSize / 2, transform.rotation);
        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<PlayableCharacter>() != null)
            {
                collider.GetComponent<PlayableCharacter>().TakeDamage(damage, 0.1f);
            }
        }

        yield return new WaitForSeconds(lightAttackCD);
        canLightAttack = true;
        isLightAttacking = false;
    }

    protected IEnumerator CancelLightAttack()
    {
        StopCoroutine(LightAttack());
        isLightAttacking = false;
        yield return new WaitForSeconds(lightAttackDelay);
        canLightAttack = true;
    }

    public virtual IEnumerator HeavyAttack()
    {
        isHeavyAttacking = true;
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
                collider.GetComponent<PlayableCharacter>().TakeDamage(damage, 0.2f);
            }
        }

        yield return new WaitForSeconds(heavyAttackCD);
        canHeavyAttack = true;
        isHeavyAttacking = false;
    }

    protected IEnumerator CancelHeavyAttack()
    {
        StopCoroutine(HeavyAttack());
        isHeavyAttacking = false;
        yield return new WaitForSeconds(heavyAttackDelay);
        canHeavyAttack = true;
    }

    public bool PlayerOnAttackRange()
    {
        bool range = false;
        Collider[] colliders = Physics.OverlapBox(transform.position + new Vector3(combatBoxOffset.x * facingDirection, combatBoxOffset.y, combatBoxOffset.y), combatBoxSize / 2, transform.rotation);
        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<PlayableCharacter>() != null)
            {
                range = true;
                break;
            }
        }
        return range;
    }

    protected PlayableCharacter FindTargetOnRange()
    {
        PlayableCharacter target = null;

        Collider[] colliders = Physics.OverlapBox(transform.position, rangeBoxSize / 2, transform.rotation);

        List<PlayableCharacter> playersOnRange = new List<PlayableCharacter>();

        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<PlayableCharacter>() != null)
            {
                playersOnRange.Add(collider.GetComponent<PlayableCharacter>());
            }
        }

        if (playersOnRange.Count > 0)
        {
            target = playersOnRange[Random.Range(0, playersOnRange.Count)];
        }

        return target;
    }

    protected virtual void OnDrawGizmos()
    {
        //draw combat box
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + new Vector3(combatBoxOffset.x * facingDirection, combatBoxOffset.y, combatBoxOffset.z), combatBoxSize);
    
        //draw xp box
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, exPBoxSize);
    }

    public IEnumerator Die(int deathTime = 0)
    {
        int timesToFlash = deathTime * 5;

        if (GetComponent<SpriteRenderer>() != null)
        {
            for (int i = 0; i < timesToFlash; i++)
            {
                sr.color = new Color(1, 1, 1, 0);
                yield return new WaitForSeconds(0.1f);
                sr.color = new Color(1, 1, 1, 1);
                yield return new WaitForSeconds(0.1f);
            }
        }
        else if (GetComponents<Renderer>() != null)
        {
            for (int i = 0; i < timesToFlash; i++)
            {
                foreach (Renderer renderer in GetComponents<Renderer>())
                {
                    renderer.material.color = new Color(1, 1, 1, 0);
                }
                yield return new WaitForSeconds(0.1f);
                foreach (Renderer renderer in GetComponents<Renderer>())
                {
                    renderer.material.color = new Color(1, 1, 1, 1);
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            Debug.Log("No sprite renderer or renderer found");
            Debug.Log("Destroying object in " + deathTime + " seconds");
            yield return new WaitForSeconds(deathTime);
        }

        //StopAllCoroutines();
        Destroy(gameObject);
    }

    public void GiveXP(int xpAmount)
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, exPBoxSize / 2, transform.rotation);
        List<PlayableCharacter> playersOnRange = new List<PlayableCharacter>();

        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<PlayableCharacter>() != null)
            {
                playersOnRange.Add(collider.GetComponent<PlayableCharacter>());
            }
        }

        foreach (PlayableCharacter player in playersOnRange)
        {
            player.AddExP(Mathf.RoundToInt(xpAmount / playersOnRange.Count));
        }

        if(debug) Debug.Log("Gave " + Mathf.RoundToInt(xpAmount / playersOnRange.Count) + " xp to " + playersOnRange.Count + " players");
    } 
}
