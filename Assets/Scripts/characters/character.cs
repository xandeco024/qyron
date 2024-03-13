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
    private int maxHealth;
    private int currentHealth;
    private int baseDamage;
    private float speed;

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