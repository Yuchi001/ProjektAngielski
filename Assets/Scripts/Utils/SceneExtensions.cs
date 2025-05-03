using UnityEngine.SceneManagement;

namespace Utils
{
    public static class SceneExtensions
    {
        public static bool IsSceneLoaded(string sceneName)
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.name == sceneName) return true;
            }
            return false;
        }
        
        public static bool IsSceneLoaded(int id)
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.buildIndex == id) return true;
            }
            return false;
        }
    }
}