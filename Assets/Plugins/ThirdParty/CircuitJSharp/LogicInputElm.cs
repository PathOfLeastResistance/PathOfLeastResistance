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
    public class LogicInputElm : SwitchElm
    {
        static int FLAG_TERNARY = 1;
        static int FLAG_NUMERIC = 2;
        double hiV, loV;

        public LogicInputElm(int xx, int yy) : base(xx, yy)
        {
            hiV = 5;
            loV = 0;
        }

        private bool isTernary()
        {
            return (flags & FLAG_TERNARY) != 0;
        }

        public bool isNumeric()
        {
            return (flags & (FLAG_TERNARY | FLAG_NUMERIC)) != 0;
        }

        public override int getPostCount() => 1;

        public override void setCurrent(int vs, double c) => current = c;

        public override void calculateCurrent()
        {
        }

        public override void stamp() => sim.stampVoltageSource(0, nodes[0], voltSource);

        public override bool isWireEquivalent() => false;

        public override bool isRemovableWire() => false;

        public override void doStep()
        {
            double v = (position == 0) ? loV : hiV;
            if (isTernary())
                v = loV + position * (hiV - loV) * .5;
            sim.updateVoltageSource(0, nodes[0], voltSource, v);
        }

        public override int getVoltageSourceCount() => 1;

        public override double getVoltageDiff() =>  volts[0];

        public override bool hasGroundConnection(int n1) => true;

        public override double getCurrentIntoNode(int n) => current;
    }
}