using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.UI.Extensions;

public class SceneController : SingletonResourse<SceneController>
{
    public enum Scene
    {
        Loading = 0,
        Menu = 1,
        Game = 2,
    }

    public Image fadeImage;
    public Gradient2 gradientText;

    public static bool sceneLoading;
    static float loadingTime;
    static float minLoadingTime = 1f;

    public static SceneController.Scene nextSceneToLoad;

    public static void LoadScene(Scene sceneToLoad, bool fade = true)
    {
        if (!sceneLoading)
        {
            nextSceneToLoad = sceneToLoad;
            Ins.StartCoroutine(LoadSceneCoroutine(sceneToLoad, fade));
        }
    }

    static IEnumerator LoadSceneCoroutine(SceneController.Scene sceneToLoad, bool fade)
    {
        sceneLoading = true;
        Ins.gradientText.gameObject.SetActive(false);

        if (fade)
        {
            Ins.FadeIn();
            yield return new WaitForSeconds(.6f);
            Ins.gradientText.gameObject.SetActive(true);
        }

        Ins.StartCoroutine(AnimateTextCoroutine());

        AsyncOperation ao = SceneManager.LoadSceneAsync((int)sceneToLoad);

        ao.allowSceneActivation = false;

        while (ao.progress <= .89f || loadingTime <= minLoadingTime)
        {
            loadingTime += Time.deltaTime;
            yield return null;
        }

        ao.allowSceneActivation = true;

        if (fade)
        {
            yield return new WaitForSeconds(.6f);
            Ins.gradientText.gameObject.SetActive(false);
            Ins.FadeOut();
        }

        sceneLoading = false;
    }

    static IEnumerator AnimateTextCoroutine()
    {
        while (Ins.gradientText.IsActive())
        {
            loadingTime += Time.deltaTime;
            Ins.gradientText.Offset = Mathf.PingPong(Time.timeSinceLevelLoad * 3, 1.6f) - .8f;
            yield return null;
        }
    }

    public void FadeIn(Action actionAfterFade = null, float duration = .5f, bool needFadeOut = false)
    {
        fadeImage.DOFade(1f, duration).SetEase(Ease.InQuad).OnComplete(delegate
        {
            if (actionAfterFade != null)
                actionAfterFade();

            if (needFadeOut)
                FadeOut();
        });
    }

    public void FadeOut(float duration = .5f)
    {
        fadeImage.DOFade(0f, duration).SetEase(Ease.OutQuad);
    }

    public override void OnInit()
    {
        DontDestroyOnLoad(gameObject);
    }
}
