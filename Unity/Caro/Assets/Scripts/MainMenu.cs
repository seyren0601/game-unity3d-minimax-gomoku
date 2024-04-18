using System;
using System.Collections;
using System.Collections.Generic;
using MiniMax;
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
            if (int.TryParse(boardSize.text, out _) && int.TryParse(lineSize.text, out _))
            {
                StartGame.sizeOfBanCo = Convert.ToInt32(boardSize.text);
                StartGame.lineSize = Convert.ToInt32(lineSize.text);
                GameInfo.GamePlaying = true;
                GameScript.GamePlaying = true;
                SceneManager.LoadSceneAsync(1);
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
