using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer.Unity;

namespace TowerDefence.Runtime.Core.Scopes
{
    public class MainMenuEntryPoint : IStartable
    {
        private readonly Button _button;
        private readonly SceneLoader _sceneLoader;

        public MainMenuEntryPoint(Button button, SceneLoader sceneLoader)
        {
            _button = button;
            _sceneLoader = sceneLoader;
        }

        void IStartable.Start()
        {
            _button.onClick.AddListener(PlayBattle);
        }

        private void PlayBattle()
        {
            _sceneLoader.LoadScene("Battle", LoadSceneMode.Single);
        }
    }
}
