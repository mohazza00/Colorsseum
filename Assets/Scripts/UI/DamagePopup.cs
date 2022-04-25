using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{

    public static DamagePopup Create(Vector3 position, int damageAmount)
    {
        GameObject damagePopupTransform = PoolingManager.Instance.GetObject(PoolObjectType.DAMGAE_POPUP);
        damagePopupTransform.transform.position = position;
        damagePopupTransform.transform.rotation = Quaternion.identity;
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount);

        return damagePopup;
    }

    private TextMeshPro textMesh;
    private float disappearTimer = 1f;
    private Color textColor;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

   
    public void Setup(int damageAmount)
    {
        textMesh.text = (damageAmount.ToString());
        textColor = textMesh.color;
        textColor.a = 1f;
    }

    //private void Update()
    //{
    //    float moveYSpeed = 3f;
    //    transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;

    //    disappearTimer -= Time.deltaTime;
    //    if (disappearTimer > 0)
    //    {
    //        float disappearSpeed = 1f;
    //        textColor.a -= disappearSpeed * Time.deltaTime;
    //        textMesh.color = textColor;
    //        if (textColor.a < 0)
    //        {
    //            Destroy(gameObject);
    //        }
    //    }

    //}
}
