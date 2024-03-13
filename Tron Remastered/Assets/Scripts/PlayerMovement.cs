using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    private GameManager gameManager = GameManager.instance;
    public bool solo = false;

    public float forwardSpeed = 0f; // Vitesse de déplacement avant
    public float forwardAcceleration = 5f;
    public float forwardDeceleration = 5f;
    public float turnSpeed = 0.1f; // Vitesse de rotation
    public float turnAcceleration = 8f;
    public float minStraightSpeed = 15f;
    public float maxStraightSpeed = 25f; // Vitesse maximale en ligne droite
    public float inclineForce = 50f;
    public float inertia = 0.5f; // Inertie de mouvement
    
    private bool died = false;
    private bool canAccel = true;
    private bool canBrake = true;
    private float currentTurnSpeed = 0f;
    private Rigidbody rb;
    private Vector3 averageDirection = Vector3.forward;

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
            if (!died)
            {
                Move();
            }
        }
    }

    private void Move()
    {
        float convergenceSpeed = 10f;
        // Mouvement automatique vers l'avant
        Vector3 targetDirection;

        if (IsGrounded())
        {
            // Lorsque le joueur est au sol, la direction cible est forward
            targetDirection = transform.forward;
        }
        else
        {
            // Lorsque le joueur est en l'air, la direction cible devient forward mais horizontale
            Vector3 originalForward = transform.forward;
            targetDirection = new Vector3(originalForward.x, 0, originalForward.z).normalized;
        }

        // Interpole progressivement averageDirection vers la direction cible
        averageDirection = Vector3.Lerp(averageDirection, targetDirection, Time.deltaTime * convergenceSpeed);

        // Utilisez averageDirection pour le déplacement
        Vector3 applyDirection = targetDirection;
        if (!IsGrounded())
            applyDirection = new Vector3(targetDirection.x, averageDirection.y, targetDirection.z);

        transform.Translate(applyDirection * forwardSpeed * Time.deltaTime, Space.World);

        //Debug.DrawRay(transform.position, targetDirection * 4f, Color.blue);
        //Debug.DrawRay(transform.position, averageDirection * 4f, Color.red);
        //Debug.DrawRay(transform.position, applyDirection * 4f, Color.yellow);

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

        // Rotation de la moto
        float horizontalInput = Input.GetAxis("Horizontal");
        currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, horizontalInput * turnSpeed, Time.deltaTime * turnAcceleration);

        // Appliquer la rotation
        transform.Rotate(Vector3.up, currentTurnSpeed);

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -currentTurnSpeed * inclineForce);
    }

    private void Die()
    {
        forwardSpeed = 0;
        died = true;
        
        gameManager.InitiateEndGame(this.gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        // Si la moto entre en collision avec un mur, arrête le mouvement
        if (other.gameObject.CompareTag("Mur"))
        {
            forwardSpeed = 10f;
            canAccel = false;
        }

        if(other.gameObject.CompareTag("Trail") || other.gameObject.CompareTag("OutsideWall"))
        {
            Die();
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Mur"))
        {
            canAccel = true;
        }
    }

    public bool IsGrounded()
    {
        RaycastHit hit;
        float rayLength = 1.3f; // Adjust based on your character's size
        //Debug.DrawRay(transform.position + Vector3.up * 2f, Vector3.down * rayLength, Color.red);
        if (Physics.Raycast(transform.position + Vector3.up * 2f, Vector3.down, out hit, rayLength))
        {
            return true;
        }
        return false;
    }
}
