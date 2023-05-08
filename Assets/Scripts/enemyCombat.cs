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

    void Start()
    {
        qyronSR = GetComponent<SpriteRenderer>();
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        StartCoroutine(FlashRed());
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
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<qyronCombat>().TakeDamage(damage, true, 5);
        }
    }
}
