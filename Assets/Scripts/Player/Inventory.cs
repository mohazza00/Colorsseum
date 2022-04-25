using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public SpriteRenderer sprite;
    public Player player;

    public int maxNumberOfArrows = 6;

    public float blueArrows;
    public float redArrows;
    public float greenArrows;
    public float yellowArrows;

    [Header("UI")]
    public Image UIBlueBar;
    public Image UIRedBar;
    public Image UIGreenBar;
    public Image UIYellowBar;
    public Image UIBlueIcon;
    public Image UIRedIcon;
    public Image UIGreenIcon;
    public Image UIYellowIcon;

    [Header("Items")]
    public List<Item> purchasedItems = new List<Item>();
    public ItemSlot[] itemsSlots;
    public Skills skills;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        player = GetComponent<Player>();
        skills = GetComponent<Skills>();
    }

    private void Start()
    {
        blueArrows = 0;
        redArrows = 0;
        greenArrows = 0;
        yellowArrows = 0;

        UIBlueBar.fillAmount = blueArrows;
        UIRedBar.fillAmount = redArrows;
        UIGreenBar.fillAmount = greenArrows;
        UIYellowBar.fillAmount = yellowArrows;
    }

    public void PickUpArrows(CollectableType type)
    {
        AudioManager.Instance.PlaySound(SoundName.ARROW_PICKUP);

        switch (type)
        {
            case CollectableType.BLUE_ARROW:
                if(blueArrows < maxNumberOfArrows)
                {
                    blueArrows++;
                    UpdateUI(UIBlueBar, blueArrows);
                }
               
                break;

            case CollectableType.RED_ARROW:
                if (redArrows < maxNumberOfArrows)
                {
                    redArrows++;
                    UpdateUI(UIRedBar, redArrows);
                }
                break;

            case CollectableType.GREEN_ARROW:
                if (greenArrows < maxNumberOfArrows)
                {
                    greenArrows++;
                    UpdateUI(UIGreenBar, greenArrows);
                }
                break;

            case CollectableType.YELLOW_ARROW:
                if (yellowArrows < maxNumberOfArrows)
                {
                    yellowArrows++;
                    UpdateUI(UIYellowBar, yellowArrows);
                }
                break;
        }
    }

    public void PurchaseItem(Item item)
    {
        purchasedItems.Add(item);
        for (int i = 0; i < itemsSlots.Length; i++)
        {
            if(!itemsSlots[i].isOccupied)
            {
                itemsSlots[i].slotUI.sprite = item.sprite;
                itemsSlots[i].slotUI.color = new Color(1, 1, 1, 1);
                itemsSlots[i].isOccupied = true;
                UnlockSkill(i, item);
                break;
            }        
        }
    }

    public void UnlockSkill(int slotNumber, Item item)
    {
        if(slotNumber == 0) skills.skill_1 = item;
        else if (slotNumber == 1) skills.skill_2 = item;   
        else if (slotNumber == 2) skills.skill_3 = item; 
        else if (slotNumber == 3) skills.skill_4 = item;      
    }

    public void UpdateUI(Image bar, float points)
    {
        if (points == 0) bar.fillAmount = 0f;
        else if (points == 1) bar.fillAmount = 0.25f;
        else if (points == 2) bar.fillAmount = 0.4f;
        else if (points == 3) bar.fillAmount = 0.5f;
        else if (points == 4) bar.fillAmount = 0.65f;
        else if (points == 5) bar.fillAmount = 0.75f;
        else if (points == 6) bar.fillAmount = 1f;
    }

    public void FillArrowsFully(int index)
    {
        if(index == 0)
        {
            yellowArrows = maxNumberOfArrows;
            UpdateUI(UIYellowBar, yellowArrows);
        }
        else if (index == 1)
        {
            redArrows = maxNumberOfArrows;
            UpdateUI(UIRedBar, redArrows);
        }
        else if (index == 2)
        {
            blueArrows = maxNumberOfArrows;
            UpdateUI(UIBlueBar, blueArrows);
        }

        else if (index == 3)
        {
            greenArrows = maxNumberOfArrows;
            UpdateUI(UIGreenBar, greenArrows);
        }
    }

    public void EmptyArrows(int index)
    {
        if (index == 0)
        {
            yellowArrows = 0;
            UpdateUI(UIYellowBar, yellowArrows);
        }
        else if (index == 1)
        {
            redArrows = 0;
            UpdateUI(UIRedBar, redArrows);
        }
        else if (index == 2)
        {
            blueArrows = 0;
            UpdateUI(UIBlueBar, blueArrows);
        }
        else if (index == 3)
        {
            greenArrows = 0;
            UpdateUI(UIGreenBar, greenArrows);
        }
    }
}

[System.Serializable]
public class ItemSlot{
    public Image slotUI;
    public bool isOccupied;
}