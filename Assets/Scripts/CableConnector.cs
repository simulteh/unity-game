using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(MouseDetector))]
public class CableConnector : MonoBehaviour
{
    const bool auto = true;
    [SerializeField] private GameObject cable;
    private GameObject from;
    private GameObject to;
    public NavMeshAgent agent;

    public void OnEnable()
    {
        var comp = GetComponent<MouseDetector>();
        comp.OnDetection += OnNewTarget;
    }

    private void OnNewTarget(object sender, DetectionEventArgs args)
    {
        if (from == null && args.Target != null)
        {
            from = args.Target;
            print($"New from target: {from.name}");
        }
        else if (to == null && args.Target != null)
        {
            to = args.Target;
            
            print($"New to target: {from.name}");

            if (auto)
            {
                //agent.SetDestination(to.transform.position);

                var dist = Vector3.Distance(from.transform.position, to.transform.position);
                var direction = Vector3.Normalize(to.transform.position - from.transform.position);
                var rotation = Quaternion.FromToRotation(from.transform.position, to.transform.position);
                var len = Mathf.CeilToInt(dist);
                for (float i = 0; i < len; i+=0.07f)
                {
                    var spawn = from.transform.position + direction * i;

                    var segment = Instantiate(cable, new Vector3(), new Quaternion());
                    
                    //segment.transform.localScale = new Vector3(dist, segment.transform.localScale.y, segment.transform.localScale.z);
                    segment.transform.position = spawn;
                    segment.transform.up = to.transform.position - from.transform.position;
                    //segment.transform.LookAt(to.transform.position);
                }
            }
            else
            {
                // TODO: reset choosed from and to ?
            }
        }
        else if (from != null && to != null && args.Target != null)
        {
            from = args.Target;
            to = null;
            print("Reset selection for cable");
        }
    }
    
}
