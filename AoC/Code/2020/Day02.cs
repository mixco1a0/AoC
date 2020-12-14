using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day02 : Day
    {
        public Day02() { }

        public override string GetSolutionVersion(TestPart testPart)
        {
            switch (testPart)
            {
                case TestPart.One:
                    return "2";
                case TestPart.Two:
                    return "2";
                default:
                    return base.GetSolutionVersion(testPart);
            }
        }

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
                    ++validPasswords;
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
                    continue;
                if (lowChar == passwordInput.SingleLetter || highChar == passwordInput.SingleLetter)
                    ++validPasswords;
            }

            return validPasswords.ToString();
        }
    }
}