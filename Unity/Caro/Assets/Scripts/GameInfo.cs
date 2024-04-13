using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
    public int n { get; set; }
    public float square_size {get;} = 2;
    public Vector3 [,] center_points { get; set; }
    public Vector3 [,] render_points { get; set; }

    private void Start()
    {
        n = Camera.main.transform.GetComponent<StartGame>().sizeOfBanCo;
        Debug.Log(n);
        render_points = new Vector3[n, n];
        center_points = new Vector3[n, n];
        float x, y = 3, z = Camera.main.GetComponent<StartGame>().z_start - 1;
        for (int i = 0; i < n; i++)
        {
            x = Camera.main.GetComponent<StartGame>().x_start - 1;
            for (int j = 0; j < n; j++)
            {
                render_points[i,j] = new Vector3(x, y, z);
                x += square_size;
            }
            z -= square_size;
        }

        y = 3;
        z = Camera.main.GetComponent<StartGame>().z_start;
        for (int i = 0; i < n; i++)
        {
            x = Camera.main.GetComponent<StartGame>().x_start;
            for(int j = 0;j < n; j++)
            {
                center_points[i,j] = new Vector3(x, y, z);
                x += square_size;
            }
            z -= square_size;
        }

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                Debug.Log($"Render points của ô {i} | {j}: {render_points[i,j]}");
                Debug.Log($"Center points của ô {i} | {j}: {center_points[i,j]}");
            }
        }
    }
}
