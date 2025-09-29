using UnityEngine.SceneManagement;
using Eflatun.SceneReference;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using System;
using NUnit.Framework;
using UnityEngine.UIElements;

namespace Bootlegger
{
    public class SceneLoader : Singleton<SceneLoader>
    {
        [System.Serializable]
        private struct SceneGroup
        {
            public SceneType SceneType;
            public SceneReference[] ScenesToLoad;
        }

        public static event Action SceneLoaded;

        public static bool IsSceneLoading { get; private set; } = false;

        [Header("Scene Loader Settings")]
        [SerializeField] private SceneGroup[] sceneGroups;
        [SerializeField] private SceneReference[] dynamicBackgroundScenes;
        [SerializeField] private List<SceneReference> cantLoadScenes;

        [Header("UI")]
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0.1f, 1f, 1f);
        [SerializeField] private float fadeDuration = 0.3f;

        private void Start()
        {
            fadeCanvasGroup.alpha = 0f;
            IsSceneLoading = false;

            // Not Bootstrap and not Main Menu, load back on what scene we was
            // When launched we should continue on scene where we started
            if (Bootstrap.ActiveSceneBuildIndex != 0 && Bootstrap.ActiveSceneBuildIndex != 1)
            {
                LoadScene(Bootstrap.ActiveSceneBuildIndex, SceneType.Level, false);
            }
            else
            {
                LoadMainMenu();
            }
        }

        public void LoadLevel(string levelName, bool showLoadingScreen = true)
        {
            LoadScene(levelName, SceneType.Level, showLoadingScreen);
        }

        public void LoadMainMenu(bool showLoadingScreen = true, string backgroundMapName = "")
        {
            if (!string.IsNullOrEmpty(backgroundMapName))
            {
                LoadScene(backgroundMapName, SceneType.MainMenu, true);
                return;
            }

            // Get random scene for background

            if (dynamicBackgroundScenes.Length > 0)
            {
                LoadScene(dynamicBackgroundScenes.GetRandom().BuildIndex, SceneType.MainMenu, showLoadingScreen);
                return;
            }

            // Load empty scene if needed
            LoadScene(-1, SceneType.MainMenu, showLoadingScreen);
        }

        private void LoadScene(string sceneName, SceneType sceneType, bool showLoadingScreen = true)
        {
            if (IsSceneLoading)
                return;

            StartCoroutine(LoadScenes(GetSceneBuildIndexByName(sceneName), sceneType, showLoadingScreen));
        }

        private void LoadScene(int buildIndex, SceneType sceneType, bool showLoadingScreen = true)
        {
            if (IsSceneLoading)
                return;

            StartCoroutine(LoadScenes(buildIndex, sceneType, showLoadingScreen));
        }

        private IEnumerator LoadScenes(int buildIndex, SceneType sceneType, bool showLoadingScreen = true)
        {
            // Stop if trying to load forbidden scene
            if (cantLoadScenes.Find(scene => scene.BuildIndex == buildIndex) != null)
            {
                Debug.LogWarning($"Can't load '{SceneManager.GetSceneByBuildIndex(buildIndex)}'");
                yield break;
            }

            IsSceneLoading = true;

            if (showLoadingScreen)
                yield return Fade(1f);

            SceneGroup sceneGroup = sceneGroups.FirstOrDefault(group => group.SceneType == sceneType);

            yield return LoadScenes(buildIndex, sceneGroup);

            SceneLoaded?.Invoke();
            IsSceneLoading = false;

            if (showLoadingScreen)
                yield return Fade(0f);
        }

        private IEnumerator LoadScenes(int buildIndex, SceneGroup sceneGroup)
        {
            yield return UnloadScenes();

            AsyncOperation operation;

            int sceneCount = sceneGroup.ScenesToLoad.Length;

            for (int i = 0; i < sceneCount; i++)
            {
                operation = SceneManager.LoadSceneAsync(sceneGroup.ScenesToLoad[i].BuildIndex, LoadSceneMode.Additive);

                yield return new WaitUntil(() => operation.isDone);
            }

            // Can't load scene that doesn't exist
            if (buildIndex < 0)
                yield break;

            operation = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);

            yield return new WaitUntil(() => operation.isDone);

            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(buildIndex));
        }

        private IEnumerator UnloadScenes()
        {
            int currentSceneCount = SceneManager.sceneCount;
            int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;

            for (int i = currentSceneCount - 1; i > 0 ; i--)
            {
                Scene sceneAt = SceneManager.GetSceneAt(i);

                if (sceneAt.isLoaded == false)
                    continue;

                int sceneIndex = sceneAt.buildIndex;

                // Bootstrap scene
                if (sceneIndex == 0)
                    continue;

                var operation = SceneManager.UnloadSceneAsync(sceneIndex);

                yield return new WaitUntil(() => operation.isDone);
            }

            // If for some unknown reason active scene was not unloaded
            if (SceneManager.GetSceneByBuildIndex(activeSceneIndex).isLoaded && activeSceneIndex != 0)
            {
                var operation = SceneManager.UnloadSceneAsync(activeSceneIndex);

                yield return new WaitUntil(() => operation.isDone);
            }

            yield return Resources.UnloadUnusedAssets();
        }

        private IEnumerator Fade(float targetValue)
        {
            float startValue = fadeCanvasGroup.alpha;
            float time = 0f;

            while (time < fadeDuration)
            {
                // Pause shoud not interrupt fading
                time += Time.unscaledDeltaTime;
                float t = time / fadeDuration;
                fadeCanvasGroup.alpha = Mathf.Lerp(startValue, targetValue, fadeCurve.Evaluate(t));

                yield return null;
            }

            fadeCanvasGroup.alpha = targetValue;

            yield return null;
        }

        private int GetSceneBuildIndexByName(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string currentSceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

                if (currentSceneName == sceneName)
                {
                    return i;
                }
            }

            return -1; // Scene not found
        }
    }
}
