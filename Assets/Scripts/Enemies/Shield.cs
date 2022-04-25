using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public Guard enemy;
    public Boss boss;

    private void Awake()
    {
        enemy = GetComponentInParent<Guard>();
        boss = GetComponentInParent<Boss>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(enemy != null)
        {
            if (enemy.shieldIsActive)
            {
                Debug.Log("Bullet NO");
                Arrow bullet = collision.gameObject.GetComponent<Arrow>();
                if (bullet != null)
                {
                    AudioManager.Instance.PlaySound(SoundName.BOUNCE);

                    if (bullet.arrowType != UnitColor.ENEMY)
                    {
                        Debug.Log("Bullet");
                        bullet.RedirectBullet(enemy.forwardDirection.normalized, Unit.SetArrowRotation(enemy.forwardDirection.normalized), enemy.bulletForce);
                        enemy.rebounceBullet = true;
                    }
                    else
                    {
                        bullet.RedirectBullet(enemy.forwardDirection.normalized, Unit.SetArrowRotation(enemy.forwardDirection.normalized), enemy.bulletForce);
                    }

                }
            }
        }

        if (boss != null)
        {
            if (boss.shieldIsActive)
            {
                Debug.Log("Bullet NO");
                Arrow bullet = collision.gameObject.GetComponent<Arrow>();
                if (bullet != null)
                {
                    AudioManager.Instance.PlaySound(SoundName.BOUNCE);

                    if (bullet.arrowType != UnitColor.ENEMY)
                    {
                        Debug.Log("Bullet");
                        bullet.RedirectBullet(boss.forwardDirection.normalized, Unit.SetArrowRotation(boss.forwardDirection.normalized), boss.bulletForce);
                        boss.rebounceBullet = true;
                    }
                    else
                    {
                        bullet.RedirectBullet(boss.forwardDirection.normalized, Unit.SetArrowRotation(boss.forwardDirection.normalized), boss.bulletForce);
                    }

                }
            }
        }
    } 
}
