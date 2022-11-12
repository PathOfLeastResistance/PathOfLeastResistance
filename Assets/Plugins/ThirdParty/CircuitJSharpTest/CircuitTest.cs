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
    private DiodeElm _diode;

    private CanvasTexture _canvasTexture;
    private bool _isReady;
    private Queue<VoltagePoint> _queue = new Queue<VoltagePoint>();

    // Init Circuit
    private async void Start()
    {
        await MaterialProvider.Initialization;
        _isReady = true;

        _canvasTexture = new CanvasTexture();
        _canvasTexture.Init(256, 128);
        _canvasTexture.AspectSettings.Aspect = 0.5f;
        _renderer.material.mainTexture = _canvasTexture.RenderTexture;

        _cirSim = new CirSim();
        _cirSim.init();

        //Create some test elements
        _resistor = new ResistorElm(2, 1);
        _voltage = new VoltageElm(0, 2, WaveForm.WF_AC);
        _diode = new DiodeElm(0, 1);
        
        _cirSim.AddElement(_resistor);
        _cirSim.AddElement(_voltage);
        _cirSim.AddElement(_diode);

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
            var startFrom = _queue.Peek().Time;
            _canvasTexture.ClearWithColor(Color.white);
            _canvasTexture.DrawPolyLine(_queue.Select(x => new float2(x.Time - startFrom, 0.5f + x.Voltage / 10f)).ToList(), 0.05f, Color.red);
            _canvasTexture.Flush();
        }
    }
    
    private void Update()
    {
        if (!_isReady)
            return;
        _cirSim.timeDelta = Time.deltaTime;
        _cirSim.updateCircuit();
    }
}