using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static void Invoke(MonoBehaviour obj, float seconds, Action action, bool realtime = false)
    {
        obj.StartCoroutine(InvokeCoroutine(seconds, action, realtime));
    }

    static IEnumerator InvokeCoroutine(float seconds, Action action, bool realtime = false)
    {
        if (!realtime)
        {
            yield return new WaitForSeconds(seconds);
        }
        else
        {
            yield return new WaitForSecondsRealtime(seconds);
        }

        if (action != null)
            action.Invoke();

    }
}
