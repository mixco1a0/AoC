using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2024
{
    class Day07 : Core.Day
    {
        public Day07() { }

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
                    Output = "3749",
                    RawInput =
@"190: 10 19
3267: 81 40 27
83: 17 5
156: 15 6
7290: 6 8 6 15
161011: 16 10 13
192: 17 8 14
21037: 9 7 18 13
292: 11 6 16 20"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "11387",
                    RawInput =
@"190: 10 19
3267: 81 40 27
83: 17 5
156: 15 6
7290: 6 8 6 15
161011: 16 10 13
192: 17 8 14
21037: 9 7 18 13
292: 11 6 16 20"
                },
            ];
            return testData;
        }

        private record Equation(long Result, List<long> Inputs)
        {
            public static Equation Parse(string input)
            {
                IEnumerable<long> values = Util.String.Split(input, ": ").Select(long.Parse);
                return new Equation(values.First(), values.Skip(1).ToList());
            }

            public bool CanUseOps(bool useThreeOps)
            {
                return CanUseOps(Inputs.First(), Inputs.Skip(1), useThreeOps);
            }

            private bool CanUseOps(long value, IEnumerable<long> inputs, bool useThreeOps)
            {
                if (!inputs.Any())
                {
                    return value == Result;
                }
                
                // try adding first
                long next = inputs.First();
                if (CanUseOps(value + next, inputs.Skip(1), useThreeOps))
                {
                    return true;
                }
                // try multiplying next
                else if (CanUseOps(value * next, inputs.Skip(1), useThreeOps))
                {
                    return true;
                }
                // try concatenation next
                else if (useThreeOps)
                {
                    StringBuilder sb = new();
                    sb.Append(value);
                    sb.Append(next);
                    long newVal = long.Parse(sb.ToString());
                    return CanUseOps(newVal, inputs.Skip(1), useThreeOps);
                }

                return false;
            }
        }

        private static string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool useThreeOps)
        {
            List<Equation> equations = inputs.Select(Equation.Parse).ToList();
            long sum = 0;
            foreach (Equation equation in equations)
            {
                if (equation.CanUseOps(useThreeOps))
                {
                    sum += equation.Result;
                }
            }
            return sum.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}