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


class WireElm : CircuitElm
{
    public WireElm(int xx, int yy) : base(xx, yy)
    {
    }

    public WireElm(int xa, int ya, int xb, int yb, int f,
        object st) : base(xa, ya, xb, yb, f)
    {
    }

    void stamp()
    {
    }

    bool mustShowCurrent()
    {
        // return 
        return false;
    }

    bool mustShowVoltage()
    {
        // return (flags & FLAG_SHOWVOLTAGE) != 0;
        return false;
    }
   
    public override double getPower()
    {
        return 0;
    }

    public override double getVoltageDiff()
    {
        return volts[0];
    }

    public override bool isWireEquivalent()
    {
        return true;
    }

    public override bool isRemovableWire()
    {
        return true;
    }

    public object getEditInfo(int n)
    {
        // if (n == 0)
        // {
        //     EditInfo ei = new EditInfo("", 0, -1, -1);
        //     ei.checkbox = new Checkbox("Show Current", mustShowCurrent());
        //     return ei;
        // }
        //
        // if (n == 1)
        // {
        //     EditInfo ei = new EditInfo("", 0, -1, -1);
        //     ei.checkbox = new Checkbox("Show Voltage", mustShowVoltage());
        //     return ei;
        // }

        return null;
    }

    public void setEditValue(int n, object ei)
    {
        // if (n == 0)
        // {
        //     if (ei.checkbox.getState())
        //         flags |= FLAG_SHOWCURRENT;
        //     else
        //         flags &= ~FLAG_SHOWCURRENT;
        // }
        //
        // if (n == 1)
        // {
        //     if (ei.checkbox.getState())
        //         flags |= FLAG_SHOWVOLTAGE;
        //     else
        //         flags &= ~FLAG_SHOWVOLTAGE;
        // }
    }
}