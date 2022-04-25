using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTutorial : MonoBehaviour
{

    [Header("Components")]
    public Unit unit;

    [Header("Health")]
    public float maxHealth = 1;
    public float currentHealth;

    [Header("Attack")]
    public int enemyLevel = 1;

    [Header("Tutorial")]
    public TutorialManager tutorialManager;

    private void Awake()
    {
        unit = GetComponent<Unit>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        PoolingManager.Instance.SpawnObj(PoolObjectType.HIT_EFFECT, transform.position, null);
        AudioManager.Instance.PlaySound(SoundName.HIT);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die() 
    {
        tutorialManager.killedEnemy = true;
        Destroy(gameObject);
    }

    public void LevelUp()
    {
        enemyLevel += 1;
        maxHealth += 1f;
        currentHealth += 1f;
        transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);     
    }
}
