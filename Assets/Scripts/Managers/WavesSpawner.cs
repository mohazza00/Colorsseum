using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WaveSpawnerState
{
    OPENNING_DOORS,
    SPAWNING_ENEMIES,
    CLOSING_DOORS,
}

public class WavesSpawner : MonoBehaviour
{
    [Header("Settings")]
    public Gate[] spawningGates = new Gate[4];
    public Wave[] waves;
    public GameObject[] bridges;
    public Wave[] minionsWaves;
    public Transform enemiesParentTransform;
    public float spawningDelay;

    [Header("State Variables")]
    public WaveSpawnerState state;
    public int nextWave = 0;
    public int nextMinionWave = -1;
    private Wave currentWave;
    private Wave currentMinionWave;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    private void Update()
    {
        if(gameManager.gameState == GameState.SPANWING_WAVE)
        {
            if (currentWave == null) return;
            if (gameManager.enemiesOnStage.Count == currentWave.count)
            {
                state = WaveSpawnerState.CLOSING_DOORS;
                StartCoroutine(CloseGatesRoutine());           
            }
        }

        if (gameManager.gameState == GameState.SPAWNING_MINIONS_WAVE)
        {
            if (currentMinionWave == null) return;
            if (gameManager.bossMinionsOnStage.Count == currentMinionWave.count)
            {
                state = WaveSpawnerState.CLOSING_DOORS;
                StartCoroutine(CloseGatesRoutine());
            }
        }
    }

    public void StartFirstWave()
    {
        currentWave = waves[nextWave];
        StartCoroutine(StartNextWaveRoutine());
        gameManager.gameState = GameState.SPANWING_WAVE;
        gameManager.wavesStarted = true;
        ClearStartingPoints();
    }

    public void StartNextWave()
    {
        nextWave += 1;
        currentWave = waves[nextWave];
        StartCoroutine(StartNextWaveRoutine());
        gameManager.gameState = GameState.SPANWING_WAVE;
        gameManager.wavesStarted = true;
        ClearStartingPoints();
    }

    public void StartMinionsWave()
    {
        currentMinionWave = minionsWaves[nextMinionWave];
        StartCoroutine(StartNextMinionWaveRoutine());
        gameManager.gameState = GameState.SPAWNING_MINIONS_WAVE;
        gameManager.minionsWavesStarted = true;
        ClearStartingPoints();
    }

    IEnumerator StartNextWaveRoutine()
    {
        AudioManager.Instance.PlaySound(SoundName.WAVE_START);
        gameManager.waveNumber.gameObject.SetActive(true);
        if (nextWave + 1 == 5)
            gameManager.waveNumber.text = "Final Wave";

        else
            gameManager.waveNumber.text = "Wave " + (nextWave + 1).ToString();

        yield return new WaitForSeconds(2f);
        gameManager.waveNumber.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        gameManager.stopCameraZoom = false;
        AudioManager.Instance.PlaySound(SoundName.GATES_OPEN);
        yield return new WaitForSeconds(0.5f);
        OpenGates();
        state = WaveSpawnerState.OPENNING_DOORS;
        yield return new WaitForSeconds(2f);
        state = WaveSpawnerState.SPAWNING_ENEMIES;
        StartCoroutine(SpawnWaveRoutine());
    }

    IEnumerator StartNextMinionWaveRoutine()
    {
        AudioManager.Instance.PlaySound(SoundName.WAVE_START);
        yield return new WaitForSeconds(2f);
        yield return new WaitForSeconds(0.4f);
        AudioManager.Instance.PlaySound(SoundName.GATES_OPEN);
        yield return new WaitForSeconds(0.5f);
        OpenGates();
        state = WaveSpawnerState.OPENNING_DOORS;
        yield return new WaitForSeconds(2f);
        state = WaveSpawnerState.SPAWNING_ENEMIES;
        StartCoroutine(SpawnMinionsWaveRoutine());
    }

    private void OpenGates()
    {
        foreach (GameObject bridge in bridges)
        {
            bridge.GetComponent<BoxCollider2D>().enabled = false;
            bridge.GetComponent<Bridge>().openTheDoor = true;
        }
    }    

    IEnumerator SpawnWaveRoutine()
    {
        for (int i = 0; i < currentWave.enemies.Length; i++)
        {
            SpawnEnemy(currentWave.enemies[i]);
            yield return new WaitForSeconds(spawningDelay);
        }

        yield break;    
    }

    IEnumerator SpawnMinionsWaveRoutine()
    {
        for (int i = 0; i < currentMinionWave.enemies.Length; i++)
        {
            SpawnEnemy(currentMinionWave.enemies[i]);
            yield return new WaitForSeconds(spawningDelay);
        }

        yield break;
    }

    IEnumerator CloseGatesRoutine()
    {
        AudioManager.Instance.PlaySound(SoundName.GATES_CLOSE);

        yield return new WaitForSeconds(1f);
        gameManager.stopCameraZoom = true;
        yield return new WaitForSeconds(1f);
        gameManager.gameState = GameState.PLAYING;
        yield return new WaitForSeconds(0.4f);

        foreach (GameObject bridge in bridges)
        {
            bridge.GetComponent<BoxCollider2D>().enabled = true;
            bridge.GetComponent<Bridge>().closeTheDoor = true;
        }
        yield return new WaitForSeconds(2f);
        
    }

    private void SpawnEnemy(GameObject enemy)
    {
        Gate randomGate = GetRandomGate();
        GameObject spawnedEnemy = Instantiate(enemy, randomGate.gate.position, randomGate.gate.rotation);
        spawnedEnemy.transform.parent = enemiesParentTransform;      
        spawnedEnemy.GetComponent<Enemy>().InitializeEnemy(GetRandomStartingPoint(randomGate), randomGate.spawningGate);          
    }

    private Gate GetRandomGate()
    {
        int rand = Random.Range(0, spawningGates.Length);
        return spawningGates[rand];
    }

    private Vector3 GetRandomStartingPoint(Gate gate)
    {
        int rand = Random.Range(0, gate.startingPoints.Length);
        if(!gate.startingPoints[rand].isOccupied)
        {
            gate.startingPoints[rand].isOccupied = true;
            return gate.startingPoints[rand].point.position;
        }
        else
        {
            Gate nextGate;
            if (gate.gateNumber < spawningGates.Length)
            {
                nextGate = spawningGates[gate.gateNumber + 1];
            }
            else
            {
                nextGate = spawningGates[0];
            }

            return GetRandomStartingPoint(nextGate);
        }
        
    }


    private void ClearStartingPoints()
    {
        foreach(Gate gate in spawningGates)
        {
            gate.ResetGate();
        }
    }
}


[System.Serializable]
public class Gate
{
    public int gateNumber;
    public SpawningGate spawningGate;
    public Transform gate;
    public StartingPoint[] startingPoints;

    public void ResetGate()
    {
        foreach (StartingPoint point in startingPoints)
        {
            point.isOccupied = false;
        }
    }
}

[System.Serializable]
public class Wave
{
    public int index;
    public int count;
    public GameObject[] enemies;
}

[System.Serializable]
public class StartingPoint
{
    public Transform point;
    public bool isOccupied;
}


