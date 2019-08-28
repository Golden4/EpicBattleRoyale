using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour
{
    public SoundGroup[] soundGroup;

    public Dictionary<string, SoundGroup> soundsDic = new Dictionary<string, SoundGroup>();

#if UNITY_EDITOR
    void OnValidate()
    {
        for (int i = 0; i < soundGroup.Length; i++)
        {
            for (int j = 0; j < soundGroup[i].clips.Length; j++)
            {
                soundGroup[i].clips[j].name = soundGroup[i].groupName + (j + 1);
            }
        }
    }
#endif

    void Awake()
    {
        for (int i = 0; i < soundGroup.Length; i++)
        {
            soundsDic[soundGroup[i].groupName] = soundGroup[i];
        }
    }

    public Sound GetSoundByName(string soundName)
    {
        SoundGroup array;

        if (soundsDic.TryGetValue(soundName, out array))
        {

            int randomIndex = Random.Range(0, array.clips.Length);
            return array.clips[randomIndex];
        }
        else
        {
            Debug.Log("Sound Not Found!!" + soundName);
            return null;
        }


    }

    [System.Serializable]
    public class SoundGroup
    {
        public string groupName;

        public Sound[] clips;

    }
}

