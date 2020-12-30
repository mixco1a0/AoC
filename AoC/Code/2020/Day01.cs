using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day01 : Day
    {
        public Day01() { }
        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                case Part.One:
                    return "v3";
                case Part.Two:
                    return "v2";
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
                TestPart = Part.Two,
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
            HashSet<int> numbers = inputs.Select(int.Parse).ToHashSet();
            return numbers.Where(n => numbers.Contains(2020 - n))
                            .Select(n => (2020 - n) * n)
                            .First().ToString();
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

            return "NaN";

            /* v3, slower than v2
            HashSet<int> numbers = inputs.Select(int.Parse).ToHashSet();
            return numbers.SelectMany(n => numbers, (x, y) => new { x, y })
                            .Where(pair => numbers.Contains(2020 - pair.x - pair.y))
                            .Select(pair => (2020 - pair.x - pair.y) * pair.x * pair.y).First().ToString();
                            */
        }
    }
}

#region previous versions
/* p1.v1
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
*/

/* p1.v2
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

            return "NaN";
        }
*/

/* p2.v1
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

*/
#endregion