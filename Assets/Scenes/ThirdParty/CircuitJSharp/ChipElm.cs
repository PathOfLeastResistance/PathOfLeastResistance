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

abstract class ChipElm : CircuitElm
{
    int bits;
    double highVoltage;

    static int FLAG_CUSTOM_VOLTAGE = 1 << 13;

    public ChipElm(int xx, int yy) : base(xx, yy)
    {
        if (needsBits())
            bits = defaultBitCount();
        highVoltage = 5;
        setupPins();
    }

    public ChipElm(int xa, int ya, int xb, int yb, int f, object st) : base(xa, ya, xb, yb, f)
    {
        // 
        // if (needsBits())
        //     if (st.hasMoreTokens())
        //         bits = new Integer(st.nextToken()).intValue();
        //     else
        //         bits = defaultBitCount();
        // highVoltage = (hasCustomVoltage()) ? Double.parseDouble(st.nextToken()) : 5;
        // setupPins();
        // setSize((f & FLAG_SMALL) != 0 ? 1 : 2);
        // int i;
        // for (i = 0; i != getPostCount(); i++)
        // {
        //     if (pins == null)
        //         volts[i] = new Double(st.nextToken()).doubleValue();
        //     else if (pins[i].state)
        //     {
        //         volts[i] = new Double(st.nextToken()).doubleValue();
        //         pins[i].value = volts[i] > getThreshold();
        //     }
        // }
    }

    bool needsBits()
    {
        return false;
    }

    bool hasCustomVoltage()
    {
        return (flags & FLAG_CUSTOM_VOLTAGE) != 0;
    }

    public virtual bool isDigitalChip()
    {
        return true;
    }

    double getThreshold()
    {
        return highVoltage / 2;
    }

    int defaultBitCount()
    {
        return 4;
    }

    public abstract void setupPins();

    public Pin[] pins;
    bool lastClock;

    void setPoints()
    {
        for (int i = 0; i != getPostCount(); i++)
        {
            Pin p = pins[i];
            //TODO: Set pin point
            p.setPoint(i, i);
        }
    }

    Point getPost(int n)
    {
        return pins[n].post;
    }

    public abstract int getVoltageSourceCount(); // output count

    public override void setVoltageSource(int j, int vs)
    {
        int i;
        for (i = 0; i != getPostCount(); i++)
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
        int i;
        int vsc = 0;
        for (i = 0; i != getPostCount(); i++)
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

    public override string dump()
    {
        if (highVoltage == 5)
            flags &= ~FLAG_CUSTOM_VOLTAGE;
        else
            flags |= FLAG_CUSTOM_VOLTAGE;

        String s = base.dump();
        if (needsBits())
            s += " " + bits;
        if (hasCustomVoltage())
            s += " " + highVoltage;
        int i;
        for (i = 0; i != getPostCount(); i++)
        {
            if (pins[i].state)
                s += " " + volts[i];
        }

        return s;
    }

    public void writeOutput(int n, bool value)
    {
        if (!pins[n].output)
            CirSim.console("pin " + n + " is not an output!");
        pins[n].value = value;
    }

    void setCurrent(int x, double c)
    {
        int i;
        for (i = 0; i != getPostCount(); i++)
            if (pins[i].output && pins[i].voltSource == x)
                pins[i].current = c;
    }

    String getChipName()
    {
        return "chip";
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

    public object getEditInfo(int n)
    {
        // if (n == 0)
        // {
        //     EditInfo ei = new EditInfo("", 0, -1, -1);
        //     ei.checkbox = new Checkbox("Flip X", (flags & FLAG_FLIP_X) != 0);
        //     return ei;
        // }
        //
        // if (n == 1)
        // {
        //     EditInfo ei = new EditInfo("", 0, -1, -1);
        //     ei.checkbox = new Checkbox("Flip Y", (flags & FLAG_FLIP_Y) != 0);
        //     return ei;
        // }
        //
        // if (n == 2)
        // {
        //     EditInfo ei = new EditInfo("", 0, -1, -1);
        //     ei.checkbox = new Checkbox("Flip X/Y", (flags & FLAG_FLIP_XY) != 0);
        //     return ei;
        // }
        //
        // if (!isDigitalChip())
        //     return getChipEditInfo(n - 3);
        //
        // if (n == 3)
        //     return new EditInfo("High Logic Voltage", highVoltage);
        //
        // return getChipEditInfo(n - 4);
        return null;
    }

    public void setEditValue(int n, object ei)
    {
        if (!isDigitalChip())
        {
            if (n >= 3)
                setChipEditValue(n - 3, ei);
            return;
        }

        // if (n == 3)
        //     highVoltage = ei.value;

        if (n >= 4)
            setChipEditValue(n - 4, ei);
    }

    public virtual object getChipEditInfo(int n)
    {
        return null;
    }

    public virtual void setChipEditValue(int n, object ei)
    {
    }

    static string writeBits(bool[] data)
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

    static void readBits(object st, bool[] output)
    {
        // int integer = 0;
        // int bitIndex = Integer.MAX_VALUE;
        // for (int i = 0; i < output.length; i++)
        // {
        //     if (bitIndex >= Integer.SIZE)
        //         if (st.hasMoreTokens())
        //         {
        //             integer = Integer.parseInt(st.nextToken()); //Load next integer
        //             bitIndex = 0;
        //         }
        //         else
        //             break; //Data is absent
        //
        //     output[i] = (integer & (1 << bitIndex)) != 0;
        //     bitIndex++;
        // }
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

        public void setPoint(int px, int py)
        {
            post = new Point(px, py);
        }
    }
}