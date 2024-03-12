using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float forwardSpeed = 5f; // Vitesse de déplacement avant
    public float turnSpeed = 2f; // Vitesse de rotation
    public float maxSpeed = 10f; // Vitesse maximale
    public float maxStraightSpeed = 7f; // Vitesse maximale en ligne droite
    public float inertia = 0.95f; // Inertie de mouvement

    public float currentTurnSpeed = 0f;

    void Update()
    {
        // Mouvement automatique vers l'avant
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        // Augmentation de la vitesse en ligne droite
        if (Mathf.Abs(Input.GetAxis("Horizontal")) < 0.1f && forwardSpeed < maxStraightSpeed)
        {
            forwardSpeed += Time.deltaTime;
        }

        // Rotation de la moto
        float horizontalInput = Input.GetAxis("Horizontal");
        currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, horizontalInput * turnSpeed, Time.deltaTime * 5f);

        // Appliquer la rotation
        transform.Rotate(Vector3.up, currentTurnSpeed);

        // Limiter la vitesse maximale
        Vector3 currentVelocity = GetComponent<Rigidbody>().velocity;
        if (currentVelocity.magnitude > maxSpeed)
        {
            currentVelocity = currentVelocity.normalized * maxSpeed;
            GetComponent<Rigidbody>().velocity = currentVelocity;
        }

        // Appliquer l'inertie
        GetComponent<Rigidbody>().velocity *= inertia;
    }
}
