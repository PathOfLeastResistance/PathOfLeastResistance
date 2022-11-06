using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitTest : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    private CirSim _cirSim;
    private ResistorElm _resistor;
    private VoltageElm _voltage;
    
    // Init Circuit
    private void Start()
    {
        _cirSim = new CirSim();
        _cirSim.init();
        
        //Create some test elements
        _resistor = new ResistorElm(0, 1);
        _voltage = new VoltageElm(0, 1, WaveForm.WF_AC);
        _cirSim.AddElement(_resistor);
        _cirSim.AddElement(_voltage);
    }

    // Update Circtuit
    private void Update()
    {
        _cirSim.timeDelta = Time.deltaTime;
        _cirSim.updateCircuit();
        Debug.Log(_resistor.getVoltageDiff());
    }
}
