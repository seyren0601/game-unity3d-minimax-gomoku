using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public GameObject duongVien;
    private GameObject banCo;

    public int sizeOfBanCo;

    GameObject[,] duongViens;

    public float x_start {  get; set; }
    public float z_start { get; set; }

    private const float sizeOneO = 2;


    // Start is called before the first frame update
    void Start()
    {
        float campos_x, campos_y, campos_z;
        campos_x = Camera.main.transform.position.x;
        campos_y = Camera.main.transform.position.y * (sizeOfBanCo * 0.3f);
        campos_z = Camera.main.transform.position.z * (sizeOfBanCo * 0.3f);
        Camera.main.transform.position = 
            new Vector3(campos_x,campos_y,campos_z);
        banCo = GameObject.FindGameObjectWithTag("MatBanCo");
        InitDuongVien();
        InitSizeBanCo();
        SpawnDuongVien();
    }

    private void InitSizeBanCo()
    {
        banCo.transform.localScale *= sizeOfBanCo;
    }

    private void SpawnDuongVien()
    {
        float y = duongVien.transform.position.y;
        float z = z_start;
        for (int i = 0; i < sizeOfBanCo; i++)
        {
            float x = x_start;
            if (i > 0)
            {
                z -= sizeOneO;
            }
            else
            {
                z = z_start;
            }
            for (int j = 0; j < sizeOfBanCo; j++)
            {
                if (j > 0)
                {
                    x += sizeOneO;
                    duongViens[i,j].transform.position = new Vector3(x, y, z);
                }
                else
                {
                    duongViens[i,j].transform.position = new Vector3(x, y, z);
                }
                Debug.Log(z);
            }
        }
    }

    private void InitDuongVien()
    {
        duongViens = new GameObject[sizeOfBanCo, sizeOfBanCo];
        x_start = duongVien.transform.position.x * sizeOfBanCo;
        z_start = duongVien.transform.position.z + (sizeOfBanCo - 1);
        Debug.Log($"{duongVien.transform.position.z}, {sizeOfBanCo}, {z_start}");
        for (int i = 0; i < sizeOfBanCo; i++)
        {
            for (int j = 0; j < sizeOfBanCo; j++)
            {
                GameObject vien = duongVien.GetComponent<GameObject>();
                vien = Instantiate(duongVien);
                vien.transform.name = $"O {i}|{j}";
                duongViens[i, j] = vien;
                Debug.Log("Thêm thành công một ô vào trong list");
            }
        }
    }
}
