using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day11 : Day
    {
        public Day11() { }
        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                case Part.One:
                    return "v1";
                case Part.Two:
                    return "v1";
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
                Output = "abcdffaa",
                RawInput =
@"abcdefgh"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "ghjaabcc",
                RawInput =
@"ghijklmn"
            });
            return testData;
        }

        private string Increment(string password)
        {
            StringBuilder newPassword = new StringBuilder(password);
            if (password.Last() == 'z')
            {
                bool carryOver = true;
                for (int i = newPassword.Length - 1; i >= 0 && carryOver; --i)
                {
                    if (newPassword[i] == 'z')
                    {
                        newPassword[i] = 'a';
                    }
                    else
                    {
                        newPassword[i] = (char)(password[i] + 1);
                        carryOver = false;
                    }
                }
            }
            else
            {
                newPassword[password.Length - 1] = (char)(password.Last() + 1);
            }
            return newPassword.ToString();
        }

        private bool HasInvalid(string password)
        {
            return password.Contains('i') || password.Contains('o') || password.Contains('l');
        }

        private bool HasStraight(string password)
        {
            const int Straight = 3;
            char[] p = password.ToCharArray();
            int curStraight = 1;
            char curP = '\0';
            for (int i = 0; i < p.Length; ++i)
            {
                if (p[i] - curP == 1)
                {
                    ++curStraight;
                }
                else
                {
                    curStraight = 1;
                }
                curP = p[i];

                if (curStraight == Straight)
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasDoubles(string password)
        {
            const int Doubles = 2;
            char[] p = password.ToCharArray();
            int doublesCount = 0;
            char curP = '\0';
            for (int i = 0; i < p.Length; ++i)
            {
                if (curP == p[i])
                {
                    ++doublesCount;
                    if (doublesCount >= Doubles)
                    {
                        return true;
                    }
                    curP = '\0';
                }
                else
                {
                    curP = p[i];
                }
            }
            return false;
        }

        private string GetNextPassword(string password)
        {
            do
            {
                password = Increment(password);
                // DebugWriteLine($"Checking {password}");
            } while (HasInvalid(password) || !HasStraight(password) || !HasDoubles(password));
            return password;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return GetNextPassword(inputs.First());
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return GetNextPassword(GetNextPassword(inputs.First()));
        }
    }
}