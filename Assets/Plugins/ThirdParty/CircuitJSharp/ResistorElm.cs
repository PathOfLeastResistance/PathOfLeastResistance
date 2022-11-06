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

class ResistorElm : CircuitElm
{
    double resistance;

    public ResistorElm(int xx, int yy) : base(xx, yy)
    {
        resistance = 1000;
    }

    public ResistorElm(int xa, int ya, int xb, int yb, int f, object st) : base(xa, ya, xb, yb, f)
    {
        // resistance = new Double(st.nextToken()).doubleValue();
    }

    public override string dump()
    {
        return base.dump() + " " + resistance;
    }

    Point ps3, ps4;

    public override void setPoints()
    {
        base.setPoints();
    }

    public override void calculateCurrent()
    {
        current = (volts[0] - volts[1]) / resistance;
        //System.out.print(this + " res current set to " + current + "\n");
    }

    public override void stamp()
    {
        sim.stampResistor(nodes[0], nodes[1], resistance);
    }

    public object getEditInfo(int n)
    {
        // // ohmString doesn't work here on linux
        // if (n == 0)
        //     return new EditInfo("Resistance (ohms)", resistance, 0, 0);
        return null;
    }

    public void setEditValue(int n, object ei)
    {
        // resistance = (ei.value <= 0) ? 1e-9 : ei.value;
    }

    public double getResistance()
    {
        return resistance;
    }

    public void setResistance(double r)
    {
        resistance = r;
    }
}