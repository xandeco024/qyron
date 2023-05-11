using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class qyronCombat : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float health;
    [SerializeField] private Color damageColor, originalColor;

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
    private int basicAttackAnimation = 1;

    [SerializeField] private float maxStamina;
    private float currentStamina;
    [SerializeField] private float staminaRecoveryRate;
    private float lastStaminaRecoveryTime;

    [SerializeField] private float invincibilityTime;
    private bool isInvincible = false;

    private Ray CombatRaycast;
    private RaycastHit CombatRaycastHit;

    [SerializeField] Vector3 CombatRaycastSize;


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
    }

    void Update()
    {
        if(Time.timeScale == 0)
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

            if (Input.GetButtonDown("Fire1") && !isAttacking && currentStamina >= 3) StartCoroutine(BasicAttack());

            if(Time.time - timeLastHit >= 2)
            {
                combo.Clear();
            }

            if(combo.Count == 3)
            {
                if(combo.Skip(combo.Count - 3).SequenceEqual(new List<string>{ "basicAttack", "basicAttack", "basicAttack"}))
                {
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
                qyronRB.AddForce(new Vector3(knockBackForce.x * -transform.localScale.x, knockBackForce.y, 0), ForceMode.Impulse);
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

    IEnumerator BasicAttack()
    {


        isAttacking = true;
        currentStamina -= 3f;
        timeLastHit = Time.time;

        if (basicAttackAnimation == 1)
        {
            basicAttackAnimation = 0;
            qyronAnimator.SetTrigger("Attack 1");
        }

        else if (basicAttackAnimation == 0)
        {
            basicAttackAnimation = 1;
            qyronAnimator.SetTrigger("Attack 2");
        }

        combo.Add("basicAttack");


        Vector3 attackDirection = new Vector3(transform.localScale.x, 0, 0);

        CombatRaycast = new Ray(transform.position, attackDirection);
        Physics.BoxCast(transform.position, CombatRaycastSize, attackDirection, out CombatRaycastHit, transform.rotation, 1);


        if (CombatRaycastHit.collider != null)
        {
            qyronSFX.PlayAttackSFX(1);

            if(CombatRaycastHit.collider.CompareTag("Dummy"))
            {
                CombatRaycastHit.collider.GetComponent<dummy>().TakeDamage(attackDamage);
            }

            else if(CombatRaycastHit.collider.CompareTag("Enemy"))
            {
                StartCoroutine(CombatRaycastHit.collider.GetComponent<enemyCombat>().TakeDamage(attackDamage, true, new Vector3(0,2,0), 0.5f));
            }

            StartCoroutine(ScreenShake(1f, 0.5f, 0.1f));
        }

        else

        {
            qyronSFX.PlayMissSFX(1);
        }

        yield return new WaitForSeconds(.2f);

        //qyronMovement.canMove = true;
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
}
