using System;
using System.Collections.Generic;
using System.Text;

namespace AoC._2022
{
    class Day12 : Core.Day
    {
        public Day12() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v3";
                case Core.Part.Two:
                    return "v3";
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
                Output = "31",
                RawInput =
@"Sabqponm
abcryxxl
accszExk
acctuvwj
abdefghi"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "29",
                RawInput =
@"Sabqponm
abcryxxl
accszExk
acctuvwj
abdefghi"
            });
            return testData;
        }

        private class Node : Util.AStarNode
        {
            public long Height { get; set; }

            public Node(long height) : base()
            {
                Height = height;
            }

            public override string ToString()
            {
                return Processed ? $"{Length,4}" : $"?{Height,2}?";
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool reverse)
        {
            Util.AStar<Node> aStar = new Util.AStar<Node>(inputs[0].Length, inputs.Count);
            Util.AStar<Node>.InitializeNode initializeNode = (int x, int y) =>
            {
                switch (inputs[y][x])
                {
                    case 'S':
                        return new Node(0);
                    case 'E':
                        return new Node('z' - 'a');
                    default:
                        return new Node(inputs[y][x] - 'a');
                }
            };

            if (reverse)
            {
                Util.AStar<Node>.IsNode isStartNode = (int x, int y) => { return inputs[y][x] == 'E'; };
                aStar.Initialize(inputs, initializeNode, isStartNode, null);

                Util.AStar<Node>.CanUseNode canUsedNode = (Node curNode, Node nextNode) =>
                {
                    return curNode.Height - 1 <= nextNode.Height;
                };
                Util.AStar<Node>.IsEnd isEnd = (Base.Vec2 pos) => { return aStar.Nodes[pos.X, pos.Y].Height == 0; };
                aStar.Process(canUsedNode, isEnd);
            }
            else
            {
                Util.AStar<Node>.IsNode isStartNode = (int x, int y) => { return inputs[y][x] == 'S'; };
                Util.AStar<Node>.IsNode isEndNode = (int x, int y) => { return inputs[y][x] == 'E'; };
                aStar.Initialize(inputs, initializeNode, isStartNode, isEndNode);

                Util.AStar<Node>.CanUseNode canUsedNode = (Node curNode, Node nextNode) =>
                {
                    return curNode.Height + 1 >= nextNode.Height;
                };
                aStar.Process(canUsedNode);
            }

            return aStar.GetOptimalPath();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}