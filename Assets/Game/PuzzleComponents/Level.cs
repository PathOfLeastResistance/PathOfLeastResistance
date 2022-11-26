using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private CircuitComponent[] m_circuitComponents;
    private TestSchemeBuilder m_SchemeBuilder;
    
    private void Init()
    {
        //The components should be initialized first
        m_circuitComponents = GetComponentsInChildren<CircuitComponent>();
        m_SchemeBuilder = GetComponent<TestSchemeBuilder>();
        foreach (var circuitComponent in m_circuitComponents)
            circuitComponent.Init();

        //And then we connect them
        m_SchemeBuilder.InitComponent();
    }
    
    private void Start()
    {
        Init();
    }
}
