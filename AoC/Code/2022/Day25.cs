using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day25 : Core.Day
    {
        public Day25() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v1";
                case Core.Part.Two:
                    return "v1";
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
                Output = "2=-1=0",
                RawInput =
@"1=-0-2
12111
2=0=
21
2=01
111
20012
112
1=-1=
1-12
12
1=
122"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private class Number
        {
            public string Snafu { get; set; }
            public long Decimal { get; set; }

            public Number(string s)
            {
                Snafu = s;
                Decimal = ProcessSnafu();
            }

            public Number(long d)
            {
                Decimal = d;
                Snafu = ProcessDecimal();
            }

            static readonly Dictionary<char, long> Conversion = new Dictionary<char, long>() { { '2', 2 }, { '1', 1 }, { '0', 0 }, { '-', -1 }, { '=', -2 } };
            static readonly Dictionary<long, char> BackwardsC = new Dictionary<long, char>() { { 2, '2' }, { 1, '1' }, { 0, '0' }, { 3, '=' }, { 4, '-' }, { -1, '-' }, { -2, '=' } };

            private long ProcessSnafu()
            {
                long sum = 0;
                for (int i = Snafu.Length - 1; i >= 0; --i)
                {
                    long digit = (long)Math.Pow(5, Snafu.Length - 1 - i);
                    sum += digit * Conversion[Snafu[i]];
                }
                return sum;
            }

            public static long GetMax(int digit)
            {
                long sum = 0;
                for (int d = 0; d <= digit; ++d)
                {
                    sum += (long)Math.Pow(5, d) * 2;
                }
                return sum;
            }

            private string ProcessDecimal()
            {
                int index = 0;
                long place = 1;
                while (place <= Decimal)
                {
                    ++index;
                    place *= 5;
                }
                StringBuilder sb = new StringBuilder();

                long working = Decimal;
                while (index >= 0)
                {
                    long maxIndexValue = GetMax(index - 1);
                    long leftover = working % place;
                    if (leftover > maxIndexValue)
                    {
                        // negate
                        if (working / place > 0)
                        {
                            sb.Append('2');
                        }
                        else
                        {
                            sb.Append('1');
                        }

                        working = leftover - place;
                    }
                    else if (leftover <  -1 * maxIndexValue)
                    {
                        // double negate
                        if (working / place < 0)
                        {
                            sb.Append('=');
                        }
                        else
                        {
                            sb.Append('-');
                        }

                        working = leftover + place;
                    }
                    else
                    {
                        // normal
                        long digit = working / place;
                        if (working < 0)
                        {
                            working -= place * digit;
                        }
                        else
                        {
                            working -= place * digit;
                        }
                        if (digit < -2)
                        {
                            digit += place;
                        }
                        sb.Append(BackwardsC[digit]);
                    }

                    --index;
                    place /= 5;
                }
                return string.Join("", sb.ToString().TrimStart('0'));
            }

            public override string ToString()
            {
                return $"{Snafu} [{Decimal}]";
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<Number> numbers = inputs.Select(i => new Number(i)).ToList();
            Number fuel = new Number(numbers.Sum(n => n.Decimal));
            return fuel.Snafu;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        { SharedSolution(inputs, variables); return "50"; }
    }
}