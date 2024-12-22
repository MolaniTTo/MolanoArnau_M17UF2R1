using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private SnakeMapGenerator mapGenerator;
    private InitGame initGame;
    public int currentRound = 1;

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
        InitializeGame();
    }

    public void StartNewRound()
    {
        Debug.Log("Comenzando una nueva ronda.");
        currentRound++;
        InitializeGame();
    }

    public void InitializeGame()
    {
        Debug.Log("Inicializando juego...");
        ClearCurrentState();
        Debug.Log("Estado previo limpiado. Generando mapa...");
        mapGenerator.GenerateSnake(); // Agregar Debug aquí para ver si se llama.
        Debug.Log("Mapa generado. Iniciando el juego...");
        initGame.StartGame(); // También agregar Debug aquí
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
        foreach (var item in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            Destroy(item);
        }

        Debug.Log("Estado previo limpiado.");
    }
}
