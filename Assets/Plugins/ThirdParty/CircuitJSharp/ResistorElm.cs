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
    public class ResistorElm : TwoNodeElm
    {
        double resistance;

        public ResistorElm(int x0, int x1, float res = 1000f) : base(x0, x1)
        {
            resistance = res;
        }

        //System.out.print(this + " res current set to " + current + "\n");
        public override void calculateCurrent() => current = (volts[0] - volts[1]) / resistance;

        public override void stamp() => sim.stampResistor(nodes[0], nodes[1], resistance);
    }
}