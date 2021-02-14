using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day19 : Day
    {
        public Day19() { }
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
                Output = "4",
                RawInput =
@"H => HO
H => OH
O => HH

HOH"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "7",
                RawInput =
@"H => HO
H => OH
O => HH

HOHOHO"
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

        public record Replacement (string pre, string post);

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<Replacement> replacements = new List<Replacement>();
            string original = "";
            foreach (string input in inputs)
            {
                if (input.Contains("=>"))
                {
                    string[] split = input.Split("=>", StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).ToArray();
                    replacements.Add(new Replacement(split[0], split[1]));
                }
                else if (input.Trim().Length > 0)
                {
                    original = input;
                }
            }

            HashSet<string> unique = new HashSet<string>();
            foreach (Replacement replacement in replacements)
            {
                for (int i = original.IndexOf(replacement.pre); i >= 0 && i < original.Length; i = original.IndexOf(replacement.pre, i + 1))
                {
                    string cur = original.Remove(i, replacement.pre.Length).Insert(i, replacement.post);
                    if (!unique.Contains(cur))
                    {
                        unique.Add(cur);
                    }
                }
            }
            return unique.Count.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}