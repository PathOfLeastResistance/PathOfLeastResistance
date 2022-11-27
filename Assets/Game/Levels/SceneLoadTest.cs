using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SceneLoadTest : MonoBehaviour
{
    [Inject] private Level.Factory m_levelFactory;
    [Inject] private ConnectionManager m_connectionManager;
    [Inject] private ILevelsProvider m_LevelsProvider;

    [SerializeField] private int m_level = 0;
    
    //We try to load and unload levels here. then we check that everything is ok in the scene.
    private void Start()
    {
        for (int i = 0; i < m_LevelsProvider.LevelsCount; i++)
        {
            var level = m_levelFactory.Create(i);
            level.Init();
            Debug.Log("Init: " + m_connectionManager.Sim.ElementsCount);
            level.Deinit();
            Destroy(level.gameObject);
            Debug.Log("Deinit: " + m_connectionManager.Sim.ElementsCount);
        }

        var finalTestLevel = m_levelFactory.Create(m_level);
        finalTestLevel.Init();
    }
}