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


class RailElm : VoltageElm
{
    public RailElm(int xx, int yy) : base(xx, yy, WaveForm.WF_DC)
    {
    }

    RailElm(int xx, int yy, WaveForm wf) : base(xx, yy, wf)
    {
    }

    public RailElm(int xa, int ya, int xb, int yb, int f, object st) : base(xa, ya, xb, yb, f, st)
    {
    }

    public static int FLAG_CLOCK = 1;

    public override int getPostCount()
    {
        return 1;
    }

    // public override void setPoints()
    // {
    //     base.setPoints();
    // }
    
    public override double getVoltageDiff()
    {
        return volts[0];
    }

    public override void stamp()
    {
        if (waveform == WaveForm.WF_DC)
            sim.stampVoltageSource(0, nodes[0], voltSource, getVoltage());
        else
            sim.stampVoltageSource(0, nodes[0], voltSource);
    }

    public override void doStep()
    {
        if (waveform != WaveForm.WF_DC)
            sim.updateVoltageSource(0, nodes[0], voltSource, getVoltage());
    }

    public override bool hasGroundConnection(int n1)
    {
        return true;
    }
}