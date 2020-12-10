using System.Collections.Generic;

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
                    return "1";
                case TestPart.Two:
                    return "1";
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
            inputs.Sort();
            for (int i = 0; i < inputs.Count; ++i)
            {
                int inputI;
                if (!int.TryParse(inputs[i], out inputI))
                {
                    continue;
                }

                for (int j = inputs.Count - 1; j >= 0; --j)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    int inputJ;
                    if (!int.TryParse(inputs[j], out inputJ))
                    {
                        continue;
                    }

                    if (inputI + inputJ == 2020)
                    {
                        return $"{inputI * inputJ}";
                    }
                }
            }

            return "";
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            inputs.Sort();
            for (int i = 0; i < inputs.Count; ++i)
            {
                int inputI;
                if (!int.TryParse(inputs[i], out inputI))
                {
                    continue;
                }

                for (int j = i + 1; j < inputs.Count; ++j)
                {
                    int inputJ;
                    if (!int.TryParse(inputs[j], out inputJ))
                    {
                        continue;
                    }

                    for (int k = j + 1; k < inputs.Count; ++k)
                    {
                        int inputK;
                        if (!int.TryParse(inputs[k], out inputK))
                        {
                            continue;
                        }

                        if (inputI + inputJ + inputK == 2020)
                        {
                            return $"{inputI * inputJ * inputK}";
                        }
                    }
                }
            }

            return "";
        }
    }
}