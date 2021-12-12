using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day12 : Day
    {
        public Day12() { }

        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                // case Part.One:
                //     return "v1";
                // case Part.Two:
                //     return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "10",
                RawInput =
@"start-A
start-b
A-c
A-b
b-d
A-end
b-end"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "19",
                RawInput =
@"dc-end
HN-start
start-kj
dc-start
dc-HN
LN-dc
HN-end
kj-sa
kj-HN
kj-dc"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "226",
                RawInput =
@"fs-end
he-DX
fs-he
start-DX
pj-DX
end-zg
zg-sl
zg-pj
pj-he
RW-he
fs-DX
pj-RW
zg-RW
start-pj
he-WI
zg-he
pj-fs
start-RW"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private const string Start = "[START]";
        private const string End = "[END]";

        private class Node : Core.Pair<string, string>
        {
            public bool IsStart { get; set; }
            public bool IsEnd { get; set; }

            public Node() : base() { }
            public Node(string first, string last) : base(first, last) { }
            public Node(Node other) : base(other) { }

            public static Node Parse(string input)
            {
                string[] split = input.Split('-');
                Node node = new Node(split[0], split[1]);
                if (split[0] == "start")
                {
                    node.First = Start;
                    node.IsStart = true;
                }
                else if (split[1] == "start")
                {
                    node.Last = Start;
                    node.IsStart = true;
                }

                if (split[1] == "end")
                {
                    node.Last = End;
                    node.IsEnd = true;
                }
                else if (split[0] == "end")
                {
                    node.First = End;
                    node.IsEnd = true;
                }
                return node;
            }
        }

        private class Path
        {
            public HashSet<string> SmallCaves { get; set; }
            public List<string> Nodes { get; set; }
            public string Last { get => Nodes.Last(); }

            public Path(Node node)
            {
                SmallCaves = new HashSet<string>();

                Nodes = new List<string>();
                bool firstIsStart = node.First == Start;
                Nodes.Add(firstIsStart ? node.First : node.Last);
                Visit(node, !firstIsStart);
            }

            public Path(Path other)
            {
                SmallCaves = new HashSet<string>(other.SmallCaves);
                Nodes = new List<string>(other.Nodes);
            }

            public void Print(Action<string> printFunc)
            {
                printFunc(ToString());
            }

            public bool Visit(Node node, bool visitFirst)
            {
                string toVisit = (visitFirst ? node.First : node.Last);
                if (char.IsLower(toVisit[0]))
                {
                    if (SmallCaves.Contains(toVisit))
                    {
                        return false;
                    }
                    SmallCaves.Add(toVisit);
                }
                Nodes.Add(toVisit);
                return true;
            }

            public override string ToString()
            {
                return $"Path => {string.Join(",", Nodes)}";
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            Node[] nodes = inputs.Select(Node.Parse).ToArray();

            Queue<Path> paths = new Queue<Path>();
            foreach (Node node in nodes.Where(n => n.IsStart))
            {
                Path start = new Path(node);
                paths.Enqueue(start);
            }

            List<Path> completedPaths = new List<Path>();
            while (paths.Count > 0)
            {
                Path cur = paths.Dequeue();
                if (cur.Last == End)
                {
                    completedPaths.Add(cur);
                    //cur.Print(DebugWriteLine);
                    continue;
                }
                foreach (Node node in nodes.Where(n => !n.IsStart && n.First == cur.Last))
                {
                    Path next = new Path(cur);
                    if (next.Visit(node, false))
                    {
                        paths.Enqueue(next);
                    }
                }
                foreach (Node node in nodes.Where(n => !n.IsStart && n.Last == cur.Last))
                {
                    Path next = new Path(cur);
                    if (next.Visit(node, true))
                    {
                        paths.Enqueue(next);
                    }
                }
            }
            return completedPaths.Count().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}