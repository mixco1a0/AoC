using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2023
{
    class Day16 : Core.Day
    {
        public Day16() { }

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
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "46",
                RawInput =
@".|...\....
|.-.\.....
.....|-...
........|.
..........
.........\
..../.\\..
.-.-/..|..
.|....-|.\
..//.|...."
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "51",
                RawInput =
@".|...\....
|.-.\.....
.....|-...
........|.
..........
.........\
..../.\\..
.-.-/..|..
.|....-|.\
..//.|...."
            });
            return testData;
        }

        private enum Node { Empty = '.', Pos45 = '/', Neg45 = '\\', VSplit = '|', HSplit = '-' }

        private enum Dir { North, South, East, West }
        private record ReDirect(Dir Dir, Base.Vec2 Re);

        private static Dictionary<Node, Dictionary<Dir, List<ReDirect>>> ReDirects = new Dictionary<Node, Dictionary<Dir, List<ReDirect>>>()
        {
            { Node.Empty, new Dictionary<Dir, List<ReDirect>>()
            {
                { Dir.North, new List<ReDirect>()
                    {
                        new ReDirect(Dir.North, new Base.Vec2(0, -1))
                    }
                },
                { Dir.South, new List<ReDirect>()
                    {
                        new ReDirect(Dir.South, new Base.Vec2(0, 1))
                    }
                },
                { Dir.East, new List<ReDirect>()
                    {
                        new ReDirect(Dir.East, new Base.Vec2(1, 0))
                    }
                },
                { Dir.West, new List<ReDirect>()
                    {
                        new ReDirect(Dir.West, new Base.Vec2(-1, 0))
                    }
                },
            }},
            { Node.Pos45, new Dictionary<Dir, List<ReDirect>>()
            {
                { Dir.North, new List<ReDirect>()
                    {
                        new ReDirect(Dir.East, new Base.Vec2(1, 0))
                    }
                },
                { Dir.South, new List<ReDirect>()
                    {
                        new ReDirect(Dir.West, new Base.Vec2(-1, 0))
                    }
                },
                { Dir.East, new List<ReDirect>()
                    {
                        new ReDirect(Dir.North, new Base.Vec2(0, -1))
                    }
                },
                { Dir.West, new List<ReDirect>()
                    {
                        new ReDirect(Dir.South, new Base.Vec2(0, 1))
                    }
                },
            }},
            { Node.Neg45, new Dictionary<Dir, List<ReDirect>>()
            {
                { Dir.North, new List<ReDirect>()
                    {
                        new ReDirect(Dir.West, new Base.Vec2(-1, 0))
                    }
                },
                { Dir.South, new List<ReDirect>()
                    {
                        new ReDirect(Dir.East, new Base.Vec2(1, 0))
                    }
                },
                { Dir.East, new List<ReDirect>()
                    {
                        new ReDirect(Dir.South, new Base.Vec2(0, 1))
                    }
                },
                { Dir.West, new List<ReDirect>()
                    {
                        new ReDirect(Dir.North, new Base.Vec2(0, -1))
                    }
                },
            }},
            { Node.VSplit, new Dictionary<Dir, List<ReDirect>>()
            {
                { Dir.North, new List<ReDirect>()
                    {
                        new ReDirect(Dir.North, new Base.Vec2(0, -1))
                    }
                },
                { Dir.South, new List<ReDirect>()
                    {
                        new ReDirect(Dir.South, new Base.Vec2(0, 1))
                    }
                },
                { Dir.East, new List<ReDirect>()
                    {
                        new ReDirect(Dir.North, new Base.Vec2(0, -1)),
                        new ReDirect(Dir.South, new Base.Vec2(0, 1))
                    }
                },
                { Dir.West, new List<ReDirect>()
                    {
                        new ReDirect(Dir.North, new Base.Vec2(0, -1)),
                        new ReDirect(Dir.South, new Base.Vec2(0, 1))
                    }
                },
            }},
            { Node.HSplit, new Dictionary<Dir, List<ReDirect>>()
            {
                { Dir.North, new List<ReDirect>()
                    {
                        new ReDirect(Dir.East, new Base.Vec2(1, 0)),
                        new ReDirect(Dir.West, new Base.Vec2(-1, 0))
                    }
                },
                { Dir.South, new List<ReDirect>()
                    {
                        new ReDirect(Dir.East, new Base.Vec2(1, 0)),
                        new ReDirect(Dir.West, new Base.Vec2(-1, 0))
                    }
                },
                { Dir.East, new List<ReDirect>()
                    {
                        new ReDirect(Dir.East, new Base.Vec2(1, 0))
                    }
                },
                { Dir.West, new List<ReDirect>()
                    {
                        new ReDirect(Dir.West, new Base.Vec2(-1, 0))
                    }
                },
            }},
        };

        private record Target(Base.Vec2 Pos, Dir Direction);
        // {
        //     public override int GetHashCode()
        //     {
        //         return HashCode.Combine(Pos, Direction);
        //     }
        // }

        private Node GetNode(char node)
        {
            switch (node)
            {
                case '/':
                    return Node.Pos45;
                case '\\':
                    return Node.Neg45;
                case '-':
                    return Node.HSplit;
                case '|':
                    return Node.VSplit;
                case '.':
                default:
                    return Node.Empty;
            }
        }

        private void ParseInput(List<string> inputs, out Node[,] nodes)
        {
            nodes = new Node[inputs[0].Length, inputs.Count()];
            for (int x = 0; x < inputs[0].Length; ++x)
            {
                for (int y = 0; y < inputs.Count; ++y)
                {
                    nodes[x, y] = GetNode(inputs[y][x]);
                }
            }
        }

        private void PrintGrid(Node[,] nodes)
        {
            StringBuilder sb = new StringBuilder();
            Core.Log.WriteLine(Core.Log.ELevel.Debug, $"Printing grid {nodes.GetLength(0)}x{nodes.GetLength(1)}:");
            for (int y = 0; y < nodes.GetLength(1); ++y)
            {
                sb.Clear();
                sb.Append($"{y,4}| ");
                sb.Append(string.Join(string.Empty, Enumerable.Range(0, nodes.GetLength(0)).Select(x => (char)nodes[x, y])));
                Core.Log.WriteLine(Core.Log.ELevel.Debug, sb.ToString());
            }
        }

        private int GetEnergizedCount(Node[,] nodes, Base.Vec2 startPos, Dir startDir)
        {
            int xMax = nodes.GetLength(0);
            int yMax = nodes.GetLength(1);

            HashSet<Target> visited = new HashSet<Target>();
            Stack<Target> targets = new Stack<Target>();
            targets.Push(new Target(startPos, startDir));
            while (targets.Count > 0)
            {
                Target target = targets.Pop();
                if (visited.Contains(target))
                {
                    continue;
                }

                visited.Add(target);

                Node node = nodes[target.Pos.X, target.Pos.Y];
                foreach (ReDirect reDirect in ReDirects[node][target.Direction])
                {
                    Base.Vec2 targetPos = target.Pos + reDirect.Re;
                    if (targetPos.X >= 0 && targetPos.X < xMax && targetPos.Y >= 0 && targetPos.Y < yMax)
                    {
                        targets.Push(new Target(targetPos, reDirect.Dir));
                    }
                }
            }
            return visited.Select(t => t.Pos).Distinct().Count();
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool checkAllEdges)
        {
            ParseInput(inputs, out Node[,] nodes);
            if (checkAllEdges)
            {
                int maxEnergy = 0;
                for (int x = 0; x < nodes.GetLength(0); ++x)
                {
                    int topEnergy = GetEnergizedCount(nodes, new Base.Vec2(x, 0), Dir.South);
                    maxEnergy = Math.Max(maxEnergy, topEnergy);
                    int bottomEnergy = GetEnergizedCount(nodes, new Base.Vec2(x, nodes.GetLength(1) - 1), Dir.North);
                    maxEnergy = Math.Max(maxEnergy, bottomEnergy);
                }
                for (int y = 0; y < nodes.GetLength(1); ++y)
                {
                    int leftEnergy = GetEnergizedCount(nodes, new Base.Vec2(0, y), Dir.East);
                    maxEnergy = Math.Max(maxEnergy, leftEnergy);
                    int rightEnergy = GetEnergizedCount(nodes, new Base.Vec2(nodes.GetLength(0) - 1, y), Dir.West);
                    maxEnergy = Math.Max(maxEnergy, rightEnergy);
                }
                return maxEnergy.ToString();
            }
            else
            {
                return GetEnergizedCount(nodes, new Base.Vec2(), Dir.East).ToString();
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}