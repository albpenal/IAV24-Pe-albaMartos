# IAV24-Refugiate

## Autores
- Alberto Peñalba Martos (albpenal)
- Sofia Sánchez Fernández (sosanc03)

## Propuesta
Este proyecto es una práctica de la asignatura de Inteligencia Artificial para Videojuegos del Grado en Desarrollo de Videojuegos de la UCM. La práctica consiste en desarrollar un prototipo de inteligencia artificial para videojuegos. Ambientado en un entorno de batalla, el juego consistirá en un soldado inteligente que intentara evitar o esquivar impactos de proyectiles, que tratarán de golpearlo, anticipándose a sus movimientos. Cabe la posibilidad de manejar al soldado por teclado o de que el soldado, a través de su inteligencia artificial, trate de sobrevivir por su cuenta. La implementación se llevará a cabo utilizando árboles de comportamiento complejos (behaviour bricks) y técnicas de búsqueda de caminos ().

## Punto de partida
Se partirá desde cero, utilizando un proyecto de Unity en la versión **Unity 2022.3.18f1**. Se buscarán assets de libre uso para el proyecto.

## Clase `NavigationMesh`
<details>
<summary>Implementación del cálculo de la posición a la que debe moverse el soldado para sobrevivir a los impactos de los proyectiles. Se utilizará el algoritmo A*</summary>

### Propiedades
- `GameObject` vertexPrefab: Prefabricado de los vértices para visualización en el juego.
- `List<Vertex>` vertices: Lista de vértices en la malla.
- `bool[,]` mapVertices: Matriz que indica la conectividad de los vértices (si el vértice es seguro y accesible).
- `float[,]` costsVertices: Matriz de costos de movimiento entre vértices.
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
La clase `NavigationMesh` es esencial para la planificación de rutas y la navegación del soldado. Facilita la interacción dinámica con el entorno, permitiendo moverse eficientemente por el espacio del juego.
***
</details>

## Clase `ProyectilGenerator`
<details>
<summary>Implementación del cálculo de la posición en la que deben caer los proyectiles para tratar de golpear al soldado</summary>

### Propiedades
- `GameObject` player: Acceso al soldado para calcular la posición del lanzamiento del proyectil.
- `float` cooldown: Tiempo entre lanzamientos.
- `int` nProyectiles: número de proyectiles a generar.
- `List<Vector3>` posiciones: Lista de posiciones en las que ya se ha instanciado un proyectil.

### Métodos
#### `public void calculatePosition()`
Cálculo de la posición a la que se lanzará un proyectil sin caer en la misma posición que otro.

#### `public void generateProyectiles()`
Instancia de un número de proyectiles dentro de un rango.

## Resumen
La clase `ProyectilGenerator` será la clase encargada de lanzar proyectiles tratando de golpear al soldado (anticipandose a sus movimientos).
***
</details>

## Diseño de la solución

### Esquema de la interacción del soldado con la malla de navegación

#### Árbol de comportamiento para el soldado:
- **Buscar salida**: El soldado busca el camino más seguro hacia la salida.
- **Evitar bombas**: Cambia de ruta si detecta que será golpeado por el rango de acción de algún proyectil.
- **Usar refugios**: Utilizará los refugios para protegerse del impacto de los proyectiles.

## Pruebas y métricas
El éxito del prototipo se medirá por la capacidad del soldado para llegar a la salidad y la capacidad de los protectiles de predecir el siguiente movimiento del soldado.
- [Vídeo]()

## Producción

Las tareas se han asignado entre los autores. Usaremos la pestaña de Proyectos de GitHub para documentar el progreso.

| Estado  |  Tarea  |  Fecha  |  
|:-:|:--|:-:|
|  | Implementación de la malla de navegación |  |
|  | Desarrollo del árbol de comportamiento del soldado |  |
|  | Desarrollo del ataque con proyectiles |  |
|  | Pruebas de rendimiento |  |

## Referencias

Los recursos y teorías utilizadas son estándares en el desarrollo de videojuegos y robótica.

- *Artificial Intelligence for Games*, Ian Millington.
- Documentación oficial de Unity.
- Recursos de Unity Asset Store para componentes visuales y de navegación.