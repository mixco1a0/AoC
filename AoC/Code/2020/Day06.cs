using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day06 : Day
    {
        public Day06() { }
        public override string GetSolutionVersion(TestPart testPart)
        {
            switch (testPart)
            {
                case TestPart.One:
                    return "v1";
                case TestPart.Two:
                    return "v1";
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
                Output = "11",
                RawInput =
@"abc

a
b
c

ab
ac

a
a
a
a

b"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "6",
                RawInput =
@"abc

a
b
c

ab
ac

a
a
a
a

b"
            });
            return testData;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int count = 0;
            string groupInput = "";
            foreach (string input in inputs)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    // group end
                    count += groupInput.ToCharArray().Select(c => c.ToString()).Distinct().Count();
                    groupInput = "";
                    continue;
                }

                groupInput += input;
            }
            count += groupInput.ToCharArray().Select(c => c.ToString()).Distinct().Count();
            return count.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int count = 0;
            List<string> sharedInput = new List<string>();
            bool newGroup = true;
            foreach (string input in inputs)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    // group end
                    count += sharedInput.Count();
                    sharedInput.Clear();
                    newGroup = true;
                    continue;
                }

                var cur = input.ToCharArray().Select(c => c.ToString());
                if (newGroup)
                {
                    sharedInput = cur.ToList();
                    newGroup = false;
                }
                else
                {
                    sharedInput = sharedInput.Intersect(cur).ToList();
                }

            }
            count += sharedInput.Count();
            return count.ToString();
        }
    }
}