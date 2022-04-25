using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingScene : MonoBehaviour
{
    public TextMeshProUGUI loadingText;

    int pointsNumber = 3;
    int currentPoint = 1;

    public int loadScene;
    public float loadTime = 2f;
    float timer = 0.2f;

    
    void OnEnable()
    {
        Invoke(nameof(LoadGameScene), loadTime);
        timer = 0.2f;
    }

    private void Update()
    {
        if(timer <= 0)
        {
            timer = 0.2f;
            if (currentPoint == 1)
            {
                loadingText.text = "Loading .";
                currentPoint = 2;
                return;
            }
            if (currentPoint == 2)
            {
                loadingText.text = "Loading ..";
                currentPoint = 3;
                return;
            }
            if (currentPoint == 3)
            {
                loadingText.text = "Loading ...";
                currentPoint = 1;
                return;
            }        
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
    private void LoadGameScene()
    {
        SceneManager.LoadScene(loadScene);
    }
}
