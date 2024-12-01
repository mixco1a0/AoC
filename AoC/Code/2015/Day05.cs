using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day05 : Core.Day
    {
        public Day05() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v2";
                case Core.Part.Two:
                    return "v2";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "2",
                RawInput =
@"ugknbfddgicrmopn
aaa
jchzalrnumimnmhp
haegwjzuvuyypxyu
dvszwmarrgswjxmb"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "2",
                RawInput =
@"qjhvhtzxzqqjkmpb
xxyxx
uurcxstgmygtbstg
ieodomkazucvgmuy"
            });
            return testData;
        }

        private bool IsNiceString(string input)
        {
            if (input.Contains("ab") || input.Contains("cd") || input.Contains("pq") || input.Contains("xy"))
            {
                return false;
            }

            if (input.Length - input.Replace("a", "").Replace("e", "").Replace("i", "").Replace("o", "").Replace("u", "").Count() < 3)
            {
                return false;
            }

            for (int i = 0; i < input.Length - 1; ++i)
            {
                if (input[i] == input[i + 1])
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsNicerString(string input)
        {
            bool valid = false;
            for (int i = 0; i < input.Length - 2; ++i)
            {
                if (input[i] == input[i + 2])
                {
                    valid = true;
                    break;
                }
            }
            if (!valid)
            {
                return false;
            }

            for (int i = 0; i < input.Length - 3; ++i)
            {
                string subString = input.Substring(i, 2);
                if (input.IndexOf(subString, i + 2) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, Func<string, bool> validFunc)
        {
            int count = 0;
            foreach (string input in inputs)
            {
                if (validFunc(input))
                {
                    ++count;
                }
            }
            return count.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, IsNiceString);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, IsNicerString);
    }
}