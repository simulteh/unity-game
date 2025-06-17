using UnityEngine;

public class EthernetVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject _framePrefab;
    [SerializeField] private float _animationSpeed = 5f;
    [SerializeField] private Color _unicastColor = Color.green;
    [SerializeField] private Color _broadcastColor = Color.red;

    public void VisualizeTransmission(EthernetFrame frame, Vector3 start, Vector3 end)
    {
        var frameObj = Instantiate(_framePrefab, start, Quaternion.identity);
        var visual = frameObj.GetComponent<FrameVisual>();

        visual.Setup(
            frame.DestinationAddress.IsBroadcast() ? _broadcastColor : _unicastColor,
            frame.Payload.Length
        );

        StartCoroutine(visual.AnimateMove(end, _animationSpeed));
    }
}