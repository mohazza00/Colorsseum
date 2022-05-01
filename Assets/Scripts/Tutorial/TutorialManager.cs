using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject[] slides;
    public GameObject healthBar;
    public GameObject minimap;
    public GameObject ammo;
    public GameObject skills;

    public int currentSlide;

    [Header("Tutorial Steps")]
    public bool checkingMovement;
    public bool movementChecked;
    public bool checkingShooting;
    public bool shootingChecked;
    public bool killedEnemy;

    [Header("Reference Variables")]
    public EnemyTutorial enemy;
    public ArrowsSpawner arrowsSpawner;
    public ShootingTutorial shootingTutorial;

    private void Start()
    {
        currentSlide = 0;
    }

    private void Update()
    {
        if(!checkingMovement)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                LoadNextSlide();
            }
        }     
    }

    public void LoadNextSlide()
    {
        if(currentSlide == 3)
        {
            StartMovementCheck();
            return;
        }
        if (currentSlide == 4)
        {
            StartShootingCheck();
            return;
        }

        if (currentSlide == 5)
        {
            DisplaySkills();
        }

        for (int i = 0; i < slides.Length; i++)
        {
            if(i == currentSlide)
            {
                slides[i].SetActive(false);
                if(i < slides.Length - 1)
                {
                    slides[i + 1].SetActive(true);
                }
                currentSlide++;
                break;
            }
        }

        if (currentSlide == 8)
        {
            SceneManager.LoadScene(5);
        }
    }

    public void StartMovementCheck()
    {
        healthBar.SetActive(true);
        minimap.SetActive(true);
        slides[3].SetActive(false);
        checkingMovement = true;
        currentSlide++;
    }

    public void FinishMovementCheck()
    {
        movementChecked = true;
        checkingMovement = false;
        slides[4].SetActive(true);
    }

    public void StartShootingCheck()
    {
        enemy.gameObject.SetActive(true);
        checkingMovement = true;
        movementChecked = false;
        ammo.SetActive(true);
        slides[4].SetActive(false);
        arrowsSpawner.spawnForTutorial = true;
        checkingShooting = true;
        shootingTutorial.arrowsUI[0].GetComponent<Animator>().SetBool("Selected", true);
        currentSlide++;
    }

    public void FinishShootingCheck()
    {
        shootingChecked = true;
        checkingMovement = false;
        checkingShooting = false;
        arrowsSpawner.spawnForTutorial = false;
        arrowsSpawner.ClearQuivers();
        slides[5].SetActive(true);
    }

    public void DisplaySkills()
    {
        skills.SetActive(true);
    }
}
