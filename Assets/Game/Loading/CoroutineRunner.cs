using UnityEngine;

public class CoroutineRunner : MonoBehaviour, ICoroutineRunner
{
#if UNITY_EDITOR
    private void Start() =>
        gameObject.name = nameof(CoroutineRunner);
#endif
}
