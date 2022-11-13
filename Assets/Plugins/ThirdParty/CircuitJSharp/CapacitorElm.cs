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
    class CapacitorElm : TwoNodeElm
    {
        private double capacitance;
        private double compResistance, voltdiff;
        private double initialVoltage;
        private double curSourceValue;

        public static int FLAG_BACK_EULER = 2;

        public CapacitorElm(int xx, int yy) : base(xx, yy)
        {
            capacitance = 1e-5;
            initialVoltage = 1e-3;
        }

        protected virtual bool isTrapezoidal()  => (flags & FLAG_BACK_EULER) == 0;

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
            double vDiff = volts[0] - volts[1];
            if (sim.dcAnalysisFlag)
            {
                current = vDiff / 1e8;
                return;
            }

            // we check compResistance because this might get called
            // before stamp(), which sets compResistance, causing
            // infinite current
            if (compResistance > 0)
                current = vDiff / compResistance + curSourceValue;
        }

        public override void doStep()
        {
            if (sim.dcAnalysisFlag)
                return;
            sim.stampCurrentSource(nodes[0], nodes[1], curSourceValue);
        }
    }
}