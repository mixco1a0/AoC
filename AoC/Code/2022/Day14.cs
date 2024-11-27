using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2022
{
    class Day14 : Core.Day
    {
        public Day14() { }

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
                Output = "24",
                RawInput =
@"498,4 -> 498,6 -> 496,6
503,4 -> 502,4 -> 502,9 -> 494,9"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "93",
                RawInput =
@"498,4 -> 498,6 -> 496,6
503,4 -> 502,4 -> 502,9 -> 494,9"
            });
            return testData;
        }

        static private readonly char AirVal = '.';
        static private readonly char RockVal = '#';
        static private readonly char SandVal = 'o';

        private Queue<Base.Vec2> Parse(string input)
        {
            int[] split = input.Split(", ->".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            Queue<Base.Vec2> positions = new Queue<Base.Vec2>();
            for (int i = 0; i < split.Length - 1; i += 2)
            {
                positions.Enqueue(new Base.Vec2(split[i], split[i + 1]));
            }
            return positions;
        }

        void UpdateMax(Base.Vec2 pos, ref int minX, ref int maxX, ref int minY, ref int maxY)
        {
            minX = Math.Min(minX, pos.X);
            maxX = Math.Max(maxX, pos.X);
            minY = 0; //Math.Min(minY, pos.Y);
            maxY = Math.Max(maxY, pos.Y);
        }

        void AddRock(ref Dictionary<Base.Vec2, char> cave, int x, int y)
        {
            Base.Vec2 rock = new Base.Vec2(x, y);
            if (!cave.ContainsKey(rock))
            {
                cave[rock] = RockVal;
            }
        }

        void GenerateCave(List<string> inputs, out Dictionary<Base.Vec2, char> cave, out int minX, out int maxX, out int minY, out int maxY)
        {
            cave = new Dictionary<Base.Vec2, char>();
            minX = int.MaxValue;
            maxX = int.MinValue;
            minY = int.MaxValue;
            maxY = int.MinValue;
            foreach (string input in inputs)
            {
                Queue<Base.Vec2> rocks = Parse(input);
                Base.Vec2 curRock = rocks.Dequeue();
                cave[curRock] = RockVal;
                UpdateMax(curRock, ref minX, ref maxX, ref minY, ref maxY);
                Base.Vec2 nextRock = null;
                while (rocks.Count > 0)
                {
                    nextRock = rocks.Dequeue();
                    UpdateMax(nextRock, ref minX, ref maxX, ref minY, ref maxY);
                    if (curRock.X == nextRock.X)
                    {
                        if (curRock.Y > nextRock.Y)
                        {
                            for (int y = nextRock.Y; y < curRock.Y; ++y)
                            {
                                AddRock(ref cave, curRock.X, y);
                            }
                        }
                        else
                        {
                            for (int y = nextRock.Y; y > curRock.Y; --y)
                            {
                                AddRock(ref cave, curRock.X, y);
                            }
                        }
                    }
                    else
                    {
                        if (curRock.X > nextRock.X)
                        {
                            for (int x = nextRock.X; x < curRock.X; ++x)
                            {
                                AddRock(ref cave, x, curRock.Y);
                            }
                        }
                        else
                        {
                            for (int x = nextRock.X; x > curRock.X; --x)
                            {
                                AddRock(ref cave, x, curRock.Y);
                            }
                        }
                    }
                    curRock = nextRock;
                }
            }
        }

        void PrintCave(Dictionary<Base.Vec2, char> cave, int minX, int maxX, int minY, int maxY, bool endlessVoid)
        {
            StringBuilder sb = new StringBuilder();
            Core.Log.WriteLine(Core.Log.ELevel.Debug, "");
            for (int y = minY; y <= maxY + 1; ++y)
            {
                sb.Clear();
                sb.Append($"{y,4}| ");
                for (int x = minX - 1; x <= maxX + 1; ++x)
                {
                    Base.Vec2 cur = new Base.Vec2(x, y);
                    if (cave.ContainsKey(cur))
                    {
                        sb.Append(cave[cur]);
                    }
                    else if (endlessVoid)
                    {
                        sb.Append(AirVal);
                    }
                    else
                    {
                        if (y == maxY)
                        {
                            if (x < minX)
                            {
                                sb.Append('←');
                            }
                            else if (x > maxX)
                            {
                                sb.Append('→');
                            }
                            else
                            {
                                sb.Append(RockVal);
                            }
                        }
                        else
                        {
                            sb.Append(AirVal);
                        }
                    }
                }
                Core.Log.WriteLine(Core.Log.ELevel.Debug, sb.ToString());
            }
            Core.Log.WriteLine(Core.Log.ELevel.Debug, "");
        }

        static readonly Base.Vec2[] Movement = new Base.Vec2[] { new Base.Vec2(0, 1), new Base.Vec2(-1, 1), new Base.Vec2(1, 1) };
        private bool DropSand(ref Dictionary<Base.Vec2, char> cave, int maxY, bool endlessVoid, out Base.Vec2 sand)
        {
            sand = new Base.Vec2(500, 0);
            while (true)
            {
                // infinite free fall
                if (endlessVoid && sand.Y > maxY)
                {
                    return false;
                }

                // try to move it
                bool moved = false;
                foreach (Base.Vec2 move in Movement)
                {
                    Base.Vec2 newSand = sand + move;
                    if (cave.ContainsKey(newSand))
                    {
                        continue;
                    }

                    // hit the floor
                    if (!endlessVoid && newSand.Y == maxY)
                    {
                        continue;
                    }

                    moved = true;
                    sand = newSand;
                    break;
                }

                if (!moved)
                {
                    return true;
                }
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool endlessVoid)
        {
            GenerateCave(inputs, out Dictionary<Base.Vec2, char> cave, out int minX, out int maxX, out int minY, out int maxY);
            if (!endlessVoid)
            {
                maxY += 2;
            }

            int sandCount = 0;
            while (DropSand(ref cave, maxY, endlessVoid, out Base.Vec2 sand))
            {
                ++sandCount;
                cave[sand] = SandVal;
                UpdateMax(sand, ref minX, ref maxX, ref minY, ref maxY);

                if (!endlessVoid && sand.X == 500 && sand.Y == 0)
                {
                    break;
                }
            }
            return sandCount.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}