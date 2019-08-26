using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAudio : MonoBehaviour
{
    const int sourceCount = 3;
    AudioSource[] source = new AudioSource[sourceCount];

    void Awake()
    {
        for (int i = 0; i < sourceCount; i++)
        {
            source[i] = gameObject.GetComponent<AudioSource>();
        }
    }

    public void PlaySound(Sound sound)
    {
        if (sound != null && sound.clip != null)
            sound.PlaySound(GetAvaibleSource());
    }

    AudioSource GetAvaibleSource()
    {
        for (int i = 0; i < source.Length; i++)
        {
            if (!source[i].isPlaying)
            {
                return source[i];
            }
        }
        return source[0];
    }
}
