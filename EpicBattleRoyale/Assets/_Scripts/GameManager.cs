using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject consoleDebugger;

    void Awake()
    {
        if (!Debug.isDebugBuild)
        {
            Destroy(consoleDebugger);
        }
        else
        {
            consoleDebugger.gameObject.SetActive(true);
        }
    }
}
