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

public class InductorElm : CircuitElm
{
    Inductor ind;
    double inductance;
    double initialCurrent;

    public InductorElm(int xx, int yy) : base(xx, yy)
    {
        ind = new Inductor(sim);
        inductance = 1;
        ind.setup(inductance, current, flags);
    }

    public InductorElm(int xa, int ya, int xb, int yb, int f, object st) : base(xa, ya, xb, yb, f)
    {
        // ind = new Inductor(sim);
        // inductance = new Double(st.nextToken()).doubleValue();
        // current = new Double(st.nextToken()).doubleValue();
        // try
        // {
        //     initialCurrent = new Double(st.nextToken()).doubleValue();
        // }
        // catch (Exception e)
        // {
        // }
        //
        // ind.setup(inductance, current, flags);
    }

    public override void setPoints()
    {
        base.setPoints();
    }

    public override void reset()
    {
        volts[0] = volts[1] = curcount = 0;
        current = initialCurrent;
        ind.resetTo(initialCurrent);
    }

    public override void stamp()
    {
        ind.stamp(nodes[0], nodes[1]);
    }

    public override void startIteration()
    {
        ind.startIteration(volts[0] - volts[1]);
    }

    public override bool nonLinear()
    {
        return ind.nonLinear();
    }

    public override void calculateCurrent()
    {
        double voltdiff = volts[0] - volts[1];
        current = ind.calculateCurrent(voltdiff);
    }

    public override void doStep()
    {
        double voltdiff = volts[0] - volts[1];
        ind.doStep(voltdiff);
    }

    public override object getEditInfo(int n)
    {
        //    if (n == 0)
        // return new EditInfo("Inductance (H)", inductance, 1e-2, 10);
        //    if (n == 1) {
        // EditInfo ei = new EditInfo("", 0, -1, -1);
        // ei.checkbox = new Checkbox("Trapezoidal Approximation",
        // 			   ind.isTrapezoidal());
        // return ei;
        //    }
        //           if (n == 2)
        //               return new EditInfo("Initial Current (on Reset) (A)", initialCurrent);
        return null;
    }

    public override void setEditValue(int n, object ei)
    {
        //    if (n == 0 && ei.value > 0)
        // inductance = ei.value;
        //    if (n == 1) {
        // if (ei.checkbox.getState())
        //     flags &= ~Inductor.FLAG_BACK_EULER;
        // else
        //     flags |= Inductor.FLAG_BACK_EULER;
        //    }
        //           if (n == 2)
        //               initialCurrent = ei.value;
        //    ind.setup(inductance, current, flags);
    }

    public double getInductance()
    {
        return inductance;
    }

    public void setInductance(double l)
    {
        inductance = l;
        ind.setup(inductance, current, flags);
    }
}