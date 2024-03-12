using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    public bool solo = false;	
    private bool canAccel = true;

    public float forwardSpeed = 1f; // Vitesse de déplacement avant
    public float forwardAcceleration = 8f;
    public float turnSpeed = 0.1f; // Vitesse de rotation
    public float turnAcceleration = 8f;
    public float maxSpeed = 10f; // Vitesse maximale
    public float maxStraightSpeed = 10f; // Vitesse maximale en ligne droite
    public float inertia = 0.5f; // Inertie de mouvement
    private float currentTurnSpeed = 0f;
    private Vector3 currentVelocity; // Nouvelle vélocité
    private Rigidbody rb;

    private PhotonView view;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        view = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (view.IsMine || solo)
        {
            Move();
        }
    }

    private void Move()
    {
        // Mouvement automatique vers l'avant
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        if (canAccel == true)
        {
            // Augmentation de la vitesse en ligne droite
            if (Mathf.Abs(Input.GetAxis("Horizontal")) < 0.1f && forwardSpeed < maxStraightSpeed)
            {
                forwardSpeed += Time.deltaTime * forwardAcceleration;
            }
        }

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);


        // Rotation de la moto
        float horizontalInput = Input.GetAxis("Horizontal");
        currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, horizontalInput * turnSpeed, Time.deltaTime * turnAcceleration);

        // Appliquer la rotation
        transform.Rotate(Vector3.up, currentTurnSpeed);

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -currentTurnSpeed * 30);

        // Limiter la vitesse maximale
        /*Vector3 currentVelocity = GetComponent<Rigidbody>().velocity;
        if (currentVelocity.magnitude > maxSpeed)
        {
            currentVelocity = currentVelocity.normalized * maxSpeed;
            GetComponent<Rigidbody>().velocity = currentVelocity;
        }

        // Appliquer l'inertie
        GetComponent<Rigidbody>().velocity *= inertia;*/
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
