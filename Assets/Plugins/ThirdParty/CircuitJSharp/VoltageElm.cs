/*    
    Copyright (C) Paul Falstad and Iain Sharp
    
    This file is part of CircuitJS1.

    CircuitJS1 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 2 of the License, or
    (at your option) any later version.

    CircuitJS1 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with CircuitJS1.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;

namespace CircuitJSharp
{
    public enum WaveForm
    {
        WF_DC = 0,
        WF_AC = 1,
        WF_SQUARE = 2,
        WF_TRIANGLE = 3,
        WF_SAWTOOTH = 4,
        WF_PULSE = 5,
        WF_NOISE = 6,
        WF_VAR = 7,
    }

    public class VoltageElm : TwoNodeElm
    {
        public static int FLAG_COS = 2;
        public static int FLAG_PULSE_DUTY = 4;
        public WaveForm waveform;

        private double frequency;
        private double maxVoltage;
        private double freqTimeZero;
        private double bias;
        private double phaseShift;
        private double dutyCycle;
        private double noiseValue;

        protected static double defaultPulseDuty = 1 / (2 * Math.PI);

        public VoltageElm(int xx, int yy, WaveForm wf) : base(xx, yy)
        {
            waveform = wf;
            maxVoltage = 5;
            frequency = 5;
            dutyCycle = .5;
            reset();
        }

        public override void reset()
        {
            freqTimeZero = 0;
            curcount = 0;
        }

        double triangleFunc(double x)
        {
            if (x < pi)
                return x * (2 / pi) - 1;
            return 1 - (x - pi) * (2 / pi);
        }

        public int getVoltageSource()
        {
            return voltSource;
        }

        public override void stamp()
        {
            if (waveform == WaveForm.WF_DC)
                sim.stampVoltageSource(nodes[0], nodes[1], voltSource,
                    getVoltage());
            else
                sim.stampVoltageSource(nodes[0], nodes[1], voltSource);
        }

        public override void doStep()
        {
            if (waveform != WaveForm.WF_DC)
                sim.updateVoltageSource(nodes[0], nodes[1], voltSource,
                    getVoltage());
        }

        public override void stepFinished()
        {
            if (waveform == WaveForm.WF_NOISE)
                noiseValue = (sim.random.NextDouble() * 2 - 1) * maxVoltage + bias;
        }

        protected double getVoltage()
        {
            if (waveform != WaveForm.WF_DC && sim.dcAnalysisFlag)
                return bias;

            double w = 2 * pi * (sim.t - freqTimeZero) * frequency + phaseShift;
            switch (waveform)
            {
                case WaveForm.WF_DC: return maxVoltage + bias;
                case WaveForm.WF_AC: return Math.Sin(w) * maxVoltage + bias;
                case WaveForm.WF_SQUARE:
                    return bias + ((w % (2 * pi) > (2 * pi * dutyCycle)) ? -maxVoltage : maxVoltage);
                case WaveForm.WF_TRIANGLE:
                    return bias + triangleFunc(w % (2 * pi)) * maxVoltage;
                case WaveForm.WF_SAWTOOTH:
                    return bias + (w % (2 * pi)) * (maxVoltage / pi) - maxVoltage;
                case WaveForm.WF_PULSE:
                    return ((w % (2 * pi)) < (2 * pi * dutyCycle)) ? maxVoltage + bias : bias;
                case WaveForm.WF_NOISE:
                    return noiseValue;
                default: return 0;
            }
        }

        public override int getVoltageSourceCount() => 1;

        public override double getPower() => -getVoltageDiff() * current;

        public override double getVoltageDiff() => volts[1] - volts[0];
    }
}