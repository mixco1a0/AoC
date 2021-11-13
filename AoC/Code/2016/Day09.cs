using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2016
{
    class Day09 : Day
    {
        public Day09() { }
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
                Output = "6",
                RawInput =
@"ADVENT"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "7",
                RawInput =
@"A(1x5)BC"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "9",
                RawInput =
@"(3x3)XYZ"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "11",
                RawInput =
@"A(2x2)BCD(2x2)EFG"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "6",
                RawInput =
@"(6x1)(1x3)A"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "18",
                RawInput =
@"X(8x2)(3x3)ABCY"
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

        public record SequenceCompression(int CharacterCount, int RepititionCount)
        {
            static public SequenceCompression Parse(string input)
            {
                string[] split = input.Split("(x)".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                return new SequenceCompression(int.Parse(split[0]), int.Parse(split[1]));
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            string compressedString = inputs.First();
            StringBuilder sb = new StringBuilder();
            for (int start = 0; start < compressedString.Length;)
            {
                int openP = compressedString.IndexOf('(', start);
                if (openP == -1)
                {
                    sb.Append(compressedString.Substring(start));
                    break;
                }
                else
                {
                    if (start < openP)
                    {
                        sb.Append(compressedString.Substring(start, openP - start));
                    }
                    int closeP = compressedString.IndexOf(')', openP) + 1;
                    SequenceCompression sc = SequenceCompression.Parse(compressedString.Substring(openP, closeP - openP));
                    string sequence = compressedString.Substring(closeP, sc.CharacterCount);
                    for (int rep = 0; rep < sc.RepititionCount; ++rep)
                    {
                        sb.Append(sequence);
                    }
                    start = closeP + sc.CharacterCount;
                }
            }
            return sb.ToString().Length.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}