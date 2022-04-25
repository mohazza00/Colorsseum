using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolObjectType
{
    NONE,
    RED_ARROW,
    BLUE_ARROW,
    YELLOW_ARROW,
    GREEN_ARROW,
    ENEMY_ARROW,
    HIT_EFFECT,
    DAMGAE_POPUP,
    CLOUD,
    FORCE,
}

public class PoolObjectLoader : MonoBehaviour
{
    public static PoolObject InstantiatePrefab(PoolObjectType objType)
    {
        GameObject obj = null;

        switch (objType)
        {
            case PoolObjectType.RED_ARROW:
                {
                    obj = Instantiate(Resources.Load("RedBullet", typeof(GameObject)) as GameObject);
                    break;
                }

            case PoolObjectType.BLUE_ARROW:
                {
                    obj = Instantiate(Resources.Load("BlueBullet", typeof(GameObject)) as GameObject);
                    break;
                }

            case PoolObjectType.YELLOW_ARROW:
                {
                    obj = Instantiate(Resources.Load("YellowBullet", typeof(GameObject)) as GameObject);
                    break;
                }

            case PoolObjectType.GREEN_ARROW:
                {
                    obj = Instantiate(Resources.Load("GreenBullet", typeof(GameObject)) as GameObject);
                    break;
                }

            case PoolObjectType.ENEMY_ARROW:
                {
                    obj = Instantiate(Resources.Load("EnemyBullet", typeof(GameObject)) as GameObject);
                    break;
                }

            case PoolObjectType.HIT_EFFECT:
                {
                    obj = Instantiate(Resources.Load("HitEffect", typeof(GameObject)) as GameObject);
                    break;
                }

            case PoolObjectType.DAMGAE_POPUP:
                {
                    obj = Instantiate(Resources.Load("DamagePopUp", typeof(GameObject)) as GameObject);
                    break;
                }

            case PoolObjectType.CLOUD:
                {
                    obj = Instantiate(Resources.Load("Cloud", typeof(GameObject)) as GameObject);
                    break;
                }

            case PoolObjectType.FORCE:
                {
                    obj = Instantiate(Resources.Load("Force", typeof(GameObject)) as GameObject);
                    break;
                }


        }

        return obj.GetComponent<PoolObject>();
    }
}