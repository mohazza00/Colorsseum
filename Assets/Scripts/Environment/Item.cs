using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{
    public ItemType type;
    public string itemName;
    [TextArea(3, 10)]
    public string description;
    public Sprite sprite;
    public float duration;
    public float coolDown;

    private Market market;


    private void Start()
    {
        market = transform.root.GetComponent<Market>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            market.infoBoxUI.gameObject.SetActive(true);
            market.nameUI.text = itemName;
            market.descriptionUI.text = description;
            market.selectedItem = this;
            market.canPurchase = true;
            collision.gameObject.GetComponent<Player>().isBuyingItems = true;

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            market.infoBoxUI.gameObject.SetActive(false);
            market.selectedItem = null;
            market.canPurchase = false;
            collision.gameObject.GetComponent<Player>().isBuyingItems = false;
        }
    }
}
