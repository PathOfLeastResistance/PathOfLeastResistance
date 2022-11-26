using UnityEngine;

public class DialogueSystemInstaller : MonoExtendedInstaller
{
    [SerializeField] private DialogueContainer m_InitialDialogue = default;

    public override void InstallBindings()
    {
        BindSingle<TransitorToNextScene>();
        BindSingle<DialogueManager>();

        BindScriptableObject(m_InitialDialogue);

        BindFromHierarchy<DialoguePanel>();

        BindMonoBehaviour<DialogueStarter>().NonLazy();
    }
}
