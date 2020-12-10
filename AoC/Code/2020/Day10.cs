using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day10 : Day
    {
        public Day10() { }

        // public override string GetSolutionVersion(TestPart testPart)
        // {
        //     switch (testPart)
        //     {
        //         case TestPart.One:
        //             return "1";
        //         case TestPart.Two:
        //             return "1";
        //         default:
        //             return base.GetSolutionVersion(testPart);
        //     }
        // }

        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "35",
                RawInput =
@"16
10
15
5
1
11
7
19
6
12
4"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "220",
                RawInput =
@"28
33
18
42
31
14
46
20
48
47
24
23
49
45
19
38
39
11
1
32
25
35
8
17
7
9
4
2
34
10
3"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "8",
                RawInput =
@"16
10
15
5
1
11
7
19
6
12
4"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "19208",
                RawInput =
@"28
33
18
42
31
14
46
20
48
47
24
23
49
45
19
38
39
11
1
32
25
35
8
17
7
9
4
2
34
10
3"
            });
            return testData;
        }
        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<long> numbers = inputs.Select(input => long.Parse(input)).ToList();
            numbers.Sort();

            int oneJoltDiff = 0, threeJoltDiff = 0;
            long prevNumber = 0;
            foreach (long number in numbers)
            {
                bool canContinue = true;
                switch (number - prevNumber)
                {
                    case 3:
                        ++threeJoltDiff;
                        break;
                    case 2:
                        break;
                    case 1:
                        ++oneJoltDiff;
                        break;
                    default:
                        canContinue = false;
                        break;

                }
                prevNumber = number;

                if (!canContinue)
                    break;
            }

            return (oneJoltDiff * (threeJoltDiff + 1)).ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<long> numbers = inputs.Select(input => long.Parse(input)).ToList();
            numbers.Add(0); numbers = numbers.Distinct().ToList(); numbers.Sort(); numbers.Reverse();

            Dictionary<long, long> sums = new Dictionary<long, long>();
            for (int i = 0; i < numbers.Count; ++i)
            {
                long curSum = 1;
                List<long> possibilities;
                long count = GetPossibilities(numbers, i, out possibilities);
                if (count > 0)
                {
                    curSum = 0;
                    foreach (long possibility in possibilities)
                    {
                        if (sums.ContainsKey(possibility))
                        {
                            curSum += sums[possibility];
                        }
                        else
                        {
                            // shouldn't happen
                        }
                    }
                }
                sums[numbers[i]] = curSum;
            }

            return sums[0].ToString();
        }

        private long GetPossibilities(List<long> numbers, int startIdx, out List<long> possibilities)
        {
            possibilities = new List<long>();
            long curPossibilies = 0;
            long cur = numbers[startIdx];
            for (int i = startIdx - 1; i < numbers.Count && i >= 0; --i)
            {
                if (numbers[i] - cur <= 3)
                {
                    possibilities.Add(numbers[i]);
                    ++curPossibilies;
                }
                else
                {
                    break;
                }
            }
            return curPossibilies;
        }
    }
}