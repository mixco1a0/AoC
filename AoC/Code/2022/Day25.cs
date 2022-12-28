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
                // case Core.Part.One:
                //     return "v1";
                // case Core.Part.Two:
                //     return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        public override bool SkipTestData => false;

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
                // Core.Log.WriteLine(Core.Log.ELevel.Debug, $"Converting {Snafu}");
                long sum = 0;
                for (int i = Snafu.Length - 1; i >= 0; --i)
                {
                    long digit = (long)Math.Pow(5, Snafu.Length - 1 - i);
                    sum += digit * Conversion[Snafu[i]];
                    // Core.Log.WriteLine(Core.Log.ELevel.Debug, $"Sum={sum} ({Snafu[i]})| Digit ({digit}) [{Snafu.Length - 1 - i}]");
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
                while (index > 0)
                {
                    long maxIndexValue = GetMax(index - 1);
                    //long curBase5 = (long)(Math.Pow(5, index));
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
                        // negate
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

                if (working < -2)
                {
                    sb.Append(BackwardsC[working + 5]);
                }
                else
                {
                    sb.Append(BackwardsC[working]);
                }
                return string.Join("", sb.ToString().TrimStart('0'));
            }

            public override string ToString()
            {
                return $"{Snafu} [{Decimal}]";
            }
        }

        private void QuickPrintDecimal(Number n)
        {
            DebugWriteLine($"{n.Snafu} => {n.ToString()}");
        }

        private void QuickPrintSnafu(Number n)
        {
            DebugWriteLine($"{n.Decimal} => {n.ToString()}");
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            // DebugWriteLine($"0 @ {Number.GetMax(0)}");
            // DebugWriteLine($"1 @ {Number.GetMax(1)}");
            // DebugWriteLine($"2 @ {Number.GetMax(2)}");
            // DebugWriteLine($"3 @ {Number.GetMax(3)}");
            // DebugWriteLine($"4 @ {Number.GetMax(4)}");
            // DebugWriteLine($"5 @ {Number.GetMax(5)}");

            // // 1 - 5
            // QuickPrintDecimal(new Number("1"));
            // QuickPrintDecimal(new Number("2"));
            // QuickPrintDecimal(new Number("1="));
            // QuickPrintDecimal(new Number("1-"));
            // QuickPrintDecimal(new Number("10"));

            // // 11 - 15
            // QuickPrintDecimal(new Number("21"));
            // QuickPrintDecimal(new Number("22"));
            // QuickPrintDecimal(new Number("1=="));
            // QuickPrintDecimal(new Number("1=-"));
            // QuickPrintDecimal(new Number("1=0"));

            // // 21 - 25
            // QuickPrintDecimal(new Number("1-1"));
            // QuickPrintDecimal(new Number("1-2"));
            // QuickPrintDecimal(new Number("10="));
            // QuickPrintDecimal(new Number("10-"));
            // QuickPrintDecimal(new Number("100"));

            // // 31 - 35
            // QuickPrintDecimal(new Number("111"));
            // QuickPrintDecimal(new Number("112"));
            // QuickPrintDecimal(new Number("12="));
            // QuickPrintDecimal(new Number("12-"));
            // QuickPrintDecimal(new Number("120"));


            // QuickPrintSnafu(new Number(300)); // 2200
            // QuickPrintSnafu(new Number(107)); // 1-12
            // QuickPrintSnafu(new Number(1747)); // 1=-0-2
            // QuickPrintSnafu(new Number(906)); // 12111
            // QuickPrintSnafu(new Number(201)); // 2=01
            // QuickPrintSnafu(new Number(198)); // 2=0=
            // for (int i = 1; i <= 40; ++i)
            // {
            //     QuickPrintSnafu(new Number(i));
            // }

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