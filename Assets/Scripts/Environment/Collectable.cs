using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CollectableType
{
    BLUE_ARROW,
    RED_ARROW,
    GREEN_ARROW,
    YELLOW_ARROW,
}

public class Collectable : MonoBehaviour
{
    public CollectableType type;
    public ArrowsSpawner arrowsSpawner;
    public float pullingSpeed;

    Vector3 vel;

    private void Update()
    {
        if(GameManager.Instance.marketIsOpen)
        {
            if (type == CollectableType.BLUE_ARROW)
            {
                arrowsSpawner.spawnedBlueArrows.Remove(this.gameObject);
                Destroy(gameObject);
            }
            if (type == CollectableType.RED_ARROW)
            {
                arrowsSpawner.spawnedRedArrows.Remove(this.gameObject);
                Destroy(gameObject);
            }
            if (type == CollectableType.GREEN_ARROW)
            {
                arrowsSpawner.spawnedGreenArrows.Remove(this.gameObject);
                Destroy(gameObject);
            }
            if (type == CollectableType.YELLOW_ARROW)
            {
                arrowsSpawner.spawnedYellowArrows.Remove(this.gameObject);
                Destroy(gameObject);
            }
        }

        if(GameManager.Instance.pullArrows)
        {
            Vector3 dir = GameManager.Instance.player.transform.position - transform.position;
            transform.position += dir * pullingSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Inventory>().PickUpArrows(type);
           
            if (type == CollectableType.BLUE_ARROW)
            {
                arrowsSpawner.spawnedBlueArrows.Remove(this.gameObject);
            }
            if (type == CollectableType.RED_ARROW)
            {
                arrowsSpawner.spawnedRedArrows.Remove(this.gameObject);
            }
            if (type == CollectableType.GREEN_ARROW)
            {
                arrowsSpawner.spawnedGreenArrows.Remove(this.gameObject);
            }
            if (type == CollectableType.YELLOW_ARROW)
            {
                arrowsSpawner.spawnedYellowArrows.Remove(this.gameObject);
            }
            
            Destroy(gameObject);
        }
    }
}
