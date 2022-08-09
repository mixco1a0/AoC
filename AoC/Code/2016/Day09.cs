using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2016
{
    class Day09 : Core.Day
    {
        public Day09() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v1";
                case Core.Part.Two:
                    return "v2";
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
                Output = "6",
                RawInput =
@"ADVENT"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "7",
                RawInput =
@"A(1x5)BC"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "9",
                RawInput =
@"(3x3)XYZ"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "11",
                RawInput =
@"A(2x2)BCD(2x2)EFG"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "6",
                RawInput =
@"(6x1)(1x3)A"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "18",
                RawInput =
@"X(8x2)(3x3)ABCY"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "9",
                RawInput =
@"(3x3)XYZ"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "20",
                RawInput =
@"X(8x2)(3x3)ABCY"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "241920",
                RawInput =
@"(27x12)(20x12)(13x14)(7x10)(1x12)A"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
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

        public class NestedSequence
        {
            public NestedSequence(SequenceCompression sequenceCompression, string sequence)
            {
                Sequence = sequence;
                CharacterCount = sequenceCompression.CharacterCount;
                RepititionCount = sequenceCompression.RepititionCount;
                Children = new List<NestedSequence>();

                int start = 0;
                int openP = sequence.IndexOf('(');
                while (openP >= 0)
                {
                    int closeP = sequence.IndexOf(')', openP) + 1;
                    SequenceCompression sc = SequenceCompression.Parse(sequence.Substring(openP, closeP - openP));
                    string childSequence = sequence.Substring(closeP, sc.CharacterCount);
                    Children.Add(new NestedSequence(sc, childSequence));
                    start = closeP + sc.CharacterCount;
                    openP = sequence.IndexOf('(', start);
                }
            }

            public string Sequence { get; set; }

            public string RawSequence
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append('(');
                    sb.Append(CharacterCount);
                    sb.Append('x');
                    sb.Append(RepititionCount);
                    sb.Append(')');
                    sb.Append(Sequence);
                    return sb.ToString();
                }
                private set
                {
                    value = string.Empty;
                }
            }

            long CharacterCount;

            long RepititionCount { get; set; }

            List<NestedSequence> Children { get; set; }

            public long GetLength()
            {
                long childSum = 0;
                if (Children.Count > 0)
                {
                    foreach (NestedSequence child in Children)
                    {
                        childSum += child.GetLength();
                    }
                }
                else
                {
                    return RepititionCount * CharacterCount;
                }
                return RepititionCount * childSum;
            }
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
                        NestedSequence nested = new NestedSequence(sc, sequence);
                        decompressedLength += nested.GetLength();
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