using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SoundLibrary))]
public class AudioManager : SingletonResourse<AudioManager>
{
    const int sourceCount = 10;
    AudioSource[] source = new AudioSource[sourceCount];

    [HideInInspector]
    public SoundLibrary soundLibrary;

    public override void OnInit()
    {
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < sourceCount; i++)
        {
            source[i] = gameObject.AddComponent<AudioSource>();
        }
        soundLibrary = GetComponent<SoundLibrary>();
    }

    public static void PlaySound(Sound sound)
    {
        if (!audioEnabled)
            return;
        if (sound != null && sound.clip != null)
            sound.PlaySound(Ins.GetAvaibleSource());
    }

    public AudioSource GetAvaibleSource()
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

    public void PlaySound(AudioClip sound)
    {
        if (!audioEnabled)
            return;

        source[0].PlayOneShot(sound);
    }

    public static bool audioEnabled = true;

    public static void EnableAudio(bool enable)
    {
        audioEnabled = enable;

        AudioListener.volume = enable ? 1 : 0;

        /*if (listener != null)
			listener.enabled = enable;*/

        //		print ("Audio " + enable);
    }

    public static void PlaySoundFromLibrary(string name)
    {
        try
        {
            Sound sound = Ins.soundLibrary.GetSoundByName(name);

            if (sound != null && sound.clip != null)
                PlaySound(sound);
            else
                Debug.LogError(name + "Sound not found");

        }
        catch (System.Exception except)
        {
            Debug.LogError(except);
            throw;
        }

    }

    public static void PlaySoundAtPosition(Sound sound, Vector3 pos)
    {
        AudioSource.PlayClipAtPoint(sound.clip, pos);
    }

    public static void PlaySoundAtObject(Sound sound, GameObject obj)
    {
        string name = sound.name;
        if (name == "")
        {
            name = "sound";
        }

        GameObject go = new GameObject(name);

        go.transform.SetParent(obj.transform, false);

        AudioSource source = go.AddComponent<AudioSource>();
        sound.PlaySound(source);
        Destroy(go, sound.clip.length + .1f);
    }

}

[System.Serializable]
public class Sound
{
    public string name = "Sound";
    public AudioClip clip;
    public bool loop;

    [Range(0, 1.5f)]
    public float volume = 1f;

    [Range(0.5f, 1.5f)]
    public float pitch = 1f;

    [Range(0f, 0.5f)]
    public float randomVolume;

    [Range(0f, 0.5f)]
    public float randomPitch = 0.2f;

    public Sound()
    {
        volume = 1f;

        pitch = 1f;
    }

    public Sound(AudioClip clip)
    {
        this.clip = clip;
        loop = false;
    }

    public Sound(string name, AudioClip clip, bool loop)
    {
        this.name = name;
        this.clip = clip;
        this.loop = loop;
    }

    public void Play()
    {
        AudioManager.PlaySound(this);
    }

    public void PlayAtObject(GameObject go)
    {
        AudioManager.PlaySoundAtObject(this, go);
    }

    public void PlaySound(AudioSource source)
    {
        source.clip = clip;
        source.volume = volume * (1 + Random.Range(-randomVolume / 2f, randomVolume / 2f));
        source.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f));
        source.Play();
    }

}
