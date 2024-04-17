using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public InputField boardSize;
    public InputField lineSize;
    public void PlayGame()
    {
        if (boardSize != null && lineSize != null)
        {
            StartGame.sizeOfBanCo = Convert.ToInt32(boardSize.text);
            StartGame.lineSize = Convert.ToInt32(lineSize.text);
            SceneManager.LoadSceneAsync(1);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
