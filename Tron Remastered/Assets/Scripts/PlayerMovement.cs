using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
   
    private bool canAccel = true;

    public float forwardSpeed = 1f; // Vitesse de déplacement avant
    public float turnSpeed = 0.1f; // Vitesse de rotation
    public float maxSpeed = 10f; // Vitesse maximale
    public float maxStraightSpeed = 10f; // Vitesse maximale en ligne droite
    public float inertia = 0.5f; // Inertie de mouvement
    private float currentTurnSpeed = 0f;
    private Vector3 currentVelocity; // Nouvelle vélocité
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Mouvement automatique vers l'avant
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        if (canAccel == true)
        {
            // Augmentation de la vitesse en ligne droite
            if (Mathf.Abs(Input.GetAxis("Horizontal")) < 0.1f && forwardSpeed < maxStraightSpeed)
            {
                forwardSpeed += Time.deltaTime * 8;
            }
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
    
    
    void OnCollisionEnter(Collision collision)
    {
        // Si la moto entre en collision avec un mur, arrête le mouvement
        if (collision.gameObject.CompareTag("Mur"))
        {
            forwardSpeed = 10f;
            canAccel = false;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        canAccel = true;
    }
}
