using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ItemType
{
    POTION,
    FREEZE_WATCH,
    COLOR_BOMB,
    ARROWS_MAGNET,
    SHIELD,
    DOUBLE_DASH,
    INFINITE_ARROWS,
    NONE,
    FORCE_FIELD,
}

public class Market : MonoBehaviour
{
    [Header("Settings")]
    public Item[] allItems;
    public Transform[] itemsSpots;
    public int maxNumberOfItems;

    [Header("UI")]
    public Transform infoBoxUI;
    public TextMeshProUGUI nameUI;
    public TextMeshProUGUI descriptionUI;

    public Item selectedItem;
    public bool canPurchase;

    private List<Item> currentItems = new List<Item>();
    private Player player; 

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }
   
    private void OnEnable()
    {    
        currentItems.Clear();
       
        PrepareItems();
        SpawnItems();
    }

    private void Update()
    {
        if(canPurchase)
        {
            if(Input.GetKeyDown(KeyCode.J))
            {
                player.inventory.PurchaseItem(selectedItem);
                AudioManager.Instance.PlaySound(SoundName.MARKET);
                GameManager.Instance.StartNextWave();

                foreach(Transform t in itemsSpots)
                {
                    t.gameObject.SetActive(false);
                }    
            }
        }
    }

    public void PrepareItems()
    {
        while(currentItems.Count != maxNumberOfItems)
        {
            currentItems.Add(GetRandomItem());
        }
    }

    private void SpawnItems()
    {
        int startIndex = GameManager.Instance.wavesSpawner.nextWave * 3;
        Debug.Log(startIndex);
        for (int i = startIndex; i < startIndex+ maxNumberOfItems; i++)
        {
            itemsSpots[i].gameObject.SetActive(true);
            Instantiate(currentItems[i - GameManager.Instance.wavesSpawner.nextWave * 3].gameObject, itemsSpots[i].position, itemsSpots[i].rotation, itemsSpots[i]);
        }
    }

    private Item GetRandomItem()
    {
        int rand = Random.Range(0, allItems.Length);

        Item randomItem = allItems[rand];
        if(currentItems.Contains(randomItem) || CheckIfAlreadyPurchased(randomItem))
        {
            return GetRandomItem();
        }
        else
        {
            return randomItem;
        }
    }

    private bool CheckIfAlreadyPurchased(Item item)
    {
        for (int i = 0; i < player.inventory.purchasedItems.Count; i++)
        {
            if(player.inventory.purchasedItems[i].type == item.type)
            {
                return true;
            }
        }

        return false;
    }
}

