using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2023
{
    class Day12 : Core.Day
    {
        public Day12() { }

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
                Output = "2",
                RawInput =
@"??.#. 1,1"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "21",
                RawInput =
@"???.### 1,1,3
.??..??...?##. 1,1,3
?#?#?#?#?#?#?#? 1,3,1,6
????.#...#... 4,1,1
????.######..#####. 1,6,5
?###???????? 3,2,1"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "534",
                RawInput =
@"??.#. 1,1"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "525152",
                RawInput =
@"???.### 1,1,3
.??..??...?##. 1,1,3
?#?#?#?#?#?#?#? 1,3,1,6
????.#...#... 4,1,1
????.######..#####. 1,6,5
?###???????? 3,2,1"
            });
            return testData;
        }

        public const char Operational = '.';
        public const char Damaged = '#';
        public const char Unknown = '?';

        public static readonly string SingleOperational = $"{Operational}";
        public static readonly string DoubleOperational = $"{Operational}{Operational}";

        private class Pattern
        {
            public string Springs { get; set; }
            public int[] Records { get; set; }
            private Dictionary<string, long> Cache { get; set; }

            public static Pattern Parse(string input)
            {
                string[] split = Util.String.Split(input, " ");

                Pattern pattern = new Pattern();
                pattern.Springs = split[0];
                pattern.Records = Util.Number.Split(split[1], ',').ToArray();
                return pattern;
            }

            private record SolveState(string Springs, int[] Records)
            {
                public static SolveState CreateState(Pattern pattern)
                {
                    return new SolveState(pattern.Springs, pattern.Records);
                }

                public override string ToString()
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(Springs);
                    stringBuilder.Append(" -> [");
                    stringBuilder.Append(string.Join(',', Records));
                    stringBuilder.Append(']');
                    return stringBuilder.ToString();
                }
            }

            public long Solve()
            {
                Cache = new Dictionary<string, long>();
                long solve = SolveRecurse(SolveState.CreateState(this));
                return solve;
            }

            private long SolveRecurse(SolveState solveState)
            {
                if (Cache.ContainsKey(solveState.ToString()))
                {
                    return Cache[solveState.ToString()];
                }

                if (solveState.Records.Length == 0)
                {
                    if (solveState.Springs.Length == 0 || !solveState.Springs.Contains(Damaged))
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }

                if (solveState.Springs.Length == 0)
                {
                    return 0;
                }

                char nextSpring = solveState.Springs.First();
                int nextRecord = solveState.Records.First();

                Func<SolveState, long> ProcessOperational = (currentState) =>
                {
                    return SolveRecurse(new SolveState(solveState.Springs[1..], solveState.Records));
                };

                Func<SolveState, long> ProcessDamaged = (currentState) =>
                {
                    int nextRecord = currentState.Records.First();
                    if (currentState.Springs.Length < nextRecord)
                    {
                        return 0;
                    }

                    if (currentState.Springs.Take(nextRecord).Contains(Operational))
                    {
                        return 0;
                    }

                    if (currentState.Springs.Length == nextRecord)
                    {
                        if (currentState.Records.Count() == 1)
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }

                    char next = currentState.Springs.Skip(nextRecord).First();
                    if (next == Damaged)
                    {
                        return 0;
                    }

                    return SolveRecurse(new SolveState(solveState.Springs[(nextRecord + 1)..], solveState.Records[1..])); ;
                };

                long count = 0;
                switch (nextSpring)
                {
                    case Operational:
                        {
                            count = ProcessOperational(solveState);
                            break;
                        }
                    case Damaged:
                        {
                            count = ProcessDamaged(solveState);
                            break;
                        }
                    case Unknown:
                        {
                            count = ProcessOperational(solveState) + ProcessDamaged(solveState);
                            break;
                        }
                }

                Cache[solveState.ToString()] = count;
                return count;
            }

            public static Pattern FoldedParse(string input)
            {
                string[] split = Util.String.Split(input, " ");
                string foldedSprings = split[0];
                int[] foldedRecords = Util.Number.Split(split[1], ',').ToArray();

                StringBuilder sb = new StringBuilder();
                List<int> fullRecords = new List<int>();
                for (int i = 0; i < 5; ++i)
                {
                    sb.Append(foldedSprings);
                    sb.Append(Unknown);
                    fullRecords.AddRange(foldedRecords);
                }
                sb.Length--;

                Pattern pattern = new Pattern();
                pattern.Springs = sb.ToString();
                pattern.Records = fullRecords.ToArray();
                return pattern;
            }

            public override string ToString()
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(Springs);
                stringBuilder.Append(" -> [");
                stringBuilder.Append(string.Join(',', Records));
                stringBuilder.Append(']');
                return stringBuilder.ToString();
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool isFolded)
        {
            if (isFolded)
            {
                List<Pattern> patterns = inputs.Select(Pattern.FoldedParse).ToList();
                return patterns.Select(p => p.Solve()).Sum().ToString();
            }
            else
            {
                List<Pattern> patterns = inputs.Select(Pattern.Parse).ToList();
                return patterns.Select(p => p.Solve()).Sum().ToString();
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}