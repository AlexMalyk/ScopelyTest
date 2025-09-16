#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TowerDefence.Editor
{
    [InitializeOnLoad]
    public static class PlayModeSceneLoader
    {
        const string ENTRY_SCENE_PATH = "Assets/Scenes/Entry.unity";
        const string LOAD_FROM_ENTRY_KEY = "_shouldLoadFromEntry";

        private const string MENU_PATH = "TowerDefence/Play from Entry scene";

        private static int _sceneID;
        private static bool _sceneValid;

        private static bool IsEnabled
        {
            get => EditorPrefs.GetBool(LOAD_FROM_ENTRY_KEY, true);
            set => EditorPrefs.SetBool(LOAD_FROM_ENTRY_KEY, value);
        }

        static PlayModeSceneLoader()
        {
            SetLoadingFromMain();
        }

        private static void SetLoadingFromMain()
        {
            if (!EditorPrefs.GetBool(LOAD_FROM_ENTRY_KEY, true))
            {
                EditorSceneManager.playModeStartScene = null;
                return;
            }

            Debug.Log($"Path: {ENTRY_SCENE_PATH}");
            var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(ENTRY_SCENE_PATH);

            Debug.Log($"Loaded: {(scene == null ? "NULL" : "Complete")}");

            EditorSceneManager.playModeStartScene = scene;
        }

        [MenuItem(MENU_PATH)]
        private static void ToggleAction()
        {
            IsEnabled = !IsEnabled;
        }
        
        [MenuItem(MENU_PATH, true)]
        private static bool ToggleActionValidate()
        {
            Menu.SetChecked(MENU_PATH, IsEnabled);

            return true;
        }
    }
}
#endif