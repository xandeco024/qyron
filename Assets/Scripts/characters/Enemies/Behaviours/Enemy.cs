using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Drop
{
    public GameObject item;
    public float dropChance;
}

public class Enemy : Character
{
    protected List<PlayableCharacter> players;
    protected PlayableCharacter target;
    public PlayableCharacter Target { get => target; }
    protected PlayableCharacter lastFrameTarget;
    [SerializeField] protected Vector3 targetSearchBoxSize;
    public Vector3 RangeBoxSize { get => targetSearchBoxSize; }
    [SerializeField] protected float loseTargetAtRange;
    public float LoseTargetRange { get => loseTargetAtRange; }

    private bool join;
    public bool Join { get => join; }

    private Vector3 joinDestination;
    public Vector3 JoinDestination { get => joinDestination; }

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
    protected bool firstAttackOnTarget = true;
    public bool FirstAttackOnTarget { get => firstAttackOnTarget; }


    
    [Header("Fake Liberty")]
    [SerializeField] protected bool freeMovement;
    public bool FreeMovement { get => freeMovement; }
    [SerializeField] protected Vector2 moveTimeRange;
    public Vector2 MoveTimeRange { get => moveTimeRange; }
    [SerializeField] protected Vector2 idleTimeRange;
    public Vector2 IdleTimeRange { get => idleTimeRange; }



    [Header("Damage Tracking")]
    // Dictionary para rastrear quanto dano cada player causou
    private Dictionary<PlayableCharacter, float> damageDealt = new Dictionary<PlayableCharacter, float>();
    public Dictionary<PlayableCharacter, float> DamageDealt { get => damageDealt; }

    [Header("Rewards")]
    [SerializeField] protected int deathTime;
    public int DeathTime { get => deathTime; }
    [SerializeField] protected int coinAmount;
    public int CoinAmount { get => coinAmount; }
    [SerializeField] protected int xpAmount;
    public int XpAmount { get => xpAmount; }

    [Header("Cooperative Multiplier")]
    [SerializeField] protected float coopMultiplierPerPlayer = 0.1f; // 10% por player adicional

    [Header("Loot")]
    [SerializeField] protected Vector3 lootBoxSize;
    public Vector3 LootBoxSize { get => lootBoxSize; }
    [SerializeField] List<Drop> drops = new List<Drop>();
    public List<Drop> Drops { get => drops; }

    // TODO: Sistema de healers - quando implementado, trackear também cura recebida
    // e distribuir XP para healers proporcionalmente

    public override void TakeDamage(float damage, float stunDuration, bool critical = false, Vector3 knockbackDir = default, float knockbackForce = 0, float knockbackDuration = 0.2f)
    {
        // Mantém compatibilidade com código antigo - tenta encontrar o attacker pelo contexto
        TakeDamage(damage, stunDuration, critical, knockbackDir, knockbackForce, knockbackDuration, null);
    }

    /// <summary>
    /// Versão com tracking de quem causou o dano
    /// </summary>
    public void TakeDamage(float damage, float stunDuration, bool critical, Vector3 knockbackDir, float knockbackForce, float knockbackDuration, PlayableCharacter attacker)
    {
        if (!isDead)
        {
            if (isLightAttacking)
            {
                StartCoroutine(CancelLightAttack());
            }

            if (isHeavyAttacking)
            {
                StartCoroutine(CancelHeavyAttack());
            }

            // Registra o dano causado por este player
            if (attacker != null)
            {
                if (damageDealt.ContainsKey(attacker))
                {
                    damageDealt[attacker] += damage;
                }
                else
                {
                    damageDealt[attacker] = damage;
                }
            }

            base.TakeDamage(damage, stunDuration, critical, knockbackDir, knockbackForce, knockbackDuration);
            damageTime = knockbackDuration;
            animator.SetTrigger("damageTrigger");
            animator.SetBool("takingDamage", true);
            animator.SetFloat("knockbackY", Mathf.Abs(knockbackDir.y));
            animator.SetFloat("knockbackX", Mathf.Abs(knockbackDir.x));
        }
    }

    public virtual IEnumerator LightAttack()
    {
        firstAttackOnTarget = false;
        isLightAttacking = true;
        canLightAttack = false;
        yield return new WaitForSeconds(lightAttackDelay);

        bool critical = Random.Range(0, 100) < criticalChance;
        float damage = attackDamage * (critical ? 2f : 1f);

        Collider[] colliders = Physics.OverlapBox(transform.position + new Vector3(combatBoxOffset.x * facingDirection, combatBoxOffset.y, combatBoxOffset.z), combatBoxSize / 2, transform.rotation);
        foreach (Collider collider in colliders)
        {
            PlayableCharacter player = collider.GetComponent<PlayableCharacter>();
            if (player != null)
            {
                player.TakeDamage(damage, 0.1f);
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
        float damage = attackDamage * 2 * (critical ? 2f : 1f);

        Collider[] colliders = Physics.OverlapBox(transform.position + new Vector3(combatBoxOffset.x * facingDirection, combatBoxOffset.y, combatBoxOffset.z), combatBoxSize / 2, transform.rotation);
        foreach (Collider collider in colliders)
        {
            PlayableCharacter player = collider.GetComponent<PlayableCharacter>();
            if (player != null)
            {
                player.TakeDamage(damage, 0.2f);
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
        Collider[] colliders = Physics.OverlapBox(transform.position + new Vector3(combatBoxOffset.x * facingDirection, combatBoxOffset.y, combatBoxOffset.z), combatBoxSize / 2, transform.rotation);
        foreach (Collider collider in colliders)
        {
            PlayableCharacter player = collider.GetComponent<PlayableCharacter>();
            if (player != null && player == target)
            {
                return true;
            }
        }
        return false;
    }

    protected PlayableCharacter FindTargetOnRange()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, targetSearchBoxSize / 2, transform.rotation);
        List<PlayableCharacter> playersOnRange = new List<PlayableCharacter>();

        foreach (Collider collider in colliders)
        {
            PlayableCharacter player = collider.GetComponent<PlayableCharacter>();
            if (player != null && !player.IsDowned)
            {
                playersOnRange.Add(player);
            }
        }

        if (playersOnRange.Count > 0)
        {
            return playersOnRange[Random.Range(0, playersOnRange.Count)];
        }

        return null;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        //draw combat box
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + new Vector3(combatBoxOffset.x * facingDirection, combatBoxOffset.y, combatBoxOffset.z), combatBoxSize);
    
        //draw xp box
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, lootBoxSize);
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

        DropLoot();
        GiveCoins(coinAmount);
        GiveXP(xpAmount);

        Destroy(gameObject);
    }

    /// <summary>
    /// Distribui coins igualmente entre todos que participaram do combate
    /// </summary>
    public void GiveCoins(int coinAmount)
    {
        // Filtra apenas jogadores que deram pelo menos 1 hit (threshold de participação)
        List<PlayableCharacter> participants = new List<PlayableCharacter>();
        foreach (var kvp in damageDealt)
        {
            if (kvp.Value > 0 && kvp.Key != null)
            {
                participants.Add(kvp.Key);
            }
        }

        if (participants.Count == 0)
        {
            if (debug) Debug.Log("No players participated in combat to receive coins");
            return;
        }

        // Divide coins igualmente entre participantes
        int coinsPerPlayer = Mathf.RoundToInt(coinAmount / (float)participants.Count);

        foreach (PlayableCharacter player in participants)
        {
            player.AddCoins(coinsPerPlayer);
        }

        if (debug) Debug.Log($"Gave {coinsPerPlayer} coins to {participants.Count} participating players");
    }

    /// <summary>
    /// Distribui XP proporcionalmente ao dano causado + multiplicador cooperativo
    /// </summary>
    public void GiveXP(int xpAmount)
    {
        // Filtra apenas jogadores que deram pelo menos 1 hit
        List<PlayableCharacter> participants = new List<PlayableCharacter>();
        float totalDamage = 0f;

        foreach (var kvp in damageDealt)
        {
            if (kvp.Value > 0 && kvp.Key != null)
            {
                participants.Add(kvp.Key);
                totalDamage += kvp.Value;
            }
        }

        if (participants.Count == 0)
        {
            if (debug) Debug.Log("No players participated in combat to receive XP");
            return;
        }

        // Calcula multiplicador cooperativo: 1.0x + (0.1x por player adicional)
        // 1 player = 1.0x, 2 players = 1.1x, 3 players = 1.2x, 4 players = 1.3x
        float coopMultiplier = 1.0f + (coopMultiplierPerPlayer * (participants.Count - 1));
        float totalXP = xpAmount * coopMultiplier;

        if (debug) Debug.Log($"Total XP: {totalXP} (base: {xpAmount}, multiplier: {coopMultiplier}x for {participants.Count} players)");

        // Distribui XP proporcionalmente ao dano causado
        foreach (PlayableCharacter player in participants)
        {
            float damagePercent = damageDealt[player] / totalDamage;
            int playerXP = Mathf.RoundToInt(totalXP * damagePercent);

            player.AddExP(playerXP);

            if (debug) Debug.Log($"{player.name}: {playerXP} XP ({damagePercent:P1} damage share - {damageDealt[player]}/{totalDamage} dmg)");
        }
    }

    private List<PlayableCharacter> GetPlayersInLootRange()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, lootBoxSize / 2, transform.rotation);
        List<PlayableCharacter> playersOnRange = new List<PlayableCharacter>();

        foreach (Collider collider in colliders)
        {
            PlayableCharacter player = collider.GetComponent<PlayableCharacter>();
            if (player != null)
            {
                playersOnRange.Add(player);
            }
        }

        return playersOnRange;
    } 

    public void DropLoot()
    {
        foreach (Drop drop in drops)
        {
            if (Random.Range(0, 100) < drop.dropChance)
            {
                Instantiate(drop.item, transform.position, Quaternion.identity);
            }
        }
    }

    public void SetJoin(bool join, Vector3 destination)
    {
        this.join = join;
        joinDestination = destination;
    }
}
