using UnityEngine;
using TMPro;
using System;
using System.Text.RegularExpressions;
using Random = UnityEngine.Random;

public class IPNetworkManager : MonoBehaviour
{
    [Header("3D Game Elements")]
    public GameObject player;
    public GameObject[] networkNodes;
    public GameObject lockedDoorPrefab;
    public Material correctMaterial;
    public Material wrongMaterial;
    
    [Header("UI Elements")]
    public TMP_Text ipDisplayText;
    public TMP_Text questionText;
    public TMP_Text feedbackText;
    public TMP_Text scoreText;
    public GameObject quizPanel;
    
    private IPGameData currentIP;
    private NetworkNode currentActiveNode;
    private int score = 0;
    private bool gameActive = true;
    
    private void Start()
    {
        InitializeNetwork();
        quizPanel.SetActive(false);
    }
    
    private void InitializeNetwork()
    {
        // Create network nodes with random IP challenges
        foreach (GameObject node in networkNodes)
        {
            NetworkNode nodeScript = node.GetComponent<NetworkNode>();
            if (nodeScript != null)
            {
                nodeScript.SetupNode(this, new IPGameData());
            }
        }
        
        // Place locked doors between nodes
        SetupNetworkDoors();
    }
    
    private void SetupNetworkDoors()
    {
        // This would be more sophisticated in a real game
        // For demo, we'll just place one door
        GameObject door = Instantiate(lockedDoorPrefab, 
            new Vector3(0, 0.5f, 5), Quaternion.identity);
        door.GetComponent<NetworkDoor>().Setup(this);
    }
    
    public void ActivateNodeChallenge(NetworkNode node)
    {
        if (!gameActive) return;
        
        currentActiveNode = node;
        currentIP = node.NodeIP;
        ShowQuestion();
    }
    
    private void ShowQuestion()
    {
        gameActive = false;
        quizPanel.SetActive(true);
        ipDisplayText.text = currentIP.IP;
        
        // Generate random question about this IP
        int questionType = Random.Range(0, 3);
        
        switch (questionType)
        {
            case 0:
                questionText.text = $"What class is this IP?\n{currentIP.IP}";
                break;
            case 1:
                questionText.text = $"Is this IP private?\n{currentIP.IP}";
                break;
            case 2:
                questionText.text = $"Convert this octet to binary:\n{currentIP.IP.Split('.')[Random.Range(0, 4)]}";
                break;
        }
    }
    
    public void SubmitAnswer(string answer)
    {
        bool correct = false;
        
        if (questionText.text.Contains("class"))
        {
            correct = answer == currentIP.GetClass();
        }
        else if (questionText.text.Contains("private"))
        {
            correct = answer == (currentIP.IsPrivate() ? "Yes" : "No");
        }
        else if (questionText.text.Contains("binary"))
        {
            string octet = questionText.text.Split('\n')[1];
            correct = answer == Convert.ToString(int.Parse(octet), 2).PadLeft(8, '0');
        }
        
        if (correct)
        {
            feedbackText.text = "Correct! Access granted.";
            feedbackText.color = Color.green;
            score++;
            scoreText.text = $"Network Score: {score}";
            
            // Reward player
            currentActiveNode.Unlock();
        }
        else
        {
            feedbackText.text = "Incorrect! Try again.";
            feedbackText.color = Color.red;
        }
        
        Invoke("HideQuizPanel", 2f);
    }
    
    private void HideQuizPanel()
    {
        quizPanel.SetActive(false);
        gameActive = true;
    }
}