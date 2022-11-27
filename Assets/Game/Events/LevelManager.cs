using UnityEngine;
using Zenject;

public class LevelManager : MonoBehaviour, IEventResolver<Level>
{
    [Inject] private ILevelsProvider m_LevelsProvider = default;
    [Inject] private LevelFactory m_LevelFactory = default;

    [SerializeField] private Transform m_Root = default;

    public void Resolve(Level state)
    {
        var number = m_LevelsProvider.GetLevelNumber(state);
        if (number == -1)
            Debug.LogError($"No prefab for level {state}", state);

        var level = m_LevelFactory.Create(number);
        level.transform.SetParent(m_Root);
    }

    //TODO Validator
}
