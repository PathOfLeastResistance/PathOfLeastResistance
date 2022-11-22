using UnityEngine;

public class DialogueSystemInstaller : MonoExtendedInstaller
{
    [SerializeField] private ExecutableScriptableObject m_ExecutableScriptableObject = default;

    public override void InstallBindings()
    {
        BindSingle<TransitorToNextScene>();
        BindSingle<DialogueManager>();

        BindScriptableObject(m_ExecutableScriptableObject);

        BindFromHierarchy<DialoguePanel>();

        BindMonoBehaviour<DialogueStarter>().NonLazy();
    }
}
