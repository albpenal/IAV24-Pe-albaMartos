/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
namespace UCM.IAV.Navegacion {
    using UnityEngine;

    using System.Collections.Generic;


    /// <summary>
    /// Abstract class for graphs
    /// </summary>
    public abstract class Graph : MonoBehaviour {
        // Aquí el grafo entero es representado con estas listas, que luego puede aprovechar el algoritmo A*.
        // El pseudocódigo de Millington no asume que tengamos toda la información del grafo representada y por eso va guardando registros de los nodos que visita.
        public GameObject vertexPrefab;
        protected List<Vertex> vertices;
        protected List<List<Vertex>> neighbourVertex;
        protected List<List<float>> costs;
        protected bool[,] mapVertices;
        protected float[,] costsVertices;
        protected int numCols, numRows;

        // this is for informed search like A*
        // Un delegado especifica la cabecera de una función, la que sea, que cumpla con esos parámetros y devuelva ese tipo.
        // Cuidado al implementarlas, porque no puede ser que la distancia -por ejemplo- entre dos casillas tenga una heurística más cara que el coste real de navegar de una a otra.
        public delegate float Heuristic(Vertex a, Vertex b);

        // Used for getting path in frames
        public List<Vertex> path;


        public virtual void Start() {
            Load();
        }

        public virtual void Load() { }

        public virtual int GetSize() {
            if (ReferenceEquals(vertices, null))
                return 0;
            return vertices.Count;
        }

        public virtual void UpdateVertexCost(Vector3 position, float costMultipliyer) { }

        public virtual Vertex GetNearestVertex(Vector3 position) {
            return null;
        }

        public virtual GameObject GetRandomPos() {
            return null;
        }

        public virtual Vertex[] GetNeighbours(Vertex v)
        {
            if (ReferenceEquals(neighbourVertex, null) || neighbourVertex.Count == 0 ||
                v.id < 0 || v.id >= neighbourVertex.Count)
                return new Vertex[0];
            return neighbourVertex[v.id].ToArray();
        }

        public virtual float[] GetNeighboursCosts(Vertex v) {
            if (ReferenceEquals(neighbourVertex, null) || neighbourVertex.Count == 0 ||
                v.id < 0 || v.id >= neighbourVertex.Count)
                return new float[0];

            Vertex[] neighs = neighbourVertex[v.id].ToArray();
            float[] costsV = new float[neighs.Length];
            for (int neighbour = 0; neighbour < neighs.Length; neighbour++) {
                int j = (int)Mathf.Floor(neighs[neighbour].id / numCols);
                int i = (int)Mathf.Floor(neighs[neighbour].id % numCols);
                costsV[neighbour] = costsVertices[j, i];
            }

            return costsV;
        }

        // Encuentra caminos óptimos
        public List<Vertex> GetPathBFS(GameObject srcO, GameObject dstO) {
            // IMPLEMENTAR ALGORITMO BFS
            return new List<Vertex>();
        }

        // No encuentra caminos óptimos
        public List<Vertex> GetPathDFS(GameObject srcO, GameObject dstO) {
            // IMPLEMENTAR ALGORITMO DFS
            return new List<Vertex>();
        }

        public List<Vertex> GetPathAstar(GameObject srcO, GameObject dstO, Heuristic h = null) {
            // Origen o destino invalidos
            if (srcO == null || dstO == null)
                return new List<Vertex>();

            // Obtener los vertices más cercanos a los puntos de inicio y destino
            Vertex start = GetNearestVertex(srcO.transform.position);
            Vertex end = GetNearestVertex(dstO.transform.position);

            BinaryHeap<Vertex> openList = new BinaryHeap<Vertex>();

            // Valores Iniciales
            foreach (Vertex v in vertices){ 
                v.previousVertexID = -1;
                v.costSoFar = float.PositiveInfinity;
                v.hCost = float.PositiveInfinity;
            }
            start.previousVertexID = start.id;
            start.costSoFar = 0;
            start.hCost = h(start, end);
            openList.Add(start);

            while (openList.Count > 0) {
                Vertex current = openList.Remove(); // Actualiza el actual (y lo saca de la openList)

                if (current.id == end.id) // Si el actual es el final termina
                    return BuildPath(start.id, end.id);

                Vertex[] neighboursAct = GetNeighbours(vertices[current.id]); // Vecinos del vertice actual
                float[] neighboursCosts = GetNeighboursCosts(current);

                for (int n = 0; n < neighboursAct.Length; n++) {
                    float tent_costSoFar = current.costSoFar + neighboursCosts[n];
                    if (tent_costSoFar < neighboursAct[n].costSoFar){
                        neighboursAct[n].previousVertexID = current.id;
                        neighboursAct[n].costSoFar = tent_costSoFar;
                        neighboursAct[n].hCost = tent_costSoFar + h(neighboursAct[n], end);

                        if (!openList.Contains(neighboursAct[n]))
                            openList.Add(neighboursAct[n]);
                    }
                }
            }
            return new List<Vertex>();
        }

        public List<Vertex> Smooth(List<Vertex> inputPath)
        {
            if (inputPath.Count < 3)
                return inputPath;

            List<Vertex> outputPath = new List<Vertex>();
            outputPath.Add(inputPath[0]); // Agrega el primer vertice

            for (int i = 1; i < inputPath.Count - 1; i++) {
                Vertex v1 = inputPath[i - 1]; // Vertice anterior
                Vertex v2 = inputPath[i];     // Vertice actual
                Vertex v3 = inputPath[i + 1]; // Vertice siguiente

                // Si los vertices no están alineados, agregamos la esquina
                //Debug.Log(AreAligned(v1, v2, v3));
                if (!AreAligned(v1, v2, v3)){
                    outputPath.Add(v2);
                }
            }

            outputPath.Add(inputPath[inputPath.Count - 1]); // Agrega el ultimo vertice
            return outputPath;
        }

        bool AreAligned(Vertex v1, Vertex v2, Vertex v3)
        {
            // Obtener las posiciones de los vertices
            Vector3 p1 = v1.transform.position;
            Vector3 p2 = v2.transform.position;
            Vector3 p3 = v3.transform.position;

            // Calcular la direccion del vector entre p1 y p2, y entre p1 y p3
            Vector3 direction1 = (p2 - p1).normalized;
            Vector3 direction2 = (p3 - p1).normalized;

            // Verificar si las direcciones son aproximadamente iguales
            return Vector3.Dot(direction1, direction2) > 0.999f;
        }

        // Reconstruir el camino, dando la vuelta a la lista de nodos 'padres' /previos que hemos ido anotando
        private List<Vertex> BuildPath(int srcId, int dstId, ref int[] prevList) {
            List<Vertex> path = new List<Vertex>();

            if (dstId < 0 || dstId >= vertices.Count) 
                return path;

            int prev = dstId;
            do {
                path.Add(vertices[prev]);
                prev = prevList[prev];
            } while (prev != srcId);
            return path;
        }

        // Version sin lista de previos
        private List<Vertex> BuildPath(int srcId, int dstId)
        {
            List<Vertex> path = new List<Vertex>();

            if (dstId < 0 || dstId >= vertices.Count)
                return path;

            int prev = dstId;
            do{
                path.Add(vertices[prev]);
                prev = vertices[prev].previousVertexID;
            } while (prev != srcId);
            return path;
        }
    }
}