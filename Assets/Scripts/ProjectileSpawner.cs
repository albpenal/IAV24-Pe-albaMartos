using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab; // Prefab del proyectil a spawnear
    [SerializeField]
    private GameObject player; // Referencia al player
    [SerializeField]
    public float forwardOffset = 1.0f; // Offset hacia adelante
    [SerializeField]
    private float spawnRadius = 2.0f; // Radio alrededor del primer proyectil
    [SerializeField]
    private int maxAdditionalProjectiles = 4; // Máximo número de proyectiles adicionales
    [SerializeField]
    private int projectileHeight = 20; // Altura en la que hacen spawn los proyectiles
    [SerializeField]
    private float cellSize = 1f; // Tamaño de las celdas en la cuadrícula
    void Update()
    {
        // Spawnear proyectil al presionar la tecla espacio
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            SpawnProjectiles();
        }
    }

    void SpawnProjectiles()
    {
        // Posición del primer proyectil con un pequeño offset hacia adelante
        Vector3 spawnPosition = player.transform.position + player.transform.forward * forwardOffset;
        spawnPosition.y += projectileHeight;
        spawnPosition = AdjustToGridCenter(spawnPosition);
        Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        // Número aleatorio de proyectiles adicionales
        int additionalProjectiles = Random.Range(0, maxAdditionalProjectiles + 1);

        for (int i = 0; i < additionalProjectiles; i++)
        {
            // Posición aleatoria alrededor del primer proyectil
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            randomOffset.y = 0; // Mantener los proyectiles en el plano horizontal
            Vector3 additionalSpawnPosition = spawnPosition + randomOffset;
            additionalSpawnPosition = AdjustToGridCenter(additionalSpawnPosition);
            Instantiate(projectilePrefab, additionalSpawnPosition, Quaternion.identity);
        }
    }

    Vector3 AdjustToGridCenter(Vector3 position)
    {
        position.x = Mathf.Round(position.x / cellSize) * cellSize;
        position.z = Mathf.Round(position.z / cellSize) * cellSize;
        return position;
    }
}