using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    public bool solo = false;

    public float forwardSpeed = 0f; // Vitesse de déplacement avant
    public float forwardAcceleration = 5f;
    public float forwardDeceleration = 5f;
    public float turnSpeed = 0.1f; // Vitesse de rotation
    public float turnAcceleration = 8f;
    public float maxSpeed = 25f; // Vitesse maximale
    public float minStraightSpeed = 15f;
    public float maxStraightSpeed = 25f; // Vitesse maximale en ligne droite
    public float inclineForce = 50f;
    public float inertia = 0.5f; // Inertie de mouvement

    private bool canAccel = true;
    private bool canBrake = true;
    private float currentTurnSpeed = 0f;
    private Vector3 currentVelocity; // Nouvelle vélocité
    private Rigidbody rb;

    private bool isBraking = false;

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
        Debug.Log(forwardSpeed);
        //rb.velocity = transform.forward * forwardSpeed * Time.deltaTime;

        if (canAccel && !isBraking)
        {
            // Augmentation de la vitesse en ligne droite
            if (Mathf.Abs(Input.GetAxis("Horizontal")) < 0.1f && forwardSpeed < maxStraightSpeed)
            {
                forwardSpeed += Time.deltaTime * forwardAcceleration;
            }
        }
        if(canBrake)
        {
            if (Input.GetKey(KeyCode.DownArrow) && forwardSpeed > minStraightSpeed)
            {
                forwardSpeed -= Time.deltaTime * forwardDeceleration;
                isBraking = true;
            }
            else
            {
                isBraking = false;
            }
        }

        //transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);

        // Rotation de la moto
        float horizontalInput = Input.GetAxis("Horizontal");
        currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, horizontalInput * turnSpeed, Time.deltaTime * turnAcceleration);

        // Appliquer la rotation
        transform.Rotate(Vector3.up, currentTurnSpeed);

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -currentTurnSpeed * inclineForce);

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
