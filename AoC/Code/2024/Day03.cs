using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC._2024
{
    class Day03 : Core.Day
    {
        public Day03() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            return part switch
            {
                Core.Part.One => "v1",
                Core.Part.Two => "v1",
                _ => base.GetSolutionVersion(part),
            };
        }

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData =
            [
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "161",
                    RawInput =
@"xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "48",
                    RawInput =
@"xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))"
                },
            ];
            return testData;
        }

        private static readonly string MulRegex = @"mul\(\d+,\d+\)";
        private static readonly string DoRegex = @"do\(\)";
        private static readonly string DontRegex = @"don't\(\)";

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int sum = 0;
            foreach (string input in inputs)
            {
                Regex regex = new(MulRegex);
                MatchCollection mc = regex.Matches(input);
                for (int i = 0; i < mc.Count; ++i)
                {
                    string match = mc[i].Value;
                    int[] values = Util.String.Split(mc[i].Value, "mul(,)").Select(int.Parse).ToArray();
                    sum += values[0] * values[1];
                }
            }
            return sum.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int sum = 0;
            string input = string.Join(' ', inputs);
            {
                Regex mulRegex = new(MulRegex);
                Regex doRegex = new(DoRegex);
                Regex dontRegex = new(DontRegex);

                MatchCollection mulMC = mulRegex.Matches(input);
                List<int> mulIndices = mulMC.Select(m => m.Index).ToList();
                List<int> doIndices = doRegex.Matches(input).Select(m => m.Index).ToList();
                List<int> dontIndices = dontRegex.Matches(input).Select(m => m.Index).ToList();

                int doIndex = 0;
                int dontIndex = int.MaxValue;
                List<Base.Range> dosAndDonts = [];
                bool checkDont = true;
                while (true)
                {
                    // get the next don't index
                    if (checkDont)
                    {
                        if (dontIndices.Count == 0)
                        {
                            break;
                        }

                        dontIndex = dontIndices.First();
                        dontIndices.RemoveAt(0);
                        while (doIndices.Count > 0 && doIndices.First() < dontIndex)
                        {
                            doIndices.RemoveAt(0);
                        }

                        dosAndDonts.Add(new(doIndex, dontIndex));
                    }
                    else
                    {
                        if (doIndices.Count == 0)
                        {
                            break;
                        }

                        doIndex = doIndices.First();
                        doIndices.RemoveAt(0);
                        while (dontIndices.Count > 0 && dontIndices.First() < doIndex)
                        {
                            dontIndices.RemoveAt(0);
                        }

                        // this will be the final range
                        if (dontIndices.Count == 0)
                        {
                            dosAndDonts.Add(new(doIndex, int.MaxValue));
                        }
                    }
                    checkDont = !checkDont;
                }

                // make sure there is at least one range
                if (dosAndDonts.Count == 0)
                {
                    dosAndDonts.Add(new(-1, int.MaxValue));
                }
                else if (doIndices.Count > 0)
                {
                    dosAndDonts.Add(new(doIndices.First(), int.MaxValue));
                }

                for (int i = 0; i < mulMC.Count; ++i)
                {
                    Capture capture = mulMC[i];
                    if (dosAndDonts.Where(dnd => dnd.HasInc(capture.Index)).Any())
                    {
                        // Log(capture.Value);
                        string match = capture.Value;
                        int[] values = Util.String.Split(capture.Value, "mul(,)").Select(int.Parse).ToArray();
                        sum += values[0] * values[1];
                    }
                }
            }
            return sum.ToString();
        }
    }
}