using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameScript : MonoBehaviour
{
    public GameObject Black;
    public GameObject White;
    Camera main_camera;
    // Start is called before the first frame update
    void Start()
    {
        main_camera = Camera.main;
        Instantiate(Black, new Vector3(-2, 3, 0), Quaternion.identity);
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
                if(clicked.x >= -1 && clicked.x <= -1 + 2 && clicked.z >= -1 && clicked.z <= -1 + 2 ){
                    Debug.Log("At [1, 1]");
                }
            }
        }
    }
}
