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
                Output = "9",
                RawInput =
@"(3x3)XYZ"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "20",
                RawInput =
@"X(8x2)(3x3)ABCY"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "241920",
                RawInput =
@"(27x12)(20x12)(13x14)(7x10)(1x12)A"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "445",
                RawInput =
@"(25x3)(3x3)ABC(2x3)XY(5x2)PQRSTX(18x9)(3x2)TWO(5x7)SEVEN"
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

        private long FullDecompressString(ref int start, int openP, int closeP, string compressedString)
        {
            long decompressedLength = 0;
            SequenceCompression sc = SequenceCompression.Parse(compressedString.Substring(openP, closeP - openP));
            string sequence = compressedString.Substring(closeP, sc.CharacterCount);
            if (sequence.IndexOf('(') > 0)
            {

            }
            decompressedLength += (sc.RepititionCount * sc.CharacterCount);
            start = closeP + sc.CharacterCount;
            return decompressedLength;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool fullDecompress)
        {
            string compressedString = inputs.First();
            long decompressedLength = 0;
            while (!string.IsNullOrWhiteSpace(compressedString))
            {
                int openP = compressedString.IndexOf('(');
                if (openP == -1)
                {
                    decompressedLength += compressedString.Length;
                    compressedString = string.Empty;
                    break;
                }
                else
                {
                    decompressedLength += openP;

                    int closeP = compressedString.IndexOf(')', openP) + 1;
                    SequenceCompression sc = SequenceCompression.Parse(compressedString.Substring(openP, closeP - openP));
                    string sequence = compressedString.Substring(closeP, sc.CharacterCount);
                    compressedString = compressedString.Remove(0, closeP + sc.CharacterCount);
                    if (fullDecompress)
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int rep = 0; rep < sc.RepititionCount; ++rep)
                        {
                            sb.Append(sequence);
                        }
                        sb.Append(compressedString);
                        compressedString = sb.ToString();
                    }
                    else
                    {
                        decompressedLength += (sc.RepititionCount * sc.CharacterCount);
                    }
                }
            }
            return decompressedLength.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}