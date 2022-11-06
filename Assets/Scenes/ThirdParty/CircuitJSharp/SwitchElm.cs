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

class SwitchElm : CircuitElm
{
    bool momentary;

    // position 0 == closed, position 1 == open
   public  int position, posCount;

    public SwitchElm(int xx, int yy) : base(xx, yy)
    {
        momentary = false;
        position = 0;
        posCount = 2;
    }

    public SwitchElm(int xx, int yy, bool mm) : base(xx, yy)
    {
        position = (mm) ? 1 : 0;
        momentary = mm;
        posCount = 2;
    }

    public SwitchElm(int xa, int ya, int xb, int yb, int f,
        object st) : base(xa, ya, xb, yb, f)
    {
        // String str = st.nextToken();
        // if (str.compareTo("true") == 0)
        //     position = (this
        //         is LogicInputElm)
        //         ? 0
        //         : 1;
        // else if (str.compareTo("false") == 0)
        //     position = (this
        //         is LogicInputElm)
        //         ? 1
        //         : 0;
        // else
        //     position = new Integer(str).intValue();
        // momentary = new Boolean(st.nextToken()).booleanValue();
        // posCount = 2;
    }

    int getDumpType()
    {
        return 's';
    }

    public override string dump()
    {
        return base.dump() + " " + position + " " + momentary;
    }

    Point ps, ps2;

    void setPoints()
    {
        base.setPoints();
        ps = new Point();
        ps2 = new Point();
    }

    public int openhs = 16;


    void calculateCurrent()
    {
        if (position == 1)
            current = 0;
    }

    void mouseUp()
    {
        if (momentary)
            toggle();
    }

    void toggle()
    {
        position++;
        if (position >= posCount)
            position = 0;
    }


    public override bool  getConnection(int n1, int n2)
    {
        return position == 0;
    }

    public override bool isWireEquivalent()
    {
        return position == 0;
    }

    public override bool isRemovableWire()
    {
        return position == 0;
    }

    public object getEditInfo(int n)
    {
        // if (n == 0)
        // {
        //     EditInfo ei = new EditInfo("", 0, -1, -1);
        //     ei.checkbox = new Checkbox("Momentary Switch", momentary);
        //     return ei;
        // }

        return null;
    }

    public void setEditValue(int n, object ei)
    {
        // if (n == 0)
        //     momentary = ei.checkbox.getState();
    }
}