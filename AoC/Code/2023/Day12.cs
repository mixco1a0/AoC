using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private class Arrangement
        {
            public const char O = '.';
            public const char D = '#';
            public const char U = '?';

            public string Springs { get; set; }
            public int[] Records { get; set; }
            public string Pattern { get; set; }

            public static Arrangement Parse(string input)
            {
                Arrangement arrangement = new Arrangement();
                string[] split = Util.String.Split(input, " ");
                arrangement.Springs = split[0];
                arrangement.Records = Util.Number.Split(split[1], ',').ToArray();

                const string pattern = @"[\.]+";
                StringBuilder sb = new StringBuilder();
                sb.Append('^');
                sb.Append(@"[\.]*");
                foreach (int r in arrangement.Records)
                {
                    sb.Append('[');
                    sb.Append(D);
                    sb.Append("]{");
                    sb.Append(r);
                    sb.Append('}');
                    sb.Append(pattern);
                }
                sb[sb.Length - 1] = '*';
                sb.Append('$');
                arrangement.Pattern = sb.ToString();
                return arrangement;
            }

            private record State(string Springs, int SpringIndex, State Prev)
            {
                public State Progress()
                {
                    return new State(Springs, SpringIndex + 1, this);
                }

                public State Progress(char replace)
                {
                    StringBuilder sb = new StringBuilder(Springs);
                    sb[SpringIndex] = replace;
                    return new State(sb.ToString(), SpringIndex + 1, this);
                }

                public bool IsU()
                {
                    if (SpringIndex < Springs.Length)
                    {
                        return Springs[SpringIndex] == Arrangement.U;
                    }
                    return false;
                }
            }

            public int GetCount()
            {
                // Core.TempLog.WriteLine($"Checking -> {Springs} | {string.Join(',', Records)}");
                int count = 0;

                Stack<State> states = new Stack<State>();
                states.Push(new State(Springs, 0, null));
                while (states.Count > 0)
                {
                    State state = states.Pop();
                    // if (state.Prev != null)
                    // {
                    //     Core.TempLog.WriteLine($" New  -> {string.Join("", state.Springs)} <- {string.Join("", state.Prev.Springs)}");
                    // }
                    // else
                    // {
                    //     Core.TempLog.WriteLine($"Start -> {string.Join("", state.Springs)}");
                    // }

                    if (!state.Springs.Contains(U))
                    {
                        if (IsValid(state))
                        {
                            // Core.TempLog.WriteLine($"FOUND -> {string.Join("", state.Springs)}");
                            ++count;
                        }
                        continue;
                    }
                    else if (!IsStatePossible(state))
                    {
                        // Core.TempLog.WriteLine($"Skip  -> {string.Join("", state.Springs)}");
                        continue;
                    }
                    else if (!state.IsU())
                    {
                        states.Push(state.Progress());
                        continue;
                    }

                    states.Push(state.Progress(O));
                    states.Push(state.Progress(D));
                }

                // Core.TempLog.WriteLine($"Checking -> {Springs} | {string.Join(',', Records)} | {count} solution(s)");
                // Core.TempLog.WriteLine("");
                // Core.TempLog.WriteLine("");
                return count;
            }


            private bool IsStatePossible(State state)
            {
                bool possible = true;
                int recordIndex = -1;
                int damagedCount = 0;
                for (int i = 0; possible && i < state.Springs.Length; ++i)
                {
                    switch (state.Springs[i])
                    {
                        case Arrangement.O:
                            damagedCount = 0;
                            break;
                        case Arrangement.D:
                            if (damagedCount == 0)
                            {
                                ++recordIndex;
                            }
                            ++damagedCount;
                            if (recordIndex >= Records.Length)
                            {
                                possible = false;
                            }
                            else if (damagedCount > Records[recordIndex])
                            {
                                possible = false;
                            }
                            break;
                        case Arrangement.U:
                            i = state.Springs.Length;
                            break;

                    }
                }
                return possible;
            }

            private bool IsValid(State state)
            {
                Regex regex = new Regex(Pattern);
                Match match = regex.Match(string.Join("", state.Springs));
                return match.Success;
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<Arrangement> arrangements = inputs.Select(Arrangement.Parse).ToList();
            return arrangements.Select(a => a.GetCount()).Sum().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}