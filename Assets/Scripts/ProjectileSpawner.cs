using System.Collections;
using System.Collections.Generic;
using UCM.IAV.Movimiento;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab; // Prefab del proyectil a spawnear
    [SerializeField]
    private GameObject fakeProjectilePrefab; // Prefab del proyectil a spawnear
    [SerializeField]
    private GameObject player; // Referencia al player
    [SerializeField]
    public float forwardOffset = 1.0f; // Offset hacia adelante
    [SerializeField]
    private float spawnRadius = 2.0f; // Radio alrededor del primer proyectil
    [SerializeField]
    private int maxAdditionalProjectiles = 4; // M�ximo n�mero de proyectiles adicionales
    [SerializeField]
    private int projectileHeight = 20; // Altura en la que hacen spawn los proyectiles
    [SerializeField]
    private float cellSize = 1f; // Tama�o de las celdas en la cuadr�cula
    [SerializeField]
    private float minSpawnInterval = 0.5f; // Intervalo m�nimo de spawn
    [SerializeField]
    private float maxSpawnInterval = 1.5f; // Intervalo m�ximo de spawn
    [SerializeField]
    private Transform[] cannons; // Transforms de los ca�ones en escena

    void SpawnProjectiles()
    {
        if (!GameManager.instance.gameOver)
            {
            // Posici�n del primer proyectil con un peque�o offset hacia adelante
            Vector3 spawnPosition = player.transform.position + player.transform.forward * forwardOffset;
            spawnPosition.y += projectileHeight;
            spawnPosition = AdjustToGridCenter(spawnPosition);
            Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

            // N�mero aleatorio de proyectiles adicionales
            int additionalProjectiles = Random.Range(0, maxAdditionalProjectiles + 1);

            for (int i = 0; i < additionalProjectiles; i++)
            {
                // Posici�n aleatoria alrededor del primer proyectil
                Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
                randomOffset.y = 0; // Mantener los proyectiles en el plano horizontal
                Vector3 additionalSpawnPosition = spawnPosition + randomOffset;
                additionalSpawnPosition = AdjustToGridCenter(additionalSpawnPosition);
                Instantiate(projectilePrefab, additionalSpawnPosition, Quaternion.identity);
            }
            for(int i = 0; i < additionalProjectiles + 1; i++)
            {
                GameObject projectile = Instantiate(fakeProjectilePrefab, cannons[i].position, Quaternion.identity);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                rb.velocity = new Vector3(0, 60, 1);
            }
        }
    }

    Vector3 AdjustToGridCenter(Vector3 position)
    {
        position.x = Mathf.Round(position.x / cellSize) * cellSize;
        position.z = Mathf.Round(position.z / cellSize) * cellSize;
        return position;
    }

    private void Start()
    {
        StartCoroutine(SpawnProjectilesRoutine());
    }

    private IEnumerator SpawnProjectilesRoutine()
    {
        while (!GameManager.instance.gameOver)
        {
            // Esperar un tiempo aleatorio entre minSpawnInterval y maxSpawnInterval
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            // Spawnear los proyectiles
            SpawnProjectiles();
        }
    }


}