using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2024
{
    class Day01 : Core.Day
    {
        public Day01() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            return part switch
            {
                Core.Part.One => "v1",
                Core.Part.Two => "v1",
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
                    Output = "11",
                    RawInput =
@"3   4
4   3
2   5
1   3
3   9
3   3"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "31",
                    RawInput =
@"3   4
4   3
2   5
1   3
3   9
3   3"
                },
            ];
            return testData;
        }

        private enum AnalyzeType
        {
            Distance,
            Similarity
        }

        private static string SharedSolution(List<string> inputs, Dictionary<string, string> variables, AnalyzeType analyzeType)
        {
            List<int> left = [], right = [];
            foreach (string i in inputs)
            {
                int[] split = Util.String.Split(i, ' ').Select(int.Parse).ToArray();
                left.Add(split[0]);
                right.Add(split[1]);
            }
            int sum = 0;
            if (analyzeType == AnalyzeType.Distance)
            {
                left.Sort();
                right.Sort();
                for (int i = 0; i < left.Count; ++i)
                {
                    sum += int.Abs(left[i] - right[i]);
                }
            }
            else
            {
                for (int i = 0; i < left.Count; ++i)
                {
                    int rCount = right.Where(r => r == left[i]).Count();
                    int simScore = left[i] * rCount;
                    sum += simScore;
                }
            }
            return sum.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, AnalyzeType.Distance);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, AnalyzeType.Similarity);
    }
}