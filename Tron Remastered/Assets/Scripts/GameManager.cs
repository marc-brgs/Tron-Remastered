using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Camera camera;
    public GameObject playerView;
    public TMP_Text speed;

    public float spawnMinX = -20f;
    public float spawnMaxX = 20f;
    public float spawnMinY = -20f;
    public float spawnMaxY = 20f;

    void Start()
    {
        SpawnPlayers();
        SetCameraFocus(playerView);
    }

    void Update()
    {
        UpdateSpeed();
    }

    private void SpawnPlayers()
    {
        if (PhotonNetwork.InRoom) // Multi
        {
            Vector3 randomPosition = new Vector3(Random.Range(spawnMinX, spawnMaxX), 0f, Random.Range(spawnMinY, spawnMaxY));
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
            if(player.GetComponent<PhotonView>().IsMine)
            {
                playerView = player;
            }
        }
        else // Solo
        {
            GameObject player = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            player.GetComponent<PlayerMovement>().solo = true;
            
            playerView = player;
        }
    }

    private void SetCameraFocus(GameObject target)
    {
        camera.GetComponent<CameraDrift>().target = target.transform;
        camera.GetComponent<CameraDrift>().enabled = true;
    }

    private void UpdateSpeed()
    {
        float speedMultiplier = 2f;
        speed.text = (playerView.GetComponent<PlayerMovement>().forwardSpeed * speedMultiplier).ToString("F1");
    }
}
