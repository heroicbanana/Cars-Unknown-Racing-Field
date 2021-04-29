using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public Sounds[] sounds;
    public float maxOffset;

    void Awake()
    {
        foreach(Sounds s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.audioClip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sounds s = Array.Find(sounds,sound => sound.name == name);
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sounds s = Array.Find(sounds,sound => sound.name == name);
        s.source.Stop();
    }

    public void ChangePitch(string name,bool change,int speed)
    {
        Sounds s = Array.Find(sounds,sound => sound.name == name);
        s.source.pitch = speed * maxOffset * Time.deltaTime;
        if(s.source.pitch >= 3)
        {
            s.source.pitch = 3;
        }
    }

    public void ChangePitch(string name,int speed)
    {
        Sounds s = Array.Find(sounds,sound => sound.name == name);
        s.source.pitch += 0.05f;
        if(s.source.pitch >= 3f)
        {
            s.source.pitch = 3;
        } 
    }

    public void ChangePitchDown(string name)
    {
        Sounds s = Array.Find(sounds,sound => sound.name == name);
        s.source.pitch -= 0.05f;
        if(s.source.pitch <= 0)
            s.source.pitch = 0;
    }

     public void ChangeOffset(int speed)
     {
        
        if(speed >= 20 && speed < 40)
            maxOffset = 2.5f;
        else if(speed >= 40 && speed < 60)
            maxOffset = 1.25f;
        else if(speed >= 60 && speed < 80)
            maxOffset = 0.83f;
        else if(speed >=80 && speed < 120)
            maxOffset = 0.625f;
        else if(speed >= 120)
            maxOffset = 0.5f;
        //else if(speed > 0 && speed < 20)
          //  maxOffset = 4f;
     }

     public void ChangeOffsetDown(int speed)
     {
        if(speed > 0 && speed < 20)
        {
            maxOffset = 3f;
        }
        maxOffset -= 0.0078f;
        if(speed >= 0 && speed < 20)
            maxOffset = 3f;
        if (speed >= 20 && speed < 30)
            maxOffset = 2.5f;
    }

     public float getPitch(string name)
    {
        Sounds s = Array.Find(sounds,sound => sound.name == name);
        return s.source.pitch;
    }
}
