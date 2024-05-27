using UnityEngine;

public class SunController : MonoBehaviour
{
    private GameManager gameManager; // Referência ao GameManager
    private Light sunLight; // Referência à luz direcional

    // Configurações para cor do sol
    public Color morningColor = new Color(1f, 0.8f, 0.6f); // Cor da manhã
    public Color noonColor = Color.white; // Cor do meio-dia
    public Color eveningColor = new Color(1f, 0.4f, 0.2f); // Cor da tarde

    [SerializeField] Gradient sunColorGradient; // Gradiente de cor do sol

    // Ângulo de rotação máximo e mínimo
    private float sunZAngle = 0; // Ângulo de rotação do sol

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>(); // Encontra o GameManager
        sunLight = GetComponent<Light>(); // Encontra a luz direcional
    }

    void Update()
    {
        UpdateSunRotationAndColor();
    }

    void UpdateSunRotationAndColor()
    {
        float currentTime = gameManager.Hours;

        float currentDayProgress = currentTime / 24f;
        
        // Calcula a rotação do sol com base no horário
        sunZAngle = currentDayProgress * 360f;
        transform.rotation = Quaternion.Euler(-230, sunZAngle/2, sunZAngle);
        //if (transform.rotation.z > 360) transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0);
        //if (transform.rotation.y > 360) transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);
        //if (transform.rotation.x > 360) transform.rotation = Quaternion.Euler(0, transform.rotation.y, transform.rotation.z);
        sunLight.intensity = Mathf.Clamp01(1 - Mathf.Abs(currentDayProgress - 0.5f) * 2);

        // Calcula a cor do sol com base no horário
        sunLight.color = sunColorGradient.Evaluate(currentTime / 24f);
    }
}