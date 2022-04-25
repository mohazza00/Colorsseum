using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowsSpawner : MonoBehaviour
{
    public GameObject[] arrows;

    [Header("Quivers")]
    public Transform[] quiverSpots;
    public Transform blueQuiver;
    public Transform redQuiver;
    public Transform yellowQuiver;
    public Transform greenQuiver;

    public float spawningInterval = 2f;

    public List<GameObject> spawnedBlueArrows = new List<GameObject>();
    public List<GameObject> spawnedRedArrows = new List<GameObject>();
    public List<GameObject> spawnedYellowArrows = new List<GameObject>();
    public List<GameObject> spawnedGreenArrows = new List<GameObject>();

    private float spawningTimer;

    public bool spawnForTutorial;


    private void Start()
    {
        ShuffleQuivers();
    }

    private void Update()
    {
        if (GameManager.Instance.gameState == GameState.SPANWING_WAVE)
            return;

        if ((GameManager.Instance.wavesStarted && !GameManager.Instance.marketIsOpen) || spawnForTutorial)
        {     
            if (spawningTimer <= 0f)
            {
                if (spawnedBlueArrows.Count < 3)
                {
                    SpawnColoredArrow(0);
                }
                if (spawnedGreenArrows.Count < 3)
                {
                    SpawnColoredArrow(1);
                }
                if (spawnedRedArrows.Count < 3)
                {
                    SpawnColoredArrow(2);
                }
                if (spawnedYellowArrows.Count < 3)
                {
                    SpawnColoredArrow(3);
                }

                spawningTimer = spawningInterval;
            }
            else
            {
                spawningTimer -= Time.deltaTime;
            }         
        }
    }

    public void SpawnColoredArrow(int index)
    {
        if (index == 0)
        {
            GameObject apple = Instantiate(arrows[0], GetPositionInsideTheQuiver(blueQuiver), transform.rotation);
            spawnedBlueArrows.Add(apple);
            apple.GetComponent<Collectable>().arrowsSpawner = this;
        }
        if (index == 1)
        {
            GameObject apple = Instantiate(arrows[1], GetPositionInsideTheQuiver(greenQuiver), transform.rotation);
            spawnedGreenArrows.Add(apple);
            apple.GetComponent<Collectable>().arrowsSpawner = this;
        }
        if (index == 2)
        {
            GameObject apple = Instantiate(arrows[2], GetPositionInsideTheQuiver(redQuiver), transform.rotation);
            spawnedRedArrows.Add(apple);
            apple.GetComponent<Collectable>().arrowsSpawner = this;
        }
        if (index == 3)
        {
            GameObject apple = Instantiate(arrows[3], GetPositionInsideTheQuiver(yellowQuiver), transform.rotation);
            spawnedYellowArrows.Add(apple);
            apple.GetComponent<Collectable>().arrowsSpawner = this;
        }
    }

    public Vector3 GetPositionInsideTheQuiver(Transform quiver)
    {
        BoxCollider2D box = quiver.GetComponent<BoxCollider2D>();

        return new Vector3(
           Random.Range(quiver.position.x - box.size.x / 2f, quiver.position.x + box.size.x / 2),
           Random.Range(quiver.position.y - box.size.y / 2f, quiver.position.y + box.size.y / 2));

    }

    public void ShuffleQuivers()
    {
        Transform tempSpot;
        List<int> occupiedSpots = new List<int>();

        for (int i = 0; i < quiverSpots.Length - 1; i++)
        {
            int failSafeCounter = 0;
            while (occupiedSpots.Count < i + 1 && failSafeCounter < 100)
            {
                int rand = Random.Range(i, quiverSpots.Length);
                if (!occupiedSpots.Contains(rand))
                {
                    tempSpot = quiverSpots[rand];
                    quiverSpots[rand] = quiverSpots[i];
                    quiverSpots[i] = tempSpot;
                    occupiedSpots.Add(rand);
                }
                failSafeCounter++;
            }                         
        }

        blueQuiver.position = quiverSpots[0].position;
        greenQuiver.position = quiverSpots[1].position;
        redQuiver.position = quiverSpots[2].position;
        yellowQuiver.position = quiverSpots[3].position;
    }

    public void ClearQuivers()
    {
        foreach (GameObject blueArrow in spawnedBlueArrows)
        {
            Destroy(blueArrow);
        }
        spawnedBlueArrows.Clear();

        foreach (GameObject redArrow in spawnedRedArrows)
        {
            Destroy(redArrow);
        }
        spawnedRedArrows.Clear();

        foreach (GameObject greenArrow in spawnedGreenArrows)
        {
            Destroy(greenArrow);
        }
        spawnedGreenArrows.Clear();

        foreach (GameObject yellowArrow in spawnedYellowArrows)
        {
            Destroy(yellowArrow);
        }
        spawnedYellowArrows.Clear();
    }
}
