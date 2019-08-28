using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    CharacterBase characterBase;

    void Awake()
    {
        characterBase = GetComponentInParent<CharacterBase>();
    }

    public void Footstep(int step)
    {
        characterBase.characterAudio.PlayFootstepSound(step);
    }
}
