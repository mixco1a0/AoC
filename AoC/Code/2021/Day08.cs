using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day08 : Day
    {
        public Day08() { }

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
                Output = "26",
                RawInput =
@"be cfbegad cbdgef fgaecd cgeb fdcge agebfd fecdb fabcd edb | fdgacbe cefdb cefbgd gcbe
edbfga begcd cbg gc gcadebf fbgde acbgfd abcde gfcbed gfec | fcgedb cgb dgebacf gc
fgaebd cg bdaec gdafb agbcfd gdcbef bgcad gfac gcb cdgabef | cg cg fdcagb cbg
fbegcd cbd adcefb dageb afcb bc aefdc ecdab fgdeca fcdbega | efabcd cedba gadfec cb
aecbfdg fbg gf bafeg dbefa fcge gcbea fcaegb dgceab fcbdga | gecf egdcabf bgf bfgea
fgeab ca afcebg bdacfeg cfaedg gcfdb baec bfadeg bafgc acf | gebdcfa ecba ca fadegcb
dbcfg fgd bdegcaf fgec aegbdf ecdfab fbedc dacgb gdcebf gf | cefg dcbef fcge gbcadfe
bdfegc cbegaf gecbf dfcage bdacg ed bedf ced adcbefg gebcd | ed bcgafe cdgba cbgef
egadfb cdbfeg cegd fecab cgb gbdefca cg fgcdab egfdb bfceg | gbdfcae bgc cg cgb
gcafb gcf dcaebfg ecagb gf abcdeg gaef cafbge fdbac fegbdc | fgae cfgab fg bagce"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "5353",
                RawInput =
@"acedgfb cdfbe gcdfa fbcad dab cefabd cdfgeb eafb cagedb ab | cdfeb fcadb cdfeb cdbaf"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "61229",
                RawInput =
@"be cfbegad cbdgef fgaecd cgeb fdcge agebfd fecdb fabcd edb | fdgacbe cefdb cefbgd gcbe
edbfga begcd cbg gc gcadebf fbgde acbgfd abcde gfcbed gfec | fcgedb cgb dgebacf gc
fgaebd cg bdaec gdafb agbcfd gdcbef bgcad gfac gcb cdgabef | cg cg fdcagb cbg
fbegcd cbd adcefb dageb afcb bc aefdc ecdab fgdeca fcdbega | efabcd cedba gadfec cb
aecbfdg fbg gf bafeg dbefa fcge gcbea fcaegb dgceab fcbdga | gecf egdcabf bgf bfgea
fgeab ca afcebg bdacfeg cfaedg gcfdb baec bfadeg bafgc acf | gebdcfa ecba ca fadegcb
dbcfg fgd bdegcaf fgec aegbdf ecdfab fbedc dacgb gdcebf gf | cefg dcbef fcge gbcadfe
bdfegc cbegaf gecbf dfcage bdacg ed bedf ced adcbefg gebcd | ed bcgafe cdgba cbgef
egadfb cdbfeg cegd fecab cgb gbdefca cg fgcdab egfdb bfceg | gbdfcae bgc cg cgb
gcafb gcf dcaebfg ecagb gf abcdeg gaef cafbge fdbac fegbdc | fgae cfgab fg bagce"
            });
            return testData;
        }

        private Dictionary<int, string> Digits = new Dictionary<int, string>()
        {
            {0, "abcefg"},
            {1, "cf"},
            {2, "acdeg"},
            {3, "acdfg"},
            {4, "bcdf"},
            {5, "abdfg"},
            {6, "abdefg"},
            {7, "acf"},
            {8, "abcdefg"},
            {9, "abcdfg"},
        };

        private class Signal
        {
            public List<string> Patterns { get; set; }
            public List<string> Output { get; set; }

            public static Signal Parse(string input)
            {
                Signal signal = new Signal();
                string[] split = input.Split('|', StringSplitOptions.RemoveEmptyEntries);
                signal.Patterns = split[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                signal.Output = split[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                return signal;
            }
        }

        private int Solve(Signal signal, List<int> uniqueValues, List<int> uniqueDigits)
        {
            var test = Digits.Values.GroupBy(k => k.Length).Select(g => g).OrderBy(g => g.Count()).ToList();
            const string all = "abcdefg";
            Dictionary<char, string> translation = new Dictionary<char, string>();
            for (char i = 'a'; i <= 'g'; ++i)
            {
                translation[i] = all;
            }
            //signal.Patterns.OrderBy(p => D)
            foreach (string pattern in signal.Patterns.Where(p => uniqueValues.Contains(p.Length)))
            {
                IEnumerable<char> toRemove = all.ToCharArray().Where(c => !pattern.Contains(c));
                string potentialDigits = Digits.Values.Where(v => v.Length == pattern.Length).First();
                foreach (char d in potentialDigits)
                {
                    foreach (char tr in toRemove)
                    {
                        translation[d] = translation[d].Replace($"{tr}", string.Empty);
                    }
                }
                toRemove = all.ToCharArray().Where(c => pattern.Contains(c));
                foreach (char d in all.Where(a => !potentialDigits.Contains(a)))
                {
                    foreach (char tr in toRemove)
                    {
                        translation[d] = translation[d].Replace($"{tr}", string.Empty);
                    }
                }
            }
            Dictionary<char, string> backup = new Dictionary<char, string>(translation);
            Dictionary<char, string> working = new Dictionary<char, string>(translation);
            foreach (string pattern in signal.Patterns.Where(p => !uniqueValues.Contains(p.Length)))
            {
                IEnumerable<string> potentialDigits = Digits.Values.Where(v => v.Length == pattern.Length);
                foreach (string potentialDigit in potentialDigits)
                {
                    IEnumerable<char> toRemove = all.ToCharArray().Where(c => !pattern.Contains(c));
                    foreach (char d in potentialDigit)
                    {
                        foreach (char tr in toRemove)
                        {
                            working[d] = working[d].Replace($"{tr}", string.Empty);
                        }
                    }
                    toRemove = all.ToCharArray().Where(c => pattern.Contains(c));
                    foreach (char d in all.Where(a => !potentialDigit.Contains(a)))
                    {
                        foreach (char tr in toRemove)
                        {
                            working[d] = working[d].Replace($"{tr}", string.Empty);
                        }
                    }

                    if (working.Values.Any(v => v.Length == 0))
                    {
                        working = new Dictionary<char, string>(backup);
                    }
                    else
                    {
                        backup = new Dictionary<char, string>(working);
                        break;
                    }
                }
            }

            string finalCode = string.Empty;
            Dictionary<char, char> finalTranslate = new Dictionary<char, char>();
            foreach (var pair in working)
            {
                finalTranslate[pair.Value[0]] = pair.Key;
            }
            foreach (string digit in signal.Output.Select(o => string.Concat(o.OrderBy(c => c))))
            {
                string converted = string.Concat(string.Join(string.Empty, digit.Select(d => finalTranslate[d])).OrderBy(c => c));
                int cur = Digits.Where(p => p.Value == converted).Select(p => p.Key).First();
                finalCode += $"{cur}";
            }

            return int.Parse(finalCode);
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool decode)
        {
            List<Signal> signals = inputs.Select(Signal.Parse).ToList();
            List<int> uniqueValues = Digits.Values.GroupBy(k => k.Length).Where(g => g.Count() == 1).Select(g => g.Key).ToList();
            List<int> uniqueDigits = Digits.Where(p => uniqueValues.Contains(p.Value.Length)).Select(p => p.Key).ToList();
            int sum = 0;
            foreach (Signal signal in signals)
            {
                if (decode)
                {
                    sum += Solve(signal, uniqueValues, uniqueDigits);
                }
                else
                {
                    sum += signal.Output.Where(p => uniqueValues.Contains(p.Length)).Count();
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