using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network : MonoBehaviour
{
    public Dictionary<string, Router> routers = new Dictionary<string, Router>();

    public void AddRouter(Router router)
    {
        routers[router.routerID] = router;
    }

    public void SimulateLinkChange(string router1, string router2, int newCost)
    {
        if (routers.TryGetValue(router1, out var r1) && routers.TryGetValue(router2, out var r2))
        {
            r1.UpdateTopology(router2, newCost);
            r2.UpdateTopology(router1, newCost);
        }
        else
        {
            Debug.LogError("Router not found!");
        }
    }

    void Start()
    {
        AddRouter(GameObject.Find("R1").GetComponent<Router>());
        AddRouter(GameObject.Find("R2").GetComponent<Router>());
        AddRouter(GameObject.Find("R3").GetComponent<Router>());

        SimulateLinkChange("R1", "R2", 1);
        SimulateLinkChange("R2", "R3", 2);
    }
}
