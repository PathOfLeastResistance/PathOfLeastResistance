using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PID_Controller
{
    public class Vector2PidController : MultiDimensionPidController
    {
        public Vector2PidController(PidSettings settings) : base(new[] { settings, settings }, 2)
        {
        }

        public Vector2 Iterate(Vector2 target, Vector2 observed, float deltaTime)
        {
            for (int i = 0; i < m_dimension; i++)
            {
                SetTarget(target[i], i);
                SetObserved(observed[i], i);
            }

            Iterate(deltaTime);
            Vector2 result = Vector2.zero;
            return new Vector2((float)GetOutput(0), (float)GetOutput(1));
        }
    }
}