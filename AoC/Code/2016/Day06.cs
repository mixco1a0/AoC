using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2016
{
    class Day06 : Day
    {
        public Day06() { }
        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                // case Part.One:
                //     return "v1";
                // case Part.Two:
                //     return "v1";
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
                Output = "easter",
                RawInput =
@"eedadn
drvtee
eandsr
raavrd
atevrs
tsrnev
sdttsa
rasrtv
nssdts
ntnada
svetve
tesnvt
vntsnd
vrdear
dvrsen
enarar"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int width = inputs.First().Length;
            Dictionary<int, int[]> counts = new Dictionary<int, int[]>();
            for (int i = 0; i < width; ++i)
            {
                counts[i] = new int[26];
                for (int j = 0; j < 26; ++j)
                {
                    counts[i][j] = 0;
                }
            }
            foreach (string input in inputs)
            {
                for (int i = 0; i < input.Length; ++i)
                {
                    ++counts[i][input[i] - 'a'];
                }
            }
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<int, int[]> pair in counts)
            {
                char val = ' ';
                int max = 0;
                for (int i = 0; i < pair.Value.Length; ++i)
                {
                    if (pair.Value[i] > max)
                    {
                        max = pair.Value[i];
                        val = (char)(i + 'a');
                    }
                }
                sb.Append(val);
            }
            return sb.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}