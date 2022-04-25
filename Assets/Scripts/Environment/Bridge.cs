using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    public bool openTheDoor;
    public bool closeTheDoor;
    public float bridgeSpeed;

    [SerializeField] Transform bridge;

    private void Update()
    {
        if(openTheDoor)
        {
            if(bridge.localPosition.x > 4f)
            {
                bridge.localPosition -= new Vector3(bridgeSpeed * Time.deltaTime, 0f, 0f);
            }
            else
            {
                bridge.localPosition = new Vector3(4f, 0f, 0f);
                openTheDoor = false;
            }
        }

        if(closeTheDoor)
        {
            if (bridge.localPosition.x < 9f)
            {
                bridge.localPosition += new Vector3(bridgeSpeed * Time.deltaTime, 0f, 0f);
            }
            else
            {
                bridge.localPosition = new Vector3(9f, 0f, 0f);
                closeTheDoor = false;
            }
        }
    }
}
