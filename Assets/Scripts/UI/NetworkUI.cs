using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace OSPF.Simulation.UI
{
    public class NetworkUI : MonoBehaviour
    {
        public Network network;
        public TMP_InputField routerIdInput;
        public TMP_InputField costInput;
        public TMP_Text routingTableText;

        public void OnAddRouterClicked()
        {
            if (string.IsNullOrEmpty(routerIdInput.text))
            {
                Debug.LogError("Router ID cannot be empty!");
                return;
            }

            GameObject newRouter = new GameObject("Router_" + routerIdInput.text);
            Router routerComp = newRouter.AddComponent<Router>();
            routerComp.routerID = routerIdInput.text;
            network.AddRouter(routerComp);

            routerIdInput.text = "";
        }

        public void UpdateRoutingTableDisplay()
        {
            string tableText = "Routing Tables:\n";
            foreach (var router in network.routers.Values)
            {
                tableText += $"{router.routerID}:\n";
                foreach (var entry in router.routingTable)
                {
                    tableText += $"  -> {entry.Key} (via {entry.Value.nextHop}, cost: {entry.Value.cost})\n";
                }
            }
            routingTableText.text = tableText;
        }

        void Update()
        {
            if (network.routers.Count > 0)
            {
                UpdateRoutingTableDisplay();
            }
        }
    }
}