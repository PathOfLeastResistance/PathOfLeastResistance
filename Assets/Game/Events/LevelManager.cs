using UnityEngine;
using Zenject;

public class LevelManager : MonoBehaviour, IEventResolver<Level>
{
    private Level m_CurrentLevel;

    [Inject] private ILevelsProvider m_LevelsProvider = default;
    [Inject] private Level.Factory m_LevelFactory = default;
    [Inject] private UIManager m_UIManager = default;
    [Inject] private EventManager m_EventManager = default;

    [SerializeField] private Transform m_Root = default;

    private void Awake()
    {
        m_UIManager.OnNextLevel += OnNextLevel;
    }

    public void Resolve(Level state)
    {
        var number = m_LevelsProvider.GetLevelNumber(state);
        if (number == -1)
            Debug.LogError($"No prefab for level {state}", state);

        var level = m_LevelFactory.Create(number);
        level.transform.SetParent(m_Root);
        level.Init();
        level.LevelCompletedValidator.OnLevelCompleted += OnLevelCompleted;
        m_UIManager.ShowObjective(level.ObjectivePhrase);
        m_CurrentLevel = level;
    }

    private void OnLevelCompleted()
    {
        m_UIManager.LevelCompleted(m_CurrentLevel.EndingPhrase);
        m_CurrentLevel.LevelCompletedValidator.OnLevelCompleted -= OnLevelCompleted;
    }

    private void OnNextLevel()
    {
        m_UIManager.LevelAbandoned();
        m_CurrentLevel.Deinit(); //Destroy
        var next = m_CurrentLevel.OnEnd;
        m_CurrentLevel = null;
        m_EventManager.Proceed(next);
    }

    private void OnDestroy()
    {
        m_UIManager.OnNextLevel -= OnNextLevel;
    }
}
