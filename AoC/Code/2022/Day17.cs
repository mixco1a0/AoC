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
                Output = "1514285714288",
                RawInput =
@">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>"
            });
            return testData;
        }

        private record LPos2(long X, long Y)
        {
            public static LPos2 operator +(LPos2 a, LPos2 b)
            {
                return new LPos2(a.X + b.X, a.Y + b.Y);
            }
        }

        static readonly List<LPos2>[] RockShapes =
        {
            new List<LPos2>() { new LPos2(0, 0), new LPos2(1, 0), new LPos2(2, 0), new LPos2(3, 0) },
            new List<LPos2>() { new LPos2(1, 0), new LPos2(0, 1), new LPos2(1, 1), new LPos2(2, 1), new LPos2(1, 2) },
            new List<LPos2>() { new LPos2(2, 0), new LPos2(2, 1), new LPos2(2, 2), new LPos2(0, 0), new LPos2(1, 0) },
            new List<LPos2>() { new LPos2(0, 0), new LPos2(0, 1), new LPos2(0, 2), new LPos2(0, 3) },
            new List<LPos2>() { new LPos2(0, 0), new LPos2(1, 0), new LPos2(0, 1), new LPos2(1, 1) },
        };
        private int MinX { get { return 0; } }
        private int MaxX { get { return 7; } }

        private void PrintRocks(Dictionary<LPos2, char> usedRocks, List<LPos2> rock, long minY, long maxY)
        {
            StringBuilder sb = new StringBuilder();
            for (long y = maxY; y > 0 && y >= minY; --y)
            {
                sb.Clear();
                sb.Append(string.Format("{0, 3} - ", y));
                for (int x = MinX - 1; x <= MaxX; ++x)
                {
                    LPos2 pos = new LPos2(x, y);
                    if (x < MinX || x >= MaxX)
                    {
                        sb.Append('|');
                    }
                    else if (rock.Contains(pos))
                    {
                        sb.Append('@');
                    }
                    else if (usedRocks.ContainsKey(pos))
                    {
                        sb.Append(usedRocks[pos]);
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

        private bool CanMove(HashSet<LPos2> usedPoints, ref List<LPos2> rock, LPos2 movement)
        {
            List<LPos2> movedRock = new List<LPos2>();
            for (int i = 0; i < rock.Count; ++i)
            {
                movedRock.Add(rock[i] + movement);

                LPos2 movedRockNode = movedRock.Last();
                if (movedRockNode.X < MinX || movedRockNode.X >= MaxX || movedRockNode.Y <= 0 || usedPoints.Contains(movedRockNode))
                {
                    return false;
                }
            }

            rock = movedRock;
            return true;
        }

        private long DetectCycle(Dictionary<long, string> cycleDetection, long[] ys, Dictionary<LPos2, char> usedRocks, out long yCycle)
        {
            yCycle = -1;
            long maxY = ys.Max();
            //long yCheck = ys.Max();
            foreach (long yCheck in ys)
            {
                string cycleCheck = cycleDetection[yCheck];
                if (cycleDetection.Any(c => c.Key < ys.Min() - 1 && c.Value == cycleCheck))
                {
                    List<long> matches = cycleDetection.Where(p => p.Value == cycleCheck && p.Key < yCheck && Math.Abs(p.Key - yCheck) > 4)
                                                       .Select(p => p.Key).OrderByDescending(_ => _).ToList();
                    if (matches.Count >= 2)
                    {
                        foreach (long m in matches)
                        {
                            long cycleLen = yCheck - m;
                            if (cycleLen <= 4)
                            {
                                continue;
                            }

                            bool cycleFound = true;
                            for (long matchY = 0; cycleFound && matchY <= cycleLen; ++matchY)
                            {
                                if (m - matchY <= 0)
                                {
                                    cycleFound = false;
                                    break;
                                }

                                if (cycleDetection[yCheck - matchY] != cycleDetection[m - matchY])
                                {
                                    cycleFound = false;
                                    break;
                                }
                            }

                            if (cycleFound)
                            {
                                yCycle = yCheck;
                                return cycleLen;
                            }
                        }
                    }
                }
            }
            return 0;
        }

        private bool ValidateCycle(Dictionary<long, string> cycleDetection, Dictionary<LPos2, char> usedRocks, ref Info info)
        {
            long validateStart = info.TargetCycleEnd();
            long cycleStart = info.CycleStart;

            string cycleCheck = cycleDetection[validateStart];

            for (long matchY = 0; matchY <= info.CycleLen; ++matchY)
            {
                if (cycleDetection[validateStart - matchY] != cycleDetection[cycleStart - matchY])
                {
                    return false;
                }
            }

            return true;
        }

        private class Info
        {
            public long RockIdx { get; set; }
            public long HighestY { get; set; }
            public bool SpawnNewRock { get; set; }
            public long RockCount { get; set; }
            public long CycleStart { get; set; }
            public long CycleLen { get; set; }
            public long CycleRockCount { get; set; }
            public long CycleCount { get; set; }

            public Info()
            {
                RockIdx = -1;
                HighestY = 0;
                SpawnNewRock = true;
                RockCount = 0;
                CycleStart = 0;
                CycleLen = 0;
                CycleRockCount = 0;
                CycleCount = 0;
            }

            public long Length()
            {
                return HighestY + CycleLen * CycleCount;
            }

            public long Rocks()
            {
                return RockCount + CycleRockCount * CycleCount;
            }

            public long TargetCycleEnd()
            {
                return CycleStart + CycleLen;
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, long maxRockCount)
        {
            char[] jets = inputs[0].ToCharArray();
            HashSet<LPos2> usedPoints = new HashSet<LPos2>();
            Dictionary<LPos2, char> usedRocks = new Dictionary<LPos2, char>();
            Dictionary<long, string> cycleDetection = new Dictionary<long, string>();
            Info info = new Info();
            List<LPos2> newRockPos = new List<LPos2>();
            long minCycleCheck = 5;
            for (int i = 0; i < jets.Length; i = (i + 1) % jets.Length)
            {
                if (info.SpawnNewRock)
                {
                    info.SpawnNewRock = false;

                    info.RockIdx = (info.RockIdx + 1) % RockShapes.Length;

                    if (info.CycleStart > 0)
                    {
                        ++info.CycleRockCount;
                    }
                    ++info.RockCount;
                    if (info.Rocks() == maxRockCount)
                    {
                        // PrintRocks(usedRocks, new List<LPos2>(), info.HighestY - 15, info.HighestY + 1);
                        return info.Length().ToString();
                    }
                    newRockPos = new List<LPos2>();
                    long newHighestY = info.HighestY;
                    foreach (LPos2 node in RockShapes[info.RockIdx])
                    {
                        newRockPos.Add(new LPos2(node.X + 2, node.Y + info.HighestY + 4));
                        newHighestY = Math.Max(newHighestY, newRockPos.Last().Y);
                    }
                }

                if (jets[i] == '>')
                {
                    CanMove(usedPoints, ref newRockPos, new LPos2(1, 0));
                }
                else
                {
                    CanMove(usedPoints, ref newRockPos, new LPos2(-1, 0));
                }

                if (!CanMove(usedPoints, ref newRockPos, new LPos2(0, -1)))
                {
                    newRockPos.ForEach(r => usedPoints.Add(r));
                    newRockPos.ForEach(r => usedRocks[r] = (char)(info.RockIdx + '0'));
                    info.SpawnNewRock = true;
                    long preHighest = info.HighestY;
                    info.HighestY = Math.Max(info.HighestY, newRockPos.Max(r => r.Y));

                    long[] ys = newRockPos.Select(p => p.Y).Distinct().ToArray();
                    foreach (long y in ys)
                    {
                        string newLine = string.Empty;
                        for (int x = MinX; x < MaxX; ++x)
                        {
                            LPos2 pos = new LPos2(x, y);
                            if (usedRocks.ContainsKey(pos))
                            {
                                newLine += usedRocks[pos];
                            }
                            else
                            {
                                newLine += '.';
                            }
                        }

                        cycleDetection[y] = newLine;
                    }

                    if (info.CycleStart > 0 && ys.Contains(info.TargetCycleEnd()))
                    {
                        if (ValidateCycle(cycleDetection, usedRocks, ref info))
                        {
                            --info.CycleRockCount; // remove the starting rock
                            DebugWriteLine($"verified cycle |{cycleDetection[info.TargetCycleEnd() - 1]}| [{info.CycleRockCount} rocks]");
                            // PrintRocks(usedRocks, new List<LPos2>(), info.CycleStart - info.CycleLen, info.CycleStart - 1);
                            // DebugWriteLine($"post verified cycle |{cycleDetection[info.CycleLen]}|");
                            // PrintRocks(usedRocks, new List<LPos2>(), info.CycleStart, info.TargetCycleEnd() - 1);
                            // fast forward
                            info.CycleStart = -1;
                            long pendingRocks = maxRockCount - info.RockCount;
                            info.CycleCount = pendingRocks / info.CycleRockCount;
                        }
                    }

                    if (info.CycleStart == 0 && ys.Max() > 1 && info.RockCount > minCycleCheck)
                    {
                        long cycleLen = DetectCycle(cycleDetection, ys, usedRocks, out long yCycle);
                        if (cycleLen > 0 && yCycle > 0)
                        {
                            info.CycleStart = yCycle + 1;
                            info.CycleLen = cycleLen;
                            DebugWriteLine($"found cycle |{cycleDetection[yCycle]}| [{cycleLen}]");
                            // PrintRocks(usedRocks, new List<LPos2>(), yCycle + 1 - cycleLen, yCycle);
                        }
                    }
                }
            }
            return string.Empty;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 2023);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 1000000000000);
            // 1541449275363 [too low]
            // 1538773148170 [too low]
            // 1542343387470 [wrong]
    }
}