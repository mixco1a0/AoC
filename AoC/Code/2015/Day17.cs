using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day17 : Core.Day
    {
        public Day17() { }
        
        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v1";
                case Core.Part.Two:
                    return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        public override bool SkipTestData => true;

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Variables = new Dictionary<string, string> { { "liters", "25" } },
                Output = "4",
                RawInput =
@"20
15
10
5
5"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Variables = new Dictionary<string, string> { { "liters", "25" } },
                Output = "3",
                RawInput =
@"20
15
10
5
5"
            });
            return testData;
        }

        private int GetTotal(IEnumerable<int> inputs)
        {
            int total = 0;
            foreach (int input in inputs)
            {
                total += input;
            }
            return total;
        }

        private int TryNext(int result, List<int> inputs, out int uniqueMin)
        {
            uniqueMin = 0;
            int total = 0;
            int count = 0;
            List<bool> bools = inputs.Select(_ => false).ToList();
            Dictionary<int, int> solutionCount = new Dictionary<int, int>();
            if (inputs.Count > 0)
            {
                for (int i = 0; i < inputs.Count; ++i)
                {
                    // impossible to reach the total using remaining numbers, skip ahead
                    if (total + GetTotal(inputs.Skip(i)) < result)
                    {
                        i = inputs.Count - 1;
                    }

                    total += inputs[i];
                    if (total > result)
                    {
                        total -= inputs[i];
                    }
                    else if (total == result)
                    {
                        bools[i] = true;
                        int used = bools.Where(b => b).Count();
                        if (!solutionCount.ContainsKey(used))
                        {
                            solutionCount[used] = 1;
                        }
                        else
                        {
                            ++solutionCount[used];
                        }
                        DebugWriteLine(Core.Log.ELevel.Spam, $"VALID: {string.Join(',', bools.Select((b, i) => new { b = b, i = i }).Where(pair => pair.b).Select(pair => $"{inputs[pair.i]}[#{pair.i}]"))}");
                        bools[i] = false;
                        total -= inputs[i];
                        ++count;
                    }
                    else
                    {
                        bools[i] = true;
                    }

                    if (i + 1 == inputs.Count)
                    {
                        bools[i] = false;
                        while (i >= 0 && !bools[i])
                        {
                            --i;
                        }
                        if (i < 0)
                        {
                            int minKey = solutionCount.Keys.Min();
                            uniqueMin = solutionCount[minKey];
                            return count;
                        }
                        bools[i] = false;

                        IEnumerable<int> used = inputs.Select((num, idx) => (num, idx)).Where(pair => bools[pair.idx]).Select(pair => pair.num);
                        total = 0;
                        foreach (int num in used)
                        {
                            total += num;
                        }
                    }
                }
            }
            return count;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int liters;
            GetVariable(nameof(liters), 150, variables, out liters);

            int dummy;
            return TryNext(liters, inputs.Select(int.Parse).OrderByDescending(_ => _).ToList(), out dummy).ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int liters;
            GetVariable(nameof(liters), 150, variables, out liters);

            int uniqueMin;
            TryNext(liters, inputs.Select(int.Parse).OrderByDescending(_ => _).ToList(), out uniqueMin);
            return uniqueMin.ToString();
        }
    }
}