using System;
using UnityEngine;

namespace TowerDefence.Runtime.Config
{
    [Serializable]
    public abstract class IdentifiableConfig : ScriptableObject
    {
        [Header("Identity")] 
        [SerializeField] private string _displayName;
        [SerializeField] private string _guidString;
        [SerializeField] private GameObject _prefab;

        private Guid? _cachedGuid;

        public string DisplayName => _displayName;
        public GameObject Prefab => _prefab;

        public Guid Id
        {
            get
            {
                if (_cachedGuid.HasValue) return _cachedGuid.Value;
                
                if (string.IsNullOrEmpty(_guidString))
                {
                    _guidString = Guid.NewGuid().ToString();
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(this);
#endif
                }

                _cachedGuid = Guid.Parse(_guidString);

                return _cachedGuid.Value;
            }
        }

        protected virtual void OnValidate()
        {
            if (!string.IsNullOrEmpty(_guidString)) return;
            
            _guidString = Guid.NewGuid().ToString();
        }
    }
}