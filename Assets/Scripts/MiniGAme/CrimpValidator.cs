using UnityEngine;

public class CrimpValidator : MonoBehaviour
{
    public SlotCollider[] slots;

    public void Validate()
    {
        foreach (var slot in slots)
        {
            if (!slot.IsCorrect())
            {
                Debug.Log(" Ошибка в порядке");
                return;
            }
        }

        Debug.Log(" Всё правильно! Кабель обжат");
    }
    public bool AllSlotsCorrect()
    {
        foreach (var slot in slots)
        {
            if (!slot.IsCorrect())
                return false;
        }
        return true;
    }
}
