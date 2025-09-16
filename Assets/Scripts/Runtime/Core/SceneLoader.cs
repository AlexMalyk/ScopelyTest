using UnityEngine.SceneManagement;

namespace TowerDefence.Runtime.Core
{
    public class SceneLoader
    {
        public void LoadScene(string sceneName, LoadSceneMode mode)
        {
            SceneManager.LoadScene(sceneName, mode);
        }
    }
}
