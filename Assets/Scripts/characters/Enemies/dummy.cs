using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dummy : Character
{
    private damageText damageText;
    [SerializeField] private float damageTaken;
    private float lastDamageTaken;
    private float lastHealth;
    private float timeLastHit;

    private void Start()
    {
        GetComponentsOnCharacter();

        damageText = GetComponentInChildren<damageText>();
        damageTaken = 0;
        timeLastHit = Time.time;
    }

    private void Update()
    {
        

        if (lastDamageTaken != damageTaken && damageTaken != 0) damageText.SetText(damageTaken.ToString());
        if (Time.time - timeLastHit >= 2.5) damageTaken = 0;
    }

    void LateUpdate()
    {
        lastDamageTaken = currentHealth - lastHealth;
        lastHealth = currentHealth;
    }
}
