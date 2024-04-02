using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

    [Header("Components")]
    protected Rigidbody rb;
    protected Animator anim;
    protected SpriteRenderer sr;
    protected BoxCollider bc;

    protected virtual void GetComponentsOnCharacter()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider>();
    }

    [Header("Character Stats")]
    [SerializeField] protected float maxHealth;
    public float MaxHealth { get { return maxHealth; } }
    protected float currentHealth;
    public float CurrentHealth {
        get { return currentHealth; } 
        set { if (value > maxHealth) currentHealth = maxHealth;
        else if (value < 0) currentHealth = 0; 
        else currentHealth = value; }
        }

    [SerializeField] protected bool invincible;
    [SerializeField] protected bool hitKillProtected;
    private bool wasProtected = true;
        
    [SerializeField] protected float baseDamage;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float jumpForce;
    protected int facingDirection = 1;

    #region Movement

    protected void LimitZ()
    {
        if (transform.position.z >= 2.5f) transform.position = new Vector3(transform.position.x, transform.position.y, 2.5f);
        if (transform.position.z <= -2.5f) transform.position = new Vector3(transform.position.x, transform.position.y, -2.5f);
    }

    protected void FlipSprite()
    {
        if (rb.velocity.x > 0.1f) 
        {
            facingDirection = 1;
            sr.flipX = false;
        }
        else if (rb.velocity.x < -0.1f) 
        {
            facingDirection = -1;
            sr.flipX = true;
        }
    }

    #endregion

    public void TakeDamage(float damage, bool flash, int flashTimes)
    {
        if (flash)
        {
            StartCoroutine(FlashRed(flashTimes));
        }

        if (hitKillProtected && currentHealth - damage <= 0 && !wasProtected)
        {
            wasProtected = true;
            currentHealth = 1;
        }

        else
        {
            currentHealth -= damage;
        }
    }

    protected void Die()
    {
        // Play death animation
        // Disable movement
        // Disable combat
        // Disable collision
        // Disable this script
        StopAllCoroutines(); // ver se acaba o bug dos pombo (nao acabou...)
        Destroy(gameObject);
    }

    protected IEnumerator FlashRed(int timesToFlash)
    {
        if (GetComponent<SpriteRenderer>() != null)
        {
            for (int i = 0; i < timesToFlash; i++)
            {
                sr.color = new Color(0.5f,0.2f,0.2f,1);
                yield return new WaitForSeconds(0.1f);
                sr.color = new Color(1,1,1,1);
                yield return new WaitForSeconds(0.1f);
            }
        }

        // if isnt a sprite, change material color

        else if (GetComponent<Renderer>() != null)
        {
            for (int i = 0; i < timesToFlash; i++)
            {
                GetComponent<Renderer>().material.color = new Color(0.5f,0.2f,0.2f,1);
                yield return new WaitForSeconds(0.1f);
                GetComponent<Renderer>().material.color = new Color(1,1,1,1);
                yield return new WaitForSeconds(0.1f);
            }
        }

        else
        {
            Debug.LogError("No SpriteRenderer or Renderer found on this object");
        }
    }
}