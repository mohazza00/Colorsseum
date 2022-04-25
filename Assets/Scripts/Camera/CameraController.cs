using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Player target;
    public GameManager gameManager;

    [Header("Zoom")]
    public float zoomingSpeed;
    public bool zoomCamera = false;

    private Camera cam;

    private void Awake()
    {
        target = FindObjectOfType<Player>();
        gameManager = FindObjectOfType<GameManager>();

        cam = Camera.main;
    }

    private void Update()
    {
        if(gameManager.wavesStarted)
        {
            if (gameManager.stopCameraZoom)
            {
                zoomCamera = false;
            }
            else
            {
                zoomCamera = true;
            }
        }     

        transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
    }

    private void LateUpdate()
    {
        if(zoomCamera)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 11, zoomingSpeed * Time.deltaTime);
        }
        else
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 6, zoomingSpeed * Time.deltaTime);
        }
    }

}
