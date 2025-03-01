using System;
using System.Collections;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public void PlaySound(String name)
    {
        if (AudioManager.instance)
        {
            AudioManager.instance.Play(name);
        }
        else
        {
            Debug.Log("no audio manager");
        }
    }

    public void StopSound(String name)
    {
        if (AudioManager.instance)
        {
            AudioManager.instance.Stop(name);
        }
        else
        {
            Debug.Log("no audio manager");
        }
    }

    public void FadeOut(String name)
    {
        if (AudioManager.instance)
        {
            AudioManager.instance.FadeOut(name);
        }
        else
        {
            Debug.Log("no audio manager");
        }
    }

    public void FadeIn(String name)
    {
        if (AudioManager.instance)
        {
            AudioManager.instance.FadeIn(name);
        }
        else
        {
            Debug.Log("no audio manager");
        }
    }
}
