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
    [SerializeField] private Image[] hits0 = new Image[4];
    [SerializeField] private Image[] hits1 = new Image[4];
    [SerializeField] private Image[] lastHits = new Image[4];


    [Header("Players Stats")]
    private PlayableCharacter[] players = new PlayableCharacter[4];
    private PlayableCharacter[] foundPlayers;

    void Start()
    {
        foundPlayers = FindObjectsOfType<PlayableCharacter>();

        for (int i = 0; i < players.Length; i++)
        {
            if (i < foundPlayers.Length)
            {
                players[i] = foundPlayers[i];
            }
            else
            {
                players[i] = null;
            }
        }

        SetupHUD(0);
        SetupHUD(1);
        SetupHUD(2);
        SetupHUD(3);
    }

    void Update()
    {
        for (int i = 0; i < players.Length; i++)
        {
            HudHandler(i);
        }
    }

    private void SetupHUD(int hudIndex)
    {
        if (players[hudIndex] != null) 
        {
            hudObject[hudIndex].SetActive(true);
            //TROCA A IMAGEM DA HUD, PRO PERSONAGEM QUE ESTIVER JOGANDO
            Debug.Log("HUD INDEX: " + hudIndex + "ACTIVE PLAYER: " + players[hudIndex].name);
        }

        else
        {
            hudObject[hudIndex].SetActive(false);

            Debug.Log("HUD INDEX: " + hudIndex + "NO PLAYER FOUND! DEACTIVATING HUD!");
        }
    }

    private void HudHandler(int hudIndex)
    {
        if (players[hudIndex] != null)
        {
            healthBar[hudIndex].fillAmount = Mathf.Lerp(healthBar[hudIndex].fillAmount, (players[hudIndex].CurrentHealth / players[hudIndex].MaxHealth), 3 * Time.deltaTime);
            healthText[hudIndex].text = players[hudIndex].CurrentHealth.ToString();

            xpBar[hudIndex].fillAmount = Mathf.Lerp(xpBar[hudIndex].fillAmount, (players[hudIndex].ExP / players[hudIndex].NextLevelExP), 3 * Time.deltaTime);
            levelText[hudIndex].text = players[hudIndex].Level.ToString();

            coinsText[hudIndex].text = players[hudIndex].Coins.ToString();

            //logica pros hits
        }
    }
}
