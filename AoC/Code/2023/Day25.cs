using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AoC._2023
{
    class Day25 : Core.Day
    {
        public Day25() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v1";
                case Core.Part.Two:
                    return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
//             testData.Add(new Core.TestDatum
//             {
//                 TestPart = Core.Part.One,
//                 Output = "54",
//                 RawInput =
// @"jqt: rhn xhk nvd
// rsh: frs pzl lsr
// xhk: hfx
// cmg: qnr nvd lhk bvb
// rhn: xhk bvb hfx
// bvb: xhk hfx
// pzl: lsr hfx nvd
// qnr: nvd
// ntq: jqt hfx bvb xhk
// nvd: lhk
// lsr: lhk
// rzs: qnr cmg lsr rsh
// frs: qnr lhk lsr"
//             });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private Dictionary<string, HashSet<string>> Parse(List<string> inputs)
        {
            Dictionary<string, HashSet<string>> components = new Dictionary<string, HashSet<string>>();

            foreach (string input in inputs)
            {
                string[] split = Util.String.Split(input, ": ");
                if (!components.ContainsKey(split[0]))
                {
                    components[split[0]] = new HashSet<string>();
                }
                foreach (string s in split[1..])
                {
                    components[split[0]].Add(s);
                    if (!components.ContainsKey(s))
                    {
                        components[s] = new HashSet<string>();
                    }
                    components[s].Add(split[0]);
                }
            }

            return components;
        }

        private record Node(int Id, int Size) : IComparable<Node>
        {
            public static Node New(string id, int size)
            {
                return new Node(id.GetHashCode(), size);
            }

            public int CompareTo(Node other)
            {
                return Id.CompareTo(other.Id);
            }
        }

        private record Edge(Node First, Node Last, int Weight)
        {
            public static Edge New(Node first, Node last, int weight)
            {
                if (first.CompareTo(last) <= 0)
                {
                    return new Edge(first, last, weight);
                }
                else
                {
                    return new Edge(last, first, weight);
                }
            }

            public Node Other(Node node)
            {
                if (First.Equals(node))
                {
                    return Last;
                }
                else if (Last.Equals(node))
                {
                    return First;
                }
                return null;
            }

            public bool Has(Node node)
            {
                return Other(node) != null;
            }

            public bool Shared(Edge edge)
            {
                Node otherF = Other(edge.First);
                Node otherL = Other(edge.Last);
                return otherF != null || otherL != null;
            }

            public Node ConvertToNode()
            {
                int newHash = HashCode.Combine(First.Id, Last.Id);
                return new Node(newHash, First.Size + Last.Size);
            }
        }

        private class Graph
        {
            public HashSet<Node> Nodes { get; set; }
            public List<Edge> Edges { get; set; }
            // public Dictionary<int, string> DebugIdToName { get; set; }

            public Graph(Dictionary<string, HashSet<string>> components)
            {
                Nodes = components.Keys.Select(k => Node.New(k, 1)).ToHashSet();
                // DebugIdToName = components.Keys.ToDictionary(k => k.GetHashCode(), k => k);
                Dictionary<int, Node> allNodes = Nodes.ToDictionary(n => n.Id, _ => _);
                List<Edge> edges = new();
                foreach (var pair in components)
                {
                    Node first = allNodes[pair.Key.GetHashCode()];
                    foreach (string val in pair.Value)
                    {
                        Node last = allNodes[val.GetHashCode()];
                        edges.Add(Edge.New(first, last, 1));
                    }
                }
                Edges = edges.Distinct().ToList();
            }

            // public void PrintState(Action<string> logFunc)
            // {
            //     logFunc($"Nodes [{Nodes.Count}]:");
            //     int index = 0;
            //     foreach (Node n in Nodes)
            //     {
            //         string node = DebugIdToName[n.Id];
            //         if (node.Length > 6)
            //         {
            //             node = string.Format("{0}...{1}", node[..3], node[^3..]);
            //         }
            //         logFunc($"[{index++,3}] {node} [{n.Size}]");
            //     }

            //     logFunc($"Edges [{Edges.Count}]:");
            //     index = 0;
            //     foreach (Edge e in Edges)
            //     {
            //         string first = DebugIdToName[e.First.Id];
            //         if (first.Length > 6)
            //         {
            //             first = string.Format("{0}...{1}", first[..3], first[^3..]);
            //         }
            //         string last = DebugIdToName[e.Last.Id];
            //         if (last.Length > 6)
            //         {
            //             last = string.Format("{0}...{1}", last[..3], last[^3..]);
            //         }
            //         logFunc($"[{index++,3}] {first} -> {last} [{e.Weight}]");
            //     }
            // }

            public void RemoveEdge(/*Action<string> logFunc*/)
            {
                int index = 0;
                Edge toRemove = Edges[index];
                Edges.RemoveAt(index);

                // if (logFunc != null)
                // {
                //     logFunc(".");
                //     logFunc("..");
                //     logFunc($"Removing ({DebugIdToName[toRemove.First.Id]}) and ({DebugIdToName[toRemove.Last.Id]})");
                // }

                List<Edge> toUpdate = Edges.Where(e => e.Shared(toRemove)).ToList();
                Edges.RemoveAll(e => toUpdate.Contains(e));
                Nodes.Remove(toRemove.First);
                Nodes.Remove(toRemove.Last);

                List<Edge> updatedEdges = new List<Edge>();
                Node newNode = toRemove.ConvertToNode();
                Nodes.Add(newNode);
                // StringBuilder sb = new StringBuilder();
                // sb.Append(DebugIdToName[toRemove.First.Id]);
                // sb.Append(DebugIdToName[toRemove.Last.Id]);
                // DebugIdToName[newNode.Id] = sb.ToString();

                // if (logFunc != null)
                // {
                //     logFunc($"Adding ({DebugIdToName[newNode.Id]})");
                // }

                foreach (Edge e in toUpdate)
                {
                    if (e.Equals(toRemove))
                    {
                        continue;
                    }

                    Node sameNode;
                    if (toRemove.Has(e.First))
                    {
                        sameNode = e.Last;
                    }
                    else
                    {
                        sameNode = e.First;
                    }
                    Edge newE = Edge.New(sameNode, newNode, 1);
                    Edges.Add(newE);
                }

                // if (logFunc != null)
                // {
                //     PrintState(logFunc);
                // }
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            Dictionary<string, HashSet<string>> components = Parse(inputs);
            Graph graph = new Graph(components);
            // graph.PrintState(Log);

            while (graph.Nodes.Count > 2)
            {
                graph.RemoveEdge();
                // graph.RemoveEdge(null);
            }

            Node first = graph.Nodes.First();
            Node last = graph.Nodes.Last();

            // Log($"{first.Size} x {last.Size}");
            // graph.PrintState(Log);

            return (first.Size * last.Size).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        { SharedSolution(inputs, variables); return "50"; }
    }
}