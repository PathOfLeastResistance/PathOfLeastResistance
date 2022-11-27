using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PID_Controller
{
    public class Vector3PidController : MultiDimensionPidController
    {
        public Vector3PidController(PidSettings settings) : base(new[] { settings, settings, settings }, 3)
        {
        }

        public Vector3 Iterate(Vector3 target, Vector3 observed, float deltaTime)
        {
            for (int i = 0; i < m_dimension; i++)
            {
                SetTarget(target[i], i);
                SetObserved(observed[i], i);
            }

            Iterate(deltaTime);
            Vector3 result = Vector3.zero;
            return new Vector3((float)GetOutput(0), (float)GetOutput(1), (float)GetOutput(2));
        }
    }
}