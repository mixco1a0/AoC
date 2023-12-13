using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                Output = "2",
                RawInput =
@"??.#. 1,1"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "534",
                RawInput =
@"??.#.???.#.???.#.???.#.???.#. 1,1,1,1,1,1,1,1,1,1"
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

        private class Arrangement
        {
            public const char O = '.';
            public const char D = '#';
            public const char U = '?';
            public readonly char[] OD = { O, D };

            public string Springs { get; set; }
            public int[] Records { get; set; }
            public string Pattern { get; set; }
            public static bool Print { get; set; }

            public static Arrangement Parse(string input)
            {
                Arrangement arrangement = new Arrangement();
                string[] split = Util.String.Split(input, " ");
                arrangement.Springs = split[0];
                while (arrangement.Springs.IndexOf($"{O}{O}") > 0)
                {
                    arrangement.Springs = arrangement.Springs.Replace($"{O}{O}", $"{O}");
                }
                arrangement.Records = Util.Number.Split(split[1], ',').ToArray();
                arrangement.GeneratePattern();
                return arrangement;
            }

            public static Arrangement FoldedParse(string input)
            {
                string[] split = Util.String.Split(input, " ");
                string foldedSprings = split[0];
                int[] foldedRecords = Util.Number.Split(split[1], ',').ToArray();
                StringBuilder sb = new StringBuilder();
                List<int> fullRecords = new List<int>();
                sb.Append(O);
                for (int i = 0; i < 5; ++i)
                {
                    sb.Append(foldedSprings);
                    sb.Append(U);
                    fullRecords.AddRange(foldedRecords);
                }
                sb.Length--;
                sb.Append(O);

                Arrangement arrangement = new Arrangement();
                arrangement.Springs = sb.ToString();
                while (arrangement.Springs.IndexOf($"{O}{O}") > 0)
                {
                    arrangement.Springs = arrangement.Springs.Replace($"{O}{O}", $"{O}");
                }
                arrangement.Records = fullRecords.ToArray();
                arrangement.GeneratePattern();
                return arrangement;
            }

            private void GeneratePattern()
            {
                const string pattern = @"[\.]+";
                StringBuilder sb = new StringBuilder();
                sb.Append('^');
                sb.Append(@"[\.]*");
                foreach (int r in Records)
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
                Pattern = sb.ToString();
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

            public long GetCount()
            {
                long count = 0;

                Stack<State> states = new Stack<State>();
                states.Push(new State(Springs, 0, null));
                while (states.Count > 0)
                {
                    State state = states.Pop();
                    if (!state.Springs.Contains(U))
                    {
                        if (IsValid(state))
                        {
                            ++count;
                        }
                        continue;
                    }
                    else if (!IsStatePossible(state, 0, out int recordCount))
                    {
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
                return count;
            }

            private bool IsStatePossible(State state, int recordStart, out int recordIndex)
            {
                bool possible = true;
                recordIndex = recordStart - 1;
                int damagedCount = 0;
                for (int i = 0; possible && i < state.Springs.Length; ++i)
                {
                    switch (state.Springs[i])
                    {
                        case Arrangement.O:
                            if (damagedCount > 0 && damagedCount != Records[recordIndex])
                            {
                                possible = false;
                                break;
                            }
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

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool isFolded)
        {
            Arrangement.Print = false;
            List<Arrangement> arrangements;
            if (isFolded)
            {
                long count = 0;
                List<Arrangement> source = inputs.Select(Arrangement.Parse).ToList();
                arrangements = inputs.Select(Arrangement.FoldedParse).ToList();
                int logAmount = arrangements.Count / 5;
                for (int i = 0; i < arrangements.Count; ++i)
                {
                    long curCount = arrangements[i].GetCount();
                    count += curCount;
                    LogFile($"{curCount,16} solution(s) for {source[i].Springs} [unfolded = {arrangements[i].Springs}]");

                    if (Debugger.IsAttached)
                    {
                        if (i > 0 && i % logAmount == 0)
                        {
                            Core.Log.WriteLine(Core.Log.ELevel.Info, $"...{i} runs completed");
                        }
                    }
                    else
                    {
                        Core.Log.WriteSameLine(Core.Log.ELevel.Info, string.Format("...{0:000.0}% complete", (double)(i + 1) / (double)arrangements.Count * 100.0f));
                    }
                }
                LogFile($"Count = {count}");
                LogFile("");
                LogFile("");
                return count.ToString();
                // return arrangements.Select(a => a.GetSmartCount(0, 0)).Sum().ToString();
            }
            else
            {
                arrangements = inputs.Select(Arrangement.Parse).ToList();
                return arrangements.Select(a => a.GetCount()).Sum().ToString();
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}

/*

            private record History(string Group, int[] Records);
            Dictionary<History, int> m_cache;
            Dictionary<string, List<History>> m_other;

            private record Impossible(int Start, int RecordStart);
            HashSet<Impossible> m_impossibles;

            public Arrangement()
            {
                m_cache = new Dictionary<History, int>();
                m_other = new Dictionary<string, List<History>>();
                m_impossibles = new HashSet<Impossible>();
            }

            private void GetCurrentGroup(int start, out string group, out int groupEnd)
            {
                int groupStart = Springs.IndexOf(U, start);
                if (groupStart == -1)
                {
                    groupStart = start;
                }

                groupEnd = Springs.IndexOf(O, groupStart + 1);
                if (groupEnd == -1)
                {
                    groupEnd = Springs.Length;
                }

                group = Springs.Substring(start, groupEnd - start);
            }

            public long GetSmartCount(int start, int recordStart)
            {
                // if (start == 0)
                {
                    // Core.TempLog.WriteLine($"Checking -> {Springs.Substring(start)} | {string.Join(',', Records.Skip(recordStart))}");
                }

                Impossible impossible = new Impossible(start, recordStart);
                if (m_impossibles.Contains(impossible))
                {
                    return 0;
                }

                GetCurrentGroup(start, out string group, out int groupEnd);
                // if (!)
                // {
                //     if (recordStart == Records.Count())
                //     {
                //         return 1;
                //     }

                //     if (!m_impossibles.Contains(impossible))
                //     {
                //         m_impossibles.Add(impossible);
                //     }
                //     return 0;
                // }

                // int count = 0;
                long matchCount = 0;
                if (m_other.ContainsKey(group))
                {
                    // cached stuff
                    matchCount = m_other[group].Count;
                }
                else
                {
                    FillUnknown(group, recordStart, out Dictionary<string, int> matches);
                    matchCount = matches.Count;

                    if (matchCount == 0 && !m_impossibles.Contains(impossible))
                    {
                        m_impossibles.Add(impossible);
                        return 0;
                    }

                    m_other[group] = new List<History>();
                    foreach (var match in matches)
                    {
                        m_other[group].Add(new History(group, Records.Skip(recordStart).Take(match.Value - recordStart).ToArray()));
                    }
                }


                if (groupEnd < Springs.Length)
                {
                    long childMatchCount = 0;
                    foreach (History history in m_other[group])
                    {
                        if (!m_impossibles.Contains(new Impossible(groupEnd, recordStart + history.Records.Count())))
                        {
                            childMatchCount += GetSmartCount(groupEnd, recordStart + history.Records.Count());
                        }
                    }

                    matchCount *= childMatchCount;
                    if (matchCount == 0)
                    {
                        if (!m_impossibles.Contains(impossible))
                        {
                            m_impossibles.Add(impossible);
                        }
                    }
                }

                if (start == 0)
                {
                    // Core.TempLog.WriteLine($"Checking -> {Springs} | {string.Join(',', Records)} --> {matchCount}");
                    // Core.TempLog.WriteLine("");
                    // Core.TempLog.WriteLine("");
                }

                return matchCount;
            }

            public void FillUnknown(string group, int recordStart, out Dictionary<string, int> matches)
            {
                // Core.TempLog.WriteLine($"FillUnknown");
                // Core.TempLog.WriteLine($"Checking -> {group} | {string.Join(',', Records.Skip(recordStart))}");

                matches = new Dictionary<string, int>();
                Stack<State> states = new Stack<State>();
                states.Push(new State(group, 0, null));
                while (states.Count > 0)
                {
                    State state = states.Pop();
                    if (!state.Springs.Contains(U))
                    {
                        if (IsStatePossible(state, recordStart, out int recordCount))
                        {
                            // Core.TempLog.WriteLine($"Match -> {state.Springs} | {string.Join(',', Records.Skip(recordStart))}");
                            matches[state.Springs] = recordCount + 1;
                        }
                        continue;
                    }
                    else if (!IsStatePossible(state, recordStart, out int recordCount))
                    {
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
            }





*/