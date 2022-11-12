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
    public class PotElm : CircuitElm
    {
        private const int FLAG_SHOW_VALUES = 1;
        private double position;
        private double maxResistance;
        private double resistance1;
        private double resistance2;

        private double current1;
        private double current2;
        private double current3;
        private double curcount1;
        private double curcount2;
        private double curcount3;

        private Point post3;
        private Point corner2;
        private Point arrowPoint;
        private Point midpoint;
        private Point arrow1;
        private Point arrow2;
        private Point ps3;
        private Point ps4;
        private int bodyLen;

        public PotElm(int xx, int yy, int zz) : base(xx, yy)
        {
            points[0] = new Point(xx);
            points[1] = new Point(yy);
            points[2] = new Point(zz);

            maxResistance = 1000;
            position = .5;
            flags = FLAG_SHOW_VALUES;
        }

        public override int getPostCount()
        {
            return 3;
        }

        public override Point getPost(int n)
        {
            return (n == 0) ? points[0] : (n == 1) ? points[1] : points[2];
        }

        public void execute()
        {
            sim.analyzeFlag = true;
            setPoints();
        }

        public override void reset()
        {
            curcount1 = curcount2 = curcount3 = 0;
            base.reset();
        }

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

        public override object getEditInfo(int n)
        {
            // // ohmString doesn't work here on linux
            // if (n == 0)
            //     return new EditInfo("Resistance (ohms)", maxResistance, 0, 0);
            // if (n == 1)
            // {
            //     EditInfo ei = new EditInfo("Slider Text", 0, -1, -1);
            //     ei.text = sliderText;
            //     return ei;
            // }
            //
            // if (n == 2)
            // {
            //     EditInfo ei = new EditInfo("", 0, -1, -1);
            //     ei.checkbox = new Checkbox("Show Values", (flags & FLAG_SHOW_VALUES) != 0);
            //     return ei;
            // }

            return null;
        }

        public override void setEditValue(int n, object ei)
        {
            // if (n == 0)
            //     maxResistance = ei.value;
            // if (n == 1)
            // {
            //     sliderText = ei.textf.getText();
            //     label.setText(sliderText);
            //     sim.setiFrameHeight();
            // }
            //
            // if (n == 2)
            //     flags = ei.changeFlag(flags, FLAG_SHOW_VALUES);
        }
    }
}