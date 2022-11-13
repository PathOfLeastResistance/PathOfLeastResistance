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
    class GroundElm : CircuitElm
    {
        public static int lastSymbolType = 0;

        // this is needed for old subcircuits which have GroundElm dumped
        static int FLAG_OLD_STYLE = 1;

        public GroundElm(int xx, int yy) : base(xx, yy)
        {
        }

        public override int getPostCount()
        {
            return 1;
        }

        public void setOldStyle()
        {
            flags |= FLAG_OLD_STYLE;
        }

        bool isOldStyle()
        {
            return (flags & FLAG_OLD_STYLE) != 0;
        }


        public override void stamp()
        {
            if (isOldStyle())
                sim.stampVoltageSource(0, nodes[0], voltSource, 0);
        }

        public override void setCurrent(int x, double c)
        {
            current = isOldStyle() ? -c : c;
        }

        public override bool isWireEquivalent()
        {
            return true;
        }

        public override bool isRemovableWire()
        {
            return true;
        }

        static Point firstGround;

        public static void resetNodeList()
        {
            firstGround = null;
        }

        public override Point getConnectedPost()
        {
            if (firstGround != null)
                return firstGround;
            firstGround = points[0];
            return null;
        }

        public override double getVoltageDiff()
        {
            return 0;
        }

        public override bool hasGroundConnection(int n1)
        {
            return true;
        }

        public override double getCurrentIntoNode(int n)
        {
            return -current;
        }
    }
}