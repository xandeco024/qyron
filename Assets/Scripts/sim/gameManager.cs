using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{

    [Header("Qyron")]
    private GameObject qyron;
    private qyronMovement qyronMovement;
    private qyronCombat qyronCombat;
    //private Rigidbody2D qyronRB;
    //private SpriteRenderer qyronSR;
    //private BoxCollider2D qyronCol;

    //private qyronSFX qyronSFX;
    //private Animator qyronAnimator;

    [Header("Manager")]
    private bool isDead = false;

    void Start()
    {
        qyron = GameObject.FindWithTag("Player");
        qyronMovement = qyron.GetComponent<qyronMovement>();
        qyronCombat = qyron.GetComponent<qyronCombat>();
    }


    void Update()
    {
        if (qyronCombat.GetCurrentHealth() <= 0)
        {
            isDead = true;
        }
    }

    public bool GetIsDead()
    {
        return isDead;
    }
}
