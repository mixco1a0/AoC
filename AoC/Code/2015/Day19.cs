using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day19 : Core.Day
    {
        public Day19() { }

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
                Output = "4",
                RawInput =
@"H => HO
H => OH
O => HH

HOH"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "7",
                RawInput =
@"H => HO
H => OH
O => HH

HOHOHO"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "3",
                RawInput =
@"e => H
e => O
H => HO
H => OH
O => HH

HOH"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "6",
                RawInput =
@"e => H
e => O
H => HO
H => OH
O => HH

HOHOHO"
            });
            return testData;
        }

        public record Replacement(string pre, string post) {}

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<Replacement> replacements = new List<Replacement>();
            string molecule = "";
            foreach (string input in inputs)
            {
                if (input.Contains("=>"))
                {
                    string[] split = input.Split("=>", StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).ToArray();
                    replacements.Add(new Replacement(split[0], split[1]));
                }
                else if (input.Trim().Length > 0)
                {
                    molecule = input;
                }
            }

            HashSet<string> unique = new HashSet<string>();
            foreach (Replacement replacement in replacements)
            {
                for (int i = molecule.IndexOf(replacement.pre); i >= 0 && i < molecule.Length; i = molecule.IndexOf(replacement.pre, i + 1))
                {
                    string cur = molecule.Remove(i, replacement.pre.Length).Insert(i, replacement.post);
                    if (!unique.Contains(cur))
                    {
                        unique.Add(cur);
                    }
                }
            }
            return unique.Count.ToString();
        }

        private int Fabricate(string molecule, List<Replacement> replacements, string target)
        {
            int minSteps = int.MaxValue;
            HashSet<string> unique = new HashSet<string>();
            Fabricate(molecule, 0, replacements, target, true, ref minSteps, ref unique);
            Fabricate(molecule, 0, replacements, target, false, ref minSteps, ref unique);
            return minSteps;
        }

        private void Fabricate(string molecule, int steps, List<Replacement> replacements, string target, bool greedy, ref int minSteps, ref HashSet<string> unique)
        {
            if (minSteps < int.MaxValue)
            {
                return;
            }
            
            if (molecule == target)
            {
                if (steps != minSteps && steps < minSteps)
                {
                    // Core.Log.WriteLine(Core.Log.ELevel.Spam, $"New min found {steps}");
                }
                minSteps = Math.Min(minSteps, steps);
                return;
            }

            if (steps >= minSteps)
            {
                return;
            }

            if (!unique.Contains(molecule))
            {
                unique.Add(molecule);
                // Core.Log.WriteLine(Core.Log.ELevel.Spam, $"Testing: {molecule}");
            }
            else
            {
                return;
            }

            List<Replacement> curUsable = replacements.Where(r => molecule.IndexOf(r.post) != -1).ToList();
            if (greedy)
            {
                foreach (Replacement replacement in curUsable)
                {
                    string cur = molecule.Replace(replacement.post, replacement.pre);
                    if (cur != molecule)
                    {
                        Fabricate(cur, steps + molecule.Split(replacement.post).Length - 1, replacements, target, greedy, ref minSteps, ref unique);
                    }
                }
            }
            else
            {
                foreach (Replacement replacement in curUsable)
                {
                    for (int i = molecule.IndexOf(replacement.post); i >= 0 && i < molecule.Length; i = molecule.IndexOf(replacement.post, i + 1))
                    {
                        string cur = molecule.Remove(i, replacement.post.Length).Insert(i, replacement.pre);
                        if (cur.Length <= molecule.Length)
                        {
                            Fabricate(cur, steps + 1, replacements, target, greedy, ref minSteps, ref unique);
                        }
                    }
                }
            }
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<Replacement> replacements = new List<Replacement>();
            string molecule = "";
            foreach (string input in inputs)
            {
                if (input.Contains("=>"))
                {
                    string[] split = input.Split("=>", StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).ToArray();
                    replacements.Add(new Replacement(split[0], split[1]));
                }
                else if (input.Trim().Length > 0)
                {
                    molecule = input;
                }
            }

            return Fabricate(molecule, replacements, "e").ToString();
        }
    }
}