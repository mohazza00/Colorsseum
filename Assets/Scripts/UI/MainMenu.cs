using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button playButton;

    private void Start()
    {
        playButton.Select();
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(2);
    }

    public void OpenTutorial()
    {
        SceneManager.LoadScene(3);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
