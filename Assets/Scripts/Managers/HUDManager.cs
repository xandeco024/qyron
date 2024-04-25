using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private bool debug;

    [Header("HUD")]
    [SerializeField] private GameObject[] hudObject = new GameObject[4];
    [SerializeField] private Image[] hudImage = new Image[4];
    [SerializeField] private Image[] damageBar = new Image[4];
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

    void Start()
    {
        PlayableCharacter[] foundPlayers;foundPlayers = FindObjectsOfType<PlayableCharacter>();

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
            if (debug) Debug.Log("HUD INDEX: " + hudIndex + "ACTIVE PLAYER: " + players[hudIndex].name);
        }

        else
        {
            hudObject[hudIndex].SetActive(false);

            if (debug) Debug.Log("HUD INDEX: " + hudIndex + "NO PLAYER FOUND! DEACTIVATING HUD!");
        }
    }

    private void HudHandler(int hudIndex)
    {
        if (players[hudIndex] != null)
        {
            damageBar[hudIndex].fillAmount = Mathf.Lerp(damageBar[hudIndex].fillAmount, (players[hudIndex].CurrentHealth / players[hudIndex].MaxHealth), 2.5f * Time.deltaTime);
            healthBar[hudIndex].fillAmount = players[hudIndex].CurrentHealth / players[hudIndex].MaxHealth;
            healthText[hudIndex].text = players[hudIndex].CurrentHealth.ToString();

            //xpBar[hudIndex].fillAmount = Mathf.Lerp(xpBar[hudIndex].fillAmount, (players[hudIndex].ExP / players[hudIndex].NextLevelExP), 3 * Time.deltaTime);
            xpBar[hudIndex].fillAmount = Mathf.Lerp(xpBar[hudIndex].fillAmount, ((float)players[hudIndex].ExP / (float)players[hudIndex].NextLevelExP), 3 * Time.deltaTime);
            levelText[hudIndex].text = players[hudIndex].Level.ToString();

            coinsText[hudIndex].text = players[hudIndex].Coins.ToString();

            //logica pros hits
            if (players[hudIndex].Combo.Count > 0)
            {
                lastHits[hudIndex].gameObject.SetActive(true);

                if (players[hudIndex].Combo[players[hudIndex].Combo.Count -1] == "L")
                {
                    lastHits[hudIndex].GetComponent<Animator>().SetTrigger("L");
                }
                else if (players[hudIndex].Combo[players[hudIndex].Combo.Count -1] == "H")
                {
                    lastHits[hudIndex].GetComponent<Animator>().SetTrigger("H");
                }
                else if (players[hudIndex].Combo[players[hudIndex].Combo.Count -1] == "G")
                {
                    lastHits[hudIndex].GetComponent<Animator>().SetTrigger("G");
                }
            }

            else
            {
                lastHits[hudIndex].gameObject.SetActive(false);
            }

            if (players[hudIndex].Combo.Count > 1)
            {
                hits0[hudIndex].gameObject.SetActive(true);

                if (players[hudIndex].Combo[players[hudIndex].Combo.Count -2] == "L")
                {
                    hits0[hudIndex].GetComponent<Animator>().SetTrigger("L");
                }
                else if (players[hudIndex].Combo[players[hudIndex].Combo.Count -2] == "H")
                {
                    hits0[hudIndex].GetComponent<Animator>().SetTrigger("H");
                }
                else if (players[hudIndex].Combo[players[hudIndex].Combo.Count -2] == "G")
                {
                    hits0[hudIndex].GetComponent<Animator>().SetTrigger("G");
                }
            }

            else
            {
                hits0[hudIndex].gameObject.SetActive(false);
            }

            if (players[hudIndex].Combo.Count > 2)
            {
                hits1[hudIndex].gameObject.SetActive(true);

                if (players[hudIndex].Combo[players[hudIndex].Combo.Count -3] == "L")
                {
                    hits1[hudIndex].GetComponent<Animator>().SetTrigger("L");
                }
                else if (players[hudIndex].Combo[players[hudIndex].Combo.Count -3] == "H")
                {
                    hits1[hudIndex].GetComponent<Animator>().SetTrigger("H");
                }
                else if (players[hudIndex].Combo[players[hudIndex].Combo.Count -3] == "G")
                {
                    hits1[hudIndex].GetComponent<Animator>().SetTrigger("G");
                }
            }

            else
            {
                hits1[hudIndex].gameObject.SetActive(false);
            }
        }
    }
}
