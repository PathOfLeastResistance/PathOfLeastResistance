using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PID_Controller
{
    public class DoublePidController : MultiDimensionPidController
    {
        public DoublePidController(PidSettings settings) : base(new[] { settings }, 1)
        {
        }

        public double Iterate(double target, double observed, float deltaTime)
        {
            SetTarget(target, 0);
            SetObserved(observed, 0);

            Iterate(deltaTime);
            return GetOutput(0);
        }
    }
}

