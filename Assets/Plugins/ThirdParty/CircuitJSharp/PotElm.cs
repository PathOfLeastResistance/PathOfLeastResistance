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
    public class PotElm : MultipleNodeElm
    {
        private const int FLAG_SHOW_VALUES = 1;
        private double position;
        private double maxResistance;
        private double resistance1;
        private double resistance2;

        private double current1;
        private double current2;

        private double current3;
        // private double curcount1;
        // private double curcount2;
        // private double curcount3;

        private Point post3;
        private Point corner2;
        private Point arrowPoint;
        private Point midpoint;
        private Point arrow1;
        private Point arrow2;
        private Point ps3;
        private Point ps4;
        private int bodyLen;

        public PotElm(int xx, int yy, int zz, double res = 1000f, double pos = 0.5f) : base(new[] { xx, yy, zz })
        {
            maxResistance = res;
            position = pos;
            flags = FLAG_SHOW_VALUES;
        }

        public double sliderPos
        {
            get => position;
            set
            {
                sim.needsStamp = true;
                position = Math.Clamp(value, 1e-3, 1 - 1e-3); //Do not allow zero resistance
            }
        }

        public double resistance
        {
            get => maxResistance;
            set
            {
                sim.needsStamp = true;
                maxResistance = value;
            }
        }

        public override int getPostCount() => 3;

        public override Point getPost(int n) => points[n];

        public override void calculateCurrent()
        {
            if (resistance1 == 0)
                return; // avoid NaN
            current1 = (volts[0] - volts[2]) / resistance1;
            current2 = (volts[1] - volts[2]) / resistance2;
            current3 = -current1 - current2;
        }

        public override double getCurrentIntoNode(int n)
        {
            if (n == 0)
                return -current1;
            if (n == 1)
                return -current2;
            return -current3;
        }

        public override void stamp()
        {
            resistance1 = maxResistance * position;
            resistance2 = maxResistance * (1 - position);
            sim.stampResistor(nodes[0], nodes[2], resistance1);
            sim.stampResistor(nodes[2], nodes[1], resistance2);
        }
    }
}