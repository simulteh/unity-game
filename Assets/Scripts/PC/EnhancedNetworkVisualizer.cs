// EnhancedNetworkVisualizer.cs
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class EnhancedNetworkVisualizer : NetworkBehaviour
{
    [SerializeField] private Material connectionMaterial;
    [SerializeField] private float lineWidth = 0.1f;
    
    private Dictionary<string, LineRenderer> connections = new();

    public void UpdateConnection(string fromIP, string toIP, Color color)
    {
        string connectionKey = $"{fromIP}-{toIP}";
        
        if (!connections.TryGetValue(connectionKey, out var lineRenderer))
        {
            var newLine = new GameObject($"Connection_{connectionKey}").AddComponent<LineRenderer>();
            newLine.material = connectionMaterial;
            newLine.startWidth = lineWidth;
            newLine.endWidth = lineWidth;
            connections[connectionKey] = newLine;
        }
        
        var fromNode = FindNode(fromIP);
        var toNode = FindNode(toIP);
        
        if (fromNode != null && toNode != null)
        {
            connections[connectionKey].SetPositions(new[]
            {
                fromNode.transform.position,
                toNode.transform.position
            });
            connections[connectionKey].startColor = color;
            connections[connectionKey].endColor = color;
        }
    }

    private NetworkNode FindNode(string ip)
    {
        foreach (var node in FindObjectsOfType<NetworkNode>())
        {
            if (node.IPAddress == ip) return node;
        }
        return null;
    }
}