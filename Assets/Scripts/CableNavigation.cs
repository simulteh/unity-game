using UnityEngine;
using UnityEngine.AI;

public class CableNavigation : MonoBehaviour
{
    public bool Move;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Move)
        {


            Move = false;
        }
    }
}
