using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSounds : MonoBehaviour
{
    public Sound[] reloadSounds;

    void Reload(int num)
    {
        if (reloadSounds.Length > num)
            AudioManager.PlaySound(reloadSounds[num]);
    }
}
