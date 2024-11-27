using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2016
{
    class Day22 : Core.Day
    {
        public Day22() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v2";
                case Core.Part.Two:
                    return "v2";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "",
                RawInput =
@""
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "7",
                RawInput =
@"root@ebhq-gridcenter# df -h
Filesystem            Size  Used  Avail  Use%
/dev/grid/node-x0-y0   10T    8T     2T   80%
/dev/grid/node-x0-y1   11T    6T     5T   54%
/dev/grid/node-x0-y2   32T   28T     4T   87%
/dev/grid/node-x1-y0    9T    7T     2T   77%
/dev/grid/node-x1-y1    8T    0T     8T    0%
/dev/grid/node-x1-y2   11T    7T     4T   63%
/dev/grid/node-x2-y0   10T    6T     4T   60%
/dev/grid/node-x2-y1    9T    8T     1T   88%
/dev/grid/node-x2-y2    9T    6T     3T   66%"
            });
            return testData;
        }

        public class Node
        {
            public Base.Vec2 Coords { get; set; }
            public uint Size { get; set; }
            public uint Used { get; set; }
            public uint Avail { get; set; }
            public string Name { get; set; }

            public Node()
            {
                Coords = new Base.Vec2();
                Size = 0;
                Used = 0;
                Avail = 0;
                Name = string.Empty;
            }

            public Node(Node other)
            {
                Coords = other.Coords;
                Size = other.Size;
                Used = other.Used;
                Avail = other.Avail;
                Name = other.Name;
            }

            public float UsePercentage()
            {
                if (Size == 0)
                {
                    return 0.0f;
                }
                return (float)Used / (float)Size * 100.0f;
            }

            public override string ToString()
            {
                string usedP = string.Format("{0, 4}", UsePercentage().ToString("F2"));
                return $"{Name} | Size={Size,3} | Avail={Avail,3} | Used={usedP}%";
            }

            public override bool Equals(object obj)
            {
                if (obj is Node)
                {
                    Node o = obj as Node;
                    return Name == o.Name;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            static public Node Parse(string input)
            {
                Node node = new Node();

                string[] split = input.Split(" T".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                node.Name = split[0].Substring(split[0].LastIndexOf('/') + 1);

                string[] coordSplit = split[0].Split("-xy".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).TakeLast(2).ToArray();
                node.Coords = new Base.Vec2(int.Parse(coordSplit[0]), int.Parse(coordSplit[1]));
                node.Size = uint.Parse(split[1]);
                node.Used = uint.Parse(split[2]);
                node.Avail = uint.Parse(split[3]);

                return node;
            }
        }

        private int GetViablePairs(Node curNode, IEnumerable<Node> nodes)
        {
            if (nodes.Count() == 0)
            {
                return 0;
            }

            int viablePairCount = 0;
            foreach (Node node in nodes)
            {
                if (node.Equals(curNode))
                {
                    continue;
                }

                if (curNode.Used != 0 && node.Avail >= curNode.Used)
                {
                    ++viablePairCount;
                }
            }
            return viablePairCount;
        }

        private record NodeStep(Node Node, uint Steps)
        {
            public NodeStep(NodeStep other)
            {
                Node = other.Node;
                Steps = other.Steps;
            }
        }

        private record NodeStepHistory(Node Node, uint Steps, Queue<Node> History) { }

        private void GetFullPathToTarget(Node[][] nodeGrid, int maxX, int maxY, Node start, Node target, out Queue<Node> optimizedPath)
        {
            optimizedPath = null;

            HashSet<string> visitedNodes = new HashSet<string>();
            Queue<NodeStepHistory> pathCheck = new Queue<NodeStepHistory>();
            pathCheck.Enqueue(new NodeStepHistory(start, 0, new Queue<Node>()));
            while (pathCheck.Count > 0)
            {
                NodeStepHistory nsh = pathCheck.Dequeue();
                if (nsh.Node.Equals(target))
                {
                    optimizedPath = nsh.History;
                    optimizedPath.Enqueue(nsh.Node);
                    pathCheck.Clear();
                    continue;
                }

                Base.Vec2[] nextCoords = new Base.Vec2[] { new Base.Vec2(0, -1), new Base.Vec2(-1, 0), new Base.Vec2(0, 1), new Base.Vec2(1, 0) };
                foreach (Base.Vec2 next in nextCoords)
                {
                    Base.Vec2 nextNodeCoords = nsh.Node.Coords + next;
                    if (nextNodeCoords.X >= 0 && nextNodeCoords.X <= maxX && nextNodeCoords.Y >= 0 && nextNodeCoords.Y <= maxY)
                    {
                        Node nextNode = nodeGrid[nextNodeCoords.Y][nextNodeCoords.X];
                        if (target.Used <= nextNode.Size && nextNode.Used <= nsh.Node.Size && !visitedNodes.Contains(nextNode.Name))
                        {
                            visitedNodes.Add(nextNode.Name);

                            NodeStepHistory nextNodeStepHistory = new NodeStepHistory(nextNode, nsh.Steps + 1, new Queue<Node>());
                            foreach (Node history in nsh.History)
                            {
                                nextNodeStepHistory.History.Enqueue(history);
                            }
                            nextNodeStepHistory.History.Enqueue(nsh.Node);
                            pathCheck.Enqueue(nextNodeStepHistory);
                        }
                    }
                }
            }

            // the first node is the starting spot, not where we want to path to
            if (optimizedPath.Count > 0)
            {
                optimizedPath.Dequeue();
            }
        }

        private uint PathToTarget(Node[][] nodeGrid, int maxX, int maxY, Node start, Node target, Node blocked)
        {
            HashSet<string> visitedNodes = new HashSet<string>();
            Queue<NodeStep> pathCheck = new Queue<NodeStep>();
            pathCheck.Enqueue(new NodeStep(start, 0));
            while (pathCheck.Count > 0)
            {
                NodeStep ns = pathCheck.Dequeue();
                if (ns.Node.Equals(target))
                {
                    return ns.Steps;
                }

                Base.Vec2[] nextCoords = new Base.Vec2[] { new Base.Vec2(0, -1), new Base.Vec2(-1, 0), new Base.Vec2(0, 1), new Base.Vec2(1, 0) };
                foreach (Base.Vec2 next in nextCoords)
                {
                    Base.Vec2 nextNodeCoords = ns.Node.Coords + next;
                    if (nextNodeCoords.X >= 0 && nextNodeCoords.X <= maxX && nextNodeCoords.Y >= 0 && nextNodeCoords.Y <= maxY)
                    {
                        Node nextNode = nodeGrid[nextNodeCoords.Y][nextNodeCoords.X];
                        if (nextNode.Used <= ns.Node.Size && !nextNode.Equals(blocked) && !visitedNodes.Contains(nextNode.Name))
                        {
                            visitedNodes.Add(nextNode.Name);

                            NodeStep nextNodeStep = new NodeStep(nextNode, ns.Steps + 1);
                            pathCheck.Enqueue(nextNodeStep);
                        }
                    }
                }
            }

            return (uint)int.MaxValue;
        }

        private void PrintGrid(Node[][] nodeGrid, Base.Vec2 start, Base.Vec2 goal, Base.Vec2 empty)
        {
            float size = 0;
            uint nodeCount = 0;
            foreach (Node[] col in nodeGrid)
            {
                foreach (Node row in col)
                {
                    size += (float)row.Size;
                    ++nodeCount;
                }
            }
            size = (size * 2.0f) / (float)nodeCount;

            Log(Core.Log.ELevel.Spam, "Grid Printout");
            uint colIdx = 1;
            foreach (Node[] cols in nodeGrid)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0,2}| ", colIdx++);
                foreach (Node row in cols)
                {
                    string node = " . ";
                    if (row.Coords.Equals(start))
                    {
                        if (row.Coords.Equals(goal))
                        {
                            node = "(G)";
                        }
                        else
                        {
                            node = "(.)";
                        }
                    }
                    else if (row.Coords.Equals(goal))
                    {
                        node = " G ";
                    }
                    else if (row.Coords.Equals(empty))
                    {
                        node = " _ ";
                    }
                    if ((float)row.Used > size)
                    {
                        node = " # ";
                    }
                    sb.Append(node);
                }
                Log(Core.Log.ELevel.Spam, sb.ToString());
            }
        }

        private string SolveGrid(IEnumerable<Node> nodes)
        {
            // set up the grid
            int maxX = nodes.Select(n => n.Coords.X).Max();
            int maxY = nodes.Select(n => n.Coords.Y).Max();
            Node[][] nodeGrid = new Node[maxY + 1][];
            for (int i = 0; i < (maxY + 1); ++i)
            {
                nodeGrid[i] = new Node[maxX + 1];
            }
            foreach (Node node in nodes)
            {
                nodeGrid[node.Coords.Y][node.Coords.X] = node;
            }

            Queue<Node> optimizedPath;
            GetFullPathToTarget(nodeGrid, maxX, maxY, nodeGrid[0][maxX], nodeGrid[0][0], out optimizedPath);

            uint optimizedSteps = 0;
            Node emptyNode = nodes.Where(n => n.Used == 0).First();
            Node goalNode = nodeGrid[0][maxX];

            //PrintGrid(nodeGrid, nodeGrid[0][0].Coords, goalNode.Coords, emptyNode.Coords);

            while (optimizedPath.Count > 0)
            {
                Node targetNode = optimizedPath.Dequeue();
                optimizedSteps += PathToTarget(nodeGrid, maxX, maxY, emptyNode, targetNode, goalNode);

                // swap empty with goal
                emptyNode = goalNode;
                goalNode = targetNode;
                ++optimizedSteps;

                //PrintGrid(nodeGrid, nodeGrid[0][0].Base.Pos2, goalNode.Base.Pos2, emptyNode.Base.Pos2);
            }
            return optimizedSteps.ToString();
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool accessData)
        {
            IEnumerable<Node> nodes = inputs.Skip(2).Select(Node.Parse);
            if (accessData)
            {
                return SolveGrid(nodes);
            }
            else
            {
                int viablePairCount = 0;
                foreach (Node node in nodes)
                {
                    viablePairCount += GetViablePairs(node, nodes);
                }
                return viablePairCount.ToString();
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}