using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneLoader : ISceneLoader
{
    [Inject] private readonly ICoroutineRunner m_CoroutineRunner = default;
    [Inject] private readonly ILoaderCurtain m_LoaderCurtain = default;

    public void Load(SceneAsset scene, Action onLoaded = null, bool useCurtain = true) =>
        m_CoroutineRunner.StartCoroutine(Load(scene.name, onLoaded, useCurtain));

    private IEnumerator Load(string nextScene, Action onLoaded = null, bool useCurtain = true)
    {
        if (SceneManager.GetActiveScene().name == nextScene)
        {
            onLoaded?.Invoke();
            yield break;
        }

        if (useCurtain)
            m_LoaderCurtain.Show();

        AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(nextScene);

        while (!waitNextScene.isDone)
        {
            if (useCurtain)
                m_LoaderCurtain.SetProgress(waitNextScene.progress);

            yield return null;
        }

        if (useCurtain)
            m_LoaderCurtain.Hide();

        onLoaded?.Invoke();
    }
}
