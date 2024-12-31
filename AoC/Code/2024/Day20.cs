using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2024
{
    class Day20 : Core.Day
    {
        public Day20() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            return part switch
            {
                // Core.Part.One => "v1",
                // Core.Part.Two => "v1",
                _ => base.GetSolutionVersion(part),
            };
        }

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData =
            [
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "",
                    Variables = new Dictionary<string, string> { { nameof(_VarName), "0" } },
                    RawInput =
@""
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "",
                    Variables = new Dictionary<string, string> { { nameof(_VarName), "0" } },
                    RawInput =
@""
                },
            ];
            return testData;
        }
#pragma warning disable IDE1006 // Naming Styles
        private static int _VarName { get; }
#pragma warning restore IDE1006 // Naming Styles

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool _)
        {
            GetVariable(nameof(_VarName), 1, variables, out int varName);
            return string.Empty;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}