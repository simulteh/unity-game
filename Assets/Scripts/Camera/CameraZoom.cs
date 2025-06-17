using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraZoom : MonoBehaviour
{

    [SerializeField] CinemachineVirtualCamera virtualCamera;
    CinemachineComponentBase componentBase;
    float cameraDistance;
    [SerializeField] float sensivity = 10f;

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (componentBase == null)
        {
            componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);

        }
        if (scroll != 0)
        {
            cameraDistance = scroll * sensivity;
            if (componentBase is CinemachineFramingTransposer)
            {
                (componentBase as CinemachineFramingTransposer).m_CameraDistance -= cameraDistance;
            }
        }
    }
}
