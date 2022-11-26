using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private CircuitComponent[] m_circuitComponents;
    private TestSchemeBuilder m_SchemeBuilder;
    private ElementsHider m_ElementsHider;
    private LevelCompletedValidator m_LevelCompletedValidator;

    private bool m_wasInited = false;
    
    public void Init()
    {
        if (m_wasInited)
            return;
        
        //The components should be initialized first
        m_circuitComponents = GetComponentsInChildren<CircuitComponent>();
        foreach (var circuitComponent in m_circuitComponents)
            circuitComponent.Init();

        //And then we connect them
        m_SchemeBuilder = GetComponent<TestSchemeBuilder>();
        m_SchemeBuilder.CreateConnections();
        
        //Now hide things we don't need to see
        m_ElementsHider = GetComponent<ElementsHider>();
        m_ElementsHider.Hide();

        //Reset level validator
        m_LevelCompletedValidator = GetComponent<LevelCompletedValidator>();
        m_LevelCompletedValidator.Reset();

        m_wasInited = true;
    }
    
    private void Start()
    {
        Init();
    }
}
