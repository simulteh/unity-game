using UnityEngine;

public class NetworkDoor : MonoBehaviour
{
    public GameObject lockedVisual;
    public GameObject unlockedVisual;
    
    private IPNetworkManager networkManager;
    private bool isLocked = true;
    
    public void Setup(IPNetworkManager manager)
    {
        networkManager = manager;
        UpdateDoorState();
    }
    
    private void UpdateDoorState()
    {
        lockedVisual.SetActive(isLocked);
        unlockedVisual.SetActive(!isLocked);
        GetComponent<Collider>().isTrigger = !isLocked;
    }
    
    public void Unlock()
    {
        isLocked = false;
        UpdateDoorState();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isLocked)
        {
            // Could activate a special door challenge
        }
    }
}