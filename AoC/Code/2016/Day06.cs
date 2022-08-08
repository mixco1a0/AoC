using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2016
{
    class Day06 : Core.Day
    {
        public Day06() { }

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
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "advent",
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
            return testData;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool checkForMin)
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
                int compare = checkForMin ? int.MaxValue : 0;
                for (int i = 0; i < pair.Value.Length; ++i)
                {
                    if (!checkForMin && pair.Value[i] > compare)
                    {
                        compare = pair.Value[i];
                        val = (char)(i + 'a');
                    }
                    else if (checkForMin && pair.Value[i] < compare && pair.Value[i] > 0)
                    {
                        compare = pair.Value[i];
                        val = (char)(i + 'a');
                    }
                }
                sb.Append(val);
            }
            return sb.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}