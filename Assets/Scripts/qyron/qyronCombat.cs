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


    [Header("Qyron")]
    private Rigidbody qyronRB;
    private SpriteRenderer qyronSR;
    private BoxCollider qyronCol;
    private qyronMovement qyronMovement;
    private qyronSFX qyronSFX;
    private Animator qyronAnimator;

    [Header("Combat & Combo")]
    [SerializeField] private float attackDamage;
    [SerializeField] private float invincibilityTime;
    [SerializeField] private List<string> combo = new List<string>();
    private bool isInvincible = false;
    private bool isAttacking;
    private float timeLastHit;

    [Header("Combat Hitbox")]
    [SerializeField] Vector3 CombatRaycastSize;
    [SerializeField] Vector3 CombatBoxOffset;
    [SerializeField] Collider[] qyronHitCollision;
    private LayerMask enemyLayer;
    private int direction;

    [Header("Health & Stamina")]
    [SerializeField] private float maxHealth;
    private float currentHealth;
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

        currentHealth = maxHealth;
        currentStamina = maxStamina;
        direction = GetDirection();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("danos");
            StartCoroutine(TakeDamage(1, true, new Vector3(2, 1, 0)));
        }

        direction = GetDirection();
        qyronHitCollision = Physics.OverlapBox(transform.position + CombatBoxOffset * direction, CombatRaycastSize / 2, transform.rotation, enemyLayer);

        if (Time.timeScale == 0)
        {
            return;
        }

        else if(currentHealth >= 1)

        {
            HealthStaminaRecoveryHandler();

            if (Input.GetMouseButtonDown(0))
            {
                if(!isAttacking && combo.Skip(combo.Count - 3).SequenceEqual(new List<string> { "basicPunch", "basicPunch", "basicPunch"}))
                {
                    StartCoroutine(Combo1(attackDamage * 2, 6f));
                }

                else if (!isAttacking && currentStamina >= 3)
                {
                    StartCoroutine(BasicPunch(attackDamage, 3f));
                }

            }

            if(Input.GetMouseButtonDown(1))
            {
                if (!isAttacking && combo.Skip(combo.Count - 3).SequenceEqual(new List<string> { "kneeStrike", "kneeStrike", "kneeStrike" }))
                {
                    StartCoroutine(Combo2(attackDamage * 3, 10f));
                }

                else if (!isAttacking && currentStamina >= 5)
                {
                    StartCoroutine(KneeStike(attackDamage * 3 / 2, 5f));
                }
            }

            if (Time.time - timeLastHit >= 2)
            {
                qyronAnimator.SetTrigger("Idle 1");
                combo.Clear();
            }
        }
    }

    public IEnumerator TakeDamage(float damage, bool takeKnockBack, Vector3 knockBackForce)
    {
        if(!isInvincible && !isAttacking)
        {
            isInvincible = true;
            qyronMovement.canMove = false;
            currentHealth -= damage;
            qyronAnimator.SetBool("isTakingDamage", true);
            StartCoroutine(FlashRed());
            if (takeKnockBack)
            {
                qyronRB.AddForce(new Vector3(knockBackForce.x * -direction, knockBackForce.y, knockBackForce.z), ForceMode.Impulse);
            }

            StartCoroutine(ScreenShake(2f, 1f, 0.2f));
            yield return new WaitForSeconds(invincibilityTime);
            qyronAnimator.SetBool("isTakingDamage", false);
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
        currentStamina -= staminaCost;
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
            qyronSFX.PlayAttackSFX("ataque 1");

            for(int i = 0; i < qyronHitCollision.Length; i++)
            {
                if(qyronHitCollision[i].tag == "Dummy")
                {
                    qyronHitCollision[i].GetComponent<dummy>().TakeDamage(damage);
                }

                else if (qyronHitCollision[i].tag == "Enemy")
                {
                    if (qyronHitCollision[i].GetComponent<enemyCombat>() != null)
                    {
                        StartCoroutine(qyronHitCollision[i].GetComponent<enemyCombat>().TakeDamage(damage, true, new Vector3(1, 2, 0), 0.5f));
                    }
                }
            }

            StartCoroutine(ScreenShake(0.5f, 0.25f, 0.1f));
        }

        else

        {
            qyronSFX.PlayMissSFX("miss 1");
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
            qyronSFX.PlayAttackSFX("combo 1");

            for (int i = 0; i < qyronHitCollision.Length; i++)
            {
                if (qyronHitCollision[i].tag == "Dummy")
                {
                    qyronHitCollision[i].GetComponent<dummy>().TakeDamage(damage);
                }

                else if (qyronHitCollision[i].tag == "Enemy")
                {
                    if (qyronHitCollision[i].GetComponent<enemyCombat>() != null)
                    {
                        StartCoroutine(qyronHitCollision[i].GetComponent<enemyCombat>().TakeDamage(damage, true, new Vector3(1, 2, 0), 0.5f));
                    }
                }
            }

            StartCoroutine(ScreenShake(2f, 0.5f, 0.1f));
        }

        else
        {
            qyronSFX.PlayMissSFX("miss 2");
        }

        yield return new WaitForSeconds(.35f);
        isAttacking = false;
    }

    IEnumerator Combo2(float damage, float staminaCost)
    {
        isAttacking = true;
        combo.Clear();
        currentStamina -= staminaCost;
        timeLastHit = Time.time;

        qyronAnimator.SetTrigger("Combo 2");

        if (qyronHitCollision.Length >= 1)
        {
            qyronSFX.PlayAttackSFX("combo 2");

            for (int i = 0; i < qyronHitCollision.Length; i++)
            {
                if (qyronHitCollision[i].tag == "Dummy")
                {
                    qyronHitCollision[i].GetComponent<dummy>().TakeDamage(damage);
                }

                else if (qyronHitCollision[i].tag == "Enemy")
                {
                    if (qyronHitCollision[i].GetComponent<enemyCombat>() != null)
                    {
                        StartCoroutine(qyronHitCollision[i].GetComponent<enemyCombat>().TakeDamage(damage, true, new Vector3(1, 2, 0), 0.5f));
                    }
                }
            }

            StartCoroutine(ScreenShake(2f, 0.5f, 0.1f));
        }

        else
        {
            qyronSFX.PlayMissSFX("miss 2");
        }

        yield return new WaitForSeconds(.35f);
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
            qyronSFX.PlayAttackSFX("ataque 2");

            for (int i = 0; i < qyronHitCollision.Length; i++)
            {
                if (qyronHitCollision[i].tag == "Dummy")
                {
                    qyronHitCollision[i].GetComponent<dummy>().TakeDamage(damage);
                }

                else if (qyronHitCollision[i].tag == "Enemy")
                {
                    if (qyronHitCollision[i].GetComponent<enemyCombat>() != null)
                    {
                        StartCoroutine(qyronHitCollision[i].GetComponent<enemyCombat>().TakeDamage(damage, true, new Vector3(1, 2, 0), 0.5f));
                    }
                }
            }

            StartCoroutine(ScreenShake(1f, 0.5f, 0.1f));   
        }

        else
        {
            qyronSFX.PlayMissSFX("miss 1");
        }

        yield return new WaitForSeconds(.3f);
        isAttacking = false;
    }

    public void SetInvincible(bool invincible)
    {
        isInvincible = invincible;
    }

    public bool GetAttacking()
    {
        return isAttacking;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
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

    private void HealthStaminaRecoveryHandler()
    {
        if (currentHealth < maxHealth)
        {
            lastHealthRecoveryTime += Time.deltaTime;

            if (lastHealthRecoveryTime >= 1f / healthRecoveryRate)
            {
                currentHealth += 1f;
                lastHealthRecoveryTime = 0f;
            }
        }

        if (currentStamina < maxStamina)
        {
            lastStaminaRecoveryTime += Time.deltaTime;

            if (lastStaminaRecoveryTime >= 1f / staminaRecoveryRate)
            {
                currentStamina += 1f;
                lastStaminaRecoveryTime = 0f;
            }
        }
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
