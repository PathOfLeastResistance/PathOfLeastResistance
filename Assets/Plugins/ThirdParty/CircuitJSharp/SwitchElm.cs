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


// SPST switch
namespace CircuitJSharp
{
    public class SwitchElm : TwoNodeElm
    {
        public int openhs = 16;

        // position 0 == closed, position 1 == open
        protected int position;
        private int posCount;

        public SwitchElm(int xx, int yy) : base(xx, yy)
        {
            position = 1;
            posCount = 2;
        }

        public void toggle()
        {
            position++;
            if (position >= posCount)
                position = 0;
            sim.needAnalyze();
        }
        
        public override void calculateCurrent()
        {
            if (position == 1)
                current = 0;
        }

        public override bool getConnection(int n1, int n2) => position == 0;

        public override bool isWireEquivalent() => position == 0;

        public override bool isRemovableWire() => position == 0;
    }
}