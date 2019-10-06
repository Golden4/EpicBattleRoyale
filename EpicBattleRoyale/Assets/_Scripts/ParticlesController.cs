using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesController : MonoBehaviour
{
    public static ParticlesController Ins;
    public ParticleSystem pfBloodSplash;
    public ParticleSystem pfHitEffect;
    public ParticleSystem pfMuzzleFlash;

    void Awake()
    {
        Ins = this;
        pfBloodSplash = Instantiate(pfBloodSplash.gameObject).GetComponent<ParticleSystem>();
        pfHitEffect = Instantiate(pfHitEffect.gameObject).GetComponent<ParticleSystem>();
        pfMuzzleFlash = Instantiate(pfMuzzleFlash.gameObject).GetComponent<ParticleSystem>();
    }

    public void PlayBloodSplashParticle(Vector2 position, Vector2 rotation)
    {
        pfBloodSplash.transform.position = position;
        pfBloodSplash.transform.rotation = Utility.DirectionToRotation(rotation, true);
        pfBloodSplash.Emit(Random.Range(15, 30));
    }

    public void PlayHitEffectParticle(Vector2 position, Vector2 rotation)
    {
        pfHitEffect.transform.position = position;
        pfHitEffect.transform.rotation = Utility.DirectionToRotation(rotation, true);
        pfHitEffect.Emit(Random.Range(15, 30));
    }

    public void PlayMuzzleFlashParticle(Vector2 position, Vector2 rotation)
    {
        pfMuzzleFlash.transform.position = position;
        pfMuzzleFlash.transform.rotation = Utility.DirectionToRotation(rotation, true);
        pfMuzzleFlash.Play(true);
    }
}
