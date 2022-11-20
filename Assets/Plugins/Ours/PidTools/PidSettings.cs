using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PID_Controller
{
    [Serializable]
    public struct PidSettings
    {
        [SerializeField] private double m_kp;
        [SerializeField] private double m_ki;
        [SerializeField] private double m_kd;
        [SerializeField] private double m_n;
        [SerializeField] private double m_outputUpperLimit;
        [SerializeField] private double m_outputLowerLimit;

        public static PidSettings Default => new PidSettings
        {
            m_kp = 1.0,
            m_ki = 1.0,
            m_kd = 1.0,
            m_n = 1.0,
            m_outputUpperLimit = double.PositiveInfinity,
            m_outputLowerLimit = double.NegativeInfinity,
        };

        public double Kp => m_kp;
        public double Ki => m_ki;
        public double Kd => m_kd;
        public double N => m_n;
        public double OutputUpperLimit => m_outputUpperLimit;
        public double OutputLowerLimit => m_outputLowerLimit;
    }
}