using UnityEditor;
using UnityEngine;

public class DialogueContainer : ScriptableObject, IExecutableObject
{
    [System.Serializable]
    public class Choice
    {
        public Phrase Phrase;
        public Object End;
    }

    [SerializeField] private Phrase[] m_Phrases = default;
    [SerializeField] private Choice[] m_Endings = default;

    public Phrase[] Phrases => m_Phrases;
    public Choice[] Endings => m_Endings;

#if UNITY_EDITOR
    private void OnValidate()
    {
        foreach (var choice in m_Endings)
            ValidateEnd(ref choice.End);
    }

    private void ValidateEnd(ref Object end)
    {
        if (end == null)
            return;

        switch (end)
        {
            case SceneAsset scene:
                EditorHelper.AddScene(scene, this);
                break;

            case DialogueContainer _:
                break;

            default:
                EditorHelper.Erase(ref end, this);
                Debug.LogWarning($"Incorrect object type. Alowed types:\n{typeof(SceneAsset)}\n{typeof(DialogueContainer)}\n");
                break;
        }
    }
#endif
}
