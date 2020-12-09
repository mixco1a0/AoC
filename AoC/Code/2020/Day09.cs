using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day09 : Day
    {
        public Day09() { }

        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "200",
                RawInput =
@"1
2
3
4
5
6
7
8
9
10
11
12
13
14
15
16
17
18
19
20
21
22
23
24
25
26
49
200"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        protected override string RunPart1Solution(List<string> inputs)
        {
            List<long> numbers = inputs.Select(num => long.Parse(num)).ToList();
            const int preamble = 25;
            for (int i = preamble; i < numbers.Count(); ++i)
            {
                if (!FindSum(numbers.Skip(i - preamble).Take(preamble).ToList(), numbers[i]))
                    return numbers[i].ToString();
            }
            return "";
        }

        private bool FindSum(List<long> list, long num)
        {
            for (int i = 0; i < list.Count(); ++i)
            {
                for (int j = i + 1; j < list.Count(); ++j)
                {
                    if (list[i] + list[j] == num)
                        return true;
                }
            }

            return false;
        }

        protected override string RunPart2Solution(List<string> inputs)
        {
            return "";
        }
    }
}