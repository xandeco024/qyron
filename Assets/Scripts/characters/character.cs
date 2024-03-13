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
    [SerializeField] private int maxHealth;
    private int currentHealth;
    public int CurrentHealth {
        get { return currentHealth; } 
        set { if (value > maxHealth) currentHealth = maxHealth;
        else if (value < 0) currentHealth = 0; 
        else currentHealth = value; }
        }
        
    [SerializeField] private int baseDamage;
    [SerializeField] private float moveSpeed;

    void Start()
    {
        GetComponentsOnCharacter();
        
        currentHealth = maxHealth;

    }

    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

}