using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDirectionalPoint : MonoBehaviour
{
    const float velocity = 5f;
    public float mouseSpeed = 5f;
    public float orbitDamping = 10f;

    Vector3 localRot;

    Vector3 startPoint = new Vector3(8, 16, -30);

    private void Start()
    {
        transform.position = startPoint;
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            localRot.y += Input.GetAxis("Mouse X") * mouseSpeed;

            Quaternion QT = Quaternion.Euler(0, localRot.y, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, QT, orbitDamping * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * velocity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * velocity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * velocity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * velocity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += transform.up * velocity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            transform.position -= transform.up * velocity * Time.deltaTime;
        }
    }
}
