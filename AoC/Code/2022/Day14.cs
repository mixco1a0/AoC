using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day14 : Core.Day
    {
        public Day14() { }

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
                Output = "24",
                RawInput =
@"498,4 -> 498,6 -> 496,6
503,4 -> 502,4 -> 502,9 -> 494,9"
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

        static private readonly char AirVal = '.';
        static private readonly char RockVal = '#';
        static private readonly char SandVal = 'o';

        private Queue<Base.Position> Parse(string input)
        {
            int[] split = input.Split(", ->".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            Queue<Base.Position> positions = new Queue<Base.Position>();
            for (int i = 0; i < split.Length - 1; i += 2)
            {
                positions.Enqueue(new Base.Position(split[i], split[i + 1]));
            }
            return positions;
        }

        void UpdateMax(Base.Position pos, ref int minX, ref int maxX, ref int minY, ref int maxY)
        {
            minX = Math.Min(minX, pos.X);
            maxX = Math.Max(maxX, pos.X);
            minY = 0; //Math.Min(minY, pos.Y);
            maxY = Math.Max(maxY, pos.Y);
        }

        void AddRock(ref Dictionary<Base.Position, char> cave, int x, int y)
        {
            Base.Position rock = new Base.Position(x, y);
            if (!cave.ContainsKey(rock))
            {
                cave[rock] = RockVal;
            }
        }

        void GenerateCave(List<string> inputs, out Dictionary<Base.Position, char> cave, out int minX, out int maxX, out int minY, out int maxY)
        {
            cave = new Dictionary<Base.Position, char>();
            minX = int.MaxValue;
            maxX = int.MinValue;
            minY = int.MaxValue;
            maxY = int.MinValue;
            foreach (string input in inputs)
            {
                Queue<Base.Position> rocks = Parse(input);
                Base.Position curRock = rocks.Dequeue();
                cave[curRock] = RockVal;
                UpdateMax(curRock, ref minX, ref maxX, ref minY, ref maxY);
                Base.Position nextRock = null;
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

        void PrintCave(Dictionary<Base.Position, char> cave, int minX, int maxX, int minY, int maxY)
        {
            StringBuilder sb = new StringBuilder();
            for (int y = minY; y <= maxY + 1; ++y)
            {
                sb.Clear();
                sb.Append($"{y,4}| ");
                for (int x = minX - 1; x <= maxX + 1; ++x)
                {
                    Base.Position cur = new Base.Position(x, y);
                    if (cave.ContainsKey(cur))
                    {
                        sb.Append(cave[cur]);
                    }
                    else
                    {
                        sb.Append(AirVal);
                    }
                }
                Core.Log.WriteLine(Core.Log.ELevel.Debug, sb.ToString());
            }
        }

        static readonly Base.Position[] Movement = new Base.Position[] { new Base.Position(0, 1), new Base.Position(-1, 1), new Base.Position(1, 1) };
        private bool DropSand(ref Dictionary<Base.Position, char> cave, int maxY, out Base.Position sand)
        {
            sand = new Base.Position(500, 0);
            while (true)
            {
                // infinite free fall
                if (sand.Y > maxY)
                {
                    return false;
                }

                // try to move it
                bool moved = false;
                foreach (Base.Position move in Movement)
                {
                    Base.Position newSand = sand + move;
                    if (cave.ContainsKey(newSand))
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

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            GenerateCave(inputs, out Dictionary<Base.Position, char> cave, out int minX, out int maxX, out int minY, out int maxY);
            PrintCave(cave, minX, maxX, minY, maxY);
            int sandCount = 0;
            while (DropSand(ref cave, maxY, out Base.Position sand))
            {
                ++sandCount;
                cave[sand] = SandVal;
            }
            //PrintCave(cave, minX, maxX, minY, maxY);
            return sandCount.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}