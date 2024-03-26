using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private GameObject[] hudObject = new GameObject[4];
    [SerializeField] private Image[] hudImage = new Image[4];
    [SerializeField] private Image[] healthBar = new Image[4];
    [SerializeField] private TextMeshProUGUI[] healthText = new TextMeshProUGUI[4];
    [SerializeField] private Image[] xpBar = new Image[4];
    [SerializeField] private TextMeshProUGUI[] levelText = new TextMeshProUGUI[4];
    [SerializeField] private TextMeshProUGUI[] coinsText = new TextMeshProUGUI[4];
    [SerializeField] private Image[] hits1 = new Image[4];
    [SerializeField] private Image[] hits2 = new Image[4];
    [SerializeField] private Image[] lastHits = new Image[4];


    [Header("Players Stats")]
    private PlayableCharacter[] players = new PlayableCharacter[4];

    void Start()
    {

    }

    void Update()
    {

    }

    private void SetupHUD(int hudIndex)
    {
        if (players[hudIndex] != null) 
        {
            hudObject[hudIndex].SetActive(true);
            //TROCA A IMAGEM DA HUD, PRO PERSONAGEM QUE ESTIVER JOGANDO
            
        }
    }

    private void HudHandler(int hudIndex)
    {
        if (players[hudIndex] != null)
        {
            healthBar[hudIndex].fillAmount = Mathf.Lerp(healthBar[hudIndex].fillAmount, (players[hudIndex].CurrentHealth / players[hudIndex].MaxHealth), 3 * Time.deltaTime);
            healthText[hudIndex].text = players[hudIndex].CurrentHealth.ToString();

            xpBar[hudIndex].fillAmount = Mathf.Lerp(xpBar[hudIndex].fillAmount, (players[hudIndex].ExP / players[hudIndex].NextLevelExP), 3 * Time.deltaTime);


        }
    }
}
