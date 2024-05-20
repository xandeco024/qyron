using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Mathematics;
public class LoadSceneManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] GameObject loadingScreen;
    [SerializeField] Image miniQyronImage;
    [SerializeField] Image loadingBar;
    [SerializeField] TextMeshProUGUI loadingText;
    [SerializeField] TextMeshProUGUI hintText;
    float screenWidth;

    List<string> hintList = new List<string> {
        "O RESPEITO FAZ ALGUNS POMBOS TEREM MEDO DE VOCÊ, EVITANDO O COMBATE E DANDO MAIS XP", 
        "NÃO SE ESQUEÇA DE FAZER A MISSÃO DO PÃO",
        "BANANA É MUITO BOM!",
        "USE LEITE VEGETAL!",
        "SEJA VEGANO!",
        "POMBOS SÃO AMIGOS!",
        "GARK FEZ TODAS AS TATUAGENS DELE NA PRISÃO!",
        "MEOWCELO É O PERSONAL TRAINER DE QYRON!",
        "O NOME DO QYRON É QYRON!",
        "QYANA É PRIMA DO QYRON!",
        "O SABOR DE WHEY PODE SER MELHORADO COM BANANA!",
        "O SABOR DE WHEY PREFERIDO DO QYRON É BANANA!",
        "TENTE IR AO LOJISTA AS 4:20",
        "FAÇA PROERD. NÃO USE DROGAS",
        };
    void Start()
    {
        screenWidth = Screen.width;
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void LoadScene(int sceneIndex)
    {
        Debug.Log("STARTOU!!");
        loadingScreen.SetActive(true); 
        hintText.text = hintList[UnityEngine.Random.Range(0, hintList.Count)];
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }

    IEnumerator LoadSceneAsync(int  sceneIndex)
    {

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            float miniQyronX = (screenWidth - 100) * progress;
            if (miniQyronX < 50) miniQyronX = 50;
            if (miniQyronX > screenWidth - 50) miniQyronX = screenWidth - 50;

            miniQyronImage.rectTransform.anchoredPosition = new Vector2(miniQyronX, 125);

            loadingBar.fillAmount = progress;
            loadingText.text = "Carregando " + (progress * 100).ToString("0") + "% ...";

            Debug.Log(progress);

            yield return null;
        }
    }
}
