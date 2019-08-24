using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static int CHARACTERS_COUNT_MAX = 100;

    void Awake()
    {
        World.OnPlayerSpawn += World_OnPlayerSpawn;
    }

    void World_OnPlayerSpawn(Player player)
    {
        Player.Ins.characterBase.OnDie += OnPlayerDead;
    }

    void OnPlayerDead(CharacterBase characterBase)
    {
        DeadScreen.Show(characterBase.killsCount, World.Ins.allCharacters.Count, 15);
    }

    void OnDestroy()
    {
        World.OnPlayerSpawn -= World_OnPlayerSpawn;

        if (Player.Ins != null)
        {
            Player.Ins.characterBase.OnDie -= OnPlayerDead;
        }
    }
}
