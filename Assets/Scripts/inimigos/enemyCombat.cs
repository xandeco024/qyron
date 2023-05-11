using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class enemyCombat : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float maxHealth;

    private float health;
    private SpriteRenderer qyronSR;

    [SerializeField] private Color damageColor, originalColor;

    private bool isGrounded;
    private LayerMask groundLayer;

    private bool _isTakingDamage;
    public bool isTakingDamage { get { return _isTakingDamage;} }

    void Start()
    {
        qyronSR = GetComponent<SpriteRenderer>();
        health = maxHealth;
        groundLayer = LayerMask.GetMask("Ground");
    }

    void Update()
    {
        if (health <= 0) Destroy(gameObject);


        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.3f, groundLayer).collider != null;
        Debug.DrawRay(transform.position, Vector2.down * 1.3f, Color.green);

        if (isGrounded)
        {
            GetComponent<Animator>().SetBool("Grounded", true);
        }
    }

    public IEnumerator TakeDamage(float damage,bool takeKnockBack, Vector2 knockBackForce, float knockBackTime)
    {
        GetComponent<Animator>().SetTrigger("Damage");

        health -= damage;
        StartCoroutine(FlashRed());

        if (takeKnockBack)
        {
            GetComponent<Rigidbody>().AddForce(new Vector2(knockBackForce.x * -transform.localScale.x, knockBackForce.y), ForceMode.Impulse);
        }

        yield return null;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
