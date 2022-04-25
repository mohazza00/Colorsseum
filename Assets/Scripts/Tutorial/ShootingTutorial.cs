using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingTutorial : MonoBehaviour
{

    public GameObject[] bulletsPrefabs;
    PlayerTutorial player;

    public Image[] bulletsUI;
    public int currentBullet = 0;

    public float bulletForce = 20f;

    public bool chargingShot;

    private bool replayUIAnimation;

    [Header("Input Checking")]
    bool change;
    bool shoot;
    bool kill;

    private void Awake()
    {
        player = GetComponent<PlayerTutorial>();
    }

    private void Start()
    {
        bulletsUI[0].GetComponent<Animator>().SetBool("Selected", true);
        
    }
    private void Update()
    {

        if (!player.tutorialManager.checkingShooting)
            return;

        CheckInputs();


        if (Input.GetKeyDown(KeyCode.K))
        {
            if (currentBullet == bulletsPrefabs.Length - 1)
            {
                currentBullet = 0;
            }
            else
            {
                currentBullet++;
            }
            for (int i = 0; i < bulletsUI.Length; i++)
            {
                if (i == currentBullet)
                {
                    bulletsUI[i].GetComponent<Animator>().SetBool("Selected", true);

                }
                else
                {
                    bulletsUI[i].GetComponent<Animator>().SetBool("Selected", false);
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

        //if (chargingShot)
        //{
        //    if(Input.GetKeyUp(KeyCode.G))
        //    {
        //        Shoot();
        //        chargingShot = false;
        //        player.sprite.color = Color.white;
        //        player.movementSpeed += 150f;


        //    }
        //}
    }

    private void Shoot()
    {
        GameObject bullet = SpawnArrow(currentBullet);
        if (bullet != null)
        {
            AudioManager.Instance.PlaySound(SoundName.SHOOT);
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Unit.SetArrowRotation(player.playerForwardDir.normalized);
            bullet.SetActive(true);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(player.playerForwardDir.normalized * bulletForce, ForceMode2D.Impulse);
            UseArrow(currentBullet);
        }

    }

    private GameObject SpawnArrow(int index)
    {
        if (index == 0)
            return PoolingManager.Instance.GetObject(PoolObjectType.YELLOW_ARROW);
        if (index == 1)
            return PoolingManager.Instance.GetObject(PoolObjectType.RED_ARROW);
        if (index == 2)
            return PoolingManager.Instance.GetObject(PoolObjectType.BLUE_ARROW);
        if (index == 3)
            return PoolingManager.Instance.GetObject(PoolObjectType.GREEN_ARROW);

        return null;

    }

    private bool CheckIfCanShoot()
    {
        if (currentBullet == 0 && player.inventory.yellowArrows > 0)
            return true;

        else if (currentBullet == 1 && player.inventory.redArrows > 0)
            return true;

        else if (currentBullet == 2 && player.inventory.blueArrows > 0)
            return true;

        else if (currentBullet == 3 && player.inventory.greenArrows > 0)
            return true;

        return false;
    }

    private void UseArrow(int bullet)
    {
        if (GameManager.Instance.infiniteArrows)
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

    public void CheckInputs()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            change = true;
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            shoot = true;
        }

        if (change && shoot && player.tutorialManager.killedEnemy)
        {
            player.tutorialManager.FinishShootingCheck();
            player.ResetPlayerTransform();
        }
    }
}
