/*    
    Copyright (C) Paul Falstad
    
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

namespace CircuitJSharp
{
    public class Expr
    {
        public double eval(ExprState state)
        {
            return 5;
        }
    }

    public class ExprState
    {
        public double[] values;

        public double t;

        public ExprState(int inputCount)
        {
        }

        public void updateLastValues(double val)
        {
        }

        public void reset()
        {
        }
    }

    class VCCSElm : ChipElm
    {
        double gain;
        int inputCount;
        Expr expr;
        ExprState exprState;
        string exprString;
        public bool broken;

        public VCCSElm(int xa, int ya, int xb, int yb, int f, object st) : base(xa, ya, xb, yb, f, st)
        {
            inputCount = 2; // Integer.parseInt(st.nextToken());
            exprString = ".1*(a-b)"; // CustomLogicModel.unescape(st.nextToken());
            parseExpr();
            setupPins();
        }

        public VCCSElm(int xx, int yy) : base(xx, yy)
        {
            inputCount = 2;
            exprString = ".1*(a-b)";
            parseExpr();
            setupPins();
        }

        double[] lastVolts;

        public override void setupPins()
        {
            pins = new Pin[inputCount + 2];
            for (int i = 0; i != inputCount; i++)
                pins[i] = new Pin(i, i, "");
            pins[inputCount] = new Pin(0, 0, "C+");
            pins[inputCount + 1] = new Pin(1, 1, "C-");
            lastVolts = new double[inputCount];
            exprState = new ExprState(inputCount);
            allocNodes();
        }

        public string getChipName()
        {
            return "VCCS~";
        } // ~ is for localization 

        public override bool nonLinear()
        {
            return true;
        }

        public override bool isDigitalChip()
        {
            return false;
        }

        public override void stamp()
        {
            sim.stampNonLinear(nodes[inputCount]);
            sim.stampNonLinear(nodes[inputCount + 1]);
        }

        double sign(double a, double b)
        {
            return a > 0 ? b : -b;
        }

        double getConvergeLimit()
        {
            // get maximum change in voltage per step when testing for convergence.  be more lenient over time
            if (sim.subIterations < 10)
                return .001;
            if (sim.subIterations < 200)
                return .01;
            return .1;
        }

        public bool hasCurrentOutput()
        {
            return true;
        }

        public int getOutputNode(int n)
        {
            return nodes[n + inputCount];
        }

        public override void doStep()
        {
            // no current path?  give up
            if (broken)
            {
                pins[inputCount].current = 0;
                pins[inputCount + 1].current = 0;
                // avoid singular matrix errors
                sim.stampResistor(nodes[inputCount], nodes[inputCount + 1], 1e8);
                return;
            }

            // converged yet?
            double convergeLimit = getConvergeLimit();
            for (int i = 0; i != inputCount; i++)
            {
                if (Math.Abs(volts[i] - lastVolts[i]) > convergeLimit)
                {
                    sim.converged = false;
//        	    sim.console("vcvs " + nodes + " " + i + " " + volts[i] + " " + lastVolts[i] + " " + sim.subIterations);
                }
//        	if (Double.isNaN(volts[i]))
//        	    volts[i] = 0;
            }

            if (expr != null)
            {
                // calculate output
                for (int i = 0; i != inputCount; i++)
                    exprState.values[i] = volts[i];
                exprState.t = sim.t;
                double v0 = -expr.eval(exprState);
//        	if (Math.abs(volts[inputCount]-v0) > Math.abs(v0)*.01 && sim.subIterations < 100)
//        	    sim.converged = false;
                double rs = v0;

                // calculate and stamp output derivatives
                for (int i = 0; i != inputCount; i++)
                {
                    double dv = volts[i] - lastVolts[i];
                    if (Math.Abs(dv) < 1e-6)
                        dv = 1e-6;
                    exprState.values[i] = volts[i];
                    double v = -expr.eval(exprState);
                    exprState.values[i] = volts[i] - dv;
                    double v2 = -expr.eval(exprState);
                    double dx = (v - v2) / dv;
                    if (Math.Abs(dx) < 1e-6)
                        dx = sign(dx, 1e-6);
                    sim.stampVCCurrentSource(nodes[inputCount], nodes[inputCount + 1], nodes[i], 0, dx);
                    //if (sim.subIterations > 1)
                    //sim.console("ccedx " + i + " " + dx + " " + sim.subIterations + " " + sim.t);
                    // adjust right side
                    rs -= dx * volts[i];
                    exprState.values[i] = volts[i];
                }

//        	sim.console("ccers " + rs);
                sim.stampCurrentSource(nodes[inputCount], nodes[inputCount + 1], rs);
                pins[inputCount].current = -v0;
                pins[inputCount + 1].current = v0;
            }

            for (int i = 0; i != inputCount; i++)
                lastVolts[i] = volts[i];
        }

        public override void stepFinished()
        {
            exprState.updateLastValues(pins[inputCount].current);
        }

        public override int getPostCount()
        {
            return inputCount + 2;
        }

        public override int getVoltageSourceCount()
        {
            return 0;
        }

        int getDumpType()
        {
            return 213;
        }

        public override bool getConnection(int n1, int n2)
        {
            return comparePair(inputCount, inputCount + 1, n1, n2);
        }

        public override bool hasGroundConnection(int n1)
        {
            return false;
        }

        public override object getChipEditInfo(int n)
        {
            // if (n == 0)
            // {
            //     EditInfo ei = new EditInfo(EditInfo.makeLink("customfunction.html", "Output Function"), 0, -1, -1);
            //     ei.text = exprString;
            //     ei.disallowSliders();
            //     return ei;
            // }
            //
            // if (n == 1)
            //     return new EditInfo("# of Inputs", inputCount, 1, 8).setDimensionless();
            return null;
        }

        public override void setChipEditValue(int n, object ei)
        {
            // if (n == 0)
            // {
            //     exprString = ei.textf.getText();
            //     parseExpr();
            //     return;
            // }
            //
            // if (n == 1)
            // {
            //     if (ei.value < 0 || ei.value > 8)
            //         return;
            //     inputCount = (int)ei.value;
            //     setupPins();
            //     allocNodes();
            //     setPoints();
            // }
        }

        public void setExpr(string expr)
        {
            exprString = expr;
            parseExpr();
        }

        void parseExpr()
        {
            //    ExprParser parser = new ExprParser(exprString);
            //    expr = parser.parseExpression();
            //    String err = parser.gotError();
            //    if (err != null)
            // Window.alert(Locale.LS("Parse error in expression") + ": " + exprString + ": " + err);
        }


        public override void reset()
        {
            base.reset();
            exprState.reset();
        }
    }
}