﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Ins;
    public static int CHARACTERS_COUNT_MAX = 100;

    public enum State
    {
        SelectingArea,
        Game,
        Dead,
    }

    public State gameState;

    public static event Action OnStartGame;

    void Awake()
    {
        Ins = this;
        gameState = State.SelectingArea;
        World.OnPlayerSpawn += World_OnPlayerSpawn;
    }

    void World_OnPlayerSpawn(Player player)
    {
        Player.Ins.characterBase.OnDie += OnPlayerDead;
    }

    void OnPlayerDead(LivingEntity characterBase)
    {
        DeadScreen.Show(((CharacterBase)characterBase).killsCount, World.Ins.allCharacters.Count, 15);
        gameState = State.Dead;
    }

    void OnDestroy()
    {
        World.OnPlayerSpawn -= World_OnPlayerSpawn;

        if (Player.Ins != null)
        {
            Player.Ins.characterBase.OnDie -= OnPlayerDead;
        }
    }

    public void StartGame()
    {
        if (OnStartGame != null)
            OnStartGame();
    }
}