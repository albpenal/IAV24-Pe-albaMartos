using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float explosionRadius = 5.0f; // Radio de la zona de explosi�n
    public float explosionForce = 700.0f; // Fuerza de la explosi�n
    public GameObject impacto; // Efecto visual de la explosi�n
    private List<GameObject> impactoPrefabs = new List<GameObject>(); // Lista para almacenar los prefabs de impacto

    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void Explode()
    {
        // Obtener todos los colliders en el radio de la explosi�n
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            // Obtener el rigidbody de los objetos cercanos
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Aplicar fuerza de explosi�n
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
        foreach (GameObject impacto in impactoPrefabs)
        {
            if (impacto != null)
            {
                Destroy(impacto);
            }
        }
        // Destruir el proyectil
        Destroy(gameObject);
    }
    private void Start()
    {
        InstantiateImpactPrefabs();
    }

    void InstantiateImpactPrefabs()
    {
        Vector3 explosionCenter = transform.position;
        explosionCenter.y = 0f;
        int gridCount = Mathf.CeilToInt(explosionRadius * 2); // Contar cu�ntos prefabs se necesitan en el di�metro

        for (int x = -gridCount; x <= gridCount; x++)
        {
            for (int z = -gridCount; z <= gridCount; z++)
            {
                Vector3 checkPosition = explosionCenter + new Vector3(x, 0, z);
                float distance = Vector3.Distance(explosionCenter, checkPosition);

                if (distance <= explosionRadius)
                {
                    // Verificar si hay una obstrucci�n desde el centro de la explosi�n hasta el punto de impacto
                    if (!Physics.Raycast(explosionCenter, checkPosition - explosionCenter, out RaycastHit hit, distance) || hit.collider.tag != "Column")
                    {
                        // Instanciar el prefab de impacto en el suelo
                        if (Physics.Raycast(checkPosition + Vector3.up * 10, Vector3.down, out RaycastHit groundHit))
                        {
                            GameObject impactoInstance = Instantiate(impacto, groundHit.point, Quaternion.identity);
                            impactoPrefabs.Add(impactoInstance); // Agregar el prefab a la lista
                        }
                    }
                }
            }
        }
    }
}
