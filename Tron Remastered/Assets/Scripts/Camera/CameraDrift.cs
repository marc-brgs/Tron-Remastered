using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrift : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public Vector3 lookAtOffset;
    public float speed;
    public bool absoluteOffsetY = false; // Contr�le si l'offset Y est relatif au monde

    private Rigidbody targetRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        targetRigidbody = target.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
            return;
        
        // Calcul de l'offset avec la composante Y optionnellement relative au monde
        Vector3 effectiveOffset = offset;
        if (absoluteOffsetY)
        {
            effectiveOffset = target.transform.TransformVector(new Vector3(offset.x, 0, offset.z))
                + new Vector3(0, offset.y, 0); // Ajoute la composante Y de l'offset relative au monde
        }
        else
        {
            effectiveOffset = target.transform.TransformVector(offset);
        }

        Vector3 targetPositionWithOffset = target.position + effectiveOffset;
        Vector3 playerForwardAdjustment = (targetRigidbody.velocity + target.transform.forward).normalized * (-5f);

        transform.position = Vector3.Lerp(transform.position,
            targetPositionWithOffset + playerForwardAdjustment,
            speed * Time.deltaTime);

        transform.LookAt(target.transform.position + lookAtOffset);
    }
}