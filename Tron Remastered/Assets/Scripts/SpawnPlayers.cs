using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    void Start()
    {
        if (PhotonNetwork.InRoom) // Multi
        {
            Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), 0f, Random.Range(minX, maxX));
            PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
        }
        else // Solo
        {
            Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}
