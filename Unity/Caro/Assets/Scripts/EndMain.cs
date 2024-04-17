using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndMain : MonoBehaviour
{
    public Text TwhoWin;
    public static string whoWin;

    private void Start()
    {
        Debug.Log(whoWin);
        TwhoWin.text = whoWin;

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ComeToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);        
    }
}
