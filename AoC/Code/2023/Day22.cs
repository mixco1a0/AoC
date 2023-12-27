using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2023
{
    class Day22 : Core.Day
    {
        public Day22() { }

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
                Output = "5",
                RawInput =
@"1,0,1~1,2,1
0,0,2~2,0,2
0,2,3~2,2,3
0,0,4~0,2,4
2,0,5~2,2,5
0,1,6~2,1,6
1,1,8~1,1,9"
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

        private enum Axis
        {
            None,
            X,
            Y,
            Z
        }

        private record Brick(int Id, Base.Pos3 Start, Base.Pos3 End, Axis Axis)
        {
            private static int s_id = 0;
            public static Brick Parse(string input)
            {
                int[] split = Util.Number.Split(input, ",~").ToArray();
                Axis axis = Axis.None;
                if (split[0] != split[3])
                {
                    axis = Axis.X;
                }
                else if (split[1] != split[4])
                {
                    axis = Axis.Y;
                }
                else if (split[2] != split[5])
                {
                    axis = Axis.Z;
                }
                return new Brick(++s_id, new Base.Pos3(split[0], split[1], split[2]), new Base.Pos3(split[3], split[4], split[5]), axis);
            }

            public override string ToString()
            {
                return $"[{Id,4} | {Axis}] {Start} -> {End}";
            }
        }

        private class BrickSpace
        {
            public int Owner { get; set; }
            public Base.Pos3 Space { get; set; }

            public BrickSpace(int owner, Base.Pos3 space)
            {
                Owner = owner;
                Space = new Base.Pos3(space);
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<Brick> bricks = inputs.Select(Brick.Parse).ToList();
            bricks.Sort((a, b) => { return a.Start.Z.CompareTo(b.Start.Z); });
            Dictionary<int, List<BrickSpace>> settled = new Dictionary<int, List<BrickSpace>>();
            Dictionary<int, HashSet<int>> supports = new Dictionary<int, HashSet<int>>();

            Action<Brick, IEnumerable<BrickSpace>> AddSupport = (brick, potential) =>
            {
                foreach (BrickSpace bs in potential)
                {
                    if (!supports.ContainsKey(bs.Owner))
                    {
                        supports[bs.Owner] = new HashSet<int>();
                    }
                    supports[bs.Owner].Add(brick.Id);
                    Log($"Brick {bs.Owner} is supporting {brick.Id}");
                }
            };

            foreach (Brick brick in bricks)
            {
                int actualZ = brick.Start.Z;
                bool canFall = actualZ > 1;
                while (canFall)
                {
                    --actualZ;
                    if (actualZ <= 0)
                    {
                        canFall = false;
                    }
                    else if (settled.ContainsKey(actualZ))
                    {
                        switch (brick.Axis)
                        {
                            case Axis.X:
                                {
                                    for (int x = brick.Start.X; x <= brick.End.X; ++x)
                                    {
                                        IEnumerable<BrickSpace> potential = settled[actualZ].Where(bs => bs.Space.X == x && bs.Space.Y == brick.Start.Y);
                                        if (potential.Count() > 0)
                                        {
                                            AddSupport(brick, potential);
                                            canFall = false;
                                        }
                                    }
                                }
                                break;
                            case Axis.Y:
                                {
                                    for (int y = brick.Start.Y; y <= brick.End.Y; ++y)
                                    {
                                        IEnumerable<BrickSpace> potential = settled[actualZ].Where(bs => bs.Space.Y == y && bs.Space.X == brick.Start.X);
                                        if (potential.Count() > 0)
                                        {
                                            AddSupport(brick, potential);
                                            canFall = false;
                                        }
                                    }
                                }
                                break;
                            case Axis.Z:
                                {
                                    IEnumerable<BrickSpace> potential = settled[actualZ].Where(bs => bs.Space.X == brick.Start.X && bs.Space.Y == brick.Start.Y);
                                    if (potential.Count() > 0)
                                    {
                                        AddSupport(brick, potential);
                                        canFall = false;
                                    }
                                }
                                break;
                        }

                    }
                }

                if (brick.Start.Z > 1)
                {
                    ++actualZ;
                }

                for (int z = actualZ; z <= brick.End.Z - (brick.Start.Z - actualZ); ++z)
                {
                    if (!settled.ContainsKey(z))
                    {
                        settled[z] = new List<BrickSpace>();
                    }

                    for (int y = brick.Start.Y; y <= brick.End.Y; ++y)
                    {
                        for (int x = brick.Start.X; x <= brick.End.X; ++x)
                        {
                            settled[z].Add(new BrickSpace(brick.Id, new Base.Pos3(x, y, z)));
                        }
                    }
                }
            }

            int safeToDestroy = 0;
            // IEnumerable<int> supportedIds = supports.SelectMany(s => s.Value);
            foreach (var pair in supports)
            {
                bool canBreak = true;
                foreach (int brickId in pair.Value)
                {
                    if (!supports.Where(s => s.Key != pair.Key).SelectMany(s => s.Value).Contains(brickId))
                    {
                        canBreak = false;
                        break;
                    }
                }
                if (canBreak)
                {
                    ++safeToDestroy;
                }
            }

            safeToDestroy += (bricks.Count(b => !supports.ContainsKey(b.Id)));

            return safeToDestroy.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
            // 454 => TOO HIGH

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}