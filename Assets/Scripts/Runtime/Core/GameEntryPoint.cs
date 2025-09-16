using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace TowerDefence.Runtime.Core
{
    public class GameEntryPoint : IStartable
    {
        private const string SceneToLoad = "MainMenu";
        
        void IStartable.Start()
        {
            SceneManager.LoadScene(SceneToLoad, LoadSceneMode.Single);
        }
    }
}