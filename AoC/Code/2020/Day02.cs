using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day02 : Day
    {
        public Day02() { }

        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "2",
                RawInput =
@"1-3 a: abcde
1-3 b: cdefg
2-9 c: ccccccccc"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "1",
                RawInput =
@"1-3 a: abcde
1-3 b: cdefg
2-9 c: ccccccccc"
            });
            return testData;
        }

        protected override string RunPart1Solution(List<string> inputs)
        {
            int validPasswords = 0;
            foreach (string input in inputs)
            {
                string[] split = input.Split("-: ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 4)
                {
                    continue;
                }

                int min;
                if (!int.TryParse(split[0], out min))
                {
                    continue;
                }

                int max;
                if (!int.TryParse(split[1], out max))
                {
                    continue;
                }

                string letter = split[2];
                string password = split[3];

                string removedLetters = password.Replace(letter, "");
                int diff = password.Length - removedLetters.Length;
                if (diff >= min && diff <= max)
                {
                    ++validPasswords;
                    //Debug($"Valid password found: {input} [{letter} was found {diff} times]");
                }
            }

            return validPasswords.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs)
        {
            int validPasswords = 0;
            foreach (string input in inputs)
            {
                string[] split = input.Split("-: ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 4)
                {
                    continue;
                }

                int idx1;
                if (!int.TryParse(split[0], out idx1))
                {
                    continue;
                }
                --idx1;

                int idx2;
                if (!int.TryParse(split[1], out idx2))
                {
                    continue;
                }
                --idx2;

                if (split[2].Length > 1)
                {
                    continue;
                }

                char letter = split[2][0];
                string password = split[3];

                char loc1 = password.ElementAtOrDefault(idx1);
                char loc2 = password.ElementAtOrDefault(idx2);
                if (loc1 == letter && loc2 != letter)
                {
                    ++validPasswords;
                    //Debug($"Valid password found: {input} [{letter} was found at index {idx1+1}]");
                }
                else if (loc1 != letter && loc2 == letter)
                {
                    ++validPasswords;
                    //Debug($"Valid password found: {input} [{letter} was found at index {idx2+1}]");
                }
            }

            return validPasswords.ToString();
        }
    }
}