using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2016
{
    class Day22 : Day
    {
        public Day22() { }

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
                Output = "",
                RawInput =
@""
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

        public class Node
        {
            public Node()
            {
                Coords = new Coords();
            }

            public Coords Coords { get; set; }
            public uint Size { get; set; }
            public uint Used { get; set; }
            public uint Avail { get; set; }
            public string Name { get; set; }

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
                node.Coords = new Coords(int.Parse(coordSplit[0]), int.Parse(coordSplit[1]));
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

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            IEnumerable<Node> nodes = inputs.Skip(2).Select(Node.Parse);
            int viablePairCount = 0;
            foreach (Node node in nodes)
            {
                viablePairCount += GetViablePairs(node, nodes);
            }
            return viablePairCount.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}