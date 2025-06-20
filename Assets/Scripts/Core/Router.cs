using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class Router : MonoBehaviour
{
    public string routerID;
    public Dictionary<string, int> neighbors = new Dictionary<string, int>();
    public Dictionary<string, (string nextHop, int cost)> routingTable = new Dictionary<string, (string, int)>();

    public void UpdateTopology(string neighbor, int cost)
    {
        if (!neighbors.ContainsKey(neighbor))
        {
            neighbors.Add(neighbor, cost);
        }
        else
        {
            neighbors[neighbor] = cost;
        }
    }

    public void Dijkstra(Network network)
    {
        var distances = new Dictionary<string, int>();
        var previous = new Dictionary<string, string>();
        var unvisited = new HashSet<string>();

        foreach (var router in network.routers)
        {
            distances[router.Key] = router.Key == routerID ? 0 : int.MaxValue;
            previous[router.Key] = null;
            unvisited.Add(router.Key);
        }

        while (unvisited.Count > 0)

        {
            string current = null;
            foreach (var router in unvisited)
            {
                if (current == null || distances[router] < distances[current])
                {
                    current = router;
                }
            }

            if (current == null || distances[current] == int.MaxValue)
                break;

            unvisited.Remove(current);

            foreach (var neighbor in network.routers[current].neighbors)
            {
                int alt = distances[current] + neighbor.Value;
                if (alt < distances[neighbor.Key])
                {
                    distances[neighbor.Key] = alt;
                    previous[neighbor.Key] = current;
                }
            }
        }
        UpdateRoutingTable(distances, previous);
    }
    private void UpdateRoutingTable(Dictionary<string, int> distances, Dictionary<string, string> previous)
    {
        routingTable.Clear();
        foreach (var dest in distances.Keys)
        {
            if (dest == routerID) continue;

            string nextHop = GetNextHop(dest, previous);
            routingTable[dest] = (nextHop, distances[dest]);
        }
    }
    private string GetNextHop(string destination, Dictionary<string, string> previous)
    {
        if (previous[destination] == null)
            return null;

        string current = destination;
        while (previous[current] != null && previous[current] != routerID)
        {
            current = previous[current];
        }
        return current;
    }
}