using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class SnakeMapGenerator : MonoBehaviour
{
    public bool IsMapGenerated { get; private set; }
    public int rows = 10; //numero de files
    public int columns = 10; //num de columnes
    public GameObject[] roomPrefabs; //prefabs de les habitacions
    public Grid grid; //el grid on es generaran les habitacions (el de la room1)

    public int snakeLength = 5; //longitud de la snake
    private GameObject[,] roomMatrix; //matriu per emmagatzemar les habitacions
    private Vector2Int[] directions = new Vector2Int[] //direccions possibles
    {
        new Vector2Int(0, 1), //dreta
        new Vector2Int(0, -1), //esquerra
        new Vector2Int(1, 0),  //amunt
        new Vector2Int(-1, 0)  //avall
    };

    private List<Vector2Int> roomPositions = new List<Vector2Int>(); //llista de posicions de les habitacions
    private List<GameObject> availablePrefabs; //llista de prefabs disponibles



    //Generem la snake de rooms
    public void GenerateSnake()
    {
        IsMapGenerated = false;
        if (roomPrefabs == null || roomPrefabs.Length == 0 || grid == null)
        {
            return;
        }

        //Inicialitzar la matriu de les habitacions
        roomMatrix = new GameObject[rows, columns];

        // Inicializar la lista de prefabs disponibles
        availablePrefabs = new List<GameObject>(roomPrefabs);

        //Obtenim la mida de les cel·les del grid (cada quadricula)
        Vector3 cellSize = grid.cellSize;

        //Obtenim les tilemaps del primer prefab de la habitació
        Tilemap[] tilemaps = roomPrefabs[0].GetComponentsInChildren<Tilemap>();

        if (tilemaps.Length == 0)
        {
            return;
        }

        //Trobem els limits de la cel·la de la habitació (els tilemaps)
        BoundsInt roomBoundsInt = tilemaps[0].cellBounds;

        //Calculem la mida de la habitació en unitats del mon
        float width = roomBoundsInt.size.x * cellSize.x;
        float height = roomBoundsInt.size.y * cellSize.y;

        //Generar la snake ara si
        GenerateSnakeRooms(width, height);
        IsMapGenerated = true;
    }

    //Metode per generar les habitacions de la snake
    void GenerateSnakeRooms(float width, float height)
    {
        //Començar per la posició central
        Vector2Int currentPos = new Vector2Int(rows / 2, columns / 2);
        roomMatrix[currentPos.x, currentPos.y] = PlaceRoom(currentPos, width, height, "Start");

        //Guarda la posició de la primera habitació
        roomPositions.Add(currentPos);

        //Generar les rooms intermitges i la final
        for (int i = 1; i < snakeLength; i++)
        {
            if (availablePrefabs.Count == 0)
            {
                return;
            }

            //Obtenir una posició vàlida per la següent habitació (adjacent)
            Vector2Int nextPos = GetNextValidPosition(currentPos);

            if (nextPos == Vector2Int.zero)
            {
                return;
            }

            //Instanciem la nova habitació
            GameObject newRoom = PlaceRoom(nextPos, width, height, i == snakeLength - 1 ? "End" : $"Room {i}");
            if(i == snakeLength - 1)
            {
                Room roomComponent = newRoom.GetComponent<Room>();
                if (roomComponent != null)
                {
                    roomComponent.isLastRoom = true;
                }
            }
            //Guardem la posició de la nova habitació
            roomPositions.Add(nextPos);

            //Connectem les habitacions basant-nos en les posicions actuals i següents
            ConnectRooms(currentPos, nextPos);

            //Actualitzem la posició actual
            currentPos = nextPos;
        }
    }

    //Posiciona una room en la posició de la matriu
    GameObject PlaceRoom(Vector2Int gridPosition, float width, float height, string name)
    {
        //Converteix la posicio de la quadricula a posicio del mon
        Vector3 position = new Vector3(gridPosition.y * width, gridPosition.x * height, 0);

        //Selecciona un prefab aleatori dels disponibles
        int randomIndex = Random.Range(0, availablePrefabs.Count);
        GameObject selectedRoom = availablePrefabs[randomIndex];

        //Elimina el prefab seleccionat de la llista de disponibles
        availablePrefabs.RemoveAt(randomIndex);

        //Instancia la room seleccionada a la posicio calculada
        GameObject roomInstance = Instantiate(selectedRoom, position, Quaternion.identity);
        roomInstance.name = name;

        //Obte el component AreaExit de la room instanciada
        AreaExit areaExit = roomInstance.GetComponentInChildren<AreaExit>();
        Room roomComponent = roomInstance.GetComponent<Room>();
        //Assigna el confiner de la room a l'AreaExit de la room
        PolygonCollider2D roomConfiner = roomInstance.GetComponentInChildren<PolygonCollider2D>();
        if (roomComponent != null && roomConfiner != null)
        {
            roomComponent.roomConfiner = roomConfiner; // Asignamos el confiner a la habitación
        }
        if (areaExit != null && roomConfiner != null)
        {
            areaExit.roomConfiner = roomConfiner; // Asignamos el confiner a la habitación
        }

        //Emmagatzema la room a la matriu de les habitacions
        roomMatrix[gridPosition.x, gridPosition.y] = roomInstance;

        return roomInstance;
    }


    //Conectem les habitacions basant-nos en les posicions actuals i següents
    void ConnectRooms(Vector2Int posA, Vector2Int posB)
    {
        GameObject roomA = roomMatrix[posA.x, posA.y];
        GameObject roomB = roomMatrix[posB.x, posB.y];

        Transform doorsA = roomA.transform.Find("Doors");
        Transform doorsB = roomB.transform.Find("Doors");

        if (doorsA == null || doorsB == null)
        {
            return;
        }

        Transform doorA = null, doorB = null;

        //Connectem les portes basant-nos en la posició de les habitacions
        if (posB.x == posA.x && posB.y == posA.y + 1) //Dreta
        {
            doorA = doorsA.Find("RightDoor");
            doorB = doorsB.Find("LeftDoor");
        }
        else if (posB.x == posA.x && posB.y == posA.y - 1) //esquerra
        {
            doorA = doorsA.Find("LeftDoor");
            doorB = doorsB.Find("RightDoor");
        }
        else if (posB.x == posA.x + 1 && posB.y == posA.y) //amunt
        {
            doorA = doorsA.Find("TopDoor");
            doorB = doorsB.Find("BottomDoor");
        }
        else if (posB.x == posA.x - 1 && posB.y == posA.y) //avall
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
                roomComponentA.DoorsToUnlock = updatedDoors.ToArray(); // Afegir la porta a la llista de portes a desbloquejar
            }

            SetupDoorConnection(doorA, doorB);
        }
    }

    //Metode per configurar la connexió entre dues portes
    void SetupDoorConnection(Transform doorA, Transform doorB)
    {
        if (doorA == null || doorB == null)
        {
            return;
        }

        //Busquem els colliders de les habitacions per desactivar-los
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


        //Activem les portes de les habitacions per a la connexió
        doorA.gameObject.SetActive(false);
        doorB.gameObject.SetActive(false);

        //Configuració de l'areaExit i entrance de les portes
        AreaExit areaExitA = doorA.GetComponentInChildren<AreaExit>();
        Transform areaEntranceB = doorB.Find("AreaEntrance");

        if (areaExitA != null && areaEntranceB != null)
        {
            areaExitA.areaEntrance = areaEntranceB;
        }

        //Fem el mateix al reves per la porta B
        AreaExit areaExitB = doorB.GetComponentInChildren<AreaExit>();
        Transform areaEntranceA = doorA.Find("AreaEntrance");

        if (areaExitB != null && areaEntranceA != null)
        {
            areaExitB.areaEntrance = areaEntranceA;
        }

        //Configurem el confiner de les habitacions
        Room roomComponentA = doorA.GetComponentInParent<Room>();
        Room roomComponentB = doorB.GetComponentInParent<Room>();

        if (areaExitA != null && roomComponentB != null)
        {
            areaExitA.areaEntrance = doorB.Find("AreaEntrance");
            areaExitA.roomConfiner = roomComponentB.roomConfiner; //Assignem el confiner de la habitació actual
        }

        if (areaExitB != null && roomComponentA != null)
        {
            areaExitB.areaEntrance = doorA.Find("AreaEntrance");
            areaExitB.roomConfiner = roomComponentA.roomConfiner;
        }
    }

    //Obtenim una posició vàlida per la següent habitació
    Vector2Int GetNextValidPosition(Vector2Int currentPos)
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();

        foreach (var direction in directions) //per cada direccio possible comprovem si la posicio es valida (similar al buscaminas)
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
            return validPositions[Random.Range(0, validPositions.Count)]; //Retorna una posicio valida random
        }

        return Vector2Int.zero; //Si no hi ha posicions valides retorna zero
    }

    public void ResetGenerator()
    {
        IsMapGenerated = false;
        roomMatrix = null;
        roomPositions.Clear();
        availablePrefabs = null;
    }

}
