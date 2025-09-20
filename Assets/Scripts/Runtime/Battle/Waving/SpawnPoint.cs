using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private Transform _cachedTransform;
    
    public Transform CachedTransform => _cachedTransform;
    
    private void Awake()
    {
        _cachedTransform = transform;
    }
}
