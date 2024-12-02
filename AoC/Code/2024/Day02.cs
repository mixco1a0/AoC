using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Core;

namespace AoC._2024
{
    class Day02 : Core.Day
    {
        public Day02() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            return part switch
            {
                // Core.Part.One => "v1",
                // Core.Part.Two => "v1",
                _ => base.GetSolutionVersion(part),
            };
        }

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData =
            [
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "2",
                    RawInput =
@"7 6 4 2 1
1 2 7 8 9
9 7 6 2 1
1 3 2 4 5
8 6 4 4 1
1 3 6 7 9"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "4",
                    RawInput =
@"7 6 4 2 1
1 2 7 8 9
9 7 6 2 1
1 3 2 4 5
8 6 4 4 1
1 3 6 7 9"
                },
            ];
            return testData;
        }

        private enum LevelType
        {
            Uninitialized,
            None,
            NoneSkip,
            Increasing,
            Decreasing
        }

        private class Report
        {
            public int[] Levels { get; set; }
            public static Report Parse(string input)
            {
                return new Report { Levels = Util.String.Split(input, ' ').Select(int.Parse).ToArray() };
            }

            public bool IsSafe()
            {
                LevelType levelType = LevelType.Uninitialized;

                static bool isDiffSafe(int prev, int cur)
                {
                    int diff = int.Abs(prev - cur);
                    return diff >= 1 && diff <= 3;
                }

                int prev = 0;
                foreach (int level in Levels)
                {
                    switch (levelType)
                    {
                        case LevelType.Uninitialized:
                            levelType = LevelType.None;
                            break;
                        case LevelType.None:
                            if (isDiffSafe(prev, level))
                            {
                                if (level > prev)
                                {
                                    levelType = LevelType.Increasing;
                                }
                                else
                                {
                                    levelType = LevelType.Decreasing;
                                }
                            }
                            else
                            {
                                // unsafe diff
                                return false;
                            }
                            break;
                        case LevelType.Increasing:
                            if (level <= prev || !isDiffSafe(prev, level))
                            {
                                return false;
                            }
                            break;
                        case LevelType.Decreasing:
                            if (level >= prev || !isDiffSafe(prev, level))
                            {
                                return false;
                            }
                            break;
                    }
                    prev = level;
                }

                return true;
            }

            public bool IsDampenedSafe()
            {
                LevelType levelType = LevelType.Uninitialized;

                static bool isDiffSafe(int prev, int cur)
                {
                    int diff = int.Abs(prev - cur);
                    return diff >= 1 && diff <= 3;
                }

                int prev = 0;
                int prevPrev = 0;
                int unsafeCount = 0;
                foreach (int level in Levels)
                {
                    switch (levelType)
                    {
                        case LevelType.Uninitialized:
                            levelType = LevelType.None;
                            break;
                        case LevelType.None:
                            if (isDiffSafe(prev, level))
                            {
                                if (level > prev)
                                {
                                    levelType = LevelType.Increasing;
                                }
                                else
                                {
                                    levelType = LevelType.Decreasing;
                                }
                            }
                            else
                            {
                                ++unsafeCount;
                                levelType = LevelType.NoneSkip;
                            }
                            break;
                        case LevelType.NoneSkip:
                            if (isDiffSafe(prev, level))
                            {
                                if (level > prev)
                                {
                                    levelType = LevelType.Increasing;
                                }
                                else
                                {
                                    levelType = LevelType.Decreasing;
                                }
                            }
                            if (isDiffSafe(prevPrev, level))
                            {
                                if (level > prevPrev)
                                {
                                    levelType = LevelType.Increasing;
                                }
                                else
                                {
                                    levelType = LevelType.Decreasing;
                                }
                            }
                            else
                            {
                                // two unsafe
                                ++unsafeCount;
                            }
                            break;
                        case LevelType.Increasing:
                            if (level >= prev && isDiffSafe(prev, level))
                            {
                                // do nothing
                            }
                            else if (level >= prevPrev && isDiffSafe(prevPrev, level))
                            {
                                ++unsafeCount;
                            }
                            else
                            {
                                // more than one unsafe diff
                                ++unsafeCount;
                            }
                            break;
                        case LevelType.Decreasing:
                            if (level <= prev && isDiffSafe(prev, level))
                            {
                                // do nothing
                            }
                            else if (level <= prevPrev && isDiffSafe(prevPrev, level))
                            {
                                ++unsafeCount;
                            }
                            else
                            {
                                // more than one unsafe diff
                                ++unsafeCount;
                            }
                            break;
                    }

                    if (unsafeCount > 1)
                    {
                        Core.TempLog.WriteLine($"Unsafe => [{prevPrev}, {prev}, {level}] | {string.Join(' ', Levels)}");
                        return false;
                    }

                    prevPrev = prev;
                    prev = level;
                }

                return true;
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool useProblemDampener)
        {
            List<Report> reports = inputs.Select(Report.Parse).ToList();
            if (!useProblemDampener)
            {
                return reports.Where(r => r.IsSafe()).Count().ToString();
            }
            return reports.Where(r => r.IsSafe() || r.IsDampenedSafe()).Count().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
        // 340 => TOO LOW
        // 372 => TOO LOW
    }
}