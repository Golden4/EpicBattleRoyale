using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public static SceneController.Scene nextSceneToLoad = SceneController.Scene.Menu;

    public static bool sceneLoading;

    [SerializeField] UnityEngine.UI.Extensions.Gradient2 loadingText;

    float minLoadingTime = 2f;
    float loadingTime;
    public Slider loadingBar;

    void Awake()
    {
        GameManager.Ins.Awake();
    }

    public void Start()
    {
        StartCoroutine(LoadingCoroutine((int)nextSceneToLoad, .5f));
    }

    IEnumerator LoadingCoroutine(int sceneIndex, float time)
    {
        float offset = 1;

        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneIndex);
        ao.allowSceneActivation = false;

        while (ao.progress <= .89f || loadingTime <= minLoadingTime)
        {
            loadingTime += Time.deltaTime;
            loadingBar.value = Mathf.Clamp01(loadingTime / minLoadingTime) * .9f;
            offset = Mathf.PingPong(Time.timeSinceLevelLoad / time, 1.6f) - .8f;
            loadingText.Offset = offset;
            yield return null;
        }

        loadingTime = 0;

        while (loadingTime >= .1f)
        {
            loadingTime += Time.deltaTime;
            loadingBar.value = .9f + Mathf.Clamp01(loadingTime / .1f) * .1f;
            yield return null;
        }

        loadingBar.value = 1;

        yield return null;

        ao.allowSceneActivation = true;
    }
}
