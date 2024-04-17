using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static MiniMax.State;
using System.Resources;
using MiniMax;

public class EndMain : MonoBehaviour
{
    public Text TwhoWin;
    public static string whoWin;
    public static State state;

    private void Start()
    {

        Debug.Log(whoWin);
        TwhoWin.text = (whoWin == "OWin" ? "Quân Trắng thắng" : whoWin == "XWin" ? "Quân Đen thắng" : "Hòa rồi! có cố gắng :\">");

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
