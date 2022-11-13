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
    class WireElm : TwoNodeElm
    {
        public WireElm(int xx, int yy) : base(xx, yy)
        {
        }

        public override void stamp()
        {
        }

        //This may be changed if we want to show data from wire TODO: Make this a setting
        public bool mustShowCurrent() => false;

        // return (flags & FLAG_SHOWVOLTAGE) != 0;
        public bool mustShowVoltage() => false;

        public override double getPower() => 0;

        public override double getVoltageDiff() =>  volts[0];

        public override bool isWireEquivalent() => true;

        public override bool isRemovableWire() => true;
    }
}