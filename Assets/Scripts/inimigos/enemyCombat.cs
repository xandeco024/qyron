using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class enemyCombat : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private Color damageColor, originalColor;
    public float enemyHealth;
    [SerializeField] float enemyMaxHealth;
    private bool _isTakingDamage = false;
    public bool isTakingDamage { get { return _isTakingDamage; } }
    private SpriteRenderer enemySR;
    private int direction = 1;

    void Start()
    {
        enemySR = GetComponent<SpriteRenderer>();
        enemyHealth = enemyMaxHealth;
    }

    void Update()
    {
        if (enemyHealth <= 0) Destroy(gameObject);
        if (!enemySR.flipX) direction = 1;
        else if (enemySR.flipX) direction = -1;
    }

    public IEnumerator TakeDamage(float damage,bool takeKnockBack, Vector3 knockBackForce, float knockBackTime)
    {
        _isTakingDamage = true;
        GetComponent<Animator>().SetBool("isTakingDamage", true);

        enemyHealth -= damage;
        StartCoroutine(FlashRed());

        if (takeKnockBack)
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(knockBackForce.x * -direction, knockBackForce.y, knockBackForce.z), ForceMode.Impulse);
        }

        if(enemyHealth > 0) yield return new WaitForSeconds(knockBackTime);
        _isTakingDamage = false;
        GetComponent<Animator>().SetBool("isTakingDamage", false);
    }

    IEnumerator FlashRed()
    {
        for (int i = 0; i < 3; i++)
        {
            enemySR.color = damageColor;
            yield return new WaitForSeconds(0.1f);
            enemySR.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }


}
