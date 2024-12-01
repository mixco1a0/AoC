using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day16 : Core.Day
    {
        public Day16() { }
        
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
                Output = "",
                RawInput =
@""
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

        private record Sue(int Number, Dictionary<string, string> Attributes)
        {
            static public Sue Parse(string input)
            {
                string[] split = input.Split(" :,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int[] values = split.Where(s => { int v; return int.TryParse(s, out v); }).Select(int.Parse).ToArray();
                Sue sue = new Sue(values[0], new Dictionary<string, string>());
                for (int i = 0; i < split.Length - 1; i += 2)
                {
                    sue.Attributes[split[i]] = split[i + 1];
                }
                return sue;
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<Sue> sues = inputs.Select(Sue.Parse).ToList();
            return sues.Where(s =>
                                   (!s.Attributes.ContainsKey("children") || (s.Attributes.ContainsKey("children") && s.Attributes["children"] == "3")) &&
                                   (!s.Attributes.ContainsKey("cats") || (s.Attributes.ContainsKey("cats") && s.Attributes["cats"] == "7")) &&
                                   (!s.Attributes.ContainsKey("samoyeds") || (s.Attributes.ContainsKey("samoyeds") && s.Attributes["samoyeds"] == "2")) &&
                                   (!s.Attributes.ContainsKey("pomeranians") || (s.Attributes.ContainsKey("pomeranians") && s.Attributes["pomeranians"] == "3")) &&
                                   (!s.Attributes.ContainsKey("akitas") || (s.Attributes.ContainsKey("akitas") && s.Attributes["akitas"] == "0")) &&
                                   (!s.Attributes.ContainsKey("vizslas") || (s.Attributes.ContainsKey("vizslas") && s.Attributes["vizslas"] == "0")) &&
                                   (!s.Attributes.ContainsKey("goldfish") || (s.Attributes.ContainsKey("goldfish") && s.Attributes["goldfish"] == "5")) &&
                                   (!s.Attributes.ContainsKey("trees") || (s.Attributes.ContainsKey("trees") && s.Attributes["trees"] == "3")) &&
                                   (!s.Attributes.ContainsKey("cars") || (s.Attributes.ContainsKey("cars") && s.Attributes["cars"] == "2")) &&
                                   (!s.Attributes.ContainsKey("perfumes") || (s.Attributes.ContainsKey("perfumes") && s.Attributes["perfumes"] == "1"))
            ).First().Number.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<Sue> sues = inputs.Select(Sue.Parse).ToList();
            return sues.Where(s =>
                                   (!s.Attributes.ContainsKey("children") || (s.Attributes.ContainsKey("children") && s.Attributes["children"] == "3")) &&
                                   (!s.Attributes.ContainsKey("cats") || (s.Attributes.ContainsKey("cats") && int.Parse(s.Attributes["cats"]) > 7)) &&
                                   (!s.Attributes.ContainsKey("samoyeds") || (s.Attributes.ContainsKey("samoyeds") && s.Attributes["samoyeds"] == "2")) &&
                                   (!s.Attributes.ContainsKey("pomeranians") || (s.Attributes.ContainsKey("pomeranians") && int.Parse(s.Attributes["pomeranians"]) < 3)) &&
                                   (!s.Attributes.ContainsKey("akitas") || (s.Attributes.ContainsKey("akitas") && s.Attributes["akitas"] == "0")) &&
                                   (!s.Attributes.ContainsKey("vizslas") || (s.Attributes.ContainsKey("vizslas") && s.Attributes["vizslas"] == "0")) &&
                                   (!s.Attributes.ContainsKey("goldfish") || (s.Attributes.ContainsKey("goldfish") && int.Parse(s.Attributes["goldfish"]) < 5)) &&
                                   (!s.Attributes.ContainsKey("trees") || (s.Attributes.ContainsKey("trees") && int.Parse(s.Attributes["trees"]) > 3)) &&
                                   (!s.Attributes.ContainsKey("cars") || (s.Attributes.ContainsKey("cars") && s.Attributes["cars"] == "2")) &&
                                   (!s.Attributes.ContainsKey("perfumes") || (s.Attributes.ContainsKey("perfumes") && s.Attributes["perfumes"] == "1"))
            ).First().Number.ToString();
        }
    }
}