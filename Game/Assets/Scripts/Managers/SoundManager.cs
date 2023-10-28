using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager 
{
    AudioSource bgm = new AudioSource();
    AudioSource effect = new AudioSource();
    public void Start()
    {
        bgm = GameObject.Find("Bgm").GetComponent<AudioSource>();
        effect = GameObject.Find("Effect").GetComponent<AudioSource>();
    }
    public void PlayEating()
    {
        effect.clip = Resources.Load<AudioClip>("Sound/eating_sound");
        effect.Play();
    }
}
