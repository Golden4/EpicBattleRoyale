using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSounds : MonoBehaviour
{
    public Sound[] reloadSounds;
    public WeaponController weaponController;

    void Reload(int num)
    {
        if (reloadSounds.Length > num)
        {
            weaponController.cb.characterAudio.PlaySound(reloadSounds[num]);
        }
    }
}
