using UnityEngine;
using System.Collections.Generic;

public class OSPF
{
    private Router router;
    private Dictionary<IPAddress, int> linkCosts = new Dictionary<IPAddress, int>();

    public OSPF(Router router)
    {
        this.router = router;
    }

    public void AddLink(IPAddress neighbor, int cost)
    {
        linkCosts[neighbor] = cost;
    }

    public void CalculateRoutes()
    {
        Debug.Log($"OSPF on {router.Name} calculating shortest paths using Dijkstra's algorithm");
        // Simplified route calculation
        foreach (var link in linkCosts)
        {
            router.AddRoute(link.Key, "255.255.255.255", link.Key);
        }
    }
}
