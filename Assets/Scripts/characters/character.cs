using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

    [Header("Components")]
    private Rigidbody rb;
    private Animator anim;
    private SpriteRenderer sr;
    private BoxCollider bc;

    void GetComponentsOnCharacter()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider>();
    }

    [Header("Character Stats")]
    [SerializeField] private float maxHealth;
    private float currentHealth;
    public float CurrentHealth {
        get { return currentHealth; } 
        set { if (value > maxHealth) currentHealth = maxHealth;
        else if (value < 0) currentHealth = 0; 
        else currentHealth = value; }
        }

    [SerializeField] private bool invincible;
        
    [SerializeField] private float baseDamage;
    [SerializeField] private float moveSpeed;

    void Start()
    {
        GetComponentsOnCharacter();
        
        currentHealth = maxHealth;

    }

    void Update()
    {
        if(!invincible && currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(float damage, bool flash, int flashTimes)
    {
        if (flash)
        {
            StartCoroutine(FlashRed(flashTimes));
        }

        currentHealth -= damage;
    }

    public void Die()
    {
        // Play death animation
        // Disable movement
        // Disable combat
        // Disable collision
        // Disable this script
        Destroy(gameObject);
    }

    IEnumerator FlashRed(int timesToFlash)
    {
        for (int i = 0; i < timesToFlash; i++)
        {
            sr.color = new Color(0.5f,0.2f,0.2f,1);
            yield return new WaitForSeconds(0.1f);
            sr.color = new Color(1,1,1,1);
            yield return new WaitForSeconds(0.1f);
        }
    }
}