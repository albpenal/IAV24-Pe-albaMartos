/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UCM.IAV.Movimiento;
using UnityEditor;
using UnityEngine.UIElements;

namespace UCM.IAV.Navegacion
{
    public class GraphGrid : Graph
    {
        const int MAX_TRIES = 1000;

        [SerializeField]
        private GameObject player;

        [SerializeField]
        private GameObject refuge;

        [SerializeField]
        private string mapsDir = "Maps"; // Directorio por defecto
        private string mapName = "Map1.map"; // Fichero por defecto
        public bool get8Vicinity = false;
        public float cellSize = 1f;
        [Range(0, Mathf.Infinity)]
        public float defaultCost = 1f;
        [Range(0, Mathf.Infinity)]
        public float maximumCost = Mathf.Infinity;
        private Vector2 salida;
        private bool exitOriginalPos;

        GameObject[] vertexObjs;

        private void Awake()
        {
            mapName = GameManager.instance.getName() + ".map";
            exitOriginalPos = true;
        }

        private int GridToId(int x, int y)
        {
            return Math.Max(numRows, numCols) * y + x;
        }

        public Vector2 IdToGrid(int id)
        {
            Vector2 location = Vector2.zero;
            location.y = Mathf.Floor(id / numCols);
            location.x = Mathf.Floor(id % numCols);
            return location;
        }

        private void LoadMap(string filename)
        {
            string path;

            path = Application.dataPath + "/" + mapsDir + "/" + filename;

            try
            {
                StreamReader strmRdr = new StreamReader(path);
                using (strmRdr)
                {
                    int j = 0, i = 0, id = 0;
                    string line;

                    Vector3 position = Vector3.zero;
                    Vector3 scale = Vector3.zero;

                    line = strmRdr.ReadLine(); // non-important line
                    line = strmRdr.ReadLine(); // height
                    numRows = int.Parse(line.Split(' ')[1]);
                    line = strmRdr.ReadLine(); // width
                    numCols = int.Parse(line.Split(' ')[1]);
                    line = strmRdr.ReadLine(); // "map" line in file

                    vertices = new List<Vertex>(numRows * numCols);
                    neighbourVertex = new List<List<Vertex>>(numRows * numCols);
                    vertexObjs = new GameObject[numRows * numCols];
                    mapVertices = new bool[numRows, numCols];
                    costsVertices = new float[numRows, numCols];

                    // Leer mapa
                    for (i = 0; i < numRows; i++)
                    {
                        line = strmRdr.ReadLine();
                        for (j = 0; j < numCols; j++)
                        {
                            bool isGround = true;
                            if (line[j] == 'e')
                            {
                                GameManager.instance.SetExit(j, i, cellSize);
                                salida = new Vector2(i, j);
                            }
                            else if (line[j] == 's')
                                GameManager.instance.SetStart(j, i, cellSize);
                            else if (line[j] == 'T')
                                isGround = false;
                            mapVertices[i, j] = isGround;
                        }
                    }

                    //Generamos terreno
                    for (i = 0; i < numRows; i++)
                    {
                        for (j = 0; j < numCols; j++)
                        {
                            position.x = j * cellSize;
                            position.z = i * cellSize;
                            id = GridToId(j, i);

                            if (mapVertices[i, j])
                                vertexObjs[id] = Instantiate(vertexPrefab, position, Quaternion.identity, this.gameObject.transform) as GameObject;
                            else
                                vertexObjs[id] = WallInstantiate(position, i, j);

                            vertexObjs[id].name = vertexObjs[id].name.Replace("(Clone)", id.ToString());
                            Vertex v = vertexObjs[id].AddComponent<Vertex>();
                            v.id = id;
                            vertices.Add(v);
                            neighbourVertex.Add(new List<Vertex>());

                            vertexObjs[id].transform.localScale *= cellSize;
                        }
                    }

                    // Leemos vecinos
                    for (i = 0; i < numRows; i++)
                        for (j = 0; j < numCols; j++)
                            SetNeighbours(j, i);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public override void Load()
        {
            if(mapName != "MapGeneration.map") LoadMap(mapName);
            else GenerateMap(mapName, 22, 15, 15); // Generata un mapa aleatorio de tamaño entre 10x10 y 15x15
        }

        protected void SetNeighbours(int x, int y, bool get8 = false)
        {
            int col = x;
            int row = y;

            int i, j;
            int vertexId = GridToId(x, y);
            neighbourVertex[vertexId] = new List<Vertex>();
            Vector2[] pos = new Vector2[0];
            if (get8)
            {
                pos = new Vector2[8];
                int c = 0;
                for (i = row - 1; i <= row + 1; i++)
                {
                    for (j = col - 1; j <= col; j++)
                    {
                        pos[c] = new Vector2(j, i);
                        c++;
                    }
                }
            }
            else
            {
                pos = new Vector2[4];
                pos[0] = new Vector2(col, row - 1);
                pos[1] = new Vector2(col - 1, row);
                pos[2] = new Vector2(col + 1, row);
                pos[3] = new Vector2(col, row + 1);
            }

            foreach (Vector2 p in pos)
            {
                i = (int)p.y;
                j = (int)p.x;

                if (i < 0 || j < 0 ||
                    i >= numRows || j >= numCols ||
                    i == row && j == col ||
                    !mapVertices[i, j])
                    continue;

                int id = GridToId(j, i);
                neighbourVertex[vertexId].Add(vertices[id]);
                costsVertices[i, j] = defaultCost;
            }
        }

        public override Vertex GetNearestVertex(Vector3 position)
        {
            int col = (int)Math.Round(position.x / cellSize);
            int row = (int)Math.Round(position.z / cellSize);
            Vector2 p = new Vector2(col, row);
            List<Vector2> explored = new List<Vector2>();
            Queue<Vector2> queue = new Queue<Vector2>();
            queue.Enqueue(p);
            do
            {
                p = queue.Dequeue();
                col = (int)p.x;
                row = (int)p.y;
                int id = GridToId(col, row);
                if (mapVertices[row, col])
                    return vertices[id];

                if (!explored.Contains(p))
                {
                    explored.Add(p);
                    int i, j;
                    for (i = row - 1; i <= row + 1; i++)
                    {
                        for (j = col - 1; j <= col + 1; j++)
                        {
                            if (i < 0 || j < 0 ||
                                j >= numCols || i >= numRows ||
                                i == row && j == col)
                                continue;
                            queue.Enqueue(new Vector2(j, i));
                        }
                    }
                }
            } while (queue.Count != 0);
            return null;
        }

        public override GameObject GetRandomPos()
        {
            GameObject pos = null;
            int tries = 0;

            int i, j;
            do
            {
                i = UnityEngine.Random.Range(0, numRows);
                j = UnityEngine.Random.Range(0, numCols);
                tries++;
            } while (tries < MAX_TRIES && !mapVertices[i, j]);

            pos = vertexObjs[GridToId(j, i)];

            return pos;
        }

        public override void UpdateVertexCost(Vector3 position, float costMultiplier)
        {
            Vertex v = GetNearestVertex(position);
            if (v != null)
            {
                Vector2 gridPos = IdToGrid(v.id);

                int x = (int)gridPos.y;
                int y = (int)gridPos.x;

                if (x > 0 && x < numRows - 1 && y > 0 && y < numCols - 1)
                    costsVertices[x, y] += costMultiplier;
            }
        }

        public int getVertexCost(Vector3 position)
        {
            Vertex v = GetNearestVertex(position);

            Vector2 gridPos = IdToGrid(v.id);

            int x = (int)gridPos.y;
            int y = (int)gridPos.x;
            return (int)costsVertices[x, y];
        }

        private GameObject WallInstantiate(Vector3 position, int i, int j)
        {
            //Suelo base e independiente
            GameObject floor = Instantiate(vertexPrefab, position, Quaternion.identity, this.gameObject.transform) as GameObject;
            floor.transform.localScale *= cellSize;
            floor.name = floor.name.Replace("(Clone)", GridToId(j, i).ToString());
            return Instantiate(refuge, position, Quaternion.identity, this.gameObject.transform) as GameObject;
        }

        public bool isGround(Vector2 locationCell)
        {
            return (mapVertices[(int)locationCell.y, (int)locationCell.x]);
        }

        public bool salidaSave()
        {
            return costsVertices[(int)salida.x, (int)salida.y] <= 1;
        }

        public void resetExit()
        {
            if (!exitOriginalPos)
            {
                Vector3 pos = new Vector3(salida.x, 0, salida.y);
                GameManager.instance.setExit(pos);
                exitOriginalPos = true;
            }
        }

        public void setExit()
        {
            if (exitOriginalPos)
            {
                GameObject gameObj;
                do
                {
                    gameObj = GetRandomPos();
                } while (costsVertices[(int)IdToGrid(GetNearestVertex(gameObj.transform.position).id).x,
                (int)IdToGrid(GetNearestVertex(gameObj.transform.position).id).y] != 1);

                GameManager.instance.setExit(gameObj.transform.position);
                exitOriginalPos = false;
            }
            if (costsVertices[(int)IdToGrid(GetNearestVertex(player.transform.position).id).x,
                (int)IdToGrid(GetNearestVertex(player.transform.position).id).y] == 1)
            {
                GameManager.instance.setExit(player.transform.position);
                if (salidaSave()) exitOriginalPos = false;
            }
        }
        private void GenerateMap(string filename, int maxWidth, int minWidth, int minHeight)
        {
            int j = 0, i = 0, id = 0;
            Vector3 position = Vector3.zero;

            int width = UnityEngine.Random.Range(minWidth, maxWidth + 1);
            int height = UnityEngine.Random.Range(minHeight, width + 1);
            numRows = height;
            numCols = width;

            vertices = new List<Vertex>(numRows * numCols);
            neighbourVertex = new List<List<Vertex>>(numRows * numCols);
            vertexObjs = new GameObject[numRows * numCols];
            //delete(mapVertices);
            mapVertices = new bool[numRows, numCols];
            for (i = 0; i < numRows; i++) { for (j = 0; j < numCols; j++) { mapVertices[i, j] = true; } }
            costsVertices = new float[numRows, numCols];
            GameManager.instance.SetStart(numCols - 2, 1, cellSize);
            GameManager.instance.SetExit(1, numRows - 2, cellSize);

            for (i = 0; i < numRows; i++)
            {
                for (j = 0; j < numCols; j++)
                {
                    if (i == 0 || i == numRows - 1 || j == 0 || j == numCols - 1)
                    {
                        mapVertices[i, j] = false; // Bordes del mapa
                    }
                    else if(!((i == 1 && j == numCols - 2) || (j == 1 && i == numRows - 2))) // ni entrada ni salida
                    {
                        float perlinValue = Mathf.PerlinNoise((float)i / numRows * 5.0f, (float)j / numCols * 5.0f);
                        if (perlinValue < 0.3f && haySalida(i, j))
                        {
                            mapVertices[i, j] = false; // Obstáculo
                        }
                    }
                }
            }

            //Generamos terreno
            for (i = 0; i < numRows; i++)
            {
                for (j = 0; j < numCols; j++)
                {
                    position.x = j * cellSize;
                    position.z = i * cellSize;
                    id = GridToId(j, i);

                    if (mapVertices[i, j])
                        vertexObjs[id] = Instantiate(vertexPrefab, position, Quaternion.identity, this.gameObject.transform) as GameObject;
                    else
                        vertexObjs[id] = WallInstantiate(position, i, j);

                    vertexObjs[id].name = vertexObjs[id].name.Replace("(Clone)", id.ToString());
                    Vertex v = vertexObjs[id].AddComponent<Vertex>();
                    v.id = id;
                    vertices.Add(v);
                    neighbourVertex.Add(new List<Vertex>());

                    vertexObjs[id].transform.localScale *= cellSize;
                }
            }

            // Leemos vecinos
            for (i = 0; i < numRows; i++)
                for (j = 0; j < numCols; j++)
                    SetNeighbours(j, i);

            Debug.Log("Map generated and placed in mapVertices.");
        }


        private bool haySalida(int obstX, int obstY)
        {
            if (obstY < 0 || obstY >= numCols || obstX < 0 || obstX >= numRows)
            {
                Debug.LogError($"Indices fuera de los límites: obstX={obstX}, obstY={obstY}");
                return false;
            }

            bool originalState = mapVertices[obstX, obstY];
            mapVertices[obstX, obstY] = false; // Añadir obstáculo temporalmente

            // Crear una cola para BFS
            Queue<Vector2> queue = new Queue<Vector2>();
            HashSet<Vector2> visited = new HashSet<Vector2>();

            // Encontrar la posición de la entrada y la salida
            Vector2 entrada = new Vector2(numRows - 2, 1);
            Vector2 salida = new Vector2(1, numCols -2);

            // Inicializar BFS desde la entrada
            queue.Enqueue(entrada);
            visited.Add(entrada);

            // Direcciones para moverse en 4 direcciones (arriba, derecha, abajo, izquierda)
            Vector2[] directions = new Vector2[]
            {
                new Vector2(0, 1),
                new Vector2(1, 0),
                new Vector2(0, -1),
                new Vector2(-1, 0)
            };

            while (queue.Count > 0)
            {
                Vector2 current = queue.Dequeue();
                if (current == salida)
                {
                    mapVertices[obstX, obstY] = originalState;
                    //Debug.Log("saalida");
                    return true; // Hay camino a la salida
                }

                foreach (Vector2 dir in directions)
                {
                    Vector2 neigh = current + dir;
                    if (neigh.x >= 0 && neigh.x < numRows && neigh.y >= 0 && neigh.y < numCols &&
                        mapVertices[(int)neigh.x, (int)neigh.y] && !visited.Contains(neigh))
                    {
                        queue.Enqueue(neigh);
                        visited.Add(neigh);
                    }
                }
            }

            mapVertices[obstX, obstY] = originalState;
            return false; // No hay camino a la salida
        }

    }
}
