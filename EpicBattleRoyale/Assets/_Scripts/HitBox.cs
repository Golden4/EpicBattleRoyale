using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public enum HitBoxType
    {
        Head,
        Body,
        Legs
    }

    public HitBoxType hitBoxType;

    public CharacterBase characterBase;

    void Awake()
    {
        characterBase = GetComponentInParent<CharacterBase>();
    }

    public void OnHitted(CharacterBase hitCharacter, Weapon hitWeapon, int damage)
    {
        characterBase.OnHitted(hitCharacter, hitWeapon, damage, hitBoxType);
    }

    public bool CanHit()
    {
        return characterBase.CanHit();
    }

    public static float GetDamagePersent(HitBoxType hitBoxType)
    {
        switch (hitBoxType)
        {
            case HitBoxType.Head:
                return 2f;
            case HitBoxType.Body:
                return 1f;
            case HitBoxType.Legs:
                return .5f;
            default:
                return 0;
        }

    }
}
