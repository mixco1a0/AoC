using System.Collections.Generic;
using System.Linq;

using AoC.Core;

namespace AoC._2015
{
    class Day17 : Day
    {
        public Day17() { }
        static private int sLiters = 150;
        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                case Part.One:
                    return "v1";
                case Part.Two:
                    return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }
        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Variables = new Dictionary<string, string> { { nameof(sLiters), "25" } },
                Output = "4",
                RawInput =
@"20
15
10
5
5"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Variables = new Dictionary<string, string> { { nameof(sLiters), "25" } },
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
                        // DebugWriteLine($"VALID: {string.Join(',', bools.Select((b, i) => new { b = b, i = i }).Where(pair => pair.b).Select(pair => $"{inputs[pair.i]}[#{pair.i}]"))}");
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
            int liters = sLiters;
            if (variables != null && variables.ContainsKey(nameof(sLiters)))
            {
                liters = int.Parse(variables[nameof(sLiters)]);
            }

            int dummy;
            return TryNext(liters, inputs.Select(int.Parse).OrderByDescending(_ => _).ToList(), out dummy).ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int liters = sLiters;
            if (variables != null && variables.ContainsKey(nameof(sLiters)))
            {
                liters = int.Parse(variables[nameof(sLiters)]);
            }

            int uniqueMin;
            TryNext(liters, inputs.Select(int.Parse).OrderByDescending(_ => _).ToList(), out uniqueMin);
            return uniqueMin.ToString();
        }
    }
}