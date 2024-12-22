using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class SnakeMapGenerator : MonoBehaviour
{
    public int rows = 10; // Número de filas
    public int columns = 10; // Número de columnas
    public GameObject[] roomPrefabs; // Lista de prefabs de habitaciones (Room1, Room2, etc.)
    public Grid grid; // El Grid del prefab Room1

    public int snakeLength = 5; // Longitud de la serpiente
    private GameObject[,] roomMatrix; // Matriz para almacenar referencias de habitaciones
    private Vector2Int[] directions = new Vector2Int[] // Direcciones posibles
    {
        new Vector2Int(0, 1),  // Derecha
        new Vector2Int(0, -1), // Izquierda
        new Vector2Int(1, 0),  // Arriba
        new Vector2Int(-1, 0)  // Abajo
    };

    private List<Vector2Int> roomPositions = new List<Vector2Int>(); // Lista para almacenar las posiciones de las habitaciones
    private List<GameObject> availablePrefabs; // Lista para manejar los prefabs disponibles

    private void Start()
    {
    }

    // Generar la serpiente de habitaciones
    public void GenerateSnake()
    {
        Debug.Log("Generando serpiente de habitaciones...");
        if (roomPrefabs == null || roomPrefabs.Length == 0 || grid == null)
        {
            Debug.LogError("No se han asignado prefabs de habitación o Grid no asignado.");
            return;
        }

        // Inicializar la matriz para almacenar las habitaciones
        roomMatrix = new GameObject[rows, columns];

        // Inicializar la lista de prefabs disponibles
        availablePrefabs = new List<GameObject>(roomPrefabs);

        // Obtener el tamaño de la celda de la cuadrícula (cellSize)
        Vector3 cellSize = grid.cellSize;

        // Obtener las dimensiones del Tilemaps dentro del primer prefab de habitación
        Tilemap[] tilemaps = roomPrefabs[0].GetComponentsInChildren<Tilemap>();

        if (tilemaps.Length == 0)
        {
            Debug.LogError("No se encontraron Tilemaps en el primer prefab de habitación.");
            return;
        }

        // Encontrar las dimensiones del Tilemap
        BoundsInt roomBoundsInt = tilemaps[0].cellBounds;

        // Calcular el tamaño de la habitación en el mundo
        float width = roomBoundsInt.size.x * cellSize.x;
        float height = roomBoundsInt.size.y * cellSize.y;

        Debug.Log($"Tamaño de la habitación: {width}x{height} (en unidades del mundo)");

        // Generar la serpiente de habitaciones
        GenerateSnakeRooms(width, height);
    }

    // Método para generar las habitaciones de la serpiente
    void GenerateSnakeRooms(float width, float height)
    {
        // Empezar en una posición inicial
        Vector2Int currentPos = new Vector2Int(rows / 2, columns / 2);
        roomMatrix[currentPos.x, currentPos.y] = PlaceRoom(currentPos, width, height, "Start");

        // Guardar la posición de la primera habitación
        roomPositions.Add(currentPos);

        // Generar las habitaciones intermedias y la última
        for (int i = 1; i < snakeLength; i++)
        {
            if (availablePrefabs.Count == 0)
            {
                Debug.LogWarning("No quedan más prefabs disponibles para generar habitaciones.");
                return;
            }

            // Obtener una posición adyacente válida
            Vector2Int nextPos = GetNextValidPosition(currentPos);

            if (nextPos == Vector2Int.zero)
            {
                Debug.LogError("No se encontraron posiciones adyacentes válidas. La serpiente no puede continuar.");
                return;
            }

            // Instanciar la habitación
            GameObject newRoom = PlaceRoom(nextPos, width, height, i == snakeLength - 1 ? "End" : $"Room {i}");

            // Guardar la posición de la nueva habitación
            roomPositions.Add(nextPos);

            // Conectar puertas basándonos en las posiciones de las habitaciones
            ConnectRooms(currentPos, nextPos);

            // Actualizar la posición actual
            currentPos = nextPos;
        }
    }

    // Coloca una habitación en la posición especificada
    GameObject PlaceRoom(Vector2Int gridPosition, float width, float height, string name)
    {
        // Convertir la posición de la cuadrícula en coordenadas del mundo
        Vector3 position = new Vector3(gridPosition.y * width, gridPosition.x * height, 0);

        // Seleccionar un prefab aleatorio de los disponibles
        int randomIndex = Random.Range(0, availablePrefabs.Count);
        GameObject selectedRoom = availablePrefabs[randomIndex];

        // Eliminar el prefab de la lista de disponibles
        availablePrefabs.RemoveAt(randomIndex);

        // Instanciar la habitación
        GameObject roomInstance = Instantiate(selectedRoom, position, Quaternion.identity);
        roomInstance.name = name;

        // Obtener el componente AreaExit de la habitación generada
        AreaExit areaExit = roomInstance.GetComponentInChildren<AreaExit>();
        Room roomComponent = roomInstance.GetComponent<Room>();
        // Asignar el roomConfiner (PolygonCollider2D) de la habitación generada
        PolygonCollider2D roomConfiner = roomInstance.GetComponentInChildren<PolygonCollider2D>();
        if (roomComponent != null && roomConfiner != null)
        {
            roomComponent.roomConfiner = roomConfiner; // Asignamos el confiner a la habitación
        }
        if (areaExit != null && roomConfiner != null)
        {
            areaExit.roomConfiner = roomConfiner; // Asignamos el confiner a la habitación
        }

        // Almacenar la habitación en la matriz
        roomMatrix[gridPosition.x, gridPosition.y] = roomInstance;

        return roomInstance;
    }


    // Conecta las habitaciones en dos posiciones de la matriz
    void ConnectRooms(Vector2Int posA, Vector2Int posB)
    {
        GameObject roomA = roomMatrix[posA.x, posA.y];
        GameObject roomB = roomMatrix[posB.x, posB.y];

        Transform doorsA = roomA.transform.Find("Doors");
        Transform doorsB = roomB.transform.Find("Doors");

        if (doorsA == null || doorsB == null)
        {
            Debug.LogError("No se encontró el objeto 'Doors' en alguna habitación.");
            return;
        }

        Transform doorA = null, doorB = null;

        // Conectar puertas y configurar áreas
        if (posB.x == posA.x && posB.y == posA.y + 1) // Derecha
        {
            doorA = doorsA.Find("RightDoor");
            doorB = doorsB.Find("LeftDoor");
        }
        else if (posB.x == posA.x && posB.y == posA.y - 1) // Izquierda
        {
            doorA = doorsA.Find("LeftDoor");
            doorB = doorsB.Find("RightDoor");
        }
        else if (posB.x == posA.x + 1 && posB.y == posA.y) // Arriba
        {
            doorA = doorsA.Find("TopDoor");
            doorB = doorsB.Find("BottomDoor");
        }
        else if (posB.x == posA.x - 1 && posB.y == posA.y) // Abajo
        {
            doorA = doorsA.Find("BottomDoor");
            doorB = doorsB.Find("TopDoor");
        }

        if(doorA != null && doorB != null)
        {
            Room roomComponentA = roomA.GetComponent<Room>();
            if (roomComponentA != null)
            {
                List<Transform> updatedDoors = new List<Transform>(roomComponentA.DoorsToUnlock);
                updatedDoors.Add(doorA);
                roomComponentA.DoorsToUnlock = updatedDoors.ToArray();
            }

            SetupDoorConnection(doorA, doorB);
        }
    }

    // Método para configurar las conexiones entre puertas y áreas
    void SetupDoorConnection(Transform doorA, Transform doorB)
    {
        if (doorA == null || doorB == null)
        {
            Debug.LogError("No se encontró alguna de las puertas al intentar configurar conexiones.");
            return;
        }

        // Buscar los colliders correspondientes dentro del objeto 'Colliders' de la habitación
        Transform collidersA = doorA.GetComponentInParent<Room>().transform.Find("colliders");
        Transform collidersB = doorB.GetComponentInParent<Room>().transform.Find("colliders");

        if (collidersA != null && collidersA.Find(doorA.name) != null)
        {
            collidersA.Find(doorA.name).gameObject.SetActive(true);
        }

        if (collidersB != null && collidersB.Find(doorB.name) != null)
        {
            collidersB.Find(doorB.name).gameObject.SetActive(true);
        }


        // Activar las puertas (no desactivamos las puertas, solo los colliders)
        doorA.gameObject.SetActive(false);
        doorB.gameObject.SetActive(false);

        // Configurar AreaExit y AreaEntrance
        AreaExit areaExitA = doorA.GetComponentInChildren<AreaExit>();
        Transform areaEntranceB = doorB.Find("AreaEntrance");

        if (areaExitA != null && areaEntranceB != null)
        {
            areaExitA.areaEntrance = areaEntranceB;
        }

        // Hacer lo mismo al revés (de B a A)
        AreaExit areaExitB = doorB.GetComponentInChildren<AreaExit>();
        Transform areaEntranceA = doorA.Find("AreaEntrance");

        if (areaExitB != null && areaEntranceA != null)
        {
            areaExitB.areaEntrance = areaEntranceA;
        }

        // Configuración de confiners de las habitaciones (por si es necesario)
        Room roomComponentA = doorA.GetComponentInParent<Room>();
        Room roomComponentB = doorB.GetComponentInParent<Room>();

        if (areaExitA != null && roomComponentB != null)
        {
            areaExitA.areaEntrance = doorB.Find("AreaEntrance");
            areaExitA.roomConfiner = roomComponentB.roomConfiner; // Asignar confiner de la siguiente habitación
        }

        if (areaExitB != null && roomComponentA != null)
        {
            areaExitB.areaEntrance = doorA.Find("AreaEntrance");
            areaExitB.roomConfiner = roomComponentA.roomConfiner; // Asignar confiner de la habitación actual
        }
    }

    // Obtiene una posición adyacente válida
    Vector2Int GetNextValidPosition(Vector2Int currentPos)
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();

        foreach (var direction in directions)
        {
            Vector2Int newPosition = currentPos + direction;

            if (newPosition.x >= 0 && newPosition.x < rows &&
                newPosition.y >= 0 && newPosition.y < columns &&
                roomMatrix[newPosition.x, newPosition.y] == null)
            {
                validPositions.Add(newPosition);
            }
        }

        if (validPositions.Count > 0)
        {
            return validPositions[Random.Range(0, validPositions.Count)];
        }

        return Vector2Int.zero; // Sin posiciones válidas
    }

}
