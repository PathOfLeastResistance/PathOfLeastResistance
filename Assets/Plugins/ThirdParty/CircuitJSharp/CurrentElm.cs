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
    class CurrentElm : CircuitElm
    {
        double currentValue;
        bool broken;

        public CurrentElm(int xx, int yy) : base(xx, yy)
        {
            currentValue = .01;
        }

        // analyzeCircuit determines if current source has a path or if it's broken
        public void setBroken(bool b)
        {
            broken = b;
        }

        // we defer stamping current sources until we can tell if they have a current path or not
        public override void stamp()
        {
            if (broken)
            {
                // no current path; stamping a current source would cause a matrix error.
                sim.stampResistor(nodes[0], nodes[1], 1e8);
                current = 0;
            }
            else
            {
                // ok to stamp a current source
                sim.stampCurrentSource(nodes[0], nodes[1], currentValue);
                current = currentValue;
            }
        }

        public override double getVoltageDiff()
        {
            return volts[1] - volts[0];
        }

        public override double getPower()
        {
            return -getVoltageDiff() * current;
        }
    }
}