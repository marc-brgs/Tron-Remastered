using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoRotation : MonoBehaviour
{
    public bool rotateX = false;
    public bool rotateY = false;
    public bool rotateZ = false;
    public float speedX = 100.0f;
    public float speedY = 100.0f;
    public float speedZ = 100.0f;

    private float rx = 0f;
    private float ry = 0f;
    private float rz = 0f;

    void Start()
    {
        rx = rotateX ? 1f : 0f;
        ry = rotateY ? 1f : 0f;
        rz = rotateZ ? 1f : 0f;
    }

    void Update()
    {

        this.gameObject.transform.Rotate(new Vector3(rx * Time.deltaTime * speedX, ry * Time.deltaTime * speedY, rz * Time.deltaTime * speedZ));
    }
}