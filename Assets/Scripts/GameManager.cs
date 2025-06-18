using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("IP Settings")]
    public IPAddress playerIP = new IPAddress();
    public IPAddress serverIP = new IPAddress("192.168.1.100");
    
    [Header("UI References")]
    public InputField ipInputField;
    public Text ipStatusText;
    public Text localIPText;
    public GameObject ipSpherePrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateLocalIPDisplay();
        CreateIPVisualization();
    }

    public void UpdateLocalIPDisplay()
    {
        localIPText.text = $"Local IP: {IPAddress.GetLocalIP()}";
    }

    public void OnIPSubmitted()
    {
        playerIP = new IPAddress(ipInputField.text);
        
        ipStatusText.text = playerIP.IsValid ? 
            $"Valid IP: {playerIP.Address}" : 
            $"Invalid IP! Default: {playerIP.Address}";
        
        ipStatusText.color = playerIP.IsValid ? Color.green : Color.red;
        
        CreateIPVisualization();
    }

    private void CreateIPVisualization()
    {
        // Удаляем старые сферы
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Создаем визуализацию IP
        byte[] octets = playerIP.GetOctets();
        for (int i = 0; i < octets.Length; i++)
        {
            GameObject sphere = Instantiate(ipSpherePrefab, transform);
            sphere.transform.position = new Vector3(i * 2, 0, 0);
            
            // Масштабируем сферу по значению октета
            float scale = octets[i] / 255f * 2f + 0.5f;
            sphere.transform.localScale = new Vector3(scale, scale, scale);
            
            // Цвет в зависимости от октета
            Renderer rend = sphere.GetComponent<Renderer>();
            rend.material.color = new Color(
                octets[0] / 255f,
                octets[1] / 255f,
                octets[2] / 255f
            );
            
            // Подпись с числовым значением
            TextMesh text = sphere.GetComponentInChildren<TextMesh>();
            text.text = octets[i].ToString();
        }
    }
}