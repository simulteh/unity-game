using UnityEngine;

public class CrimpCheckButton : MonoBehaviour
{
    public CrimpValidator validator;

    private void OnMouseDown()
    {
        validator.Validate();
    }
}
