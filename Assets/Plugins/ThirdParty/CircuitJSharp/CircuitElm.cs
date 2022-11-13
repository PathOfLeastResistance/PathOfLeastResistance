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
// circuit element class
    public class CircuitElm
    {
        public const double pi = 3.14159265358979323846;
        
        public Point[] points = new Point [2];

        protected static CirSim sim;
        protected int flags;
        protected int[] nodes;
        protected int voltSource;

        // voltages at each node
        protected double[] volts;
        protected double current;
        protected double curcount;

        public bool hasWireInfo; // used in calcWireInfo()

        int getDefaultFlags() => 0;

        bool hasFlag(int f) =>  (flags & f) != 0;

        public static void initClass(CirSim s)
        {
            sim = s;
        }

        // create new element with one post at xx,yy, to be dragged out by user
        public CircuitElm(int xx1, int xx2)
        {
            points = newPointArray(2);
            points[0] = new Point(xx1);
            points[1] = new Point(xx2);

            flags = getDefaultFlags();
            allocNodes();
        }

        // allocate nodes/volts arrays we need
        protected void allocNodes()
        {
            int n = getPostCount() + getInternalNodeCount();
            // preserve voltages if possible
            if (nodes == null || nodes.Length != n)
            {
                nodes = new int[n];
                volts = new double[n];
            }
        }

        // handle reset button
        public virtual void reset()
        {
            for (int i = 0; i != getPostCount() + getInternalNodeCount(); i++)
                volts[i] = 0;
            curcount = 0;
        }

        // set current for voltage source vn to c.  vn will be the same value as in a previous call to setVoltageSource(n, vn) 
        public virtual void setCurrent(int vn, double c)
        {
            current = c;
        }

        // get current for one- or two-terminal elements
        public double getCurrent()
        {
            return current;
        }

        public void setParentList(List<CircuitElm> elmList)
        {
        }

        // stamp matrix values for linear elements.
        // for non-linear elements, use this to stamp values that don't change each iteration, and call stampRightSide() or stampNonLinear() as needed
        public virtual void stamp()
        {
        }

        // stamp matrix values for non-linear elements
        public virtual void doStep()
        {
        }

        public virtual void startIteration()
        {
        }

        // get voltage of x'th node
        public double getPostVoltage(int x)
        {
            return volts[x];
        }

        // set voltage of x'th node, called by simulator logic
        public virtual void setNodeVoltage(int n, double c)
        {
            volts[n] = c;
            calculateCurrent();
        }

        // calculate current in response to node voltages changing
        public virtual void calculateCurrent()
        {
        }

        // number of voltage sources this element needs 
        public virtual int getVoltageSourceCount() => 0;

        // number of internal nodes (nodes not visible in UI that are needed for implementation)
        public virtual int getInternalNodeCount() => 0;

        // notify this element that its pth node is n.  This value n can be passed to stampMatrix()
        public void setNode(int p, int n) => nodes[p] = n;

        // notify this element that its nth voltage source is v.  This value v can be passed to stampVoltageSource(), etc and will be passed back in calls to setCurrent()
        // default implementation only makes sense for subclasses with one voltage source.  If we have 0 this isn't used, if we have >1 this won't work 
        public virtual void setVoltageSource(int n, int v) => voltSource = v;

        public virtual double getVoltageDiff() => volts[0] - volts[1];

        public virtual bool nonLinear() => false;

        public virtual int getPostCount() => 2;

        // get (global) node number of nth node
        public int getNode(int n) => nodes[n];

        // get position of nth node
        public virtual Point getPost(int n) => points[n];

        // return post we're connected to (for wires, so we can optimize them out in calculateWireClosure())
        public virtual Point getConnectedPost() => points[1];

        public int getNodeAtPoint(int xp)
        {
            for (int i = 0; i != getPostCount(); i++)
            {
                Point p = getPost(i);
                if (p.x == xp)
                    return i;
            }

            return 0;
        }

        public virtual double getPower() => getVoltageDiff() * current;

        // get number of nodes that can be retrieved by getConnectionNode()
        public int getConnectionNodeCount() => getPostCount();

        // get nodes that can be passed to getConnection(), to test if this element connects
        // those two nodes; this is the same as getNode() for all but labeled nodes.
        public int getConnectionNode(int n) => getNode(n);

        // are n1 and n2 connected by this element?  this is used to determine
        // unconnected nodes, and look for loops
        public virtual bool getConnection(int n1, int n2) => true;

        // is n1 connected to ground somehow?
        public virtual bool hasGroundConnection(int n1) => false;

        // is this a wire or equivalent to a wire?  (used for circuit validation)
        public virtual bool isWireEquivalent() => false;

        // is this a wire we can remove?
        public virtual bool isRemovableWire() => false;

        protected Point[] newPointArray(int n)
        {
            var a = new Point[n];
            while (n > 0)
                a[--n] = new Point();
            return a;
        }
        
        public bool comparePair(int x1, int x2, int y1, int y2) => (x1 == y1 && x2 == y2) || (x1 == y2 && x2 == y1);
        
        public virtual void stepFinished()
        {
        }

        // get current flowing into node n out of this element
        // if we take out the getPostCount() == 2 it gives the wrong value for rails
        public virtual double getCurrentIntoNode(int n) =>  (n == 0 && getPostCount() == 2) ? -current : current;


        public virtual void updateModels()
        {
        }
    }
}