using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageDetector : MonoBehaviour
{
    public Player player;

    [Header("Settings")]
    public float maxHealth;
    public Image healthBar;

    [Header("State Variables")]
    public float currentHealth;
    public bool invincible;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        if (player.gameManager.gameState == GameState.SPANWING_WAVE || player.gameManager.gameState == GameState.SPAWNING_MINIONS_WAVE)
            return;

        StartCoroutine(_TakeDamage());
        currentHealth -= damage;
        
        UpdateHealthUI();
        if (currentHealth <= 0)
        {
            Die();
        }
       
    }

    IEnumerator _TakeDamage()
    {
        PoolingManager.Instance.SpawnObj(PoolObjectType.HIT_EFFECT, transform.position, null);
        AudioManager.Instance.PlaySound(SoundName.HIT);

        player.sprite.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        player.sprite.color = Color.white;

    }

    public void UpdateHealthUI()
    {
        //To match the HP bar Graphics
        if(currentHealth == 10)
        {
            healthBar.fillAmount = 1f;
        }
        else if(currentHealth == 9)
        {
            healthBar.fillAmount = 0.83f;
        }
        else if (currentHealth == 8)
        {
            healthBar.fillAmount = 0.78f;
        }
        else if (currentHealth == 7)
        {
            healthBar.fillAmount = 0.75f;
        }
        else if (currentHealth == 6)
        {
            healthBar.fillAmount = 0.65f;
        }
        else if (currentHealth == 5)
        {
            healthBar.fillAmount = 0.55f;
        }
        else if (currentHealth == 4)
        {
            healthBar.fillAmount = 0.48f;
        }
        else if (currentHealth == 3)
        {
            healthBar.fillAmount = 0.42f;
        }
        else if (currentHealth == 2)
        {
            healthBar.fillAmount = 0.35f;
        }
        else if (currentHealth == 1)
        {
            healthBar.fillAmount = 0.3f;
        }
        else if (currentHealth == 0)
        {
            healthBar.fillAmount = 0f;
        }
    }

    public void Die()
    {
        GameManager.Instance.gameOverPanel.SetActive(true);
        player.gameOver = true;
        healthBar.fillAmount = 0f;

    }

    public void RegainHealth(int amount)
    {
        StartCoroutine(_RegainHealth());
        currentHealth += amount;      
        UpdateHealthUI();      

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    IEnumerator _RegainHealth()
    {
        player.sprite.color = Color.green;
        yield return new WaitForSeconds(0.15f);
        player.sprite.color = Color.white;       
    }
}
