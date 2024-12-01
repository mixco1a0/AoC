using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2016
{
    class Day18 : Core.Day
    {
        public Day18() { }

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
                Variables = new Dictionary<string, string>() { { "rowCount", "3" } },
                Output = "6",
                RawInput =
@"..^^."
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Variables = new Dictionary<string, string>() { { "rowCount", "10" } },
                Output = "38",
                RawInput =
@".^^.^.^^^^"
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

        private char Safe { get { return '.'; } }
        private char Trap { get { return '^'; } }

        public char GetTile(string prevRow, int pos)
        {
            char l = Safe, r = Safe;

            // left
            if (pos != 0)
            {
                l = prevRow[pos - 1];
            }

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

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int defaultRowCount)
        {
            int rowCount;
            GetVariable(nameof(rowCount), defaultRowCount, variables, out rowCount);
            string prevRow = inputs.First();
            StringBuilder allTiles = new StringBuilder();
            for (int r = 0; r < rowCount; ++r)
            {
                //DebugWriteLine($"Row {r,2} - {prevRow}");
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
            => SharedSolution(inputs, variables, 40);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 400000);
    }
}