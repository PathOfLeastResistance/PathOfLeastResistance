using UnityEngine;

public class DialogueSystemInstaller : MonoExtendedInstaller
{
    [SerializeField] private DialogueContainer m_InitialDialogue = default;

    public override void InstallBindings()
    {
        BindSingle<EventManager>();

        BindScriptableObject(m_InitialDialogue);

        BindFromHierarchy<CharacterManager>();
        BindFromHierarchy<BackgroundManager>();
        BindFromHierarchy<DialogueManager>();
        BindFromHierarchy<UIManager>();
        BindFromHierarchy<LevelManager>();

        BindMonoBehaviour<EventStarter>().NonLazy();
    }
}
