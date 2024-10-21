using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2023
{
    class Day02 : Core.Day
    {
        public Day02() { }

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

        public override bool SkipTestData => true;

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "8",
                RawInput =
@"Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "2286",
                RawInput =
@"Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green"
            });
            return testData;
        }

        public class BagOfCubes
        {
            public int Id { get; set; }
            public int MaxBlue { get; set; }
            public int MaxRed { get; set; }
            public int MaxGreen { get; set; }

            public bool IsValid(int b, int r, int g)
            {
                return MaxBlue <= b && MaxRed <= r && MaxGreen <= g;
            }

            public int GetPower()
            {
                return MaxBlue * MaxRed * MaxGreen;
            }

            public static BagOfCubes Parse(string input)
            {
                BagOfCubes boc = new BagOfCubes();
                string[] inputs = Util.String.Split(input, ":;");
                boc.Id = int.Parse(string.Join("", inputs[0].Skip(5)));
                foreach (string curI in inputs.Skip(1))
                {
                    string[] curISplits = Util.String.Split(curI, " ,;");
                    int count = 0;
                    foreach (string curISplit in curISplits)
                    {
                        if (char.IsAsciiDigit(curISplit[0]))
                        {
                            count = int.Parse(curISplit);
                        }
                        else
                        {
                            switch (curISplit[0])
                            {
                                case 'b':
                                    boc.MaxBlue = Math.Max(boc.MaxBlue, count);
                                    break;
                                case 'r':
                                    boc.MaxRed = Math.Max(boc.MaxRed, count);
                                    break;
                                case 'g':
                                    boc.MaxGreen = Math.Max(boc.MaxGreen, count);
                                    break;
                            }
                        }
                    }
                }
                return boc;
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool findPossible)
        {
            IEnumerable<BagOfCubes> bocs = inputs.Select(BagOfCubes.Parse).ToList();
            if (findPossible)
            {
                return bocs.Where(b => b.IsValid(14, 12, 13)).Select(b => b.Id).Sum().ToString();
            }
            else
            {
                return bocs.Select(b => b.GetPower()).Sum().ToString();
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}