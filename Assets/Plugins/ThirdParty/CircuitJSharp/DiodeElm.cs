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
using System.Collections.Generic;

namespace CircuitJSharp
{
    public class DiodeElm : TwoNodeElm
    {
        private const int hs = 8;
        private const int FLAG_FWDROP = 1;
        private const int FLAG_MODEL = 2;

        private Diode diode;
        private string modelName;
        private DiodeModel model;
        private static string lastModelName = "spice-default";
        private bool hasResistance;
        private int diodeEndNode;
        private Point[] cathode;
        private List<DiodeModel> models;

        public DiodeElm(int xx, int yy) : this(xx, yy, lastModelName)
        {
        }

        public DiodeElm(int xx, int yy, string diodeModel) : base(xx, yy)
        {
            modelName = diodeModel;
            diode = new Diode(sim);
            setup();
        }

        public override bool nonLinear() => true;

        private void setup()
        {
            //	CirSim.console("setting up for model " + modelName + " " + model);
            model = DiodeModel.getModelWithNameOrCopy(modelName, model);
            modelName = model.name; // in case we couldn't find that model
            diode.setup(model);
            hasResistance = (model.seriesResistance > 0);
            diodeEndNode = (hasResistance) ? 2 : 1;
            allocNodes();
        }

        public override int getInternalNodeCount() => hasResistance ? 1 : 0;

        public override void updateModels() => setup();

        public override void reset()
        {
            diode.reset();
            volts[0] = volts[1] = curcount = 0;
            if (hasResistance)
                volts[2] = 0;
        }

        public override void stamp()
        {
            if (hasResistance)
            {
                // create diode from node 0 to internal node
                diode.stamp(nodes[0], nodes[2]);
                // create resistor from internal node to node 1
                sim.stampResistor(nodes[1], nodes[2], model.seriesResistance);
            }
            else
                // don't need any internal nodes if no series resistance
                diode.stamp(nodes[0], nodes[1]);
        }

        public override void doStep() => diode.doStep(volts[0] - volts[diodeEndNode]);

        public override void calculateCurrent() => current = diode.calculateCurrent(volts[0] - volts[diodeEndNode]);

        public void newModelCreated(DiodeModel dm)
        {
            model = dm;
            modelName = model.name;
            setup();
        }

        private void setLastModelName(String n) => lastModelName = n;

        public override void stepFinished()
        {
            // stop for huge currents that make simulator act weird
            if (Math.Abs(current) > 1e12)
                sim.stop("max current exceeded", this);
        }
    }
}