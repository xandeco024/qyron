using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class qyronCombat : MonoBehaviour
{


    [SerializeField] private float attackDamage;
    [SerializeField] private bool isAttacking;

    private LayerMask enemyLayer;

    [Header("Qyron")]
    private Rigidbody qyronRB;
    private SpriteRenderer qyronSR;
    private BoxCollider qyronCol;
    private qyronMovement qyronMovement;
    private qyronSFX qyronSFX;
    private Animator qyronAnimator;

    [SerializeField] private List<string> combo = new List<string>();
    private float timeLastHit;

    [SerializeField] private float invincibilityTime;
    private bool isInvincible = false;

    [SerializeField] Vector3 CombatRaycastSize;
    [SerializeField] float maxSize;
    [SerializeField] Vector3 CombatBoxOffset;
    Vector3 attackDirection;

    [SerializeField] Collider[] qyronHitCollision;

    private int direction;

    [Header("Health & Stamina")]
    [SerializeField] private float maxHealth;
    private float health;
    [SerializeField] private float healthRecoveryRate;
    private float lastHealthRecoveryTime;
    private Color damageColor = new Color(0.5f,0.2f,0.2f,1), originalColor = new Color(1,1,1,1);
    [SerializeField] private float maxStamina;
    private float currentStamina;
    [SerializeField] private float staminaRecoveryRate;
    private float lastStaminaRecoveryTime;


    [Header("Attack Animations")]
    private int basicPunchAnimation = 1;
    private int kneeStrikeAnimation = 1;


    void Start()
    {
        enemyLayer = LayerMask.GetMask("Enemy");

        qyronMovement = GetComponent<qyronMovement>();
        qyronCol = GetComponent<BoxCollider>();
        qyronSR = GetComponent<SpriteRenderer>();
        qyronRB = GetComponent<Rigidbody>();
        qyronSFX = GetComponent<qyronSFX>();
        qyronAnimator = GetComponent<Animator>();

        health = maxHealth;
        currentStamina = maxStamina;
        direction = GetDirection();
    }

    void Update()
    {
        direction = GetDirection();
        attackDirection = new Vector3(direction, 0, 0);
        qyronHitCollision = Physics.OverlapBox(transform.position + CombatBoxOffset * direction, CombatRaycastSize / 2, transform.rotation, enemyLayer);

        if (Time.timeScale == 0)
        {
            return;
        }

        else

        {
            if (currentStamina < maxStamina)
            {
                lastStaminaRecoveryTime += Time.deltaTime;

                if (lastStaminaRecoveryTime >= 1f / staminaRecoveryRate)
                {
                    currentStamina += 1f;
                    lastStaminaRecoveryTime = 0f;
                }
            }

            if (Input.GetMouseButtonDown(0) && !isAttacking && currentStamina >= 3) StartCoroutine(BasicPunch(attackDamage, 3f));
            if (Input.GetMouseButtonDown(1) && !isAttacking && currentStamina >= 3) StartCoroutine(KneeStike(attackDamage, 5f));

            if (Time.time - timeLastHit >= 2)
            {
                qyronAnimator.SetTrigger("Idle 1");
                combo.Clear();
            }

            if(combo.Count == 3)
            {
                if(combo.Skip(combo.Count - 3).SequenceEqual(new List<string>{ "basicAttack", "basicAttack", "basicAttack"}))
                {
                    qyronSFX.PlayAttackSFX(1);
                    Debug.Log("Combo1");
                    combo.Clear();
                }
            }
        }
    }

    public IEnumerator TakeDamage(float damage, bool takeKnockBack, Vector3 knockBackForce)
    {
        if(!isInvincible)
        {
            isInvincible = true;
            qyronMovement.canMove = false;
            health -= damage;
            StartCoroutine(FlashRed());
            if (takeKnockBack)
            {
                qyronRB.AddForce(new Vector3(knockBackForce.x * -direction, knockBackForce.y, knockBackForce.z), ForceMode.Impulse);
            }

            StartCoroutine(ScreenShake(2f, 1f, 0.2f));
            yield return new WaitForSeconds(invincibilityTime);
            isInvincible = false;
            qyronMovement.canMove = true;
        }
        else
        {
            yield return null;
        }
    }

    IEnumerator FlashRed()
    {
        for (int i = 0; i < 3; i++)
        {
            qyronSR.color = damageColor;
            yield return new WaitForSeconds(0.1f);
            qyronSR.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator BasicPunch(float damage, float staminaCost)
    {


        isAttacking = true;
        combo.Add("basicPunch");
        currentStamina -= 3f;
        timeLastHit = Time.time;

        if (basicPunchAnimation == 1)
        {
            basicPunchAnimation = 0;
            qyronAnimator.SetTrigger("Attack 1");
        }

        else if (basicPunchAnimation == 0)
        {
            basicPunchAnimation = 1;
            qyronAnimator.SetTrigger("Attack 2");
        }

        if (qyronHitCollision.Length >= 1)
        {
            qyronSFX.PlayAttackSFX(0);

            for(int i = 0; i < qyronHitCollision.Length; i++)
            {
                if(qyronHitCollision[i].tag == "Dummy")
                {
                    qyronHitCollision[i].GetComponent<dummy>().TakeDamage(damage);
                }

                else if (qyronHitCollision[i].tag == "Enemy")
                {
                    StartCoroutine(qyronHitCollision[i].GetComponent<enemyCombat>().TakeDamage(damage, true, new Vector3(1, 2, 0), 0.5f));
                }
            }

            StartCoroutine(ScreenShake(1f, 0.5f, 0.1f));
        }

        else

        {
            qyronSFX.PlayMissSFX(0);
        }

        yield return new WaitForSeconds(.2f);

        //qyronMovement.canMove = true;
        isAttacking = false;
    }

    IEnumerator Combo1(float damage, float staminaCost)
    {
        isAttacking = true;
        combo.Clear();
        currentStamina -= staminaCost;
        timeLastHit = Time.time;

        qyronAnimator.SetTrigger("Combo 1");

        if (qyronHitCollision.Length >= 1)
        {
            qyronSFX.PlayAttackSFX(2);

            for (int i = 0; i < qyronHitCollision.Length; i++)
            {
                if (qyronHitCollision[i].tag == "Dummy")
                {
                    qyronHitCollision[i].GetComponent<dummy>().TakeDamage(damage);
                }

                else if (qyronHitCollision[i].tag == "Enemy")
                {
                    StartCoroutine(qyronHitCollision[i].GetComponent<enemyCombat>().TakeDamage(damage, true, new Vector3(1, 2, 0), 0.5f));
                }
            }

            StartCoroutine(ScreenShake(2f, 0.5f, 0.1f));
        }

        else
        {
            qyronSFX.PlayMissSFX(1);
        }

        yield return new WaitForSeconds(.5f);
        isAttacking = false;
    }

    IEnumerator KneeStike(float damage, float staminaCost)
    {
        isAttacking = true;
        combo.Add("kneeStrike");
        currentStamina -= staminaCost;
        timeLastHit = Time.time;

        if(kneeStrikeAnimation == 1)
        {
            kneeStrikeAnimation = 0;
            qyronAnimator.SetTrigger("Knee Strike 1");
        }

        else if (kneeStrikeAnimation == 0)
        {
            kneeStrikeAnimation = 1;
            qyronAnimator.SetTrigger("Knee Strike 2");
        }

        if (qyronHitCollision.Length >= 1)
        {
            qyronSFX.PlayAttackSFX(1);

            for (int i = 0; i < qyronHitCollision.Length; i++)
            {
                if (qyronHitCollision[i].tag == "Dummy")
                {
                    qyronHitCollision[i].GetComponent<dummy>().TakeDamage(damage);
                }

                else if (qyronHitCollision[i].tag == "Enemy")
                {
                    StartCoroutine(qyronHitCollision[i].GetComponent<enemyCombat>().TakeDamage(damage, true, new Vector3(1, 2, 0), 0.5f));
                }
            }

            StartCoroutine(ScreenShake(1f, 0.5f, 0.1f));   
        }

        else
        {
            qyronSFX.PlayMissSFX(0);
        }

        yield return new WaitForSeconds(.5f);
        isAttacking = false;
    }

    public float GetCurrentHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetCurrentStamina()
    {
        return currentStamina;
    }

    public float GetMaxStamina()
    {
        return maxStamina;
    }

    public IEnumerator ScreenShake(float amplitude, float frequency, float duration)
    {
       CinemachineVirtualCamera CMCamera = GameObject.FindWithTag("CMMainCamera").GetComponent<CinemachineVirtualCamera>();
        if (CMCamera != null)
        {
            CinemachineBasicMultiChannelPerlin perlin = CMCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (perlin != null)
            {
                perlin.m_AmplitudeGain = amplitude;
                perlin.m_FrequencyGain = frequency;

                yield return new WaitForSeconds(duration);

                perlin.m_AmplitudeGain = 0;
                perlin.m_FrequencyGain = 0;
            }
        }

        yield return null;
    }

    private int GetDirection()
    {
        if (qyronSR.flipX)
        {
            return -1;
        }

        else return 1;
    }

    private void OnDrawGizmos()
    {
        if(qyronHitCollision.Length >= 1)
        {
            Gizmos.color = Color.red;
        }

        else

        {
            Gizmos.color = Color.yellow;
        }

        Gizmos.DrawWireCube(transform.position + CombatBoxOffset * direction, CombatRaycastSize);
    }
}
