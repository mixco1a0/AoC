using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day01 : Day
    {
        public Day01() { }

        public override string GetSolutionVersion(TestPart testPart)
        {
            switch (testPart)
            {
                case TestPart.One:
                    return "2";
                case TestPart.Two:
                    return "2";
                default:
                    return base.GetSolutionVersion(testPart);
            }
        }

        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "514579",
                RawInput =
@"1721
979
366
299
675
1456"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "241861950",
                RawInput =
@"1721
979
366
299
675
1456"
            });
            return testData;
        }
        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<int> numList = inputs.Select(int.Parse).OrderBy(_ => _).ToList();
            for (int i = 0; i < inputs.Count; ++i)
            {
                int numI = numList[i];
                for (int j = inputs.Count - 1; j >= 0 && j > i; --j)
                {
                    int numJ = numList[j];
                    if (numI + numJ == 2020)
                    {
                        return $"{numI * numJ}";
                    }
                }
            }

            return "";
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<int> numList = inputs.Select(int.Parse).OrderBy(_ => _).ToList();
            for (int i = 0; i < inputs.Count; ++i)
            {
                int numI = numList[i];
                for (int j = i + 1; j < inputs.Count; ++j)
                {
                    int numJ = numList[j];
                    for (int k = j + 1; k < inputs.Count; ++k)
                    {
                        int numK = numList[k];
                        if (numI + numJ + numK == 2020)
                        {
                            return $"{numI * numJ * numK}";
                        }
                    }
                }
            }

            return "";
        }
    }
}