using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private CircuitComponent[] m_circuitComponents;
    private TestSchemeBuilder m_SchemeBuilder;
    private ElementsHider m_ElementsHider;
    
    private void Init()
    {
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
    }
    
    private void Start()
    {
        Init();
    }
}
