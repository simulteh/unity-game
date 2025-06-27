using UnityEngine;

public class SlotCollider : MonoBehaviour
{
    public string expectedWireTag;
    public int slotNumber;

    private string currentWireTag = null;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"🟡 Slot {slotNumber}: столкнулся с {other.tag}");

        currentWireTag = other.tag;

        if (other.CompareTag(expectedWireTag))
        {
            Debug.Log($" Slot {slotNumber} — правильный провод!");
        }
        else
        {
            Debug.Log($" Slot {slotNumber} — неправильный провод ({other.tag})");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (currentWireTag == other.tag)
        {
            Debug.Log($" Slot {slotNumber}: провод {other.tag} вышел");
            currentWireTag = null;
        }
    }

    public bool IsCorrect()
    {
        return currentWireTag == expectedWireTag;
    }
}
