using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PID_Controller
{
    public class MultiDimensionPidController
    {
        protected readonly int m_dimension;

        protected readonly PID[] m_pids;
        protected readonly PidSettings[] m_pidSettings;
        protected readonly double[] m_iterationResult;
        protected readonly double[] m_observedValues;
        protected readonly double[] m_targetValues;

        public int Dimension => m_dimension;

        public void SetTarget(double value, int dimension) => m_targetValues[dimension] = value;

        public void SetObserved(double value, int dimension) => m_observedValues[dimension] = value;

        public double GetOutput(int dimension) => m_iterationResult[dimension];

        public MultiDimensionPidController(PidSettings[] settings, int dimension)
        {
            if (settings.Length != dimension)
                throw new Exception("Incorrect number of PID settings");

            m_dimension = dimension;

            m_iterationResult = new double[m_dimension];
            m_observedValues = new double[m_dimension];
            m_targetValues = new double[m_dimension];
            m_pidSettings = new PidSettings[m_dimension];
            m_pids = new PID[m_dimension];

            for (int i = 0; i < m_dimension; i++)
            {
                var pidSettings = settings[i];
                m_pidSettings[i] = pidSettings;
                m_pids[i] = new PID(pidSettings.Kp, pidSettings.Ki, pidSettings.Kd, pidSettings.N, pidSettings.OutputUpperLimit, pidSettings.OutputLowerLimit);
            }
        }

        public void SetSettings(PidSettings settings, bool reset = true)
        {
            for (int i = 0; i < m_dimension; i++)
                SetSettings(settings, i, reset);
        }

        public void SetSettings(PidSettings settings, int dimension, bool reset = true)
        {
            m_pidSettings[dimension] = settings;
            var pid = m_pids[dimension];
            pid.Kd = settings.Kd;
            pid.Ki = settings.Ki;
            pid.Kp = settings.Kp;
            pid.N = settings.N;
            pid.OutputUpperLimit = settings.OutputUpperLimit;
            pid.OutputLowerLimit = settings.OutputLowerLimit;

            if (reset)
                ResetDimension(dimension);
        }

        public void Iterate(double deltaTime)
        {
            for (int i = 0; i < m_dimension; i++)
                m_iterationResult[i] = m_pids[i].PID_iterate(m_targetValues[i], m_observedValues[i], deltaTime);
        }

        public void ResetDimension(int dimension)
        {
            m_pids[dimension].ResetController();
        }

        public void ResetAllDimensions()
        {
            for (int i = 0; i < m_dimension; i++)
                ResetDimension(i);
        }
    }
}