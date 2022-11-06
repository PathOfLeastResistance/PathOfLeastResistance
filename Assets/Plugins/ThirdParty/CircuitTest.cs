using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitTest : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    private CirSim _cirSim;
    
    // Init Circuit
    private void Start()
    {
        _cirSim = new CirSim();
        _cirSim.init();
        
        //Create some test elements
        var res = new ResistorElm(0, 1);
        var volt = new VoltageElm(0, 1, WaveForm.WF_DC);
    }

    // Update Circtuit
    private void Update()
    {
        _cirSim.updateCircuit();
    }
}
