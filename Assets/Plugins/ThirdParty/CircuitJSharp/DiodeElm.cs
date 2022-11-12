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
    public class DiodeElm : CircuitElm
    {
        Diode diode;
        const int FLAG_FWDROP = 1;
        const int FLAG_MODEL = 2;
        string modelName;
        DiodeModel model;
        static string lastModelName = "default";
        bool hasResistance;
        int diodeEndNode;

        public DiodeElm(int xx, int yy) : base(xx, yy)
        {
            modelName = lastModelName;
            diode = new Diode(sim);
            setup();
        }

        public DiodeElm(int xa, int ya, int xb, int yb, int f, object st) : base(xa, ya, xb, yb, f)
        {
            double defaultdrop = .805904783;
            diode = new Diode(sim);
            double fwdrop = defaultdrop;
            double zvoltage = 0;
            if ((f & FLAG_MODEL) != 0)
            {
                // modelName = CustomLogicModel.unescape(st.nextToken());
            }
            else
            {
                if ((f & FLAG_FWDROP) > 0)
                {
                    try
                    {
                        // fwdrop = new Double(st.nextToken()).doubleValue();
                    }
                    catch (Exception)
                    {
                    }
                }

                model = DiodeModel.getModelWithParameters(fwdrop, zvoltage);
                modelName = model.name;
//	    CirSim.console("model name wparams = " + modelName);
            }

            setup();
        }

        public override bool nonLinear()
        {
            return true;
        }

        void setup()
        {
//	CirSim.console("setting up for model " + modelName + " " + model);
            model = DiodeModel.getModelWithNameOrCopy(modelName, model);
            modelName = model.name; // in case we couldn't find that model
            diode.setup(model);
            hasResistance = (model.seriesResistance > 0);
            diodeEndNode = (hasResistance) ? 2 : 1;
            allocNodes();
        }

        public override int getInternalNodeCount()
        {
            return hasResistance ? 1 : 0;
        }

        public override void updateModels()
        {
            setup();
        }

        const int hs = 8;
        Point[] cathode;

        public override void setPoints()
        {
            base.setPoints();
        }

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

        public override void doStep()
        {
            diode.doStep(volts[0] - volts[diodeEndNode]);
        }

        public override void calculateCurrent()
        {
            current = diode.calculateCurrent(volts[0] - volts[diodeEndNode]);
        }

        List<DiodeModel> models;

        public override object getEditInfo(int n)
        {
            // if (n == 0)
            // {
            //     EditInfo ei = new EditInfo("Model", 0, -1, -1);
            //     models = DiodeModel.getModelList(this instanceof ZenerElm);
            //     ei.choice = new Choice();
            //     int i;
            //     for (i = 0; i != models.size(); i++)
            //     {
            //         DiodeModel dm = models.get(i);
            //         ei.choice.add(dm.getDescription());
            //         if (dm == model)
            //             ei.choice.select(i);
            //     }
            //
            //     return ei;
            // }
            //
            // if (n == 1)
            // {
            //     EditInfo ei = new EditInfo("", 0, -1, -1);
            //     ei.button = new Button(Locale.LS("Create New Simple Model"));
            //     return ei;
            // }
            //
            // if (n == 2)
            // {
            //     EditInfo ei = new EditInfo("", 0, -1, -1);
            //     ei.button = new Button(Locale.LS("Create New Advanced Model"));
            //     return ei;
            // }
            //
            // if (n == 3)
            // {
            //     if (model.readOnly)
            //         return null;
            //     EditInfo ei = new EditInfo("", 0, -1, -1);
            //     ei.button = new Button(Locale.LS("Edit Model"));
            //     return ei;
            // }

            return null;
        }

        public void newModelCreated(DiodeModel dm)
        {
            model = dm;
            modelName = model.name;
            setup();
        }

        public override void setEditValue(int n, object ei)
        {
            // if (n == 0)
            // {
            //     model = models.get(ei.choice.getSelectedIndex());
            //     modelName = model.name;
            //     setup();
            //     ei.newDialog = true;
            //     return;
            // }
            //
            // if (n == 1 || n == 2)
            // {
            //     DiodeModel newModel = new DiodeModel(model);
            //     newModel.setSimple(n == 1);
            //     if (newModel.isSimple())
            //         newModel.setForwardVoltage();
            //     EditDialog editDialog = new EditDiodeModelDialog(newModel, sim, this);
            //     CirSim.diodeModelEditDialog = editDialog;
            //     editDialog.show();
            //     return;
            // }
            //
            // if (n == 3)
            // {
            //     if (model.readOnly)
            //     {
            //         // probably never reached
            //         Window.alert(Locale.LS("This model cannot be modified.  Change the model name to allow customization."));
            //         return;
            //     }
            //
            //     if (model.isSimple())
            //         model.setForwardVoltage();
            //     EditDialog editDialog = new EditDiodeModelDialog(model, sim, null);
            //     CirSim.diodeModelEditDialog = editDialog;
            //     editDialog.show();
            //     return;
            // }
        }

        void setLastModelName(String n)
        {
            lastModelName = n;
        }

        public override void stepFinished()
        {
            // stop for huge currents that make simulator act weird
            if (Math.Abs(current) > 1e12)
                sim.stop("max current exceeded", this);
        }
    }
}