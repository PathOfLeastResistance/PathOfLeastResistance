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

namespace CircuitJSharp
{
    class CapacitorElm : CircuitElm
    {
        double capacitance;
        double compResistance, voltdiff;
        double initialVoltage;
        public static int FLAG_BACK_EULER = 2;

        public CapacitorElm(int xx, int yy) : base(xx, yy)
        {
            capacitance = 1e-5;
            initialVoltage = 1e-3;
        }

        public CapacitorElm(int xa, int ya, int xb, int yb, int f,
            object st) : base(xa, ya, xb, yb, f)
        {
            // capacitance = new Double(st.nextToken()).doubleValue();
            // voltdiff = new Double(st.nextToken()).doubleValue();
            // initialVoltage = 1e-3;
            // try
            // {
            //     initialVoltage = new Double(st.nextToken()).doubleValue();
            // }
            // catch (Exception e)
            // {
            // }
        }

        bool isTrapezoidal()
        {
            return (flags & FLAG_BACK_EULER) == 0;
        }

        public override void reset()
        {
            base.reset();
            current = curcount = curSourceValue = 0;
            // put small charge on caps when reset to start oscillators
            voltdiff = initialVoltage;
        }

        public void shorted()
        {
            base.reset();
            voltdiff = current = curcount = curSourceValue = 0;
        }

        public override void stamp()
        {
            if (sim.dcAnalysisFlag)
            {
                // when finding DC operating point, replace cap with a 100M resistor
                sim.stampResistor(nodes[0], nodes[1], 1e8);
                curSourceValue = 0;
                return;
            }

            // capacitor companion model using trapezoidal approximation
            // (Norton equivalent) consists of a current source in
            // parallel with a resistor.  Trapezoidal is more accurate
            // than backward euler but can cause oscillatory behavior
            // if RC is small relative to the timestep.
            if (isTrapezoidal())
                compResistance = sim.timeStep / (2 * capacitance);
            else
                compResistance = sim.timeStep / capacitance;
            sim.stampResistor(nodes[0], nodes[1], compResistance);
            sim.stampRightSide(nodes[0]);
            sim.stampRightSide(nodes[1]);
        }

        public override void startIteration()
        {
            if (isTrapezoidal())
                curSourceValue = -voltdiff / compResistance - current;
            else
                curSourceValue = -voltdiff / compResistance;
        }

        public override void stepFinished()
        {
            voltdiff = volts[0] - volts[1];
            calculateCurrent();
        }

        public override void setNodeVoltage(int n, double c)
        {
            // do not calculate current, that only gets done in stepFinished().  otherwise calculateCurrent() may get
            // called while stamping the circuit, which might discharge the cap (since we use that current to calculate
            // curSourceValue in startIteration)
            volts[n] = c;
        }

        public override void calculateCurrent()
        {
            double voltdiff = volts[0] - volts[1];
            if (sim.dcAnalysisFlag)
            {
                current = voltdiff / 1e8;
                return;
            }

            // we check compResistance because this might get called
            // before stamp(), which sets compResistance, causing
            // infinite current
            if (compResistance > 0)
                current = voltdiff / compResistance + curSourceValue;
        }

        double curSourceValue;

        public override void doStep()
        {
            if (sim.dcAnalysisFlag)
                return;
            sim.stampCurrentSource(nodes[0], nodes[1], curSourceValue);
        }

        public override object getEditInfo(int n)
        {
            // if (n == 0)
            //     return new EditInfo("Capacitance (F)", capacitance, 1e-6, 1e-3);
            // if (n == 1)
            // {
            //     EditInfo ei = new EditInfo("", 0, -1, -1);
            //     ei.checkbox = new Checkbox("Trapezoidal Approximation", isTrapezoidal());
            //     return ei;
            // }
            //
            // if (n == 2)
            //     return new EditInfo("Initial Voltage (on Reset)", initialVoltage);
            // if you add more things here, check PolarCapacitorElm
            return null;
        }

        public override void setEditValue(int n, object ei)
        {
            // if (n == 0)
            //     capacitance = (ei.value > 0) ? ei.value : 1e-12;
            // if (n == 1)
            // {
            //     if (ei.checkbox.getState())
            //         flags &= ~FLAG_BACK_EULER;
            //     else
            //         flags |= FLAG_BACK_EULER;
            // }
            //
            // if (n == 2)
            //     initialVoltage = ei.value;
        }

        public double getCapacitance()
        {
            return capacitance;
        }

        public void setCapacitance(double c)
        {
            capacitance = c;
        }
    }
}