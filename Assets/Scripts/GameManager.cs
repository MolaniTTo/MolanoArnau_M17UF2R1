using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private SnakeMapGenerator mapGenerator;
    private InitGame initGame;
    public ScreenFade screenFade;
    private EnemyCounter enemyCounter;
    public int currentRound = 1;
    public List<WeaponSO> weapons;

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

    public IEnumerator InitializeGameWithDelay()
    {
        yield return new WaitForSeconds(0.1f);
        mapGenerator = FindObjectOfType<SnakeMapGenerator>();
        initGame = FindObjectOfType<InitGame>();
        screenFade = FindObjectOfType<ScreenFade>();
        enemyCounter = FindObjectOfType<EnemyCounter>();

        if (mapGenerator == null || initGame == null)
        {
            yield break;
        }

        StartCoroutine(StartNewGame()); //comença un nou joc
    }

    public IEnumerator StartNewGame() //iniciem el joc
    {
        yield return new WaitForSeconds(2f);
        currentRound = 1;
        if(initGame != null)
        {
            initGame.SetRound(currentRound);
        }
        InitializeGame();
    }

    public IEnumerator StartNewRound()
    {
        screenFade.FadeOut();
        yield return new WaitForSeconds(2f);
        currentRound++;
        EnemyCounter enemyCounter = FindObjectOfType<EnemyCounter>();
       
        if (initGame != null)
        {
            initGame.SetRound(currentRound);
        }

        InitializeGame();
        if (enemyCounter != null)
        {
            enemyCounter.StartNewRound(currentRound);
        }
    }

    public void InitializeGame()
    {
        ClearCurrentState();
        mapGenerator.GenerateSnake(); 
        StartCoroutine(StartGameAfterMapGenerated());
    }

    private IEnumerator StartGameAfterMapGenerated()
    {
        yield return new WaitUntil(() => mapGenerator.IsMapGenerated);
        initGame.StartGame();
        yield return new WaitForSeconds(1.5f);
        screenFade.FadeIn();
        yield return new WaitForSeconds(3.5f);
        initGame.FlickerAndActive();

    }

    private void ClearCurrentState()
    {
        //neteja enemics i habitacions
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

        SnakeMapGenerator mapGenerator = FindObjectOfType<SnakeMapGenerator>();
        mapGenerator.ResetGenerator();

    }

    public void ClearGame() //reinicio els objectes i restauro els valors per defecte
    {
        foreach (var obj in FindObjectsOfType<GameObject>())
        {
            Destroy(obj);
        }
        foreach (var obj in FindObjectsOfType<DamageSource>())
        {
            //si es la espada
            if (obj.gameObject.name == "WeaponCollider")
            {
                obj.GetComponent<DamageSource>().Resetdamage(1);
            }
            else if (obj.gameObject.name == "FireCollider")
            {
                obj.GetComponent<DamageSource>().Resetdamage(0.1f);
            }
            else if (obj.gameObject.name == "Arrow")
            {
                obj.GetComponent<DamageSource>().Resetdamage(1.5f);
            }
        }
        foreach (WeaponSO weapon in weapons)
        {
            weapon.weaponCooldown = weapon.defaultCooldown;
        }
    }
   
}
