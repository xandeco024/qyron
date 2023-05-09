using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class hud : MonoBehaviour
{
    private qyronCombat qyronCombat;

    [SerializeField] private Image healthBar;
    [SerializeField] private Image staminaBar;

    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI staminaText;

    private float health, maxHealth;
    private float stamina, maxStamina;

    void Start()
    {
        qyronCombat = GameObject.FindWithTag("Player").GetComponent<qyronCombat>();
    }

    void Update()
    {
        health = qyronCombat.GetCurrentHealth();
        maxHealth = qyronCombat.GetMaxHealth();

        HealthHandler();
    }

    private void HealthHandler()
    {
        healthText.text = health.ToString();
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, (health / maxHealth), 3 * Time.deltaTime);

        Color healthBarColor = Color.Lerp(Color.red, Color.green, (health / maxHealth));
        healthBar.color = healthBarColor;
    }

    private void StaminaHandler()
    {
        //staminaText.text = stami
    }
}
