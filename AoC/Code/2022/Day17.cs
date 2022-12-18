using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day17 : Core.Day
    {
        public Day17() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                // case Core.Part.One:
                //     return "v1";
                // case Core.Part.Two:
                //     return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        public override bool SkipTestData => false;

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "3068",
                RawInput =
@">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        static readonly List<Base.Position>[] RockShapes =
        {
            new List<Base.Position>() { new Base.Position(0, 0), new Base.Position(1, 0), new Base.Position(2, 0), new Base.Position(3, 0) },
            new List<Base.Position>() { new Base.Position(1, 0), new Base.Position(0, 1), new Base.Position(1, 1), new Base.Position(2, 1), new Base.Position(1, 2) },
            new List<Base.Position>() { new Base.Position(2, 0), new Base.Position(2, 1), new Base.Position(2, 2), new Base.Position(0, 0), new Base.Position(1, 0) },
            new List<Base.Position>() { new Base.Position(0, 0), new Base.Position(0, 1), new Base.Position(0, 2), new Base.Position(0, 3) },
            new List<Base.Position>() { new Base.Position(0, 0), new Base.Position(1, 0), new Base.Position(0, 1), new Base.Position(1, 1) },
        };

        private void PrintRocks(HashSet<Base.Position> usedPoints, List<Base.Position> rock, int highestY, int minX, int maxX)
        {
            StringBuilder sb = new StringBuilder();
            for (int y = highestY; y > 0; --y)
            {
                sb.Clear();
                sb.Append(string.Format("{0, 3} - ", y));
                for (int x = minX - 1; x <= maxX; ++x)
                {
                    Base.Position pos = new Base.Position(x, y);
                    if (x < minX || x >= maxX)
                    {
                        sb.Append('|');
                    }
                    else if (rock.Contains(pos))
                    {
                        sb.Append('@');
                    }
                    else if (usedPoints.Contains(pos))
                    {
                        sb.Append('#');
                    }
                    else
                    {
                        sb.Append('.');
                    }
                }
                DebugWriteLine(sb.ToString());
            }
            DebugWriteLine("  0 - +-------+");
        }

        private bool CanMove(HashSet<Base.Position> usedPoints, ref List<Base.Position> rock, Base.Position movement, int minX, int maxX)
        {
            List<Base.Position> movedRock = new List<Base.Position>();
            for (int i = 0; i < rock.Count; ++i)
            {
                movedRock.Add(rock[i] + movement);

                Base.Position movedRockNode = movedRock.Last();
                if (movedRockNode.X < minX || movedRockNode.X >= maxX || movedRockNode.Y <= 0 || usedPoints.Contains(movedRockNode))
                {
                    return false;
                }
            }

            rock = movedRock;
            return true;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            int rockIdx = 0;
            char[] jets = inputs[0].ToCharArray();
            HashSet<Base.Position> usedPoints = new HashSet<Base.Position>();
            int highestY = 0;
            const int minX = 0;
            const int maxX = 7;
            bool newRock = true;
            int rockCount = 0;
            List<Base.Position> newRockPos = new List<Base.Position>();
            for (int i = 0; i < jets.Length; i = (i + 1) % jets.Length)
            {
                if (newRock)
                {
                    ++rockCount;
                    if (rockCount == 2023)
                    {
                        return highestY.ToString();
                    }
                    newRock = false;
                    newRockPos = new List<Base.Position>();
                    int newHighestY = highestY;
                    foreach (Base.Position node in RockShapes[rockIdx])
                    {
                        newRockPos.Add(new Base.Position(node.X + 2, node.Y + highestY + 4));
                        newHighestY = Math.Max(newHighestY, newRockPos.Last().Y);
                    }
                    rockIdx = (rockIdx + 1) % RockShapes.Length;
                    // DebugWriteLine($"New rock");
                    // PrintRocks(usedPoints, newRockPos, newHighestY, minX, maxX);
                }

                bool moved = false;
                if (jets[i] == '>')
                {
                    moved = CanMove(usedPoints, ref newRockPos, new Base.Position(1, 0), minX, maxX);
                }
                else
                {
                    moved = CanMove(usedPoints, ref newRockPos, new Base.Position(-1, 0), minX, maxX);
                }
                // if (moved)
                // {
                //     DebugWriteLine($"Jet moved [{jets[i]}]");
                //     PrintRocks(usedPoints, newRockPos, highestY + 4, minX, maxX);
                // }
                // else
                // {
                //     DebugWriteLine($"Jet still [{jets[i]}]");
                // }

                moved = CanMove(usedPoints, ref newRockPos, new Base.Position(0, -1), minX, maxX);
                if (!moved)
                {
                    newRockPos.ForEach(r => usedPoints.Add(r));
                    newRock = true;
                    foreach (var r in newRockPos)
                    {
                        if (r.Y > highestY)
                        {
                            highestY = r.Y;
                        }
                    }

                    // DebugWriteLine($"Rock added");
                    // PrintRocks(usedPoints, newRockPos, highestY + 4, minX, maxX);
                    // DebugWriteLine(".");
                    // DebugWriteLine("..");
                }
                // else
                // {
                //     DebugWriteLine($"Rock fell");
                //     PrintRocks(usedPoints, newRockPos, highestY + 4, minX, maxX);
                //     DebugWriteLine(".");
                //     DebugWriteLine("..");
                // }
            }
            return string.Empty;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}