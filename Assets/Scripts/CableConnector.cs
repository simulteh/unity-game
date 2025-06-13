using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[RequireComponent(typeof(MouseDetector))]
public class CableConnector : MonoBehaviour
{
    const bool auto = true;
    [SerializeField] private GameObject cable;
    private GameObject from;
    private GameObject to;

    public void Update()
    {
        var comp = GetComponent<MouseDetector>();
        if (comp == null)
            return;
        if (from == null)
        {
            from = comp.target;
        }
        else if (to == null)
        {
            to = comp.target;

            if (auto)
            {
                var dist = Vector3.Distance(from.transform.position, to.transform.position);
                var direction = Vector3.Normalize(from.transform.position - to.transform.position);
                var len = Mathf.CeilToInt(dist);
                for (int i = 0; i < len; i++)
                {
                    Instantiate(cable, from.transform.position + direction * i, new Quaternion());
                }
            }
            else
            {
                // TODO: reset choosed from and to ?
            }
        }
    }
}
