using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UCM.IAV.Navegacion;
using static UnityEditor.PlayerSettings;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float explosionRadius = 3.0f; // Radio de la zona de explosión
    [SerializeField]
    private float explosionForce = 700.0f; // Fuerza de la explosión
    [SerializeField]
    private GameObject impacto; // Efecto visual de la explosión
    private GraphGrid graphGrid_; // Referencia al graphGrid
    private List<GameObject> impactoPrefabs = new List<GameObject>(); // Lista para almacenar los prefabs de impacto

    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void Explode()
    {
        // Obtener todos los colliders en el radio de la explosión
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            // Obtener el rigidbody de los objetos cercanos
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Aplicar fuerza de explosión si no hay una pared en medio
                if (!IsBlockedByWall(transform.position, rb.position))
                {
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                }
            }
        }
        foreach (GameObject impacto in impactoPrefabs)
        {
            if (impacto != null)
            {
                graphGrid_.UpdateVertexCost(impacto.transform.position, -100);
                Destroy(impacto);
            }
        }
        // Destruir el proyectil
        Destroy(gameObject);
    }

    private void Start()
    {
        graphGrid_ = GameObject.Find("GraphGrid").GetComponent<GraphGrid>();
        InstantiateImpactPrefabs();
    }

    void InstantiateImpactPrefabs()
    {
        Vector3 explosionCenter = transform.position;
        explosionCenter.y = 0.5f;
        int gridCount = Mathf.CeilToInt(explosionRadius * 2); // Contar cuántos prefabs se necesitan en el diámetro
        //Debug.Log((graphGrid_.IdToGrid(graphGrid_.GetNearestVertex(explosionCenter).id)));
        //if (graphGrid_.isGround(graphGrid_.IdToGrid(graphGrid_.GetNearestVertex(explosionCenter).id)))
        if (Physics.Raycast(explosionCenter + Vector3.up * 10, Vector3.down, out RaycastHit initHit) && initHit.collider.tag != "Column")
        {
            for (int x = -gridCount; x <= gridCount; x++)
            {
                for (int z = -gridCount; z <= gridCount; z++)
                {
                    Vector3 checkPosition = explosionCenter + new Vector3(x, 0, z);
                    float distance = Vector3.Distance(explosionCenter, checkPosition);

                    if (distance <= explosionRadius)
                    {
                        // Verificar si hay una obstrucción desde el centro de la explosión hasta el punto de impacto
                        if (!Physics.Raycast(explosionCenter, checkPosition - explosionCenter, out RaycastHit hit, distance) || !hit.collider.CompareTag("Column"))
                        {
                            // Instanciar el prefab de impacto en el suelo
                            if (Physics.Raycast(checkPosition + Vector3.up * 10, Vector3.down, out RaycastHit groundHit))
                            {
                                Vector3 pos = groundHit.point;
                                pos.y = 0f;
                                GameObject impactoInstance = Instantiate(impacto, pos, Quaternion.identity);
                                impactoPrefabs.Add(impactoInstance); // Agregar el prefab a la lista
                                graphGrid_.UpdateVertexCost(pos, 100);
                            }
                        }
                    }
                }
            }
        }
    }

    bool IsBlockedByWall(Vector3 start, Vector3 end)
    {
        // Comprobar si hay una pared entre el centro de la explosión y el punto de impacto
        return Physics.Raycast(start, end - start, out RaycastHit hit, Vector3.Distance(start, end)) && hit.collider.tag == "Column";
    }
}
