using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    private GameManager gameManager = GameManager.instance;
    public bool solo = false;

    public float forwardSpeed = 30f; // Vitesse de déplacement avant
    public float forwardAcceleration = 8f;
    public float brakePower = 8f;
    public float turnSpeed = 0.5f; // Vitesse de rotation
    public float turnAcceleration = 8f;
    public float minStraightSpeed = 30f;
    public float maxStraightSpeed = 60f; // Vitesse maximale en ligne droite
    public float inclineForce = 35f;
    public float maxBoost = 100f;
    public float currentBoost = 100f;
    public float boostPower = 10f;
    public float boostConsumption = 20f;
    public float boostRechargePower = 10f;
    public float airResistance = 5f;
    
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
            /*if (Mathf.Abs(Input.GetAxis("Horizontal")) < 0.1f && forwardSpeed < maxStraightSpeed)
            {
                forwardSpeed += Time.deltaTime * forwardAcceleration;
            }*/
            if(Input.GetKey(KeyCode.UpArrow) && currentBoost > 0f)
            {

                forwardSpeed += boostPower * Time.deltaTime;
                currentBoost -= boostConsumption * Time.deltaTime;
                if (forwardSpeed > maxStraightSpeed) forwardSpeed = maxStraightSpeed;
                if (currentBoost < 0) currentBoost = 0;
            }
            else
            {
                // Recharge boost
                if(currentBoost < maxBoost)
                {
                    currentBoost += boostRechargePower * Time.deltaTime;
                    if (currentBoost > maxBoost) currentBoost = maxBoost;
                }
                // Deceleration
                if (forwardSpeed > minStraightSpeed)
                {
                    forwardSpeed -= airResistance * Time.deltaTime;
                    if (forwardSpeed < minStraightSpeed) forwardSpeed = minStraightSpeed;
                }
            }
            Debug.Log(currentBoost);
        }
        if(canBrake)
        {
            if (Input.GetKey(KeyCode.DownArrow) && forwardSpeed > minStraightSpeed)
            {
                forwardSpeed -= brakePower * Time.deltaTime;
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
            forwardSpeed = 20f;
            canAccel = false;
            Debug.Log("hit wall");
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

    // Should be in another script
    public void SetRandomColor()
    {
        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
        // Générer une couleur aléatoire
        Color randomColor = new Color(
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            1f
        );
        Debug.Log(randomColor);

        Material randomMaterial = new Material(Shader.Find("Standard"));
        randomMaterial.color = randomColor;

        this.gameObject.GetComponent<MeshGenerator>().mat = randomMaterial;
        GameObject.Find("Moto_TRON orange/corp.001").GetComponent<MeshRenderer>().materials[2] = randomMaterial;
        GameObject.Find("Moto_TRON orange/moteur.001").GetComponent<MeshRenderer>().materials[2] = randomMaterial;
        GameObject.Find("Moto_TRON orange/vitesse.001").GetComponent<MeshRenderer>().materials[2] = randomMaterial;
        GameObject.Find("Moto_TRON orange/vitre.001").GetComponent<MeshRenderer>().materials[2] = randomMaterial;

        //this.gameObject.GetComponent<MeshGenerator>().mat.SetColor("_EmissionColor", randomColor);
        //GameObject.Find("Moto_TRON orange/corp.001").GetComponent<MeshRenderer>().materials[2].SetColor("_EmissionColor", randomColor);
        //GameObject.Find("Moto_TRON orange/moteur.001").GetComponent<MeshRenderer>().materials[2].SetColor("_EmissionColor", randomColor);
        //GameObject.Find("Moto_TRON orange/vitesse.001").GetComponent<MeshRenderer>().materials[2].SetColor("_EmissionColor", randomColor);
        //GameObject.Find("Moto_TRON orange/vitre.001").GetComponent<MeshRenderer>().materials[2].SetColor("_EmissionColor", randomColor);
    }

    public void SetPlayerMaterial()
    {
        int index = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % GameManager.instance.playerMaterials.Length;

        Material playerMaterial = gameManager.playerMaterials[index];

        this.gameObject.GetComponent<MeshGenerator>().mat = playerMaterial;
        GameObject corp = this.gameObject.transform.Find("Moto_TRON orange/corp.001").gameObject;
        GameObject moteur = this.gameObject.transform.Find("Moto_TRON orange/moteur.001").gameObject;
        GameObject vitesse = this.gameObject.transform.Find("Moto_TRON orange/vitesse.001").gameObject;
        GameObject vitre = this.gameObject.transform.Find("Moto_TRON orange/vitre.001").gameObject;
        ReplaceMaterial(corp, 2, playerMaterial);
        ReplaceMaterial(moteur, 2, playerMaterial);
        ReplaceMaterial(vitesse, 2, playerMaterial);
        ReplaceMaterial(vitre, 2, playerMaterial);
    }

    private void ReplaceMaterial(GameObject obj, int materialIndex, Material newMaterial)
    {
        if (obj != null)
        {
            MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Material[] materials = renderer.materials; // Récupère une copie du tableau de matériaux
                if (materials.Length > materialIndex)
                {
                    materials[materialIndex] = newMaterial; // Modifie la copie du tableau
                    renderer.materials = materials; // Réaffecte le tableau modifié au MeshRenderer
                }
            }
        }
    }
}
