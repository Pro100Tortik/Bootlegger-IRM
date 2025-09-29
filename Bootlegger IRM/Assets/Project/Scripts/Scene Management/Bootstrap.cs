using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bootlegger
{
    public class Bootstrap
    {
        public static int ActiveSceneBuildIndex = -1;

        /// <summary>
        /// Ensure that bootstrap scene is always loaded
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LoadStartScene()
        {
            ActiveSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;

            if (!SceneManager.GetSceneByBuildIndex(0).isLoaded && ActiveSceneBuildIndex != 0)
                SceneManager.LoadScene(0);
        }
    }
}
