using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    PhotonView view;

    public float speed = 2f;
    public bool moving = false;

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            // Inputs

            // Move

            if (moving)
            {
                moveForward();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                moving = true;
            }
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                moving = false;
            }
        }
        
    }

    private void moveForward()
    {
        transform.position += speed * transform.forward.normalized * Time.deltaTime;
    }
}
