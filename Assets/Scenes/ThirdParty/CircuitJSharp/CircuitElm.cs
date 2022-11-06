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

// circuit element class
public class CircuitElm
{
    static Point ps1, ps2;
    public static CirSim sim;
    public const double pi = 3.14159265358979323846;

    // initial point where user created element.  For simple two-terminal elements, this is the first node/post.
    public int x, y;

    // point to which user dragged out element.  For simple two-terminal elements, this is the second node/post
    int x2, y2;

    public int flags;
    public int[] nodes;
    public int voltSource;

    // length along x and y axes, and sign of difference
    int dx, dy, dsign;

    public double dn;

    double dpx1, dpy1;

    // (x,y) and (x2,y2) as Point objects
    public Point point1, point2;

    // voltages at each node
    public double[] volts;

    public double current, curcount;

    public bool hasWireInfo; // used in calcWireInfo()

    int getDefaultFlags()
    {
        return 0;
    }

    bool hasFlag(int f)
    {
        return (flags & f) != 0;
    }

    public static void initClass(CirSim s)
    {
        sim = s;
        ps1 = new Point();
        ps2 = new Point();
    }

    // create new element with one post at xx,yy, to be dragged out by user
    public CircuitElm(int xx, int yy)
    {
        x = x2 = xx;
        y = y2 = yy;
        flags = getDefaultFlags();
        allocNodes();
    }

    // create element between xa,ya and xb,yb from undump
    public CircuitElm(int xa, int ya, int xb, int yb, int f)
    {
        x = xa;
        y = ya;
        x2 = xb;
        y2 = yb;
        flags = f;
        allocNodes();
    }

    // allocate nodes/volts arrays we need
    public void allocNodes()
    {
        int n = getPostCount() + getInternalNodeCount();
        // preserve voltages if possible
        if (nodes == null || nodes.Length != n)
        {
            nodes = new int[n];
            volts = new double[n];
        }
    }

    // dump component state for export/undo
    public virtual string dump()
    {
        return "";
    }

    // handle reset button
    public virtual void reset()
    {
        int i;
        for (i = 0; i != getPostCount() + getInternalNodeCount(); i++)
            volts[i] = 0;
        curcount = 0;
    }

    // set current for voltage source vn to c.  vn will be the same value as in a previous call to setVoltageSource(n, vn) 
    public void setCurrent(int vn, double c)
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
    public void setNodeVoltage(int n, double c)
    {
        volts[n] = c;
        calculateCurrent();
    }

    // calculate current in response to node voltages changing
    public virtual void calculateCurrent()
    {
    }

    // calculate post locations and other convenience values used for drawing.  Called when element is moved 
    public virtual void setPoints()
    {
        point1 = new Point(x, y);
        point2 = new Point(x2, y2);
    }

    /**
     * Calculates two points fraction f along the line between a and b and offest perpendicular by +/-g
     * @param a 1st point (In)
     * @param b 2nd point (In)
     * @param c 1st point (Out)
     * @param d 2nd point (Out)
     * @param f Fraction along line
     * @param g Fraction perpendicular to line
     */
    Point[] newPointArray(int n)
    {
        Point[] a = new Point[n];
        while (n > 0)
            a[--n] = new Point();
        return a;
    }

    int CURRENT_TOO_FAST = 100;

    double addCurCount(double c, double a)
    {
        if (c == CURRENT_TOO_FAST || c == -CURRENT_TOO_FAST)
            return c;
        return c + a;
    }

    // this is used to set the position of an internal element so we can draw it inside the parent
    void setPosition(int x_, int y_, int x2_, int y2_)
    {
        x = x_;
        y = y_;
        x2 = x2_;
        y2 = y2_;
        setPoints();
    }

    // number of voltage sources this element needs 
    public int getVoltageSourceCount()
    {
        return 0;
    }

    // number of internal nodes (nodes not visible in UI that are needed for implementation)
    public int getInternalNodeCount()
    {
        return 0;
    }

    // notify this element that its pth node is n.  This value n can be passed to stampMatrix()
    public void setNode(int p, int n)
    {
        nodes[p] = n;
    }

    // notify this element that its nth voltage source is v.  This value v can be passed to stampVoltageSource(), etc and will be passed back in calls to setCurrent()
    public virtual void setVoltageSource(int n, int v)
    {
        // default implementation only makes sense for subclasses with one voltage source.  If we have 0 this isn't used, if we have >1 this won't work 
        voltSource = v;
    }

    public virtual double getVoltageDiff()
    {
        return volts[0] - volts[1];
    }

    public virtual bool nonLinear()
    {
        return false;
    }

    public int getPostCount()
    {
        return 2;
    }

    // get (global) node number of nth node
    public int getNode(int n)
    {
        return nodes[n];
    }

    // get position of nth node
    public Point getPost(int n)
    {
        return (n == 0) ? point1 : (n == 1) ? point2 : null;
    }

    // return post we're connected to (for wires, so we can optimize them out in calculateWireClosure())
    public Point getConnectedPost()
    {
        return point2;
    }

    public int getNodeAtPoint(int xp, int yp)
    {
        int i;
        for (i = 0; i != getPostCount(); i++)
        {
            Point p = getPost(i);
            if (p.x == xp && p.y == yp)
                return i;
        }

        return 0;
    }

    void doAdjust()
    {
    }

    void setupAdjust()
    {
    }

    public virtual double getPower()
    {
        return getVoltageDiff() * current;
    }

    public virtual object getEditInfo(int n)
    {
        return null;
    }

    public virtual void setEditValue(int n, object ei)
    {
    }

    // get number of nodes that can be retrieved by getConnectionNode()
    public int getConnectionNodeCount()
    {
        return getPostCount();
    }

    // get nodes that can be passed to getConnection(), to test if this element connects
    // those two nodes; this is the same as getNode() for all but labeled nodes.
    public int getConnectionNode(int n)
    {
        return getNode(n);
    }

    // are n1 and n2 connected by this element?  this is used to determine
    // unconnected nodes, and look for loops
    public virtual bool getConnection(int n1, int n2)
    {
        return true;
    }

    // is n1 connected to ground somehow?
    public virtual bool hasGroundConnection(int n1)
    {
        return false;
    }

    // is this a wire or equivalent to a wire?  (used for circuit validation)
    public virtual bool isWireEquivalent()
    {
        return false;
    }

    // is this a wire we can remove?
    public virtual bool isRemovableWire()
    {
        return false;
    }

    bool canViewInScope()
    {
        return getPostCount() <= 2;
    }

    public bool comparePair(int x1, int x2, int y1, int y2)
    {
        return ((x1 == y1 && x2 == y2) || (x1 == y2 && x2 == y1));
    }

    static int abs(int x)
    {
        return x < 0 ? -x : x;
    }

    static int sign(int x)
    {
        return (x < 0) ? -1 : (x == 0) ? 0 : 1;
    }

    static int min(int a, int b)
    {
        return (a < b) ? a : b;
    }

    static int max(int a, int b)
    {
        return (a > b) ? a : b;
    }

    static double distance(Point p1, Point p2)
    {
        double x = p1.x - p2.x;
        double y = p1.y - p2.y;
        return Math.Sqrt(x * x + y * y);
    }

    bool needsShortcut()
    {
        return getShortcut() > 0;
    }

    int getShortcut()
    {
        return 0;
    }

    string dumpModel()
    {
        return null;
    }

    public virtual void updateModels()
    {
    }

    public virtual void stepFinished()
    {
    }

    // get current flowing into node n out of this element
    public virtual double getCurrentIntoNode(int n)
    {
        // if we take out the getPostCount() == 2 it gives the wrong value for rails
        if (n == 0 && getPostCount() == 2)
            return -current;
        else
            return current;
    }

    void flipPosts()
    {
        int oldx = x;
        int oldy = y;
        x = x2;
        y = y2;
        x2 = oldx;
        y2 = oldy;
        setPoints();
    }

    double getVoltageJS(int n)
    {
        if (n >= volts.Length)
            return 0;
        return volts[n];
    }
}