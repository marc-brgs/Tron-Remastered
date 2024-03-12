using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrift : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public Vector3 lookAtOffset;
    public float speed;

    private Rigidbody targetRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        targetRigidbody = target.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerForward = (targetRigidbody.velocity + target.transform.forward).normalized;
        transform.position = Vector3.Lerp(transform.position,
            target.position + target.transform.TransformVector(offset + lookAtOffset) + playerForward * (-5f),
            speed * Time.deltaTime);
        transform.LookAt(target.transform.position + lookAtOffset);
    }
}
