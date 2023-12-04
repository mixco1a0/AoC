using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2023
{
    class Day03 : Core.Day
    {
        public Day03() { }

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

        public override bool SkipTestData => false;

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "4361",
                RawInput =
@"467..114..
...*......
..35..633.
......#...
617*......
.....+.58.
..592.....
......755.
...$.*....
.664.598.."
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "467835",
                RawInput =
@"467..114..
...*......
..35..633.
......#...
617*......
.....+.58.
..592.....
......755.
...$.*....
.664.598.."
            });
            return testData;
        }

        public class Part
        {
            public int Number { get; set; }
            public Base.Range Rows { get; set; }
            public int Col { get; set; }
            public override string ToString()
            {
                return $"{Number} @ [[{Rows.ToString()}], {Col}]";
            }
        }

        public record Symbol(Base.Pos2 Pos, char Character);

        public class Schematic
        {
            private record Parser(char Character, int Index);

            public List<Part> Parts { get; set; }
            public List<Symbol> Symbols { get; set; }

            public Schematic(List<string> rawSchematic)
            {
                Parts = new List<Part>();
                Symbols = new List<Symbol>();

                int curRow = 0;
                foreach (string line in rawSchematic)
                {
                    List<Parser> split = line.ToList().Select((value, index) => new Parser(value, index)).Where(p => p.Character != '.').ToList();

                    int prevNumIndex = 0;
                    Base.Range curRange = new Base.Range();
                    StringBuilder sb = new StringBuilder();
                    Action updateParts = () =>
                    {
                        Parts.Add(new Part { Number = int.Parse(sb.ToString()), Rows = new Base.Range(curRange), Col = curRow });
                        sb.Clear();
                    };
                    foreach (Parser parser in split)
                    {
                        if (char.IsAsciiDigit(parser.Character))
                        {
                            if (sb.Length > 0 && prevNumIndex + 1 != parser.Index)
                            {
                                updateParts();
                            }

                            if (sb.Length == 0)
                            {
                                curRange.Min = parser.Index;
                            }
                            sb.Append(parser.Character);
                            curRange.Max = parser.Index;
                            prevNumIndex = parser.Index;
                        }
                        else
                        {
                            if (sb.Length > 0)
                            {
                                updateParts();
                            }
                            Symbols.Add(new Symbol(new Base.Pos2(parser.Index, curRow), parser.Character));
                        }
                    }
                    if (sb.Length > 0)
                    {
                        updateParts();
                    }
                    ++curRow;
                }
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool findGears)
        {
            Schematic schematic = new Schematic(inputs);
            int runningTotal = 0;
            if (findGears)
            {
                foreach (Symbol symbol in schematic.Symbols.Where(s => s.Character == '*'))
                {
                    Base.Range maxRange = new Base.Range(symbol.Pos.X - 1, symbol.Pos.X + 1);
                    IEnumerable<Part> touching = schematic.Parts.Where(p => Math.Abs(symbol.Pos.Y - p.Col) <= 1 && (maxRange.HasInc(p.Rows.Min) || maxRange.HasInc(p.Rows.Max)));
                    if (touching.Count() == 2)
                    {
                        runningTotal += (touching.First().Number * touching.Last().Number);
                    }
                }
            }
            else
            {
                foreach (Part part in schematic.Parts)
                {
                    Base.Range maxRange = new Base.Range(part.Rows.Min - 1, part.Rows.Max + 1);
                    IEnumerable<Symbol> aboveBelow = schematic.Symbols.Where(s => (Math.Abs(s.Pos.Y - part.Col) <= 1) && maxRange.HasInc(s.Pos.X));
                    if (aboveBelow.Any())
                    {
                        runningTotal += part.Number;
                    }
                }
            }
            return runningTotal.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}