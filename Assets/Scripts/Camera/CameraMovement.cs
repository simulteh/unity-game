using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    [SerializeField] GameObject dirPoint;
    float mouseSpeed;
    float orbitDamping;

    Vector3 localRot;

    private void Awake()
    {
        mouseSpeed = dirPoint.GetComponent<MoveDirectionalPoint>().mouseSpeed;
        orbitDamping = dirPoint.GetComponent<MoveDirectionalPoint>().orbitDamping;
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            transform.position = dirPoint.transform.position;

            localRot.y += Input.GetAxis("Mouse X") * mouseSpeed;
            localRot.x -= Input.GetAxis("Mouse Y") * mouseSpeed;

            Quaternion QT = Quaternion.Euler(localRot.x, localRot.y, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, QT, orbitDamping * Time.deltaTime);
        }
    }
}
