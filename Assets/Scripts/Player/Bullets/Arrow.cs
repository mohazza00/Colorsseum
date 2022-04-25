using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("Settings")]
    public UnitColor arrowType;
    public int damage;

    public bool forceBack;
    public bool canDamagePlayer;

    private PoolObject poolObject;
    private bool damageTaken;

    private void Awake()
    {
        poolObject = GetComponent<PoolObject>();
    }

    private void OnEnable()
    {
        forceBack = false;
        canDamagePlayer = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (damageTaken)
            return;

        Enemy enemy = collision.transform.GetComponent<Enemy>();

        if (enemy != null)
        {
            if (arrowType == UnitColor.ENEMY)
                return;

            if (enemy.unit.unitColor == arrowType)
            {
                if (enemy.enemyType == EnemyType.BOSS)
                {
                    enemy.GetComponent<Boss>().BossTakeDamage(1);
                }
                else
                {
                    enemy.TakeDamage(1);
                }

            }
            else
            {
                if (enemy.enemyType == EnemyType.BOSS)
                {
                    enemy.GetComponent<Boss>().BossLevelUp();
                }
                else
                {
                    enemy.LevelUp();
                }
            }

            if (!PoolingManager.Instance.PoolDictionary[poolObject.poolObjectType].Contains(this.gameObject))
            {
                poolObject.TurnOff();
            }
            damageTaken = true;
            Destroy(gameObject);
        }

        Player player = collision.transform.GetComponent<Player>();

        if (player != null && (arrowType == UnitColor.ENEMY || canDamagePlayer))
        {
            if(!player.damageDetector.invincible)
            {
                player.damageDetector.TakeDamage(damage);
                if (!PoolingManager.Instance.PoolDictionary[poolObject.poolObjectType].Contains(this.gameObject))
                {
                    poolObject.TurnOff();
                }
            }           
        }

        #region tutorial
        EnemyTutorial enemyTutorial = collision.transform.GetComponent<EnemyTutorial>();

        if (enemyTutorial != null)
        {
            if (arrowType == UnitColor.ENEMY)
                return;

            if (enemyTutorial.unit.unitColor == arrowType)
            {
                enemyTutorial.TakeDamage(1);
            }
            else
            {
                enemyTutorial.LevelUp();
            }
            if (!PoolingManager.Instance.PoolDictionary[poolObject.poolObjectType].Contains(this.gameObject))
            {
                poolObject.TurnOff();
            }

            damageTaken = true;
            Destroy(gameObject);
        }
        #endregion     
    }

    public void RedirectBullet(Vector3 dir, Quaternion rotation, float force)
    {
        canDamagePlayer = true;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        transform.rotation = rotation;
        rb.velocity = Vector2.zero;
        rb.AddForce(dir * force, ForceMode2D.Impulse);
    }
}
