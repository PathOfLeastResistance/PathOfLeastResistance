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

class VoltageElm : CircuitElm
{
    static int FLAG_COS = 2;
    static int FLAG_PULSE_DUTY = 4;
    public WaveForm waveform;

    double frequency,
        maxVoltage,
        freqTimeZero,
        bias,
        phaseShift,
        dutyCycle,
        noiseValue;

    static double defaultPulseDuty = 1 / (2 * Math.PI);

    public VoltageElm(int xx, int yy, WaveForm wf) : base(xx, yy)
    {
        waveform = wf;
        maxVoltage = 5;
        frequency = 40;
        dutyCycle = .5;
        reset();
    }

    public VoltageElm(int xa, int ya, int xb, int yb, int f, object st) : base(xa, ya, xb, yb, f)
    {
        maxVoltage = 5;
        frequency = 40;
        waveform = WaveForm.WF_DC;
        dutyCycle = .5;
        try
        {
            // waveform = new Integer(st.nextToken()).intValue();
            // frequency = new Double(st.nextToken()).doubleValue();
            // maxVoltage = new Double(st.nextToken()).doubleValue();
            // bias = new Double(st.nextToken()).doubleValue();
            // phaseShift = new Double(st.nextToken()).doubleValue();
            // dutyCycle = new Double(st.nextToken()).doubleValue();
        }
        catch (Exception e)
        {
        }

        if ((flags & FLAG_COS) != 0)
        {
            flags &= ~FLAG_COS;
            phaseShift = pi / 2;
        }

        // old circuit files have the wrong duty cycle for pulse waveforms (wasn't configurable in the past)
        if ((flags & FLAG_PULSE_DUTY) == 0 && waveform == WaveForm.WF_PULSE)
        {
            dutyCycle = defaultPulseDuty;
        }

        reset();
    }

    int getDumpType()
    {
        return 'v';
    }

    public override string dump()
    {
        // // set flag so we know if duty cycle is correct for pulse waveforms
        // if (waveform == WF_PULSE)
        //     flags |= FLAG_PULSE_DUTY;
        // else
        //     flags &= ~FLAG_PULSE_DUTY;
        //
        // return super.dump() + " " + waveform + " " + frequency + " " +
        //        maxVoltage + " " + bias + " " + phaseShift + " " +
        //        dutyCycle;
        // // VarRailElm adds text at the end
        return "";
    }

    void reset()
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

    int getVoltageSource()
    {
        return voltSource;
    }

    void stamp()
    {
        if (waveform == WaveForm.WF_DC)
            sim.stampVoltageSource(nodes[0], nodes[1], voltSource,
                getVoltage());
        else
            sim.stampVoltageSource(nodes[0], nodes[1], voltSource);
    }

    void doStep()
    {
        if (waveform != WaveForm.WF_DC)
            sim.updateVoltageSource(nodes[0], nodes[1], voltSource,
                getVoltage());
    }

    void stepFinished()
    {
        if (waveform == WaveForm.WF_NOISE)
            noiseValue = (sim.random.NextDouble() * 2 - 1) * maxVoltage + bias;
    }

    public double getVoltage()
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

    public override void setPoints()
    {
        base.setPoints();
    }

    int getVoltageSourceCount()
    {
        return 1;
    }

    double getPower()
    {
        return -getVoltageDiff() * current;
    }

    double getVoltageDiff()
    {
        return volts[1] - volts[0];
    }

    public override object getEditInfo(int n)
    {
        // if (n == 0)
        //     return new EditInfo(waveform == WF_DC ? "Voltage" : "Max Voltage", maxVoltage, -20, 20);
        // if (n == 1)
        // {
        //     EditInfo ei = new EditInfo("Waveform", waveform, -1, -1);
        //     ei.choice = new Choice();
        //     ei.choice.add("D/C");
        //     ei.choice.add("A/C");
        //     ei.choice.add("Square Wave");
        //     ei.choice.add("Triangle");
        //     ei.choice.add("Sawtooth");
        //     ei.choice.add("Pulse");
        //     ei.choice.add("Noise");
        //     ei.choice.select(waveform);
        //     return ei;
        // }
        //
        // if (n == 2)
        //     return new EditInfo("DC Offset (V)", bias, -20, 20);
        // if (waveform == WF_DC || waveform == WF_NOISE)
        //     return null;
        // if (n == 3)
        //     return new EditInfo("Frequency (Hz)", frequency, 4, 500);
        // if (n == 4)
        //     return new EditInfo("Phase Offset (degrees)", phaseShift * 180 / pi,
        //         -180, 180).setDimensionless();
        // if (n == 5 && (waveform == WF_PULSE || waveform == WF_SQUARE))
        //     return new EditInfo("Duty Cycle", dutyCycle * 100, 0, 100).setDimensionless();
        return null;
    }

    public void setEditValue(int n, object ei)
    {
        // if (n == 0)
        //     maxVoltage = ei.value;
        // if (n == 2)
        //     bias = ei.value;
        // if (n == 3)
        // {
        //     // adjust time zero to maintain continuity ind the waveform
        //     // even though the frequency has changed.
        //     double oldfreq = frequency;
        //     frequency = ei.value;
        //     double maxfreq = 1 / (8 * sim.maxTimeStep);
        //     if (frequency > maxfreq)
        //     {
        //         if (Window.confirm(Locale.LS("Adjust timestep to allow for higher frequencies?")))
        //             sim.maxTimeStep = 1 / (32 * frequency);
        //         else
        //             frequency = maxfreq;
        //     }
        //
        //     double adj = frequency - oldfreq;
        //     freqTimeZero = (frequency == 0) ? 0 : sim.t - oldfreq * (sim.t - freqTimeZero) / frequency;
        // }
        //
        // if (n == 1)
        // {
        //     int ow = waveform;
        //     waveform = ei.choice.getSelectedIndex();
        //     if (waveform == WF_DC && ow != WF_DC)
        //     {
        //         ei.newDialog = true;
        //         bias = 0;
        //     }
        //     else if (waveform != ow)
        //         ei.newDialog = true;
        //
        //     // change duty cycle if we're changing to or from pulse
        //     if (waveform == WF_PULSE && ow != WF_PULSE)
        //         dutyCycle = defaultPulseDuty;
        //     else if (ow == WF_PULSE && waveform != WF_PULSE)
        //         dutyCycle = .5;
        //
        //     setPoints();
        // }
        //
        // if (n == 4)
        //     phaseShift = ei.value * pi / 180;
        // if (n == 5)
        //     dutyCycle = ei.value * .01;
    }
}