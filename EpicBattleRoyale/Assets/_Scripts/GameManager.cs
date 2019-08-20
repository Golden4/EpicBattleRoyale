using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonResourse<GameManager>
{
    public GameObject consoleDebugger;

    public override void OnInit()
    {
        DontDestroyOnLoad(gameObject);

        if (Debug.isDebugBuild)
        {
            consoleDebugger = Instantiate(consoleDebugger);
        }
    }
}
