using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum GameState
{
    SPANWING_WAVE,
    PLAYING,
    SPAWNING_MINIONS_WAVE,
}

public class GameManager : Singleton<GameManager>
{
    public GameState gameState;

    [Header("Colors")]
    public Color blue;
    public Color red;
    public Color green;
    public Color yellow;

    [Header("Waves")]
    public WavesSpawner wavesSpawner;
    public List<GameObject> enemiesOnStage = new List<GameObject>();
    public List<GameObject> bossMinionsOnStage = new List<GameObject>();
    public bool startNextWave;
    public bool waveIsCompleted;
    public bool wavesStarted;

    [Header("Boss")]
    public GameObject bossHealthBar;
    public bool minionsWaveIsCompleted;
    public bool minionsWavesStarted;
    public bool bossDefeated;
    public bool bossEntered;

    [Header("Camera")]
    public bool zoomCamera;
    public bool stopCameraZoom = true;

    [Header("Player")]
    public Player player;

    [Header("Market")]
    public Market market;
    public bool marketIsOpen = false;

    [Header("UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI waveNumber;
    public TextMeshProUGUI waveCleared;
    public GameObject gameClear;
    public GameObject pauseMenuUI;

    public ArrowsSpawner appleSpawner;

    [Header("Skills")]
    public bool pullArrows;
    public bool freezeEnemies;
    public bool changeColor;
    public UnitColor randomColor;
    public bool infiniteArrows;
    public int infiniteArrowIndex;
    public GameObject forceField;
    public bool activateForceField;

    public bool tutorial;

    public static bool gameIsPaused = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PoolingManager.Instance.PooledObjectsParent = GameObject.FindWithTag("PooledObjectParent");
    }

    public  void Awake()
    {
        wavesSpawner = GetComponent<WavesSpawner>();
        player = FindObjectOfType<Player>();
        appleSpawner = FindObjectOfType<ArrowsSpawner>();
    }

    private void Start()
    {
        //startNextWave = true;
        gameState = GameState.PLAYING;
        if(!tutorial)
        Invoke(nameof(StartGame), 1.5f);
    }

    private void Update()
    {
        if(wavesStarted)
        {
            if (waveIsCompleted)
            {
                
            }
            else
            {
                if (gameState != GameState.SPANWING_WAVE)
                {
                    if (enemiesOnStage.Count == 0)
                    {
                        if(wavesSpawner.nextWave == wavesSpawner.waves.Length - 1)
                        {
                            gameClear.SetActive(true);
                            player.gameOver = true;
                        }
                        else
                        {
                            StartCoroutine(_WaveCleared());
                        }
                    }

                    if (bossDefeated)
                    {
                        gameClear.SetActive(true);
                        player.gameOver = true;
                    }
                    
                }

            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

    }

    public void StartGame()
    {
        wavesSpawner.StartFirstWave();
    }
 

    IEnumerator _WaveCleared()
    {
        waveIsCompleted = true;
        waveCleared.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(2f);
        

        market.gameObject.SetActive(true);
        waveCleared.gameObject.SetActive(false);
        player.transform.position = Vector3.zero + new Vector3(0f, -3f, 0f);

        marketIsOpen = true;
        waveIsCompleted = true;
    }

    public void StartNextWave()
    {
        PoolingManager.Instance.SpawnObj(PoolObjectType.CLOUD, market.transform.position, null);
        wavesSpawner.StartNextWave();
        waveIsCompleted = false;
        marketIsOpen = false;
        market.gameObject.SetActive(false);
        gameState = GameState.SPANWING_WAVE;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        gameIsPaused = false;
        SceneManager.LoadScene(5);

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        wavesSpawner.nextWave = 0;
        player.transform.position = Vector3.zero;
        Invoke(nameof(StartGame), 1.5f);
        enemiesOnStage.Clear();
        wavesStarted = false;
        gameOverPanel.SetActive(false);
    }
}
