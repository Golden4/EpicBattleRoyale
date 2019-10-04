using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LivingEntity : EntityBase
{
    public event Action<LivingEntity> OnDie;
    public static event Action<LivingEntity> OnDieStatic;

    protected bool isDead;
    public HealthSystem healthSystem;

    protected virtual void Awake()
    {
        healthSystem.OnHealthZero += HealthSystem_OnHealthZero;
    }

    void HealthSystem_OnHealthZero(object sender, EventArgs e)
    {
        if (!IsDead())
            Die();
    }

    public bool IsDead()
    {
        return isDead;
    }

    void Die()
    {
        isDead = true;

        OnEntityDie();

        if (OnDie != null)
            OnDie(this);

        if (OnDieStatic != null)
            OnDieStatic(this);
    }

    public virtual void OnEntityDie()
    {

    }
}
