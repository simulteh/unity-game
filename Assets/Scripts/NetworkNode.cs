using UnityEngine;

public class NetworkNode : MonoBehaviour
{
    public IPGameData NodeIP { get; private set; }
    public GameObject lockedIndicator;
    
    private IPNetworkManager networkManager;
    private bool isLocked = true;
    
    public void SetupNode(IPNetworkManager manager, IPGameData ipData)
    {
        networkManager = manager;
        NodeIP = ipData;
        UpdateLockedState();
    }
    
    private void UpdateLockedState()
    {
        lockedIndicator.SetActive(isLocked);
    }
    
    public void Unlock()
    {
        isLocked = false;
        UpdateLockedState();
        // Visual effects could be added here
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isLocked)
        {
            networkManager.ActivateNodeChallenge(this);
        }
    }
}