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


class LogicInputElm : SwitchElm
{
    static int FLAG_TERNARY = 1;
    static int FLAG_NUMERIC = 2;
    double hiV, loV;

    public LogicInputElm(int xx, int yy) :  base(xx, yy, false)
    {
        hiV = 5;
        loV = 0;
    }

    public LogicInputElm(int xa, int ya, int xb, int yb, int f, object st) :    base(xa, ya, xb, yb, f, st)
    {
        hiV = 5;
        loV = 0;
        if (isTernary())
            posCount = 3;
        // try
        // {
        //     hiV = new Double(st.nextToken()).doubleValue();
        //     loV = new Double(st.nextToken()).doubleValue();
        // }
        // catch (Exception e)
        // {
        //     hiV = 5;
        //     loV = 0;
        // }
        //
        // if (isTernary())
        //     posCount = 3;
    }

    bool isTernary()
    {
        return (flags & FLAG_TERNARY) != 0;
    }

    public bool isNumeric()
    {
        return (flags & (FLAG_TERNARY | FLAG_NUMERIC)) != 0;
    }

    public override string dump()
    {
        return base.dump() + " " + hiV + " " + loV;
    }

    public override int getPostCount()
    {
        return 1;
    }

    // public override void setPoints()
    // {
    //     base.setPoints();
    // }


  public override   void setCurrent(int vs, double c)
    {
        current = c;
    }

    public override void calculateCurrent()
    {
    }

    public override void stamp()
    {
        sim.stampVoltageSource(0, nodes[0], voltSource);
    }

    public override bool isWireEquivalent()
    {
        return false;
    }

    public override bool isRemovableWire()
    {
        return false;
    }

    public override void doStep()
    {
        double v = (position == 0) ? loV : hiV;
        if (isTernary())
            v = loV + position * (hiV - loV) * .5;
        sim.updateVoltageSource(0, nodes[0], voltSource, v);
    }

    public override int getVoltageSourceCount()
    {
        return 1;
    }

    public override double getVoltageDiff()
    {
        return volts[0];
    }

    public override bool hasGroundConnection(int n1)
    {
        return true;
    }

    public override object getEditInfo(int n)
    {
        // if (n == 0)
        // {
        //     EditInfo ei = new EditInfo("", 0, 0, 0);
        //     ei.checkbox = new Checkbox("Momentary Switch", momentary);
        //     return ei;
        // }
        //
        // if (n == 1)
        //     return new EditInfo("High Logic Voltage", hiV, 10, -10);
        // if (n == 2)
        //     return new EditInfo("Low Voltage", loV, 10, -10);
        // if (n == 3)
        // {
        //     EditInfo ei = new EditInfo("", 0, 0, 0);
        //     ei.checkbox = new Checkbox("Numeric", isNumeric());
        //     return ei;
        // }
        //
        // if (n == 4)
        // {
        //     EditInfo ei = new EditInfo("", 0, 0, 0);
        //     ei.checkbox = new Checkbox("Ternary", isTernary());
        //     return ei;
        // }
        //
        return null;
    }

    public override void setEditValue(int n, object ei)
    {
        // if (n == 0)
        //     momentary = ei.checkbox.getState();
        // if (n == 1)
        //     hiV = ei.value;
        // if (n == 2)
        //     loV = ei.value;
        // if (n == 3)
        // {
        //     if (ei.checkbox.getState())
        //         flags |= FLAG_NUMERIC;
        //     else
        //         flags &= ~FLAG_NUMERIC;
        // }
        //
        // if (n == 4)
        // {
        //     if (ei.checkbox.getState())
        //         flags |= FLAG_TERNARY;
        //     else
        //         flags &= ~FLAG_TERNARY;
        //     posCount = (isTernary()) ? 3 : 2;
        // }
    }

    public override double getCurrentIntoNode(int n)
    {
        return current;
    }
}