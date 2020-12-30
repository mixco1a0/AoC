using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day05 : Day
    {
        public Day05() { }

        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "2",
                RawInput =
@"ugknbfddgicrmopn
aaa
jchzalrnumimnmhp
haegwjzuvuyypxyu
dvszwmarrgswjxmb"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "2",
                RawInput =
@"qjhvhtzxzqqjkmpb
xxyxx
uurcxstgmygtbstg
ieodomkazucvgmuy"
            });
            return testData;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int count = 0;
            foreach (string input in inputs)
            {
                if (input.Contains("ab") || input.Contains("cd") || input.Contains("pq") || input.Contains("xy"))
                {
                    continue;
                }

                if (input.Length - input.Replace("a", "").Replace("e", "").Replace("i", "").Replace("o", "").Replace("u", "").Count() < 3)
                {
                    continue;
                }

                for (int i = 0; i < input.Length - 1; ++i)
                {
                    if (input[i] == input[i + 1])
                    {
                        ++count;
                        break;
                    }
                }
            }
            return count.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int count = 0;
            foreach (string input in inputs)
            {
                bool rule1 = false;
                for (int i = 0; !rule1 && i < input.Length - 3; ++i)
                {
                    if (input.Length - input.Replace(input.Substring(i, 2), "").Length >= 4)
                    {
                        rule1 = true;
                    }
                }
                if (rule1)
                {
                    for (int i = 0; i < input.Length - 2; ++i)
                    {
                        if (input[i] == input[i + 2])
                        {
                            ++count;
                            break;
                        }
                    }
                }
            }
            return count.ToString();
        }
    }
}