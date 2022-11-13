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
using System.Text;

namespace CircuitJSharp
{
    abstract class ChipElm : MultipleNodeElm
    {
        protected int bits;
        protected double highVoltage;
        protected static int FLAG_CUSTOM_VOLTAGE = 1 << 13;

        public ChipElm(int[] points) : base(points)
        {
            if (needsBits())
                bits = defaultBitCount();
            highVoltage = 5;
            setupPins();
        }

        protected virtual bool needsBits() => false;

        protected virtual bool hasCustomVoltage() => (flags & FLAG_CUSTOM_VOLTAGE) != 0;

        protected virtual bool isDigitalChip() => true;

        double getThreshold()
        {
            return highVoltage / 2;
        }

        int defaultBitCount() => 4;

        public abstract void setupPins();

        public Pin[] pins;
        public bool lastClock;

        //TODO: think about pin initialization. 
        // public override void setPoints()
        // {
        //     for (int i = 0; i != getPostCount(); i++)
        //     {
        //         Pin p = pins[i];
        //         //TODO: Set pin point
        //         p.setPoint(i);
        //     }
        // }

        public override Point getPost(int n)
        {
            return pins[n].post;
        }

        public override abstract int getVoltageSourceCount(); // output count

        public override void setVoltageSource(int j, int vs)
        {
            for (int i = 0; i != getPostCount(); i++)
            {
                Pin p = pins[i];
                if (p.output && j-- == 0)
                {
                    p.voltSource = vs;
                    return;
                }
            }

            CirSim.console("setVoltageSource failed for " + this);
        }

        public override void stamp()
        {
            int vsc = 0;
            for (int i = 0; i != getPostCount(); i++)
            {
                Pin p = pins[i];
                if (p.output)
                {
                    sim.stampVoltageSource(0, nodes[i], p.voltSource);
                    vsc++;
                }
            }

            if (vsc != getVoltageSourceCount())
                CirSim.console("voltage source count does not match number of outputs");
        }

        public virtual void execute()
        {
        }

        public override void doStep()
        {
            int i;
            for (i = 0; i != getPostCount(); i++)
            {
                Pin p = pins[i];
                if (!p.output)
                    p.value = volts[i] > getThreshold();
            }

            execute();
            for (i = 0; i != getPostCount(); i++)
            {
                Pin p = pins[i];
                if (p.output)
                    sim.updateVoltageSource(0, nodes[i], p.voltSource,
                        p.value ? highVoltage : 0);
            }
        }

        public override void reset()
        {
            int i;
            for (i = 0; i != getPostCount(); i++)
            {
                pins[i].value = false;
                pins[i].curcount = 0;
                volts[i] = 0;
            }

            lastClock = false;
        }

        public void writeOutput(int n, bool value)
        {
            if (!pins[n].output)
                CirSim.console("pin " + n + " is not an output!");
            pins[n].value = value;
        }

        public override void setCurrent(int x, double c)
        {
            int i;
            for (i = 0; i != getPostCount(); i++)
                if (pins[i].output && pins[i].voltSource == x)
                    pins[i].current = c;
        }

        public override bool getConnection(int n1, int n2)
        {
            return false;
        }

        public override bool hasGroundConnection(int n1)
        {
            return pins[n1].output;
        }

        public override double getCurrentIntoNode(int n)
        {
            return pins[n].current;
        }

        public abstract object getChipEditInfo(int n);

        public abstract void setChipEditValue(int n, object ei);

        public static string writeBits(bool[] data)
        {
            StringBuilder sb = new StringBuilder();
            int integer = 0;
            int bitIndex = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (bitIndex >= sizeof(int))
                {
                    //Flush completed integer
                    sb.Append(' ');
                    sb.Append(integer);
                    integer = 0;
                    bitIndex = 0;
                }

                if (data[i])
                    integer |= 1 << bitIndex;
                bitIndex++;
            }

            if (bitIndex > 0)
            {
                sb.Append(' ');
                sb.Append(integer);
            }

            return sb.ToString();
        }

        public class Pin
        {
            public Pin(int p, int s, string t)
            {
                pos = p;
                side0 = side = s;
            }

            public Point post;
            public int pos, side, side0, voltSource, bubbleX, bubbleY;
            public bool lineOver, bubble, clock, output, value, state, selected;
            public double curcount, current;

            public void setPoint(int px)
            {
                post = new Point(px);
            }
        }
    }
}