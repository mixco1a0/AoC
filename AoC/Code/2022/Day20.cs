using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day20 : Core.Day
    {
        public Day20() { }

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
                Output = "0",
                RawInput =
@"0
1
1
8"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "1",
                RawInput =
@"0
2
-4
3
6
3
17
-1
-19"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "6",
                RawInput =
@"1
2
3
4
5
1
2
3
0"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "12",
                RawInput =
@"0
2
-4
3
6
3
7
-1
-3"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            bool print = false;
            List<int> file = inputs.Select(int.Parse).ToList();
            List<string> fileWithIds = inputs.Select((i, index) => string.Format("{0}[{1}]", i, index)).ToList();
            List<string> mixing = new List<string>(fileWithIds);
            if (print) DebugWriteLine($"start -> {string.Join(", ", fileWithIds)}");
            string zeroKey = string.Empty;
            foreach (string m in mixing)
            {
                int[] split = m.Split("[]".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                if (split[0] == 0)
                {
                    zeroKey = m;
                    continue;
                }

                if (split[0] == mixing.Count)
                {
                    continue;
                }

                int index = fileWithIds.IndexOf(m);
                int newIndex = split[0];
                if (split[0] < 0)
                {
                    while (newIndex < 0)
                    {
                        newIndex = newIndex + mixing.Count - 1;
                    }
                }
                else
                {
                    while (newIndex >= mixing.Count)
                    {
                        newIndex = newIndex - mixing.Count + 1;
                    }
                }
                newIndex += index;
                while (newIndex >= mixing.Count)
                {
                    newIndex = newIndex - mixing.Count + 1;
                }
                
                fileWithIds.RemoveAt(index);
                fileWithIds.Insert(newIndex, m);
                if (print) DebugWriteLine($"{string.Format("{0, -5}", m)} -> {string.Join(", ", fileWithIds)}");
            }

            if (print) DebugWriteLine($"end   -> {string.Join(", ", fileWithIds)}");
            int sum = 0;
            int start = fileWithIds.IndexOf(zeroKey);
            int[] indices = new int[] { (start + 1000) % mixing.Count, (start + 2000) % mixing.Count, (start + 3000) % mixing.Count };
            foreach (int i in indices)
            {
                string[] split = fileWithIds[i].Split("[]".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                sum += int.Parse(split[0]);
            }
            return sum.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}