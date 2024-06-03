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
    [SerializeField] TextMeshProUGUI[] loadingText;
    [SerializeField] TextMeshProUGUI[] hintText;
    float screenWidth;

    /*List<string> hintList = new List<string> {
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
        "QYRON JA FOI COMPETIDOR OLÍMPICO",
        "UTILIZE BEM SEUS COMBOS",
        "COMBINE SEUS GOLPES COM SEUS STATUS",
        "TREMBOLONA PODE SER BOM AGORA, POREM DEPOIS...",
        "COMBINE SEUS STATUS COM SUA FORMA DE LUTAR",
        "OS SHAKES IRÃO FACILITAR MUITO SUA JORNADA",
        "APRENDA NOVAS COMBINAÇÕES DE COMBOS",
        "A FRUTA PREFERIDA DE QYANA É MAÇA",
        "MEOWCELO É UM GRANDE FÃ DE GATONAIS",
        "GARK É MUITO RESPEITADO NAS RUAS, NÃO BRINQUE COM ELE",
        "GARK NÃO É MUITO PACIENTE COM SEUS INIMIGOS",
        "SHAKES SÃO DELICIOSOS",
        "DERROTE INIMIGOS E DESTRUA OBJETOS DO CENARIO PARA COLETAR ITENS",
        "COLETE O MAXIMO TE ITENS QUE PUDER",
        "PEGUE O MAXIMO DE XP QUE PUDER",
        "POMBOS E RATOS SÃO ALIADOS DE LONGA DATA",
        "NÃO CONFIE EM QUASE NINGUEM NA RUA",
        "APRENDA A CONFIAR NAS PESSOAS CERTAS",
        "APRENDA ARTES MARCIAIS",
        "COMBINE SUAS ARTE MARCIAIS COM SEUS STATUS",
        "SE O SEU AMIGO CAIR, AJUDE ELE A LEVANTAR",
        "OS PETS SÃO SEUS MELHORES COMPANHEIROS",
        "PETS AJUDAM VOCÊ, APROVEITE",
        "O MERCADOR É UM HOMEM MISTERIOSO",
        "DESENHAR É DRIBLAR A REALIDADE VIER NO MUNDO CHEIO DE ESPERANÇA",
        "CHAME SEUS AMIGOS E JOGUE EM GRUPO",
        "VOCÊ PODE TER UMA MELHOR EXPERIENCIA JOGANDO EM AMIGOS",
        "VOCÊ TOMOU MEU WHEYYYYY!!!",
        "A FRUTE PREFERIDA DE GARK É LARANJA",
        "MEOWCELO É UM GRANDE PERSONAL TRAINER",
        "POMBAS NÃO SÃO MUITO AMIGAVEIS",
        "NÃO ESQUEÇA DO SEU DASH",
        };*/

    List<string> hintList = new List<string> {
        "o respeito faz alguns pombos terem medo de voce, evitando o combate e dando mais xp",
        "nao se esqueca de fazer a missao do pao",
        "banana e muito bom!",
        "use leite vegetal!",
        "seja vegano!",
        "pombos sao amigos!",
        "gark fez todas as tatuagens dele na prisao!",
        "meowcelo e o personal trainer do qyron!",
        "o nome do qyron e qyron!",
        "qyana e prima do qyron!",
        "o sabor do whey pode ser melhorado com banana!",
        "o sabor de whey preferido do qyron e banana!",
        "tente ir ao lojista as 4:20",
        "faca proerd. nao use drogas",
        "qyron ja foi competidor olimpico",
        "utilize bem seus combos",
        "combine seus golpes com seus status",
        "trembolona pode ser bom agora, porem depois...",
        "combine seus status com sua forma de lutar",
        "os shakes irao facilitar muito sua jornada",
        "aprenda novas combinacoes de combos",
        "a fruta preferida de qyana e maca",
        "meowcelo e um grande fa de gatonais",
        "gark e muito respeitado nas ruas, nao brinque com ele",
        "gark nao e muito paciente com seus inimigos",
        "shakes sao deliciosos",
        "derrote inimigos e destrua objetos do cenario para coletar itens",
        "colete o maximo de itens que puder",
        "pegue o maximo de xp que puder",
        "pombos e ratos sao aliados de longa data",
        "nao confie em quase ninguem na rua",
        "aprenda a confiar nas pessoas certas",
        "aprenda artes marciais",
        "combine suas artes marciais com seus status",
        "se o seu amigo cair, ajude-o a levantar",
        "os pets sao seus melhores companheiros",
        "pets ajudam voce, aproveite",
        "o mercador e um homem misterioso",
        "desenhar e driblar a realidade viver no mundo cheio de esperanca",
        "chame seus amigos e jogue em grupo",
        "voce pode ter uma melhor experiencia jogando com amigos",
        "voce tomou meu whey!!!",
        "a fruta preferida de gark e laranja",
        "meowcelo e um grande personal trainer",
        "pombas nao sao muito amigaveis",
        "nao esqueca do seu dash",
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
        //Debug.Log("STARTOU!!");
        loadingScreen.SetActive(true); 
        for (int i = 0; i < hintText.Length; i++)
        {
            hintText[i].text = hintList[UnityEngine.Random.Range(0, hintList.Count)];
        }
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
            for (int i = 0; i < loadingText.Length; i++)
            {
                loadingText[i].text = "Carregando " + (progress * 100).ToString("F0") + "%";
            }

            //Debug.Log(progress);

            yield return null;
        }
    }
}
