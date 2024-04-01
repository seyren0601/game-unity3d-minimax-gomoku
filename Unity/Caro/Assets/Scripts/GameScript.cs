using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameScript : MonoBehaviour
{
    public GameObject Black;
    public GameObject White;
    GameInfo gameInfo;
    Camera main_camera;
    // Start is called before the first frame update
    void Start()
    {
        main_camera = Camera.main;
        gameInfo = gameObject.GetComponent<GameInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        Mouse mouse = Mouse.current;
        if(mouse.leftButton.wasPressedThisFrame){
            Vector3 mousePosition = mouse.position.ReadValue();
            //Debug.Log(mousePosition);
            Ray ray = main_camera.ScreenPointToRay(mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit)){
                Vector3 clicked = hit.point;
                Vector3? center_point = GetCenterPoint(hit.point);
                if(center_point is not null){
                    Instantiate(Black, (Vector3)center_point, Quaternion.identity);
                }
            }
        }
    }

    Vector3? GetCenterPoint(Vector3 Clicked){
        for(int i=0;i<gameInfo.n;i++){
            for(int j=0;j<gameInfo.n;j++){
                float x_clicked = Clicked.x;
                float z_clicked = Clicked.z;
                Vector3 render_point = gameInfo.render_points[i,j];
                if(x_clicked >= render_point.x && x_clicked <= render_point.x + gameInfo.square_size
                   && z_clicked >= render_point.z && z_clicked <= render_point.z + gameInfo.square_size)
                {
                    return gameInfo.center_points[i,j];
                }
            }
        }
        return null;
    }
}
