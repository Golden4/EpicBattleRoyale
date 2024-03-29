﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEntity : LivingEntity
{
    public GameAssets.WorldEntityList entityName;

    public virtual void Setup(Vector3 position)
    {
        Init();
        InitRenederers();
        MoveTo(position, 0);
    }
}
