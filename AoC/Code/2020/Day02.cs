using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day02 : Core.Day
    {
        public Day02() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v2";
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
                Output = "2",
                RawInput =
@"1-3 a: abcde
1-3 b: cdefg
2-9 c: ccccccccc"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "1",
                RawInput =
@"1-3 a: abcde
1-3 b: cdefg
2-9 c: ccccccccc"
            });
            return testData;
        }

        class PasswordInput
        {
            public int LowValue { get; private set; }
            public int HighValue { get; private set; }
            public string Letter { get; private set; }
            public char SingleLetter { get { return Letter[0]; } }
            public string Word { get; private set; }

            public static PasswordInput Parse(string input)
            {
                string[] split = input.Split("-: ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                return new PasswordInput
                {
                    LowValue = int.Parse(split[0]),
                    HighValue = int.Parse(split[1]),
                    Letter = split[2],
                    Word = split[3]
                };
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<PasswordInput> passwordInputs = inputs.Select(PasswordInput.Parse).ToList();
            int validPasswords = 0;
            for (int i = 0; i < passwordInputs.Count; ++i)
            {
                PasswordInput passwordInput = passwordInputs[i];
                string removedLetters = passwordInput.Word.Replace(passwordInput.Letter, "");
                int diff = passwordInput.Word.Length - removedLetters.Length;
                if (diff >= passwordInput.LowValue && diff <= passwordInput.HighValue)
                {
                    ++validPasswords;
                }
            }

            return validPasswords.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<PasswordInput> passwordInputs = inputs.Select(PasswordInput.Parse).ToList();
            int validPasswords = 0;
            for (int i = 0; i < passwordInputs.Count; ++i)
            {
                PasswordInput passwordInput = passwordInputs[i];
                char lowChar = passwordInput.Word.ElementAt(passwordInput.LowValue - 1);
                char highChar = passwordInput.Word.ElementAt(passwordInput.HighValue - 1);
                if (lowChar == highChar)
                {
                    continue;
                }

                if (lowChar == passwordInput.SingleLetter || highChar == passwordInput.SingleLetter)
                {
                    ++validPasswords;
                }
            }

            return validPasswords.ToString();
        }
    }
}

#region previous versions
/* p1.v1
        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int validPasswords = 0;
            foreach(string input in inputs)
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

                string removedLetters = split[3].Replace(split[2], "");
                int diff = split[3].Length - removedLetters.Length;
                if (diff >= min && diff <= max)
                {
                    ++validPasswords;
                    //Debug($"Valid password found: {input} [{split[2]} was found {diff} times]");
                }
            }

            return validPasswords.ToString();
        }
*/

/* p2.v1
        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int validPasswords = 0;
            foreach(string input in inputs)
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
*/
#endregion