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
                TestPart = Core.Part.One,
                Output = "3",
                RawInput =
@"1
2
-3
3
-2
0
4"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "1623178306",
                RawInput =
@"1
2
-3
3
-2
0
4"
            });
            return testData;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int mixCount, long decryptionKey)
        {
            List<long> file = inputs.Select(long.Parse).ToList();
            List<string> fileWithIds = inputs.Select((i, index) => string.Format("{0}[{1}]", long.Parse(i) * decryptionKey, index)).ToList();
            List<string> mixing = new List<string>(fileWithIds);
            string zeroKey = string.Empty;
            for (int mc = 0; mc < mixCount; ++mc)
            {
                foreach (string m in mixing)
                {
                    long[] split = m.Split("[]".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
                    if (split[0] == 0)
                    {
                        zeroKey = m;
                        continue;
                    }

                    if (split[0] == mixing.Count)
                    {
                        continue;
                    }

                    long index = fileWithIds.IndexOf(m);
                    long newIndex = split[0];
                    if (split[0] < 0)
                    {
                        newIndex = newIndex % (mixing.Count - 1);
                        while (newIndex < 0)
                        {
                            newIndex = newIndex + mixing.Count - 1;
                        }
                    }
                    else
                    {
                        newIndex = newIndex % (mixing.Count - 1);
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

                    fileWithIds.RemoveAt((int)index);
                    fileWithIds.Insert((int)newIndex, m);
                }
            }
            
            long sum = 0;
            long start = fileWithIds.IndexOf(zeroKey);
            long[] indices = new long[] { (start + 1000) % mixing.Count, (start + 2000) % mixing.Count, (start + 3000) % mixing.Count };
            foreach (int i in indices)
            {
                string[] split = fileWithIds[i].Split("[]".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                sum += long.Parse(split[0]);
            }
            return sum.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 1, 1);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 10, 811589153);
    }
}