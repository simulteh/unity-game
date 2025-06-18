using UnityEngine;

public class UIController : MonoBehaviour
{
    public void OnSubmitButtonClick()
    {
        GameManager.Instance.OnIPSubmitted();
    }
}