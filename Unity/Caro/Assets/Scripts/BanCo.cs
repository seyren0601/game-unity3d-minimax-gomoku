using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanCo : MonoBehaviour
{
    public AudioSource soundWhenDetect;

    // Start is called before the first frame update
    void Start()
    {
        soundWhenDetect.Pause();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "BlackChess(Clone)")
        {
            PlaySoundAtTime(0.043f); // đoạn này truyền tham số float để bắt đầu vô đoạn mình cần trong đoạn file mp3
        }
    }

    void PlaySoundAtTime(float time)
    {
        if (time > soundWhenDetect.clip.length)
        {
            return;
        }
        else
        {
            soundWhenDetect.time = time;
            soundWhenDetect.Play();
        }
    }
}
