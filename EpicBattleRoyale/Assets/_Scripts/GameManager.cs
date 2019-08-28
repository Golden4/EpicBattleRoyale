using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class GameManager : SingletonResourse<GameManager>
{
    public GameObject consoleDebugger;

    public override void OnInit()
    {
        DontDestroyOnLoad(gameObject);

        if (Debug.isDebugBuild)
        {
            consoleDebugger = Instantiate(consoleDebugger);
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }

        // if (Application.isMobilePlatform)
        // {
        //     CrossPlatformInputManager.SwitchActiveInputMethod(CrossPlatformInputManager.ActiveInputMethod.Touch);
        // }
        // else
        // {

        //     CrossPlatformInputManager.SwitchActiveInputMethod(CrossPlatformInputManager.ActiveInputMethod.Hardware);
        // }
    }
}
