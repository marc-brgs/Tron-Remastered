using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public Material mat;
    public GameObject player;
    private GameObject line;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    
    private Material tmpMat;
    
    private bool firstTime = true;
    private bool isEven = false;
    
    private float width = 0.15f;
    
    private List<Vector3> verticesDef;
    private List<int> trianglesDef;
    
    private float spawnTime = 0f;
    private float monoSpawnDelay = 0.06f;
    
    private int x = -1;

    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player = gameObject;
        //CreateCube();
    }


    private void Update()
    {
        if (Time.time > spawnTime)
        {
            spawnTime = Time.time + monoSpawnDelay;
            Monoline();
        }
    }




    private void Monoline()
    {
        Vector3[] vertices = null;
        int[] triangles = null;

        float backwardDistance = 1.5f;
        var backward = player.transform.position - (player.transform.forward * backwardDistance);
        
        // Calcul de la position Y pour la trace
        float traceHeight = 1f; // Hauteur souhaitée de la trace par rapport au joueur
        var tracePosition = new Vector3(0, traceHeight, 0);

        if (firstTime)
        {
            vertices = new Vector3[]
            {
                backward + (player.transform.right * -width) + tracePosition,
                backward - (player.transform.right * -width) + tracePosition,
                backward - (player.transform.right * -width) + player.transform.up * width * 6 + tracePosition,
                backward + (player.transform.right * -width) + player.transform.up * width * 6 + tracePosition
            };

            triangles = new int[]
            {
                0, 2, 1,
                0, 3, 2,
            };

            line = new GameObject("Player Trail");
            line.tag = "Trail";
            line.layer = 7; // Trail

            meshFilter = line.AddComponent<MeshFilter>();
            line.AddComponent<MeshRenderer>().material = mat;

            meshCollider = line.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = meshFilter.mesh;

            meshFilter.mesh.vertices = vertices;
            meshFilter.mesh.triangles = triangles;

            verticesDef = new List<Vector3>();
            trianglesDef = new List<int>();
            foreach (var v in vertices)
            {
                verticesDef.Add(v);
            }
            foreach (var t in triangles)
            {
                trianglesDef.Add(t);
            }

            isEven = false;
            firstTime = false;
            x = 4;
            return;
        }

        if (isEven)
        {
            verticesDef.Add(backward + (player.transform.right * -width) + tracePosition);
            verticesDef.Add(backward - (player.transform.right * -width) + tracePosition);
            verticesDef.Add(backward - (player.transform.right * -width) + player.transform.up * width * 6 + tracePosition) ;
            verticesDef.Add(backward + (player.transform.right * -width) + player.transform.up * width * 6 + tracePosition);

            trianglesDef.Add(x - 4);
            trianglesDef.Add(x - 1);
            trianglesDef.Add(x);
            
            trianglesDef.Add(x - 4);
            trianglesDef.Add(x);
            trianglesDef.Add(x + 3);
            
            trianglesDef.Add(x - 4);
            trianglesDef.Add(x + 3);
            trianglesDef.Add(x + 2);
            
            trianglesDef.Add(x - 4);
            trianglesDef.Add(x + 2);
            trianglesDef.Add(x - 3);
            
            
            trianglesDef.Add(x - 3);
            trianglesDef.Add(x + 2);
            trianglesDef.Add(x + 1);
            
            trianglesDef.Add(x - 3);
            trianglesDef.Add(x + 1);
            trianglesDef.Add(x - 2);

            isEven = false;
        }
        else
        {
            verticesDef.Add(backward + (player.transform.right * -width) + player.transform.up * width * 6 + tracePosition);
            verticesDef.Add(backward - (player.transform.right * -width) + player.transform.up * width * 6 + tracePosition);
            verticesDef.Add(backward - (player.transform.right * -width) + tracePosition);
            verticesDef.Add(backward + (player.transform.right * -width) + tracePosition);
            
            trianglesDef.Add(x - 4);
            trianglesDef.Add(x + 3);
            trianglesDef.Add(x);
           
            trianglesDef.Add(x - 4);
            trianglesDef.Add(x);
            trianglesDef.Add(x - 1);
            
            trianglesDef.Add(x - 2);
            trianglesDef.Add(x - 1);
            trianglesDef.Add(x);
            
            trianglesDef.Add(x - 2);
            trianglesDef.Add(x);
            trianglesDef.Add(x + 1);
            
            trianglesDef.Add(x - 3);
            trianglesDef.Add(x - 2);
            trianglesDef.Add(x + 1);
            
            trianglesDef.Add(x - 3);
            trianglesDef.Add(x + 1);
            trianglesDef.Add(x + 2);

            isEven = true;
        }

        x += 4;
        meshFilter.mesh.vertices = verticesDef.ToArray();
        meshFilter.mesh.triangles = trianglesDef.ToArray();

        meshCollider.sharedMesh = meshFilter.mesh;
    }
    
    
    
    
    
    

    private void CreateCube()
    {

        GameObject cube = new GameObject();
        cube.name = "exampleCube";

        var meshFilter = cube.AddComponent<MeshFilter>();
        var meshRenderer = cube.AddComponent<MeshRenderer>();
        meshRenderer.material = mat;

        Vector3[] vertices =
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(1, 0, 1),
            new Vector3(0, 0, 1)
        };

        meshFilter.mesh.vertices = vertices;

        int[] triangles =
        {
            0, 2, 1,
            0, 3, 2,
            2, 3, 4,
            2, 4, 5,
            1, 2, 5,
            1, 5, 6,
            0, 7, 4,
            0, 4, 3,
            5, 4, 7,
            5, 7, 6,
            0, 6, 7,
            0, 1, 6
        };
        
    }
}
