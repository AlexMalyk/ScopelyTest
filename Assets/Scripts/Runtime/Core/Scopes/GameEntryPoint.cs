using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace TowerDefence.Runtime.Core.Scopes
{
    public class GameEntryPoint : IStartable
    {
        private const string SceneToLoad = "MainMenu";
        
        private readonly SceneLoader _sceneLoader;

        public GameEntryPoint(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        void IStartable.Start()
        {
            _sceneLoader.LoadScene(SceneToLoad, LoadSceneMode.Single);
        }
    }
}