using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Camera camera;

    public float minX = -20f;
    public float maxX = 20f;
    public float minY = -20f;
    public float maxY = 20f;

    void Start()
    {
        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        if (PhotonNetwork.InRoom) // Multi
        {
            Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), 0f, Random.Range(minX, maxX));
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
            if(player.GetComponent<PhotonView>().IsMine)
            {
                camera.GetComponent<CameraDrift>().target = player.transform;
            }
        }
        else // Solo
        {
            GameObject player = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            camera.GetComponent<CameraDrift>().target = player.transform;
        }
    }

    private void SetCamera()
    {

    }
}
