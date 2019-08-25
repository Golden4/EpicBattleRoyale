using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesController : MonoBehaviour
{
    public static ParticlesController Ins;
    public ParticleSystem pfBloodSplash;
    public ParticleSystem pfHitEffect;

    void Awake()
    {
        Ins = this;
        pfBloodSplash = Instantiate(Ins.pfBloodSplash.gameObject).GetComponent<ParticleSystem>();
        pfHitEffect = Instantiate(Ins.pfHitEffect.gameObject).GetComponent<ParticleSystem>();
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
}
