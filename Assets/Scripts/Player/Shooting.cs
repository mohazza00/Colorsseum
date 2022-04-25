using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    public Image[] arrowsUI;
    public float shootingForce = 20f;
    public int currentArrow = 0;

    private int numberOfArrowColors = 4;
    private Player player;
    private bool replayUIAnimation;
    private GameObject arrow;
    private PoolingManager poolingManager;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        arrowsUI[0].GetComponent<Animator>().SetBool("Selected", true);
        poolingManager = PoolingManager.Instance;
    }
    private void Update()
    {
        if(GameManager.Instance.gameState == GameState.SPANWING_WAVE || GameManager.Instance.gameState == GameState.SPAWNING_MINIONS_WAVE)
        {
            replayUIAnimation = true;
            for (int i = 0; i < arrowsUI.Length; i++)
            {
                arrowsUI[i].GetComponent<Animator>().SetBool("Selected", false);
            }
        }
        else
        {
            if (GameManager.gameIsPaused)
                return;


            if (replayUIAnimation)
            {
                for (int i = 0; i < arrowsUI.Length; i++)
                {
                    if (i == currentArrow)
                    {
                        arrowsUI[i].GetComponent<Animator>().SetBool("Selected", true);

                    }
                    else
                    {
                        arrowsUI[i].GetComponent<Animator>().SetBool("Selected", false);
                    }
                }
                replayUIAnimation = false;
            }
           
        }
        

        if (player.isBuyingItems || GameManager.Instance.gameState == GameState.SPANWING_WAVE || GameManager.Instance.gameState == GameState.SPAWNING_MINIONS_WAVE)
            return;

       
        if (Input.GetKeyDown(KeyCode.K))
        {
            currentArrow = (currentArrow + 1) % numberOfArrowColors;
           
            for (int i = 0; i < arrowsUI.Length; i++)
            {
                if (i == currentArrow)
                {
                    arrowsUI[i].GetComponent<Animator>().SetBool("Selected", true);

                }
                else
                {
                    arrowsUI[i].GetComponent<Animator>().SetBool("Selected", false);
                }
            }
        }             

        if (Input.GetKeyDown(KeyCode.J))
        {
            if (CheckIfCanShoot())
            {
                Shoot();
            }
        }       
    }
 
    private void Shoot()
    {
        arrow = SpawnBullet();
        if (arrow != null)
        {
            player.animator.SetTrigger("Attack");
            AudioManager.Instance.PlaySound(SoundName.SHOOT);
            arrow.transform.position = transform.position;
            arrow.transform.rotation = Unit.SetArrowRotation(player.playerForwardDir.normalized);
            arrow.SetActive(true);
            Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
            rb.AddForce(player.playerForwardDir.normalized * shootingForce, ForceMode2D.Impulse);
            UseArrow(currentArrow);
        }     
    }

    private GameObject SpawnBullet()
    {
        GameObject obj = null;
        if (currentArrow == 0)
            obj = poolingManager.GetObject(PoolObjectType.YELLOW_ARROW);
        if (currentArrow == 1)
            obj = poolingManager.GetObject(PoolObjectType.RED_ARROW);
        if (currentArrow == 2)
            obj = poolingManager.GetObject(PoolObjectType.BLUE_ARROW);
        if (currentArrow == 3)
            obj = poolingManager.GetObject(PoolObjectType.GREEN_ARROW);

        return obj;
    }

    private bool CheckIfCanShoot()
    {
        if (currentArrow == 0 && player.inventory.yellowArrows > 0)
            return true;

        else if (currentArrow == 1 && player.inventory.redArrows > 0)
            return true;

        else if(currentArrow == 2 && player.inventory.blueArrows > 0)
            return true;

        else if(currentArrow == 3 && player.inventory.greenArrows > 0)
            return true;

        return false;
    }

    private void UseArrow(int bullet)
    {
        if(GameManager.Instance.infiniteArrows)
        {
            if (bullet == GameManager.Instance.infiniteArrowIndex)
            {
                return;
            }
        }
        if (bullet == 0)
        {
            player.inventory.yellowArrows--;
            player.inventory.UpdateUI(player.inventory.UIYellowBar, player.inventory.yellowArrows);
        }

        if (bullet == 1)
        {
            player.inventory.redArrows--;
            player.inventory.UpdateUI(player.inventory.UIRedBar, player.inventory.redArrows);
        }

        if (bullet == 2)
        {
            player.inventory.blueArrows--;
            player.inventory.UpdateUI(player.inventory.UIBlueBar, player.inventory.blueArrows);
        }

        if (bullet == 3)
        {
            player.inventory.greenArrows--;
            player.inventory.UpdateUI(player.inventory.UIGreenBar, player.inventory.greenArrows);
        }
    } 
}
