using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface ILevelsProvider
{
    Level GetLevelPrefab(int levelNumber);
    int GetLevelNumber(Level level);
    int LevelsCount { get; }
}

public class LevelFactory : IFactory<int, Level>
{
    [Inject] ILevelsProvider m_levelsProvider;
    [Inject] DiContainer m_container;

    public Level Create(int param)
    {
        return m_container.InstantiatePrefabForComponent<Level>(m_levelsProvider.GetLevelPrefab(param));
    }
}

public class Level : MonoBehaviour, IEventState
{
    public class Factory : PlaceholderFactory<int, Level>
    {
    }

    [Inject] private ConnectionManager m_connectionsManager;
    
    private CircuitComponent[] m_circuitComponents;
    private TestSchemeBuilder[] m_SchemeBuilders;
    private ElementsHider m_ElementsHider;
    private LevelCompletedValidator m_LevelCompletedValidator;

    private bool m_wasInited = false;

    [SerializeField] private UnityEngine.Object m_OnEnd = default;
    [SerializeField] private Phrase m_ObjectivePhrase = default;
    [SerializeField] private Phrase m_EndingPhrase = default;

    public LevelCompletedValidator LevelCompletedValidator => m_LevelCompletedValidator;

    public UnityEngine.Object OnEnd => m_OnEnd;
    public Phrase ObjectivePhrase => m_ObjectivePhrase;
    public Phrase EndingPhrase => m_EndingPhrase;

    public void Init()
    {
        if (m_wasInited)
            return;
        
        m_connectionsManager.Reinit();

        //The components should be initialized first
        m_circuitComponents = GetComponentsInChildren<CircuitComponent>();
        foreach (var circuitComponent in m_circuitComponents)
            circuitComponent.Init();

        //And then we connect them
        m_SchemeBuilders = GetComponents<TestSchemeBuilder>();
        foreach (var builder in m_SchemeBuilders)
            builder.CreateConnections();

        //Now hide things we don't need to see
        m_ElementsHider = GetComponent<ElementsHider>();
        m_ElementsHider.Hide();

        //Reset level validator
        m_LevelCompletedValidator = GetComponent<LevelCompletedValidator>();
        m_LevelCompletedValidator.Reset();

        m_wasInited = true;
    }

    public void Deinit()
    {
        if (!m_wasInited)
            return;

        foreach (var component in m_circuitComponents)
            component.Dispose();
        m_circuitComponents = Array.Empty<CircuitComponent>(); 
        m_connectionsManager.Reinit();
        Destroy(gameObject);
    }
}