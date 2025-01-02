using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private SnakeMapGenerator mapGenerator;
    private InitGame initGame;
    public ScreenFade screenFade;
    public int currentRound = 2;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(InitializeGameWithDelay());
    }

    private IEnumerator InitializeGameWithDelay()
    {
        yield return new WaitForSeconds(0.1f);
        mapGenerator = FindObjectOfType<SnakeMapGenerator>();
        initGame = FindObjectOfType<InitGame>();
        screenFade = FindObjectOfType<ScreenFade>();

        if (mapGenerator == null || initGame == null)
        {
            Debug.LogError("No se ha encontrado el generador de mapas o el script de inicio del juego.");
            yield break;
        }

        Debug.Log("Componentes encontrados correctamente.");
        StartNewGame();
    }

    public void StartNewGame()
    {
        Debug.Log("Comenzando un nuevo juego.");
        currentRound = 1;
        if(initGame != null)
        {
            initGame.SetRound(currentRound);
        }
        InitializeGame();
    }

    public void StartNewRound()
    {
        screenFade.FadeOut();
        Debug.Log("Comenzando una nueva ronda.");
        currentRound++;
        if(initGame != null)
        {
            initGame.SetRound(currentRound);
        }
        InitializeGame();
    }

    public void InitializeGame()
    {
        Debug.Log("Inicializando juego...");
        ClearCurrentState();
        Debug.Log("Estado previo limpiado. Generando mapa...");
        mapGenerator.GenerateSnake(); // Agregar Debug aquí para ver si se llama.
        StartCoroutine(StartGameAfterMapGenerated());
    }

    private IEnumerator StartGameAfterMapGenerated()
    {
        Debug.Log("Esperando a que se genere el mapa...");
        yield return new WaitUntil(() => mapGenerator.IsMapGenerated);
        Debug.Log("Mapa generado. Iniciando el juego...");
        initGame.StartGame();
        yield return new WaitForSeconds(2f);
        screenFade.FadeIn();
    }

    private void ClearCurrentState()
    {
        Debug.Log("Limpiando estado previo...");
        // Limpia enemigos
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }

        // Limpia ítems o cualquier otro objeto necesario
        foreach (var room in GameObject.FindGameObjectsWithTag("Room"))
        {
            Room roomComponent = room.GetComponent<Room>();
            if(roomComponent != null)
            {
                roomComponent.EnemySpawnPoints = null;
                roomComponent.DoorsToUnlock = null;
                roomComponent.roomConfiner = null;
                roomComponent.spawnPoint = null;
            }
            Destroy(room);
        }

        Debug.Log("Estado previo limpiado.");
    }
}
