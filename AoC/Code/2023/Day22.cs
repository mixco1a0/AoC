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
                Output = "7",
                RawInput =
@"1,0,1~1,2,1
0,0,2~2,0,2
0,2,3~2,2,3
0,0,4~0,2,4
2,0,5~2,2,5
0,1,6~2,1,6
1,1,8~1,1,9"
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
                else
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

        private int SettleBricks(List<Brick> bricks, out Dictionary<int, List<BrickSpace>> brickOccupiedSpaces, out List<Brick> settledBricks, out Dictionary<int, HashSet<int>> supports)
        {
            int settledBrickCount = 0;
            brickOccupiedSpaces = new Dictionary<int, List<BrickSpace>>();
            settledBricks = new List<Brick>();

            Dictionary<int, HashSet<int>> localSupports = new Dictionary<int, HashSet<int>>();
            Action<Brick, IEnumerable<BrickSpace>> addSupport = (brick, potential) =>
            {
                foreach (BrickSpace bs in potential)
                {
                    if (!localSupports.ContainsKey(bs.Owner))
                    {
                        localSupports[bs.Owner] = new HashSet<int>();
                    }
                    localSupports[bs.Owner].Add(brick.Id);
                    // Log($"Brick {bs.Owner} is supporting {brick.Id}");
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
                    else if (brickOccupiedSpaces.ContainsKey(actualZ))
                    {
                        switch (brick.Axis)
                        {
                            case Axis.X:
                                {
                                    for (int x = brick.Start.X; x <= brick.End.X; ++x)
                                    {
                                        IEnumerable<BrickSpace> potential = brickOccupiedSpaces[actualZ].Where(bs => bs.Space.X == x && bs.Space.Y == brick.Start.Y);
                                        if (potential.Count() > 0)
                                        {
                                            addSupport(brick, potential);
                                            canFall = false;
                                        }
                                    }
                                }
                                break;
                            case Axis.Y:
                                {
                                    for (int y = brick.Start.Y; y <= brick.End.Y; ++y)
                                    {
                                        IEnumerable<BrickSpace> potential = brickOccupiedSpaces[actualZ].Where(bs => bs.Space.Y == y && bs.Space.X == brick.Start.X);
                                        if (potential.Count() > 0)
                                        {
                                            addSupport(brick, potential);
                                            canFall = false;
                                        }
                                    }
                                }
                                break;
                            case Axis.Z:
                                {
                                    IEnumerable<BrickSpace> potential = brickOccupiedSpaces[actualZ].Where(bs => bs.Space.X == brick.Start.X && bs.Space.Y == brick.Start.Y);
                                    if (potential.Count() > 0)
                                    {
                                        addSupport(brick, potential);
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
                    settledBricks.Add(new Brick(brick.Id, new Base.Pos3(brick.Start.X, brick.Start.Y, actualZ), new Base.Pos3(brick.End.X, brick.End.Y, brick.End.Z - (brick.Start.Z - actualZ)), brick.Axis));
                    if (actualZ != brick.Start.Z)
                    {
                        ++settledBrickCount;
                    }
                }
                else
                {
                    settledBricks.Add(brick);
                }

                for (int z = actualZ; z <= brick.End.Z - (brick.Start.Z - actualZ); ++z)
                {
                    if (!brickOccupiedSpaces.ContainsKey(z))
                    {
                        brickOccupiedSpaces[z] = new List<BrickSpace>();
                    }

                    for (int y = brick.Start.Y; y <= brick.End.Y; ++y)
                    {
                        for (int x = brick.Start.X; x <= brick.End.X; ++x)
                        {
                            brickOccupiedSpaces[z].Add(new BrickSpace(brick.Id, new Base.Pos3(x, y, z)));
                        }
                    }
                }
            }

            supports = localSupports;
            return settledBrickCount;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool disintegrateBlocks)
        {
            List<Brick> originalBricks = inputs.Select(Brick.Parse).ToList();
            originalBricks.Sort((a, b) => { return a.Start.Z.CompareTo(b.Start.Z); });
            SettleBricks(originalBricks, out Dictionary<int, List<BrickSpace>> brickOccupiedSpaces, out List<Brick> settledBricks, out Dictionary<int, HashSet<int>> supports);

            int safeToDestroy = 0;
            HashSet<int> supportBricks = new HashSet<int>();
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
                else
                {
                    supportBricks.Add(pair.Key);
                }
            }

            if (!disintegrateBlocks)
            {
                safeToDestroy += (originalBricks.Count(b => !supports.ContainsKey(b.Id)));
                return safeToDestroy.ToString();
            }

            int settleCount = 0;
            foreach (int supportBrick in supportBricks)
            {
                List<Brick> leftOvers = settledBricks.Where(b => b.Id != supportBrick).ToList();
                settleCount += SettleBricks(leftOvers, out Dictionary<int, List<BrickSpace>> bos, out List<Brick> sb, out Dictionary<int, HashSet<int>> s);
            }

            return settleCount.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}