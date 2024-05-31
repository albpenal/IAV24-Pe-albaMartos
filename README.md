# IAV24-Refúgiate

## Autores
- Alberto Peñalba Martos (albpenal)
- Sofia Sánchez Fernández (sosanc03)

## Propuesta
Este proyecto es una práctica de la asignatura de Inteligencia Artificial para Videojuegos del Grado en Desarrollo de Videojuegos de la UCM. La práctica consiste en desarrollar un prototipo de inteligencia artificial para videojuegos. Ambientado en un entorno de batalla, el juego consistirá en un soldado inteligente que intentara evitar o esquivar impactos de proyectiles, que tratarán de golpearlo, anticipándose a sus movimientos. Cabe la posibilidad de manejar al soldado por teclado o de que el soldado, a través de su inteligencia artificial, trate de sobrevivir por su cuenta. La implementación se llevará a cabo utilizando árboles de comportamiento complejos (behaviour bricks) y técnicas de búsqueda de caminos (algoritmo A*).

## Punto de partida
Se partirá desde cero, utilizando un proyecto de Unity en la versión **Unity 2022.3.18f1**. Se buscarán assets de libre uso para el proyecto.

## Clase `Teseo`
<details>
<summary>Implementación del cálculo de la posición a la que debe moverse el soldado para sobrevivir a los impactos de los proyectiles. Se utilizará el algoritmo A*</summary>

### Propiedades
- `GameObject` vertexPrefab: Prefabricado de los vértices para visualización en el juego.
- `List<Vertex>` vertex: Lista de vértices en la malla.
- `bool[,]` mapVertex: Matriz que indica la conectividad de los vértices (si el vértice es seguro y accesible).
- `float[,]` costsVertex: Matriz de costos de movimiento entre vértices.
- `int` numCols, `numRows`: Número de columnas y filas en la malla.

### Métodos
#### `public void Initialize()`
Inicializa la malla de navegación con los vértices y costos predeterminados.

#### `public Vertex GetNearestVertex(Vector3 position)`
Obtiene el vértice más cercano a una posición dada en la malla.

#### `public Vertex[] GetNeighbours(Vertex vertex)`
Devuelve los vecinos de un vértice dado.

#### `public float[] GetNeighboursCosts(Vertex vertex)`
Devuelve los costos para llegar a cada vecino de un vértice dado.

#### `public List<Vertex> FindPath(Vertex start, Vertex end)`
Utiliza el algoritmo de A* para encontrar el camino más corto entre dos vértices.

#### `public void UpdateVertexCost(Vector3 position, float costMultiplier)`
Actualiza el costo de movimiento a través de un vértice específico, basado en interacciones dinámicas como puertas cerradas o guardias patrullando.

## Resumen
La clase `Teseo` es esencial para la planificación de rutas y la navegación del soldado. Facilita la interacción dinámica con el entorno, permitiendo moverse eficientemente por el espacio del juego.
***
</details>

## Clase `ProjectileSpawner`
<details>
<summary>Implementación del cálculo de la posición en la que deben caer los proyectiles para tratar de golpear al soldado</summary>

### Propiedades
- `GameObject` player: Acceso al soldado para calcular la posición del lanzamiento del proyectil.
- `GameObject` projectile: prefab del proyectil.
- `float` cooldown: Tiempo entre lanzamientos.
- `int` nProjectiles: número de proyectiles a generar.

### Métodos
#### `public void AdjustToGridCenter()`
Centra la posición del proyectil al centro de un tile del grid.

#### `public void SpawnProjectiles()`
Cálculo de la posición a la que se lanzará un proyectil sin caer en la misma posición que otro. Instancia de un número de proyectiles dentro de un rango.

## Resumen
La clase `ProjectileSpawner` será la clase encargada de lanzar proyectiles tratando de golpear al soldado (anticipandose a sus movimientos).
***
</details>

## Diseño de la solución

### Esquema de la interacción del soldado con la malla de navegación

#### Árbol de comportamiento para el soldado:
- **Buscar salida**: El soldado busca el camino más seguro hacia la salida.
- **Evitar bombas**: Cambia de ruta si detecta que será golpeado por el rango de acción de algún proyectil.
- **Usar refugios**: Utilizará los refugios para protegerse del impacto de los proyectiles.

## Pruebas y métricas
El éxito del prototipo se medirá por la capacidad del soldado para llegar a la salida y la capacidad de los protectiles de predecir el siguiente movimiento del soldado, además de la correcta generación del mapa. 
- [Vídeo]()

## Producción

Las tareas se han asignado entre los autores. Usaremos la pestaña de Proyectos de GitHub para documentar el progreso.

| Autor | Estado | Tarea | Fecha |
|:--:|:--:|:--:|:--:|
| Alberto Peñalba | ✔️ | Instanciación de proyectiles | 24-05-2024 |
| Sofía Sánchez | ✔️ | Impacto del proyectil | 25-05-2024 |
| Alberto Peñalba | ✔️ | Pathing del jugador | 26-05-2024 |
| Sofía Sánchez | ✔️ | Generación del mapa por perlin | 31-05-2024 |
| Ambos | ✔️ | Búsqueda y creación de assets | 22-05-2024/31-05-2024 |

## Aportaciones individuales

**Alberto Peñalba Martos**:
- Creación de los proyectiles en base a la posición del jugador.
- Behaviour Bricks del jugador.
- Pathing del jugador.
- Búsqueda e implementación de assets y sonidos.

**Sofía Sánchez Fernández**:
- Creación área de impacto del proyectil en base a los obstáculos cercanos a este.
- Generación por perlin noise del mapa.
- Creación de menús.
- Flujo de juego.

## Referencias

Los recursos y teorías utilizadas son estándares en el desarrollo de videojuegos y robótica.

- *Artificial Intelligence for Games*, Ian Millington.
- Documentación oficial de Unity.
- Recursos de Unity Asset Store, Sketchfab y pixabay para assets.
