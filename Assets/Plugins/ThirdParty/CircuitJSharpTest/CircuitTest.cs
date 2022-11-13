using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using ViJApps.CanvasTexture;
using CircuitJSharp;


public struct VoltagePoint
{
    public float Time;
    public float Voltage;
}

public class CircuitTest : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;

    private CirSim _cirSim;
    private ResistorElm _resistor;
    private VoltageElm _voltage;
    private DiodeElm[] _diodes;

    private CanvasTexture _canvasTexture;
    private bool _isReady;
    private Queue<VoltagePoint> _queue = new Queue<VoltagePoint>();

    private async void Start()
    {
        await MaterialProvider.Initialization;
        _isReady = true;

        _canvasTexture = new CanvasTexture();
        _canvasTexture.Init(128, 64);
        _canvasTexture.AspectSettings.Aspect = 0.5f;
        _renderer.material.mainTexture = _canvasTexture.RenderTexture;

        //Init cirsim
        _cirSim = new CirSim();
        _cirSim.init();

        //Create diode rectifier
        _resistor = new ResistorElm(1, 3, 500f);
        _voltage = new VoltageElm(0, 2, WaveForm.WF_AC);
        _cirSim.AddElement(_resistor);
        _cirSim.AddElement(_voltage);

        //Diode full rectifier
        _diodes = new DiodeElm[4];
        _diodes[0] = new DiodeElm(0, 1);
        _diodes[1] = new DiodeElm(2, 1);
        _diodes[2] = new DiodeElm(3, 2);
        _diodes[3] = new DiodeElm(3, 0);
        foreach (var diode in _diodes)
            _cirSim.AddElement(diode);

        _cirSim.OnTimeStepHook += OnTimeStep;
    }

    private double _lastT = double.NegativeInfinity;

    private void OnTimeStep()
    {
        if (_cirSim.t - _lastT > 0.01)
        {
            _lastT = _cirSim.t;
            var point = new VoltagePoint()
            {
                Time = (float)_cirSim.t,
                Voltage = (float)_resistor.getVoltageDiff(),
            };
            _queue.Enqueue(point);
            if (_queue.Count > 100)
                _queue.Dequeue();
        }
    }
    
    private void Update()
    {
        if (!_isReady)
            return;
        _cirSim.timeDelta = Time.deltaTime;
        _cirSim.updateCircuit();
        
        //Refresh screen
        var startFrom = _queue.Peek().Time;
        _canvasTexture.ClearWithColor(Color.white);
        _canvasTexture.DrawPolyLine(_queue.Select(x => new float2(x.Time - startFrom, 0.5f + x.Voltage / 10f)).ToList(), 0.025f, Color.red);
        _canvasTexture.Flush();
    }
}