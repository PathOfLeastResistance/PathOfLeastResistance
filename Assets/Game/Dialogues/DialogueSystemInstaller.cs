using UnityEngine;

public class DialogueSystemInstaller : MonoExtendedInstaller
{
    [SerializeField] private DialogueContainer m_InitialDialogue = default;

    public override void InstallBindings()
    {
        BindSingle<TransitorToNextScene>();
        BindSingle<EventManager>();

        BindScriptableObject(m_InitialDialogue);

        BindFromHierarchy<DialogueManager>();
        BindFromHierarchy<DialogueBackgroundManager>();

        BindMonoBehaviour<EventStarter>().NonLazy();
    }
}
