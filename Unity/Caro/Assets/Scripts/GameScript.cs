using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using MiniMax;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.InputSystem;
using static MiniMax.MiniMax;

public class GameScript : MonoBehaviour
{
    bool playerTurn {get;set;} = true;
    State state {get;set;}
    public GameObject Black;
    public GameObject White;
    public AudioSource audioSource;
    public AudioSource sGameOver;
    Result result {get;set;}
    GameInfo gameInfo;
    Camera main_camera;
    // Start is called before the first frame update
    void Start()
    {
        main_camera = Camera.main;
        gameInfo = gameObject.GetComponent<GameInfo>();
        string[,] startBoard = new string[gameInfo.n, gameInfo.n];
        for(int i=0;i<gameInfo.n;i++){
            for(int j=0;j<gameInfo.n;j++){
                startBoard[i, j] = " ";
            }
        }
        state = new State(startBoard, null);
        result = Result.Pending;
    }

    // Update is called once per frame
    void Update()
    {
        if(result == Result.Pending){
            if(playerTurn){
                Mouse mouse = Mouse.current;
                if(mouse.leftButton.wasPressedThisFrame){
                    audioSource.Play();
                    Vector3 mousePosition = mouse.position.ReadValue();
                    //Debug.Log(mousePosition);
                    Ray ray = main_camera.ScreenPointToRay(mousePosition);
                    if(Physics.Raycast(ray, out RaycastHit hit)){
                        Vector3 clicked = hit.point;
                        (int, int, Vector3)? move_point = GetCenterPoint(hit.point);
                        if(move_point is not null){
                            Vector3 center_point = move_point.Value.Item3;
                            int i = move_point.Value.Item1;
                            int j = move_point.Value.Item2;
                            if(state.board[i,j] == " "){
                                Instantiate(Black, center_point, Quaternion.identity);
                                string[,] newboard = (string[,])state.board.Clone();
                                newboard[i,j] = "X";
                                state = new State(newboard, (state, new MiniMax.Point(i, j), "X"));
                                playerTurn = false;
                            }
                        }
                    }
                }
            }
            else{
                MiniMax.Point move = MiniMax.MiniMax.AutoPlay_GetMove(state, "O");
                Instantiate(White, gameInfo.center_points[move.x, move.y], Quaternion.identity);
                string[,] newboard = (string[,])state.board.Clone();
                newboard[move.x, move.y] = "O";
                state = new State(newboard, (state, new MiniMax.Point(move.x, move.y), "O"));
                playerTurn = true;
            }
            result = CurrentState(state);
        }
    }

    (int, int, Vector3)? GetCenterPoint(Vector3 Clicked){
        for(int i=0;i<gameInfo.n;i++){
            for(int j=0;j<gameInfo.n;j++){
                float x_clicked = Clicked.x;
                float z_clicked = Clicked.z;
                Vector3 render_point = gameInfo.render_points[i,j];
                if(x_clicked >= render_point.x && x_clicked <= render_point.x + gameInfo.square_size
                   && z_clicked >= render_point.z && z_clicked <= render_point.z + gameInfo.square_size)
                {
                    return (i, j, gameInfo.center_points[i,j]);
                }
            }
        }
        return null;
    }


}
