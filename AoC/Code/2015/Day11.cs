using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2015
{
    class Day11 : Core.Day
    {
        public Day11() { }

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
                Output = "abcdffaa",
                RawInput =
@"abcdefgh"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "ghjaabcc",
                RawInput =
@"ghijklmn"
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

        private string RemoveInvalid(string password)
        {
            int curIndex = 0;
            StringBuilder newPassword = new StringBuilder();
            foreach (char p in password)
            {
                ++curIndex;
                if (p == 'i' || p == 'l' || p == 'o')
                {
                    newPassword.Append((char)(p + 1));
                    break;
                }
                else
                {
                    newPassword.Append(p);
                }
            }
            newPassword.Append(new string('a', password.Length - curIndex));
            return newPassword.ToString();
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
            return RemoveInvalid(newPassword.ToString());
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
            } while (!HasDoubles(password) || !HasStraight(password));
            return password;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int cycles)
        {
            string password = inputs.First();
            for (int i = 0; i < cycles; ++i)
            {
                password = GetNextPassword(password);
            }
            return password;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 1);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 2);
    }
}