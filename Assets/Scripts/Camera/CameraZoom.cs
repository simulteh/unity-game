using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraZoom : MonoBehaviour
{

    [SerializeField] CinemachineVirtualCamera virtualCamera;
    CinemachineComponentBase componentBase;
    float cameraDistanceOffset;
    [SerializeField] float sensivity = 10f;

    const float MIN_DISTANCE = 5;
    const float MAX_DISTANCE = 150;

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (componentBase == null)
        {
            componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);

        }
        if (scroll != 0)
        {
            if (componentBase is CinemachineFramingTransposer)
            {
                float currentDistance = (componentBase as CinemachineFramingTransposer).m_CameraDistance;



                if (currentDistance >= MIN_DISTANCE && currentDistance <= MAX_DISTANCE)
                {
                    cameraDistanceOffset = scroll * sensivity;
                    float future_distance = currentDistance - cameraDistanceOffset;
                    if (future_distance < MIN_DISTANCE) future_distance = MIN_DISTANCE;
                    if (future_distance > MAX_DISTANCE) future_distance = MAX_DISTANCE;
                    
                    (componentBase as CinemachineFramingTransposer).m_CameraDistance = future_distance;
                    //Debug.Log((componentBase as CinemachineFramingTransposer).m_CameraDistance);
                }
            }            
        }
    }
}
