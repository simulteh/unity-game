// NetworkQuestSystem.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[Serializable]
public struct NetworkQuest
{
    public string questId;
    public string title;
    public string description;
    public QuestType type;
    public string targetIP;
    public string requiredData;
    public int reward;
}

public enum QuestType
{
    PING,
    DATA_TRANSFER,
    TRACEROUTE,
    SECURITY
}

public class NetworkQuestSystem : NetworkBehaviour
{
    [SerializeField] private List<NetworkQuest> availableQuests;
    [SerializeField] private TMPro.TextMeshProUGUI questUI;
    
    private NetworkVariable<int> currentQuestIndex = new(-1);

    void Start()
    {
        if (IsServer)
        {
            StartNextQuest();
        }
    }

    public void StartNextQuest()
    {
        currentQuestIndex.Value = (currentQuestIndex.Value + 1) % availableQuests.Count;
        UpdateQuestUI();
    }

    public void CheckPacketForQuestCompletion(AdvancedNetworkPacket.PacketData packet)
    {
        if (currentQuestIndex.Value == -1) return;
        
        var currentQuest = availableQuests[currentQuestIndex.Value];
        
        switch (currentQuest.type)
        {
            case QuestType.PING:
                if (packet.DestinationIP == currentQuest.targetIP && 
                    packet.Payload == "PING_REPLY")
                {
                    CompleteQuest();
                }
                break;
                
            case QuestType.DATA_TRANSFER:
                if (packet.DestinationIP == currentQuest.targetIP && 
                    packet.Payload == currentQuest.requiredData)
                {
                    CompleteQuest();
                }
                break;
        }
    }

    private void CompleteQuest()
    {
        Debug.Log($"Quest {availableQuests[currentQuestIndex.Value].title} completed!");
        StartNextQuest();
    }

    private void UpdateQuestUI()
    {
        if (currentQuestIndex.Value == -1) return;
        
        var quest = availableQuests[currentQuestIndex.Value];
        questUI.text = $"<b>{quest.title}</b>\n{quest.description}";
    }
}