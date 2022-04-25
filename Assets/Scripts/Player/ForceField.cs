using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    public float knockbackForce;
    private Player player;
    private List<Enemy> affectedEnemies = new List<Enemy>();

    private void Awake()
    {
        player = transform.root.GetComponent<Player>();    
    }

    private void OnEnable()
    {
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = true;
    }

    private void OnDisable()
    {
        affectedEnemies.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            Arrow bullet = collision.gameObject.GetComponent<Arrow>();
            if (!bullet.forceBack)

            {
                if (bullet.arrowType == UnitColor.ENEMY || bullet.canDamagePlayer)
                {
                    Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                    rb.velocity = Vector2.zero;
                    PoolObject pool = bullet.GetComponent<PoolObject>();
                    pool.TurnOff();
                   

                }
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (enemy.enemyType == EnemyType.BOSS)
                    return;

                if(!affectedEnemies.Contains(enemy))
                {
                    enemy.KnockBack(player.transform.position, knockbackForce);
                    affectedEnemies.Add(enemy);
                }                        
             
            }

        }
    }

}
