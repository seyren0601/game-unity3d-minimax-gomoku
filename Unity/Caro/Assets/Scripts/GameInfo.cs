using UnityEngine;

public class GameInfo : MonoBehaviour
{
    public int n { get; set; }
    public float square_size {get;} = 2;
    public Vector3 [,] center_points { get; set; }
    public Vector3 [,] render_points { get; set; }

    private float center_point_start_x { get; set; }
    private float center_point_start_z { get; set; }

    StartGame startGame;

    private void Start()
    {
        startGame = FindObjectOfType<StartGame>();

        n = startGame.sizeOfBanCo;
        center_point_start_x = startGame.x_start;
        center_point_start_z = startGame.z_start;
        center_points = new Vector3[n, n];
        render_points = new Vector3[n, n];

        InputRenderPoints();
        InputCenterPoinst();



        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                Debug.Log($"render points của ô {i} | {j}: {render_points[i, j]}");
                Debug.Log($"center points của ô {i} | {j}: {center_points[i, j]}");
            }
        }
    }

    private void InputRenderPoints()
    {
        float x, y, z;
        x = center_point_start_x - 1;
        y = 3;
        z = center_point_start_z - 1;
        for (int i = 0; i < n; i++)
        {
            x = center_point_start_x - 1;
            for (int j = 0; j < n; j++)
            {
                render_points[i, j] = new Vector3(x, y, z);
                Debug.Log($"center points vừa được thêm vào là: {x}, {y}, {z}");
                x += square_size;
            }
            z -= square_size;
        }
    }

    private void InputCenterPoinst()
    {
        float x, y, z;
        x = center_point_start_x;
        y = 3;
        z = center_point_start_z;
        for (int i = 0; i < n; i++)
        {
            x = center_point_start_x;
            for (int j = 0; j < n; j++)
            {
                center_points[i, j] = new Vector3(x, y, z);
                x += square_size;
            }
            z -= square_size;
        }
    }


}
