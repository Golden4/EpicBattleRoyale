using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAudio : MonoBehaviour
{
    public Sound headshotHitSound;
    public Sound hitSound;
    public Sound[] footsteps;
    AudioSource[] sources;
    CharacterBase characterBase;

    void Awake()
    {
        characterBase = GetComponent<CharacterBase>();

        sources = GetComponents<AudioSource>();
    }

    public void PlaySound(Sound sound)
    {
        if (sound != null && sound.clip != null)
            sound.PlaySound(GetAvaibleSource());
    }

    public void PlaySound(Sound[] sounds)
    {
        if (sounds != null)
        {
            Sound sound = GetRandomSound(sounds);
            PlaySound(sound);
        }
    }

    public void PlayFootstepSound(int step)
    {
        if (step == 0)
        {
            AudioManager.PlaySoundAtPosition(footsteps[0], (Vector2)characterBase.worldPosition);
        }
        else
        {
            AudioManager.PlaySoundAtPosition(footsteps[1], (Vector2)characterBase.worldPosition);
        }
    }

    Sound GetRandomSound(Sound[] sound)
    {
        int index = Random.Range(0, sound.Length);
        return sound[index];
    }

    AudioSource GetAvaibleSource()
    {
        for (int i = 0; i < sources.Length; i++)
        {
            if (!sources[i].isPlaying)
            {
                return sources[i];
            }
        }
        return sources[0];
    }
}
