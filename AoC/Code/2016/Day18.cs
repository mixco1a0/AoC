using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2016
{
    class Day18 : Day
    {
        public Day18() { }
        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                // case Part.One:
                //     return "v1";
                // case Part.Two:
                //     return "v1";
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
                Variables = new Dictionary<string, string>() { { "rowCount", "3" } },
                Output = "6",
                RawInput =
@"..^^."
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Variables = new Dictionary<string, string>() { { "rowCount", "10" } },
                Output = "38",
                RawInput =
@".^^.^.^^^^"
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

        private char Safe { get { return '.'; } }
        private char Trap { get { return '^'; } }

        public char GetTile(string prevRow, int pos)
        {
            char l = Safe, c = Safe, r = Safe;

            // left
            if (pos != 0)
            {
                l = prevRow[pos - 1];
            }

            // center
            c = prevRow[pos];

            //right
            if (pos < prevRow.Length - 1)
            {
                r = prevRow[pos + 1];
            }

            if (l != r)
            {
                return Trap;
            }
            return Safe;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            int rowCount;
            Util.GetVariable(nameof(rowCount), 40, variables, out rowCount);
            string prevRow = inputs.First();
            StringBuilder allTiles = new StringBuilder();
            for (int r = 0; r < rowCount; ++r)
            {
                DebugWriteLine($"Row {r,2} - {prevRow}");
                allTiles.AppendLine(prevRow);
                StringBuilder sb = new StringBuilder();
                for (int c = 0; c < prevRow.Length; ++c)
                {
                    sb.Append(GetTile(prevRow, c));
                }
                prevRow = sb.ToString();
            }
            return allTiles.ToString().Where(c => c == Safe).Count().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}