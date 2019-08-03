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

    public static void LerpSprite(MonoBehaviour obj, SpriteRenderer spriteRenderer, float delay = 2, float fadeTime = 2, Color fadeColor = default, Action OnEndFade = null, bool realtime = false, bool fixedUpdate = false)
    {
        LerpSprite(obj, new SpriteRenderer[] { spriteRenderer }, delay, fadeTime, fadeColor, OnEndFade, realtime, fixedUpdate);
    }

    public static void LerpSprite(MonoBehaviour obj, SpriteRenderer[] spriteRenderers, float delay = 2, float fadeTime = 2, Color fadeColor = default, Action OnEndFade = null, bool relatime = false, bool fixedUpdate = false)
    {
        obj.StartCoroutine(LerpSpriteCorutine(spriteRenderers, delay, fadeTime, fadeColor, OnEndFade, relatime, fixedUpdate));
    }

    public static IEnumerator LerpSpriteCorutine(SpriteRenderer[] spriteRenderers, float delay = 2, float fadeTime = 2, Color fadeColor = default, Action OnEndFade = null, bool realtime = false, bool fixedUpdate = false)
    {
        if (!realtime)
            yield return new WaitForSeconds(delay);
        else
            yield return new WaitForSecondsRealtime(delay);

        float curFadeTime = fadeTime;

        while (curFadeTime > 0)
        {
            if (!fixedUpdate)
                curFadeTime -= Time.deltaTime;
            else
                curFadeTime -= Time.fixedDeltaTime;

            foreach (SpriteRenderer item in spriteRenderers)
            {
                Color color = Color.Lerp(fadeColor, item.color, curFadeTime / fadeTime);

                if (curFadeTime <= 0.05f)
                {
                    color = fadeColor;
                    curFadeTime = 0;
                }

                Debug.Log(color);

                item.color = color;
            }

            if (!fixedUpdate)
                yield return null;
            else yield return new WaitForFixedUpdate();

        }

        if (OnEndFade != null)
            OnEndFade();
    }


}
