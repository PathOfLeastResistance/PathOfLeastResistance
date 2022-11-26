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

// GWT conversion (c) 2015 by Iain Sharp

// For information about the theory behind this, see Electronic Circuit & System Simulation Methods by Pillage
// or https://github.com/sharpie7/circuitjs1/blob/master/INTERNALS.md

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace CircuitJSharp
{
    public class CirSim
    {
        public Random random;

        // Class addingClass;
        public static double pi = 3.14159265358979323846;
        int dragGridX, dragGridY, dragScreenX, dragScreenY, initDragGridX, initDragGridY;
        long mouseDownTime;
        long zoomTime;
        int gridSize, gridMask, gridRound;
        bool dragging;
        public bool analyzeFlag, needsStamp, savedFlag;
        bool dumpMatrix;
        public bool dcAnalysisFlag;
        String ctrlMetaKey;
        public double t;
        String stopMessage;

        // current timestep (time between iterations)
        public double timeStep;

        // maximum timestep (== timeStep unless we reduce it because of trouble
        // converging)
        double maxTimeStep;
        double minTimeStep;

        // accumulated time since we incremented timeStepCount
        double timeStepAccum;

        // incremented each time we advance t by maxTimeStep
        int timeStepCount;

        bool adjustTimeStep;
        bool developerMode;

        List<CircuitElm> elmList;

        // Vector setupList;
        CircuitElm dragElm, menuElm, stopElm;
        CircuitElm[] elmArr;
        CircuitElm plotXElm, plotYElm;
        int draggingPost;
        double[,] circuitMatrix;
        private double[] circuitRightSide, lastNodeVoltages, nodeVoltages, origRightSide;
        double[,] origMatrix;
        int[] circuitPermute;
        bool simRunning;
        bool circuitNonLinear;
        int voltageSourceCount;
        int circuitMatrixSize, circuitMatrixFullSize;
        RowInfo[] circuitRowInfo;

        bool circuitNeedsMap;

        // public boolean useFrame;
        int scopeCount;
        double[] transform;

        // canvas width/height in px (before device pixel ratio scaling)
        int canvasWidth, canvasHeight;

        const int MENUBARHEIGHT = 30;
        const int VERTICALPANELWIDTH = 166; // default
        const int POSTGRABSQ = 25;
        const int MINPOSTGRABSIZE = 256;

        public static void console(string str)
        {
            Debug.LogError(str);
        }

        int getrand(int x)
        {
            int q = random.Next();
            if (q < 0)
                q = -q;
            return q % x;
        }

        public CirSim()
        {
            theSim = this;
        }

        public void init()
        {
            CircuitElm.initClass(this);
            elmList = new List<CircuitElm>();
            random = new Random();
            // setSimRunning(running);
            setSimRunning(true);

            // maxTimeStep = 5e-5;
            // minTimeStep = 50e-12;
            maxTimeStep = 5e-4;
            minTimeStep = 50e-6;
        }

        public void AddElement(CircuitElm elm)
        {
            elmList.Add(elm);
            needAnalyze();
        }

        public void RemoveElement(CircuitElm elm)
        {
            elmList.Remove(elm);
            needAnalyze();
        }

        public int ElementsCount => elmList.Count;

        private long lastTime = 0, lastFrameTime, lastIterTime;
        public long secTime = 0;
        int frames = 0;
        int steps = 0;
        public int framerate = 0;
        public int steprate = 0;
        static CirSim theSim;

        public void setSimRunning(bool s)
        {
            if (s)
            {
                if (stopMessage != null)
                    return;
                simRunning = true;
                //timer.scheduleRepeating(FASTTIMER); VJ
            }
            else
            {
                simRunning = false;
                //timer.cancel(); VJ
            }
        }

        public bool simIsRunning()
        {
            return simRunning;
        }

        // *****************************************************************
        //                     UPDATE CIRCUIT
        public void updateCircuit()
        {
            // Analyze circuit
            bool didAnalyze = analyzeFlag;
            if (analyzeFlag || dcAnalysisFlag)
            {
                analyzeCircuit();
                analyzeFlag = false;
            }

            // Stamp circuit
            if (needsStamp && simRunning)
            {
                try
                {
                    stampCircuit();
                }
                catch (Exception)
                {
                    stop("Exception in stampCircuit()", null);
                }
            }

            // setupScopes(); VJ

            // Run circuit
            if (simRunning)
            {
                if (needsStamp)
                {
                    console("needsStamp while simRunning?");
                }

                try
                {
                    runCircuit(didAnalyze);
                }
                catch (Exception e)
                {
                    //LOGGER
                    // debugger();; 
                    console("exception in runCircuit " + e);
                }
            }

            // CircuitElm.powerMult = Math.exp(powerBar.getValue() / 4.762 - 7);

            frames++;

            // if we did DC analysis, we need to re-analyze the circuit with that flag
            // cleared.
            if (dcAnalysisFlag)
            {
                dcAnalysisFlag = false;
                analyzeFlag = true;
            }

            lastFrameTime = lastTime;

            // This should always be the last 
            // thing called by updateCircuit();
            // callUpdateHook();
        }

        public void needAnalyze()
        {
            analyzeFlag = true;
        }

        public List<CircuitNode> nodeList;
        public List<Point> postDrawList = new List<Point>();
        public List<Point> badConnectionList = new List<Point>();
        public CircuitElm[] voltageSources;

        public CircuitNode getCircuitNode(int n)
        {
            if (n >= nodeList.Count)
                return null;
            return nodeList[n];
        }

        public CircuitElm getElm(int n)
        {
            if (n >= elmList.Count)
                return null;
            return elmList[n];
        }

        class NodeMapEntry
        {
            public int node;

            public NodeMapEntry()
            {
                node = -1;
            }

            public NodeMapEntry(int n)
            {
                node = n;
            }
        }

        // map points to node numbers
        Dictionary<Point, NodeMapEntry> nodeMap;
        Dictionary<Point, int> postCountMap;

        class WireInfo
        {
            public CircuitElm wire;
            public List<CircuitElm> neighbors;
            public int post;

            public WireInfo(CircuitElm w)
            {
                wire = w;
            }
        }

        // info about each wire and its neighbors, used to calculate wire currents
        List<WireInfo> wireInfoList;

        // find groups of nodes connected by wire equivalents and map them to the same node.  this speeds things
        // up considerably by reducing the size of the matrix.  We do this for wires, labeled nodes, and ground.
        // The actual node we map to is not assigned yet.  Instead we map to the same NodeMapEntry.
        void calculateWireClosure()
        {
            int i;
            // LabeledNodeElm.resetNodeList(); VJ
            GroundElm.resetNodeList();
            nodeMap = new Dictionary<Point, NodeMapEntry>();
//	int mergeCount = 0;
            wireInfoList = new List<WireInfo>();
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                if (!ce.isRemovableWire())
                    continue;
                ce.hasWireInfo = false;
                wireInfoList.Add(new WireInfo(ce));
                Point p0 = ce.getPost(0);
                NodeMapEntry cn = nodeMap[p0];

                // what post are we connected to
                Point p1 = ce.getConnectedPost();
                if (p1 == null)
                {
                    // no connected post (true for labeled node the first time it's encountered, or ground)
                    if (cn == null)
                    {
                        cn = new NodeMapEntry();
                        nodeMap.Add(p0, cn);
                    }

                    continue;
                }

                nodeMap.TryGetValue(p1, out var cn2);
                if (cn != null && cn2 != null)
                {
                    // merge nodes; go through map and change all keys pointing to cn2 to point to cn
                    // for (Map.Entry < Point, NodeMapEntry > entry : nodeMap.entrySet())
                    var toReplace = new List<KeyValuePair<Point, NodeMapEntry>>();
                    foreach (var entry in nodeMap)
                    {
                        if (entry.Value == cn2)
                        {
                            // entry.setValue(cn);
                            toReplace.Add(new KeyValuePair<Point, NodeMapEntry>(entry.Key, cn));
                        }
                    }

                    foreach (var replacer in toReplace)
                        nodeMap[replacer.Key] = replacer.Value;

//		mergeCount++;
                    continue;
                }

                if (cn != null)
                {
                    nodeMap.Add(p1, cn);
                    continue;
                }

                if (cn2 != null)
                {
                    nodeMap.Add(p0, cn2);
                    continue;
                }

                // new entry
                cn = new NodeMapEntry();
                nodeMap.Add(p0, cn);
                nodeMap.Add(p1, cn);
            }

            //console("got " + (groupCount-mergeCount) + " groups with " + nodeMap.size() + " nodes " + mergeCount);
        }

        // generate info we need to calculate wire currents.  Most other elements calculate currents using
        // the voltage on their terminal nodes.  But wires have the same voltage at both ends, so we need
        // to use the neighbors' currents instead.  We used to treat wires as zero voltage sources to make
        // this easier, but this is very inefficient, since it makes the matrix 2 rows bigger for each wire.
        // We create a list of WireInfo objects instead to help us calculate the wire currents instead,
        // so we make the matrix less complex, and we only calculate the wire currents when we need them
        // (once per frame, not once per subiteration).  We need the WireInfos arranged in the correct order,
        // each one containing a list of neighbors and which end to use (since one end may be ready before
        // the other)
        bool calcWireInfo()
        {
            int i;
            int moved = 0;

            for (i = 0; i != wireInfoList.Count; i++)
            {
                WireInfo wi = wireInfoList[i];
                CircuitElm wire = wi.wire;
                CircuitNode cn1 = nodeList[wire.getNode(0)]; // both ends of wire have same node #
                int j;

                List<CircuitElm> neighbors0 = new List<CircuitElm>();
                List<CircuitElm> neighbors1 = new List<CircuitElm>();

                // assume each end is ready (except ground nodes which have one end)
                // labeled nodes are treated as having 2 terminals, see below
                bool isReady0 = true, isReady1 = !(wire is GroundElm);

                // go through elements sharing a node with this wire (may be connected indirectly
                // by other wires, but at least it's faster than going through all elements)
                for (j = 0; j != cn1.links.Count; j++)
                {
                    CircuitNodeLink cnl = cn1.links[j];
                    CircuitElm ce = cnl.elm;
                    if (ce == wire)
                        continue;
                    Point pt = ce.getPost(cnl.num);

                    // is this a wire that doesn't have wire info yet?  If so we can't use it yet.
                    // That would create a circular dependency.  So that side isn't ready.
                    bool notReady = (ce.isRemovableWire() && !ce.hasWireInfo);

                    // which post does this element connect to, if any?
                    if (pt.x == wire.points[0].x)
                    {
                        neighbors0.Add(ce);
                        if (notReady) isReady0 = false;
                    }
                    else if (wire.getPostCount() > 1)
                    {
                        Point p2 = wire.getConnectedPost();
                        if (pt.x == p2.x)
                        {
                            neighbors1.Add(ce);
                            if (notReady) isReady1 = false;
                        }
                    }
                    // else if (ce
                    //              is LabeledNodeElm && wire is LabeledNodeElm &&
                    //          ((LabeledNodeElm)ce).text == ((LabeledNodeElm)wire).text)
                    // {
                    //     // ce and wire are both labeled nodes with matching labels.  treat them as neighbors
                    //     neighbors1.Add(ce);
                    //     if (notReady) isReady1 = false;
                    // }
                }

                // does one of the posts have all information necessary to calculate current?
                if (isReady0)
                {
                    wi.neighbors = neighbors0;
                    wi.post = 0;
                    wire.hasWireInfo = true;
                    moved = 0;
                }
                else if (isReady1)
                {
                    wi.neighbors = neighbors1;
                    wi.post = 1;
                    wire.hasWireInfo = true;
                    moved = 0;
                }
                else
                {
                    // wireInfoList.Add(wireInfoList.remove(i--));
                    var el = wireInfoList[i];
                    wireInfoList.RemoveAt(i--);
                    wireInfoList.Add(el);
                    moved++;
                    if (moved > wireInfoList.Count * 2)
                    {
                        stop("wire loop detected", wire);
                        return false;
                    }
                }
            }

            return true;
        }

        // find or allocate ground node
        void setGroundNode()
        {
            int i;
            bool gotGround = false;
            bool gotRail = false;
            CircuitElm volt = null;

            //System.out.println("ac1");
            // look for voltage or ground element
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                if (ce is GroundElm)
                {
                    gotGround = true;

                    // set ground node to 0
                    NodeMapEntry nme = nodeMap[ce.getPost(0)];
                    nme.node = 0;
                    break;
                }

                if (ce is RailElm)
                    gotRail = true;
                if (volt == null && ce is VoltageElm)
                    volt = ce;
            }

            // if no ground, and no rails, then the voltage elm's first terminal
            // is ground
            if (!gotGround && volt != null && !gotRail)
            {
                CircuitNode cn = new CircuitNode();
                Point pt = volt.getPost(0);
                nodeList.Add(cn);

                // update node map
                nodeMap.TryGetValue(pt, out var cln);
                if (cln != null)
                    cln.node = 0;
                else
                    nodeMap.Add(pt, new NodeMapEntry(0));
            }
            else
            {
                // otherwise allocate extra node for ground
                CircuitNode cn = new CircuitNode();
                nodeList.Add(cn);
            }
        }

        // make list of nodes
        void makeNodeList()
        {
            int i, j;
            int vscount = 0;
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                int inodes = ce.getInternalNodeCount();
                int ivs = ce.getVoltageSourceCount();
                int posts = ce.getPostCount();

                // allocate a node for each post and match posts to nodes
                for (j = 0; j != posts; j++)
                {
                    Point pt = ce.getPost(j);
                    // var found = postCountMap.TryGetValue(pt, out var g);
                    // postCountMap.Add(pt, !found ? 1 : g + 1);
                    // NodeMapEntry cln = nodeMap[pt];
                    nodeMap.TryGetValue(pt, out var cln);
                    // is this node not in map yet?  or is the node number unallocated?
                    // (we don't allocate nodes before this because changing the allocation order
                    // of nodes changes circuit behavior and breaks backward compatibility;
                    // the code below to connect unconnected nodes may connect a different node to ground) 
                    if (cln == null || cln.node == -1)
                    {
                        CircuitNode cn = new CircuitNode();
                        CircuitNodeLink cnl = new CircuitNodeLink();
                        cnl.num = j;
                        cnl.elm = ce;
                        cn.links.Add(cnl);
                        ce.setNode(j, nodeList.Count);
                        if (cln != null)
                            cln.node = nodeList.Count;
                        else
                            nodeMap.Add(pt, new NodeMapEntry(nodeList.Count));
                        nodeList.Add(cn);
                    }
                    else
                    {
                        int n = cln.node;
                        CircuitNodeLink cnl = new CircuitNodeLink();
                        cnl.num = j;
                        cnl.elm = ce;
                        getCircuitNode(n).links.Add(cnl);
                        ce.setNode(j, n);
                        // if it's the ground node, make sure the node voltage is 0,
                        // cause it may not get set later
                        if (n == 0)
                            ce.setNodeVoltage(j, 0);
                    }
                }

                for (j = 0; j != inodes; j++)
                {
                    CircuitNode cn = new CircuitNode();
                    cn.internaL = true;
                    CircuitNodeLink cnl = new CircuitNodeLink();
                    cnl.num = j + posts;
                    cnl.elm = ce;
                    cn.links.Add(cnl);
                    ce.setNode(cnl.num, nodeList.Count);
                    nodeList.Add(cn);
                }

// also count voltage sources so we can allocate array
                vscount += ivs;
            }

            voltageSources = new CircuitElm[vscount];
        }

        public List<int> unconnectedNodes;
        public List<CircuitElm> nodesWithGroundConnection;
        public int nodesWithGroundConnectionCount;

        void findUnconnectedNodes()
        {
            int i, j;

            // determine nodes that are not connected indirectly to ground.
            // all nodes must be connected to ground somehow, or else we
            // will get a matrix error.
            bool[] closure = new bool[nodeList.Count];
            bool changed = true;
            unconnectedNodes = new List<int>();
            nodesWithGroundConnection = new List<CircuitElm>();
            closure[0] = true;
            while (changed)
            {
                changed = false;
                for (i = 0; i != elmList.Count; i++)
                {
                    CircuitElm ce = getElm(i);
                    if (ce is WireElm)
                        continue;
                    // loop through all ce's nodes to see if they are connected
                    // to other nodes not in closure
                    bool hasGround = false;
                    for (j = 0; j < ce.getConnectionNodeCount(); j++)
                    {
                        bool hg = ce.hasGroundConnection(j);
                        if (hg)
                            hasGround = true;
                        if (!closure[ce.getConnectionNode(j)])
                        {
                            if (hg)
                                closure[ce.getConnectionNode(j)] = changed = true;
                            continue;
                        }

                        int k;
                        for (k = 0; k != ce.getConnectionNodeCount(); k++)
                        {
                            if (j == k)
                                continue;
                            int kn = ce.getConnectionNode(k);
                            if (ce.getConnection(j, k) && !closure[kn])
                            {
                                closure[kn] = true;
                                changed = true;
                            }
                        }
                    }

                    if (hasGround)
                        nodesWithGroundConnection.Add(ce);
                }

                if (changed)
                    continue;

                // connect one of the unconnected nodes to ground with a big resistor, then try again
                for (i = 0; i != nodeList.Count; i++)
                    if (!closure[i] && !getCircuitNode(i).internaL)
                    {
                        unconnectedNodes.Add(i);
                        // console("node " + i + " unconnected"); LOGGER
//		    stampResistor(0, i, 1e8);   // do this later in connectUnconnectedNodes()
                        closure[i] = true;
                        changed = true;
                        break;
                    }
            }
        }

// take list of unconnected nodes, which we identified earlier, and connect them to ground
// with a big resistor.  otherwise we will get matrix errors.  The resistor has to be big,
// otherwise circuits like 555 Square Wave will break
        void connectUnconnectedNodes()
        {
            int i;
            for (i = 0; i != unconnectedNodes.Count; i++)
            {
                int n = unconnectedNodes[i];
                stampResistor(0, n, 1e8);
            }
        }

        bool validateCircuit()
        {
            for (int i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                // look for inductors with no current path
                if (ce is InductorElm)
                {
                    FindPathInfo fpi = new FindPathInfo(this, FindPathInfo.INDUCT, ce,
                        ce.getNode(1));
                    if (!fpi.findPath(ce.getNode(0)))
                    {
                        console(ce + " no path");
                        ce.reset();
                    }
                }

                // look for current sources with no current path
                if (ce is CurrentElm)
                {
                    CurrentElm cur = (CurrentElm)ce;
                    FindPathInfo fpi = new FindPathInfo(this, FindPathInfo.INDUCT, ce,
                        ce.getNode(1));
                    cur.setBroken(!fpi.findPath(ce.getNode(0)));
                }

                if (ce is VCCSElm)
                {
                    VCCSElm cur = (VCCSElm)ce;
                    FindPathInfo fpi = new FindPathInfo(this, FindPathInfo.INDUCT, ce, cur.getOutputNode(0));
                    if (cur.hasCurrentOutput() && !fpi.findPath(cur.getOutputNode(1)))
                    {
                        cur.broken = true;
                    }
                    else
                        cur.broken = false;
                }

                // look for voltage source or wire loops.  we do this for voltage sources
                if (ce.getPostCount() == 2)
                {
                    if (ce is VoltageElm)
                    {
                        FindPathInfo fpi = new FindPathInfo(this, FindPathInfo.VOLTAGE, ce,
                            ce.getNode(1));
                        if (fpi.findPath(ce.getNode(0)))
                        {
                            stop("Voltage source/wire loop with no resistance!", ce);
                            return false;
                        }
                    }
                }

                // look for path from rail to ground
                if (ce is RailElm || ce is LogicInputElm)
                {
                    FindPathInfo fpi = new FindPathInfo(this, FindPathInfo.VOLTAGE, ce, ce.getNode(0));
                    if (fpi.findPath(0))
                    {
                        stop("Path to ground with no resistance!", ce);
                        return false;
                    }
                }

                // look for shorted caps, or caps w/ voltage but no R
                if (ce is CapacitorElm)
                {
                    FindPathInfo fpi = new FindPathInfo(this, FindPathInfo.SHORT, ce,
                        ce.getNode(1));
                    if (fpi.findPath(ce.getNode(0)))
                    {
                        console(ce + " shorted");
                        ((CapacitorElm)ce).shorted();
                    }
                    else
                    {
                        // a capacitor loop used to cause a matrix error. but we changed the capacitor model
                        // so it works fine now. The only issue is if a capacitor is added in parallel with
                        // another capacitor with a nonzero voltage; in that case we will get oscillation unless
                        // we reset both capacitors to have the same voltage. Rather than check for that, we just
                        // give an error.
                        fpi = new FindPathInfo(this, FindPathInfo.CAP_V, ce, ce.getNode(1));
                        if (fpi.findPath(ce.getNode(0)))
                        {
                            stop("Capacitor loop with no resistance!", ce);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

// analyze the circuit when something changes, so it can be simulated
        void analyzeCircuit()
        {
            stopMessage = null;
            stopElm = null;
            if (elmList.Count == 0)
            {
                postDrawList = new List<Point>();
                badConnectionList = new List<Point>();
                return;
            }

            int i, j;
            nodeList = new List<CircuitNode>();
            // postCountMap = new Dictionary<Point, int>();

            calculateWireClosure();
            setGroundNode();

            // allocate nodes and voltage sources
            makeNodeList();

            // makePostDrawList(); VJ
            if (!calcWireInfo())
                return;
            nodeMap = null; // done with this

            int vscount = 0;
            circuitNonLinear = false;

            // determine if circuit is nonlinear.  also set voltage sources
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                if (ce.nonLinear())
                    circuitNonLinear = true;
                int ivs = ce.getVoltageSourceCount();
                for (j = 0; j != ivs; j++)
                {
                    voltageSources[vscount] = ce;
                    ce.setVoltageSource(j, vscount++);
                }
            }

            voltageSourceCount = vscount;

            // show resistance in voltage sources if there's only one.
            // can't use voltageSourceCount here since that counts internal voltage sources, like the one in GroundElm
            // bool gotVoltageSource = false;
            // showResistanceInVoltageSources = true;
            // for (i = 0; i != elmList.Count; i++)
            // {
            //     CircuitElm ce = getElm(i);
            //     if (ce is VoltageElm)
            //     {
            //         if (gotVoltageSource)
            //             showResistanceInVoltageSources = false;
            //         else
            //             gotVoltageSource = true;
            //     }
            // }

            findUnconnectedNodes();
            if (!validateCircuit())
                return;

            nodesWithGroundConnectionCount = nodesWithGroundConnection.Count;
            // only need this for validation
            nodesWithGroundConnection = null;

            timeStep = maxTimeStep;
            needsStamp = true;

            // callAnalyzeHook();
        }

// stamp the matrix, meaning populate the matrix as required to simulate the circuit (for all linear elements, at least)
        void stampCircuit()
        {
            int i;
            int matrixSize = nodeList.Count - 1 + voltageSourceCount;
            circuitMatrix = new double[matrixSize, matrixSize];
            circuitRightSide = new double[matrixSize];
            nodeVoltages = new double[nodeList.Count - 1];
            if (lastNodeVoltages == null || lastNodeVoltages.Length != nodeVoltages.Length)
                lastNodeVoltages = new double[nodeList.Count - 1];
            origMatrix = new double[matrixSize, matrixSize];

            origRightSide = new double[matrixSize];
            circuitMatrixSize = circuitMatrixFullSize = matrixSize;
            circuitRowInfo = new RowInfo[matrixSize];
            circuitPermute = new int[matrixSize];
            for (i = 0; i != matrixSize; i++)
                circuitRowInfo[i] = new RowInfo();
            circuitNeedsMap = false;

            connectUnconnectedNodes();

            // stamp linear circuit elements
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                ce.setParentList(elmList);
                ce.stamp();
            }

            if (!simplifyMatrix(matrixSize))
                return;

            // check if we called stop()
            if (circuitMatrix == null)
                return;

            // if a matrix is linear, we can do the lu_factor here instead of
            // needing to do it every frame
            if (!circuitNonLinear)
            {
                if (!lu_factor(circuitMatrix, circuitMatrixSize, circuitPermute))
                {
                    stop("Singular matrix!", null);
                    return;
                }
            }

            // copy elmList to an array to avoid a bunch of calls to canCast() when doing simulation
            elmArr = new CircuitElm[elmList.Count];
            // int scopeElmCount = 0;
            for (i = 0; i != elmList.Count; i++)
            {
                elmArr[i] = elmList[i];
                // if (elmArr[i] is ScopeElm)
                //     scopeElmCount++;
            }

            needsStamp = false;
        }

// simplify the matrix; this speeds things up quite a bit, especially for digital circuits.
// or at least it did before we added wire removal
        bool simplifyMatrix(int matrixSize)
        {
            int i, j;
            for (i = 0; i != matrixSize; i++)
            {
                int qp = -1;
                double qv = 0;
                RowInfo re = circuitRowInfo[i];
                /*System.out.println("row " + i + " " + re.lsChanges + " " + re.rsChanges + " " +
                           re.dropRow);*/

//	    if (qp != -100) continue;   // uncomment this line to disable matrix simplification for debugging purposes

                if (re.lsChanges || re.dropRow || re.rsChanges)
                    continue;
                double rsadd = 0;

                // see if this row can be removed
                for (j = 0; j != matrixSize; j++)
                {
                    double q = circuitMatrix[i, j];
                    if (circuitRowInfo[j].type == RowInfo.ROW_CONST)
                    {
                        // keep a running total of const values that have been
                        // removed already
                        rsadd -= circuitRowInfo[j].value * q;
                        continue;
                    }

                    // ignore zeroes
                    if (q == 0)
                        continue;
                    // keep track of first nonzero element that is not ROW_CONST
                    if (qp == -1)
                    {
                        qp = j;
                        qv = q;
                        continue;
                    }

                    // more than one nonzero element?  give up
                    break;
                }

                if (j == matrixSize)
                {
                    if (qp == -1)
                    {
                        // probably a singular matrix, try disabling matrix simplification above to check this
                        stop("Matrix error", null);
                        return false;
                    }

                    RowInfo elt = circuitRowInfo[qp];
                    // we found a row with only one nonzero nonconst entry; that value
                    // is a constant
                    if (elt.type != RowInfo.ROW_NORMAL)
                    {
                        //System.out.println("type already " + elt.type + " for " + qp + "!"); LOGGER
                        continue;
                    }

                    elt.type = RowInfo.ROW_CONST;
                    //		console("ROW_CONST " + i + " " + rsadd);
                    elt.value = (circuitRightSide[i] + rsadd) / qv;
                    circuitRowInfo[i].dropRow = true;
                    // find first row that referenced the element we just deleted
                    for (j = 0; j != i; j++)
                        if (circuitMatrix[j, qp] != 0)
                            break;
                    // start over just before that
                    i = j - 1;
                }
            }
            //System.out.println("ac7");

            // find size of new matrix
            int nn = 0;
            for (i = 0; i != matrixSize; i++)
            {
                RowInfo elt = circuitRowInfo[i];
                if (elt.type == RowInfo.ROW_NORMAL)
                {
                    elt.mapCol = nn++;
                    //System.out.println("col " + i + " maps to " + elt.mapCol);
                    continue;
                }

                if (elt.type == RowInfo.ROW_CONST)
                    elt.mapCol = -1;
            }

            // make the new, simplified matrix
            int newsize = nn;
            double[,] newmatx = new double[newsize, newsize];
            double[] newrs = new double[newsize];
            int ii = 0;
            for (i = 0; i != matrixSize; i++)
            {
                RowInfo rri = circuitRowInfo[i];
                if (rri.dropRow)
                {
                    rri.mapRow = -1;
                    continue;
                }

                newrs[ii] = circuitRightSide[i];
                rri.mapRow = ii;
                //System.out.println("Row " + i + " maps to " + ii);
                for (j = 0; j != matrixSize; j++)
                {
                    RowInfo ri = circuitRowInfo[j];
                    if (ri.type == RowInfo.ROW_CONST)
                        newrs[ii] -= ri.value * circuitMatrix[i, j];
                    else
                        newmatx[ii, ri.mapCol] += circuitMatrix[i, j];
                }

                ii++;
            }

            //	console("old size = " + matrixSize + " new size = " + newsize);

            circuitMatrix = newmatx;
            circuitRightSide = newrs;
            matrixSize = circuitMatrixSize = newsize;
            for (i = 0; i != matrixSize; i++)
                origRightSide[i] = circuitRightSide[i];
            for (i = 0; i != matrixSize; i++)
            for (j = 0; j != matrixSize; j++)
                origMatrix[i, j] = circuitMatrix[i, j];
            circuitNeedsMap = true;
            return true;
        }

        public void stop(string s, CircuitElm ce)
        {
            stopMessage = s;
            circuitMatrix = null; // causes an exception
            stopElm = ce;
            setSimRunning(false);
            analyzeFlag = false;
            console(stopMessage);
            //	cv.repaint();
        }

        // control voltage source vs with voltage from n1 to n2 (must
        // also call stampVoltageSource())
        public void stampVCVS(int n1, int n2, double coef, int vs)
        {
            int vn = nodeList.Count + vs;
            stampMatrix(vn, n1, coef);
            stampMatrix(vn, n2, -coef);
        }

        // stamp independent voltage source #vs, from n1 to n2, amount v
        public void stampVoltageSource(int n1, int n2, int vs, double v)
        {
            int vn = nodeList.Count + vs;
            stampMatrix(vn, n1, -1);
            stampMatrix(vn, n2, 1);
            stampRightSide(vn, v);
            stampMatrix(n1, vn, 1);
            stampMatrix(n2, vn, -1);
        }

        // use this if the amount of voltage is going to be updated in doStep(), by updateVoltageSource()
        public void stampVoltageSource(int n1, int n2, int vs)
        {
            int vn = nodeList.Count + vs;
            stampMatrix(vn, n1, -1);
            stampMatrix(vn, n2, 1);
            stampRightSide(vn);
            stampMatrix(n1, vn, 1);
            stampMatrix(n2, vn, -1);
        }

        // update voltage source in doStep()
        public void updateVoltageSource(int n1, int n2, int vs, double v)
        {
            int vn = nodeList.Count + vs;
            stampRightSide(vn, v);
        }

        public void stampResistor(int n1, int n2, double r)
        {
            double r0 = 1 / r;
            if (Double.IsNaN(r0) || Double.IsInfinity(r0))
            {
                //System.out.print("bad resistance " + r + " " + r0 + "\n"); LOGGER
                throw new Exception("bad resistance");
            }

            stampMatrix(n1, n1, r0);
            stampMatrix(n2, n2, r0);
            stampMatrix(n1, n2, -r0);
            stampMatrix(n2, n1, -r0);
        }

        public void stampConductance(int n1, int n2, double r0)
        {
            stampMatrix(n1, n1, r0);
            stampMatrix(n2, n2, r0);
            stampMatrix(n1, n2, -r0);
            stampMatrix(n2, n1, -r0);
        }

        // specify that current from cn1 to cn2 is equal to voltage from vn1 to 2, divided by g
        public void stampVCCurrentSource(int cn1, int cn2, int vn1, int vn2, double g)
        {
            stampMatrix(cn1, vn1, g);
            stampMatrix(cn2, vn2, g);
            stampMatrix(cn1, vn2, -g);
            stampMatrix(cn2, vn1, -g);
        }

        public void stampCurrentSource(int n1, int n2, double i)
        {
            stampRightSide(n1, -i);
            stampRightSide(n2, i);
        }

        // stamp a current source from n1 to n2 depending on current through vs
        void stampCCCS(int n1, int n2, int vs, double gain)
        {
            int vn = nodeList.Count + vs;
            stampMatrix(n1, vn, gain);
            stampMatrix(n2, vn, -gain);
        }

        // stamp value x in row i, column j, meaning that a voltage change
        // of dv in node j will increase the current into node i by x dv.
        // (Unless i or j is a voltage source node.)
        void stampMatrix(int i, int j, double x)
        {
            if (Double.IsInfinity(x))
            {
                // debugger(); //LOGGER
            }

            if (i > 0 && j > 0)
            {
                if (circuitNeedsMap)
                {
                    i = circuitRowInfo[i - 1].mapRow;
                    RowInfo ri = circuitRowInfo[j - 1];
                    if (ri.type == RowInfo.ROW_CONST)
                    {
                        //System.out.println("Stamping constant " + i + " " + j + " " + x);
                        circuitRightSide[i] -= x * ri.value;
                        return;
                    }

                    j = ri.mapCol;
                    //System.out.println("stamping " + i + " " + j + " " + x);
                }
                else
                {
                    i--;
                    j--;
                }

                circuitMatrix[i, j] += x;
            }
        }

        // stamp value x on the right side of row i, representing an
        // independent current source flowing into node i
        public void stampRightSide(int i, double x)
        {
            if (i > 0)
            {
                if (circuitNeedsMap)
                {
                    i = circuitRowInfo[i - 1].mapRow;
                    //System.out.println("stamping " + i + " " + x);
                }
                else
                    i--;

                circuitRightSide[i] += x;
            }
        }

        // indicate that the value on the right side of row i changes in doStep()
        public void stampRightSide(int i)
        {
            //System.out.println("rschanges true " + (i-1));
            if (i > 0)
                circuitRowInfo[i - 1].rsChanges = true;
        }

        // indicate that the values on the left side of row i change in doStep()
        public void stampNonLinear(int i)
        {
            if (i > 0)
                circuitRowInfo[i - 1].lsChanges = true;
        }

        double getIterCount()
        {
            // IES - remove interaction
            return 0.1 * Math.Exp((1 - 61) / 24.0);
        }

        // we need to calculate wire currents for every iteration if someone is viewing a wire in the
        // scope.  Otherwise we can do it only once per frame.
        bool canDelayWireProcessing()
        {
            return false;
            // int i;
            // for (i = 0; i != scopeCount; i++)
            //     if (scopes[i].viewingWire())
            // 	return false;
            // for (i=0; i != elmList.size(); i++)
            //     if (getElm(i) instanceof ScopeElm && ((ScopeElm)getElm(i)).elmScope.viewingWire())
            // 	return false;
            // return true;
        }

        public bool converged;
        public int subIterations;
        public double timeDelta = 0.1;

        void runCircuit(bool didAnalyze)
        {
            var lastT = t;

            if (circuitMatrix == null || elmList.Count == 0)
            {
                circuitMatrix = null;
                return;
            }

            int iter;
            //int maxIter = getIterCount();
            bool debugprint = dumpMatrix;
            dumpMatrix = false;

            // Check if we don't need to run simulation (for very slow simulation speeds).
            // If the circuit changed, do at least one iteration to make sure everything is consistent.
            // if (!didAnalyze)
            //     return;

            bool delayWireProcessing = canDelayWireProcessing();
            int timeStepCountAtFrameStart = timeStepCount;

            // keep track of iterations completed without convergence issues
            int goodIterations = 100;
            bool goodIteration = true;

            for (iter = 1;; iter++)
            {
                if (goodIterations >= 3 && timeStep < maxTimeStep && goodIteration)
                {
                    // things are going well, double the time step
                    timeStep = Math.Min(timeStep * 2, maxTimeStep);
                    // console("timestep up = " + timeStep + " at " + t); LOGGER
                    stampCircuit();
                    goodIterations = 0;
                }

                int i, j, subiter;
                for (i = 0; i != elmArr.Length; i++)
                    elmArr[i].startIteration();
                steps++;
                int subiterCount = 5000; // (adjustTimeStep && timeStep / 2 > minTimeStep) ? 100 : 5000;
                for (subiter = 0; subiter != subiterCount; subiter++)
                {
                    converged = true;
                    subIterations = subiter;
//		if (t % .030 < .002 && timeStep > 1e-6)  // force nonconvergence for debugging
//		    converged = false;
                    for (i = 0; i != circuitMatrixSize; i++)
                        circuitRightSide[i] = origRightSide[i];
                    if (circuitNonLinear)
                    {
                        for (i = 0; i != circuitMatrixSize; i++)
                        for (j = 0; j != circuitMatrixSize; j++)
                            circuitMatrix[i, j] = origMatrix[i, j];
                    }

                    for (i = 0; i != elmArr.Length; i++)
                        elmArr[i].doStep();
                    if (stopMessage != null)
                        return;
                    bool printit = debugprint;
                    debugprint = false;
                    if (circuitMatrixSize < 8)
                    {
                        // we only need this for debugging purposes, so skip it for large matrices 
                        for (j = 0; j != circuitMatrixSize; j++)
                        {
                            for (i = 0; i != circuitMatrixSize; i++)
                            {
                                double x = circuitMatrix[i, j];
                                if (Double.IsNaN(x) || Double.IsInfinity(x))
                                {
                                    stop("nan/infinite matrix!", null);
                                    console("circuitMatrix " + i + " " + j + " is " + x);
                                    return;
                                }
                            }
                        }
                    }

                    if (printit)
                    {
                        for (j = 0; j != circuitMatrixSize; j++)
                        {
                            String x = "";
                            for (i = 0; i != circuitMatrixSize; i++)
                                x += circuitMatrix[j, i] + ",";
                            x += "\n";
                            console(x);
                        }

                        console("done");
                    }

                    if (circuitNonLinear)
                    {
                        // stop if converged (elements check for convergence in doStep())
                        if (converged && subiter > 0)
                            break;
                        if (!lu_factor(circuitMatrix, circuitMatrixSize,
                                circuitPermute))
                        {
                            stop("Singular matrix!", null);
                            return;
                        }
                    }

                    lu_solve(circuitMatrix, circuitMatrixSize, circuitPermute,
                        circuitRightSide);
                    applySolvedRightSide(circuitRightSide);
                    if (!circuitNonLinear)
                        break;
                }

                if (subiter == subiterCount)
                {
                    // convergence failed
                    goodIterations = 0;
                    if (adjustTimeStep)
                    {
                        timeStep /= 2;
                        console("timestep down to " + timeStep + " at " + t);
                    }

                    if (timeStep < minTimeStep || !adjustTimeStep)
                    {
                        console("convergence failed after " + subiter + " iterations");
                        stop("Convergence failed!", null);
                        break;
                    }

                    // we reduced the timestep.  reset circuit state to the way it was at start of iteration
                    setNodeVoltages(lastNodeVoltages);
                    stampCircuit();
                    continue;
                }

                if (subiter > 5 || timeStep < maxTimeStep)
                {
                    //console("converged after " + subiter + " iterations, timeStep = " + timeStep);
                }

                if (subiter < 3 && goodIteration)
                    goodIterations++;
                else
                    goodIterations = 0;
                t += timeStep;
                timeStepAccum += timeStep;
                goodIteration = true;
                if (timeStepAccum >= maxTimeStep)
                {
                    timeStepAccum -= maxTimeStep;
                    timeStepCount++;
                }

                for (i = 0; i != elmArr.Length; i++)
                    elmArr[i].stepFinished();
                if (!delayWireProcessing)
                    calcWireCurrents();
                // for (i = 0; i != scopeCount; i++)
                //     scopes[i].timeStep();
                // for (i = 0; i != scopeElmArr.length; i++)
                //     scopeElmArr[i].stepScope();

                callTimeStepHook();
                // save last node voltages so we can restart the next iteration if necessary
                for (i = 0; i != lastNodeVoltages.Length; i++)
                    lastNodeVoltages[i] = nodeVoltages[i];
//	    console("set lastrightside at " + t + " " + lastNodeVoltages);

                // Check whether enough time has elapsed to perform an *additional* iteration after
                // those we have already completed.  But limit total computation time to 50ms (20fps)
                // if ((timeStepCount - timeStepCountAtFrameStart) * 1000 >= steprate * (tm - lastIterTime) || (tm - lastFrameTime > 50))
                //     break;
                if (t - lastT > timeDelta)
                    break;

                if (!simRunning)
                    break;
            } // for (iter = 1; ; iter++)

            if (delayWireProcessing)
                calcWireCurrents();
//	System.out.println((System.currentTimeMillis()-lastFrameTime)/(double) iter);
        }

// set node voltages given right side found by solving matrix
        void applySolvedRightSide(double[] rs)
        {
            // console("setvoltages " + rs);
            int j;
            for (j = 0; j != circuitMatrixFullSize; j++)
            {
                RowInfo ri = circuitRowInfo[j];
                double res = 0;
                if (ri.type == RowInfo.ROW_CONST)
                    res = ri.value;
                else
                    res = rs[ri.mapCol];
                if (Double.IsNaN(res))
                {
                    converged = false;
                    break;
                }

                if (j < nodeList.Count - 1)
                {
                    nodeVoltages[j] = res;
                }
                else
                {
                    int ji = j - (nodeList.Count - 1);
                    voltageSources[ji].setCurrent(ji, res);
                }
            }

            setNodeVoltages(nodeVoltages);
        }

// set node voltages in each element given an array of node voltages
        void setNodeVoltages(double[] nv)
        {
            int j, k;
            for (j = 0; j != nv.Length; j++)
            {
                double res = nv[j];
                CircuitNode cn = getCircuitNode(j + 1);
                for (k = 0; k != cn.links.Count; k++)
                {
                    CircuitNodeLink cnl = cn.links[k];
                    cnl.elm.setNodeVoltage(cnl.num, res);
                }
            }
        }

// we removed wires from the matrix to speed things up.  in order to display wire currents,
// we need to calculate them now.
        void calcWireCurrents()
        {
            int i;

            // for debugging
            //for (i = 0; i != wireInfoList.size(); i++)
            //   wireInfoList.get(i).wire.setCurrent(-1, 1.23);

            for (i = 0; i != wireInfoList.Count; i++)
            {
                WireInfo wi = wireInfoList[i];
                double cur = 0;
                int j;
                Point p = wi.wire.getPost(wi.post);
                for (j = 0; j != wi.neighbors.Count; j++)
                {
                    CircuitElm ce = wi.neighbors[j];
                    int n = ce.getNodeAtPoint(p.x);
                    cur += ce.getCurrentIntoNode(n);
                }

                // get correct current polarity
                // (LabeledNodes may have wi.post == 1, in which case we flip the current sign)
                if (wi.post == 0) //|| (wi.wire is LabeledNodeElm))
                    wi.wire.setCurrent(-1, cur);
                else
                    wi.wire.setCurrent(-1, -cur);
            }
        }

        int min(int a, int b)
        {
            return (a < b) ? a : b;
        }

        int max(int a, int b)
        {
            return (a > b) ? a : b;
        }

        public void resetAction()
        {
            int i;
            analyzeFlag = true;
            if (t == 0)
                setSimRunning(true);
            t = timeStepAccum = 0;
            timeStepCount = 0;
            for (i = 0; i != elmList.Count; i++)
                getElm(i).reset();
        }

        const int RC_RETAIN = 1;
        const int RC_NO_CENTER = 2;
        const int RC_SUBCIRCUITS = 4;
        const int RC_KEEP_TITLE = 8;

// boolean doSwitch(int x, int y) {
// 	if (mouseElm == null || !(mouseElm instanceof SwitchElm))
// 		return false;
// 	SwitchElm se = (SwitchElm) mouseElm;
// 	if (!se.getSwitchRect().contains(x, y))
// 	    return false;
// 	se.toggle();
// 	if (se.momentary)
// 	    heldSwitchElm = se;
// 	if (!(se instanceof LogicInputElm))
// 	    needAnalyze();
// 	return true;
// }

        int locateElm(CircuitElm elm)
        {
            int i;
            for (i = 0; i != elmList.Count; i++)
                if (elm == elmList[i])
                    return i;
            return -1;
        }

// factors a matrix into upper and lower triangular matrices by
// gaussian elimination.  On entry, a[0..n-1][0..n-1] is the
// matrix to be factored.  ipvt[] returns an integer vector of pivot
// indices, used in the lu_solve() routine.
        static bool lu_factor(double[,] a, int n, int[] ipvt)
        {
            int i, j, k;

            // check for a possible singular matrix by scanning for rows that
            // are all zeroes
            for (i = 0; i != n; i++)
            {
                bool row_all_zeros = true;
                for (j = 0; j != n; j++)
                {
                    if (a[i, j] != 0)
                    {
                        row_all_zeros = false;
                        break;
                    }
                }

                // if all zeros, it's a singular matrix
                if (row_all_zeros)
                    return false;
            }

            // use Crout's method; loop through the columns
            for (j = 0; j != n; j++)
            {
                // calculate upper triangular elements for this column
                for (i = 0; i != j; i++)
                {
                    double q = a[i, j];
                    for (k = 0; k != i; k++)
                        q -= a[i, k] * a[k, j];
                    a[i, j] = q;
                }

                // calculate lower triangular elements for this column
                double largest = 0;
                int largestRow = -1;
                for (i = j; i != n; i++)
                {
                    double q = a[i, j];
                    for (k = 0; k != j; k++)
                        q -= a[i, k] * a[k, j];
                    a[i, j] = q;
                    double x = Math.Abs(q);
                    if (x >= largest)
                    {
                        largest = x;
                        largestRow = i;
                    }
                }

                // pivoting
                if (j != largestRow)
                {
                    if (largestRow == -1)
                    {
                        console("largestRow == -1");
                        return false;
                    }

                    double x;
                    for (k = 0; k != n; k++)
                    {
                        x = a[largestRow, k];
                        a[largestRow, k] = a[j, k];
                        a[j, k] = x;
                    }
                }

                // keep track of row interchanges
                ipvt[j] = largestRow;

                // check for zeroes; if we find one, it's a singular matrix.
                // we used to avoid them, but that caused weird bugs.  For example,
                // two inverters with outputs connected together should be flagged
                // as a singular matrix, but it was allowed (with weird currents)
                if (a[j, j] == 0.0)
                {
                    // console("didn't avoid zero"); LOGGER
                    // a[j][j]=1e-18;
                    return false;
                }

                if (j != n - 1)
                {
                    double mult = 1.0 / a[j, j];
                    for (i = j + 1; i != n; i++)
                        a[i, j] *= mult;
                }
            }

            return true;
        }

// Solves the set of n linear equations using a LU factorization
// previously performed by lu_factor.  On input, b[0..n-1] is the right
// hand side of the equations, and on output, contains the solution.
        static void lu_solve(double[,] a, int n, int[] ipvt, double[] b)
        {
            int i;

            // find first nonzero b element
            for (i = 0; i != n; i++)
            {
                int row = ipvt[i];

                double swap = b[row];
                b[row] = b[i];
                b[i] = swap;
                if (swap != 0)
                    break;
            }

            int bi = i++;
            for (; i < n; i++)
            {
                int row = ipvt[i];
                int j;
                double tot = b[row];

                b[row] = b[i];
                // forward substitution using the lower triangular matrix
                for (j = bi; j < i; j++)
                    tot -= a[i, j] * b[j];
                b[i] = tot;
            }

            for (i = n - 1; i >= 0; i--)
            {
                double tot = b[i];

                // back-substitution using the upper triangular matrix
                int j;
                for (j = i + 1; j != n; j++)
                    tot -= a[i, j] * b[j];
                b[i] = tot / a[i, i];
            }
        }

        public void updateModels()
        {
            for (int i = 0; i != elmList.Count; i++)
                elmList[i].updateModels();
        }

        private void doDCAnalysis()
        {
            dcAnalysisFlag = true;
            resetAction();
        }

        private static void invertMatrix(double[,] a, int n)
        {
            int[] ipvt = new int[n];
            lu_factor(a, n, ipvt);
            int i, j;
            double[] b = new double[n];
            double[,] inva = new double[n, n];

            // solve for each column of identity matrix
            for (i = 0; i != n; i++)
            {
                for (j = 0; j != n; j++)
                    b[j] = 0;
                b[i] = 1;
                lu_solve(a, n, ipvt, b);
                for (j = 0; j != n; j++)
                    inva[j, i] = b[j];
            }

            // return in original matrix
            for (i = 0; i != n; i++)
            for (j = 0; j != n; j++)
                a[i, j] = inva[i, j];
        }

        public event Action OnTimeStepHook;

        private void callTimeStepHook()
        {
            OnTimeStepHook?.Invoke();
        }
    }
}