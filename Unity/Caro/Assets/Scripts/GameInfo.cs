using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
    public const int n = 3;
    public const float square_size = 2;
    public Vector3 [,] center_points { get; set; } = new Vector3[n, n];
    public Vector3 [,] render_points { get; set; } = new Vector3[n, n];

    private void Start()
    {
        float x, y = 3, z = 1;
        for (int i = 0; i < n; i++)
        {
            x = -3;
            for (int j = 0; j < n; j++)
            {
                render_points[i,j] = new Vector3(x, y, z);
                x += square_size;
            }
            z -= square_size;
        }

        y = 3;
        z = 2;
        for (int i = 0; i < n; i++)
        {
            x = -2;
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
                Debug.Log(center_points[i, j]);
            }
        }
    }
}
