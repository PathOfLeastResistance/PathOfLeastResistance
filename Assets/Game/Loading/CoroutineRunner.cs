using UnityEngine;

public class CoroutineRunner : MonoBehaviour, ICoroutineRunner
{
    private void Awake() =>
        DontDestroyOnLoad(this);

#if UNITY_EDITOR
    private void Start() =>
        gameObject.name = nameof(CoroutineRunner);
#endif
}
