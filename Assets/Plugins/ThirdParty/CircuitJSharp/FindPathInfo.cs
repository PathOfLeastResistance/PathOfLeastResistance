using System;

namespace CircuitJSharp
{
    public class FindPathInfo
    {
        public const int INDUCT = 1;
        public const int VOLTAGE = 2;
        public const int SHORT = 3;
        public const int CAP_V = 4;
       
        private bool[] visited;
        private int dest;
        private CircuitElm firstElm;
        private int type;

        private CirSim cirsim;

        // State object to help find loops in circuit subject to various conditions (depending on type_)
        // elm_ = source and destination element.  dest_ = destination node.
        public FindPathInfo(CirSim sim, int type_, CircuitElm elm_, int dest_)
        {
            dest = dest_;
            type = type_;
            firstElm = elm_;
            cirsim = sim;
            visited = new bool[sim.nodeList.Count];
        }

        // look through circuit for loop starting at node n1 of firstElm, for a path back to
        // dest node of firstElm
        public bool findPath(int n1)
        {
            if (n1 == dest)
                return true;

            // depth first search, don't need to revisit already visited nodes!
            if (visited[n1])
                return false;

            visited[n1] = true;
            CircuitNode cn = cirsim.getCircuitNode(n1);
            int i;
            if (cn == null)
                return false;
            for (i = 0; i != cn.links.Count; i++)
            {
                CircuitNodeLink cnl = cn.links[i];
                CircuitElm ce = cnl.elm;
                if (checkElm(n1, ce))
                    return true;
            }

            if (n1 == 0)
            {
                for (i = 0; i != cirsim.nodesWithGroundConnection.Count; i++)
                    if (checkElm(0, cirsim.nodesWithGroundConnection[i]))
                        return true;
            }

            return false;
        }

        bool checkElm(int n1, CircuitElm ce)
        {
            if (ce == firstElm)
                return false;
            if (type == INDUCT)
            {
                // inductors need a path free of current sources
                if (ce is CurrentElm)
                    return false;
            }

            if (type == VOLTAGE)
            {
                // when checking for voltage loops, we only care about voltage sources/wires/ground
                if (!(ce.isWireEquivalent() || ce is VoltageElm || ce is GroundElm))
                    return false;
            }

            // when checking for shorts, just check wires
            if (type == SHORT && !ce.isWireEquivalent())
                return false;
            if (type == CAP_V)
            {
                // checking for capacitor/voltage source loops
                if (!(ce.isWireEquivalent() || ce is CapacitorElm || ce is VoltageElm))
                    return false;
            }

            if (n1 == 0)
            {
                // look for posts which have a ground connection;
                // our path can go through ground
                for (int j = 0; j != ce.getConnectionNodeCount(); j++)
                    if (ce.hasGroundConnection(j) && findPath(ce.getConnectionNode(j)))
                        return true;
            }

            for (int j = 0; j != ce.getConnectionNodeCount(); j++)
            {
                if (ce.getConnectionNode(j) == n1)
                {
                    if (ce.hasGroundConnection(j) && findPath(0))
                        return true;
                    if (type == INDUCT && ce is InductorElm)
                    {
                        // inductors can use paths with other inductors of matching current
                        double c = ce.getCurrent();
                        if (j == 0)
                            c = -c;
                        if (Math.Abs(c - firstElm.getCurrent()) > 1e-10)
                            continue;
                    }

                    int k;
                    for (k = 0; k != ce.getConnectionNodeCount(); k++)
                    {
                        if (j == k)
                            continue;
                        if (ce.getConnection(j, k) && findPath(ce.getConnectionNode(k)))
                        {
                            //System.out.println("got findpath " + n1);
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}