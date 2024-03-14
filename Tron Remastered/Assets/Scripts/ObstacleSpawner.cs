using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public GameObject rampPrefab;
    public GameObject cylinderPrefab;
    public int numberOfObstacles = 10;
    public int numberOfRamps = 5;
    public int numberOfCylinders = 5;
    public float spawnRadius = 200f;
    public float minDistance = 10f; // Distance minimale souhaitée entre les obstacles

    void Start()
    {
        Random.InitState(42); // Sync random seed

        SpawnObstacles();
        SpawnRamps();
        SpawnCylinder();
    }

    void SpawnObstacles()
    {
        for (int i = 0; i < numberOfObstacles; i++)
        {
            Vector3 randomPosition = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = new Vector3(randomPosition.x, 0, randomPosition.y);
            spawnPosition += transform.position; // Offset by spawner position
            Quaternion spawnRotation = Quaternion.Euler(0, Random.Range(0, 2) * 90, 0); // Random rotation on Y axis

            // Vérifie si la nouvelle position est suffisamment éloignée des autres obstacles
            if (IsPositionValid(spawnPosition))
            {
                // Créer l'obstacle
                GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, spawnRotation);
                // Définir l'échelle de manière aléatoire
                Vector3 scale = obstacle.transform.localScale;
                float randomScaleFactor = Random.Range(1f, 2f); // Random scale factor between 1 and 2
                obstacle.transform.localScale = new Vector3(scale.x * randomScaleFactor, scale.y * randomScaleFactor, scale.z * randomScaleFactor);
                // Vous pouvez ajouter toute personnalisation supplémentaire pour l'obstacle ici
            }
        }
    }

    void SpawnRamps()
    {
        for (int i = 0; i < numberOfRamps; i++)
        {
            Vector3 randomPosition = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = new Vector3(randomPosition.x, -1, randomPosition.y);
            spawnPosition += transform.position; // Offset by spawner position
            Quaternion spawnRotation = Quaternion.Euler(20, Random.Range(0, 360), 0); // Random rotation on Y axis

            // Vérifie si la nouvelle position est suffisamment éloignée des autres obstacles
            if (IsPositionValid(spawnPosition))
            {
                // Créer la rampe
                GameObject ramp = Instantiate(rampPrefab, spawnPosition, spawnRotation);
                // Définir l'échelle de manière aléatoire
                Vector3 scale = ramp.transform.localScale;
                float randomScaleFactor = Random.Range(1f, 3f); // Random scale factor between 1 and 2
                ramp.transform.localScale = new Vector3(scale.x * randomScaleFactor, scale.y * randomScaleFactor, scale.z * randomScaleFactor);
                // Vous pouvez ajouter toute personnalisation supplémentaire pour la rampe ici
            }
        }
    }
    
    void SpawnCylinder()
    {
        for (int i = 0; i < numberOfCylinders; i++)
        {
            Vector3 randomPosition = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = new Vector3(randomPosition.x, 25, randomPosition.y);
            spawnPosition += transform.position; // Offset by spawner position
            Quaternion spawnRotation = Quaternion.Euler(Random.Range(-80, 80), 0, Random.Range(-20, 20)); // Random rotation on Y axis

            // Vérifie si la nouvelle position est suffisamment éloignée des autres obstacles
            if (IsPositionValid(spawnPosition))
            {
                // Créer la rampe
                GameObject cylinder = Instantiate(cylinderPrefab, spawnPosition, spawnRotation);
                // Définir l'échelle de manière aléatoire
                Vector3 scale = cylinder.transform.localScale;
                float randomScaleFactor = Random.Range(1f, 2f); // Random scale factor between 1 and 2
                cylinder.transform.localScale = new Vector3(scale.x * randomScaleFactor, scale.y * randomScaleFactor, scale.z * randomScaleFactor);
                // Vous pouvez ajouter toute personnalisation supplémentaire pour la rampe ici
            }
        }
    }
    
    
    
    
    

    // Vérifie si la position spécifiée est suffisamment éloignée des autres obstacles
    private bool IsPositionValid(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, minDistance);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Ground"))
            {
                // Si le collider est marqué comme sol, ignore-le et continue à vérifier les autres colliders
                continue;
            }

            // Si un autre collider est détecté (et n'est pas le sol), la position n'est pas valide
            return false;
        }
        // Si aucun autre collider (autre que le sol) n'est détecté, la position est valide
        return true;
    }
}
