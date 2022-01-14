using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2016
{
    class Day08 : Day
    {
        public Day08() { }
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
                Variables = new Dictionary<string, string> { { "gridW", "7" }, { "gridH", "3" } },
                Output = "6",
                RawInput =
@"rect 3x2
rotate column x=1 by 1
rotate row y=0 by 4
rotate column x=1 by 1"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private string[] InitGrid(Dictionary<string, string> variables, out int gridW, out int gridH)
        {
            gridW = 50;
            gridH = 6;
            if (variables != null)
            {
                if (variables.ContainsKey(nameof(gridW)))
                {
                    gridW = int.Parse(variables[nameof(gridW)]);
                }
                if (variables.ContainsKey(nameof(gridH)))
                {
                    gridH = int.Parse(variables[nameof(gridH)]);
                }
            }

            string[] grid = new string[gridH];
            for (int i = 0; i < gridH; ++i)
            {
                grid[i] = new string(Util.Glyph.Off, gridW);
            }
            return grid;
        }

        enum InstructionType
        {
            Invalid,
            Rect,
            RotRow,
            RotCol
        }

        record Instruction(InstructionType Type, int Val1, int Val2)
        {
            public static Instruction Parse(string input)
            {
                InstructionType type = InstructionType.Invalid;
                string[] split = input.Split(" xby=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (split[0] == "rect")
                {
                    type = InstructionType.Rect;
                }
                else if (split[1] == "row")
                {
                    type = InstructionType.RotRow;
                }
                else if (split[1] == "column")
                {
                    type = InstructionType.RotCol;
                }

                return new Instruction(type, int.Parse(split.SkipLast(1).Last()), int.Parse(split.Last()));
            }
        }

        private void PerformRect(ref string[] grid, int x, int y)
        {
            StringBuilder sb = new StringBuilder();
            string newRow = new string(Util.Glyph.On, x);
            for (int i = 0; i < y; ++i)
            {
                sb.Clear();
                sb.Append(newRow);
                sb.Append(grid[i][x..]);
                grid[i] = sb.ToString();
            }
        }

        private void PerformRow(ref string[] grid, int row, int count)
        {
            count = (count + grid[0].Length) % grid[0].Length;
            StringBuilder sb = new StringBuilder();
            string val = grid[row];
            sb.Append(val[^count..]);
            sb.Append(val.Substring(0, val.Length - count));
            grid[row] = sb.ToString();
        }

        private void PerformCol(ref string[] grid, int col, int count)
        {
            count = (count + grid.Length) % grid.Length;
            string val = string.Join("", grid.Select(g => g[col]));
            StringBuilder sb = new StringBuilder();
            sb.Append(val[count..]);
            sb.Append(val[0..count]);
            grid = grid.Select((g, i) => $"{g[..col]}{sb[i]}{g[(col + 1)..]}").ToArray();
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool readGlyph)
        {
            int gridW, gridH;
            string[] grid = InitGrid(variables, out gridW, out gridH);
            List<Instruction> instructions = inputs.Select(Instruction.Parse).ToList();
            foreach (Instruction instruction in instructions)
            {
                switch (instruction.Type)
                {
                    case InstructionType.Rect:
                        PerformRect(ref grid, instruction.Val1, instruction.Val2);
                        break;
                    case InstructionType.RotRow:
                        PerformRow(ref grid, instruction.Val1, instruction.Val2);
                        break;
                    case InstructionType.RotCol:
                        PerformCol(ref grid, instruction.Val1, -instruction.Val2);
                        break;
                }
            }
            if (readGlyph)
            {
                return Util.GlyphConverter.Process(grid, Util.GlyphConverter.EType._5x6);
            }
            else
            {
                return string.Join("", grid).Replace(Util.Glyph.Off.ToString(), "").Length.ToString();
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}