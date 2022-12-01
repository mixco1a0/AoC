using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2021
{
    class Day03 : Core.Day
    {
        public Day03() { }

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
                Output = "198",
                RawInput =
@"00100
11110
10110
10111
10101
01111
00111
11100
10000
11001
00010
01010"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "230",
                RawInput =
@"00100
11110
10110
10111
10101
01111
00111
11100
10000
11001
00010
01010"
            });
            return testData;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool useLifeSupportRating)
        {
            IEnumerable<string> filteredInputs = inputs;
            StringBuilder mostCommon = new StringBuilder();
            StringBuilder leastCommon = new StringBuilder();
            int maxIdx = inputs[0].Length;
            for (int curIdx = 0; curIdx < maxIdx; ++curIdx)
            {
                int zeroesCount = 0;
                int onesCount = 0;
                if (mostCommon.Length < maxIdx)
                {
                    if (useLifeSupportRating)
                    {
                        filteredInputs = inputs.Where(i => i.StartsWith(mostCommon.ToString()));
                    }
                    if (filteredInputs.Count() > 1)
                    {
                        foreach (string input in filteredInputs)
                        {
                            zeroesCount += (input[curIdx] == '0' ? 1 : 0);
                            onesCount += (input[curIdx] == '1' ? 1 : 0);
                        }

                        if (zeroesCount > onesCount)
                        {
                            mostCommon.Append(0);
                        }
                        else if (onesCount >= zeroesCount)
                        {
                            mostCommon.Append(1);
                        }
                    }
                    else
                    {
                        string temp = filteredInputs.First();
                        mostCommon.Clear();
                        mostCommon.Append(temp);
                    }
                }

                if (leastCommon.Length < maxIdx)
                {
                    if (useLifeSupportRating)
                    {
                        filteredInputs = inputs.Where(i => i.StartsWith(leastCommon.ToString()));
                    }
                    if (filteredInputs.Count() > 1)
                    {
                        zeroesCount = 0;
                        onesCount = 0;
                        foreach (string input in filteredInputs)
                        {
                            zeroesCount += (input[curIdx] == '0' ? 1 : 0);
                            onesCount += (input[curIdx] == '1' ? 1 : 0);
                        }

                        if (zeroesCount == onesCount)
                        {
                            leastCommon.Append(0);
                        }
                        else if (zeroesCount > onesCount)
                        {
                            leastCommon.Append(1);
                        }
                        else if (onesCount > zeroesCount)
                        {
                            leastCommon.Append(0);
                        }
                    }
                    else
                    {
                        string temp = filteredInputs.First();
                        leastCommon.Clear();
                        leastCommon.Append(temp);
                    }
                }
            }

            return (Convert.ToInt32(mostCommon.ToString(), 2) * Convert.ToInt32(leastCommon.ToString(), 2)).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}