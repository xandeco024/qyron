using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dummy : MonoBehaviour
{
    private damageText damageText;

    private SpriteRenderer dummySR;

    [SerializeField] private float damageTaken;
    private float lastDamageTaken;
    private float timeLastHit;

    [SerializeField] private Color damageColor, originalColor;

    private void Start()
    {
        damageText = GetComponentInChildren<damageText>();
        dummySR = GetComponent<SpriteRenderer>();
        damageTaken = 0;
        timeLastHit = Time.time;
    }

    private void Update()
    {
        if (lastDamageTaken != damageTaken && damageTaken != 0) damageText.SetText(damageTaken.ToString());
        if (Time.time - timeLastHit >= 2.5) damageTaken = 0;
    }

    public void TakeDamage(float damage)
    {
        lastDamageTaken = damageTaken;
        damageTaken += damage;
        timeLastHit = Time.time;
        StartCoroutine(FlashRed());
    }

    IEnumerator FlashRed()
    {
        for (int i = 0; i < 3; i++)
        {
            dummySR.color = damageColor;
            yield return new WaitForSeconds(0.1f);
            dummySR.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
