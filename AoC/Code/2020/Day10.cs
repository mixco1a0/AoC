using System.Collections.Generic;

namespace AoC._2020
{
    class Day10 : Day
    {
        public Day10() { }

        // public override string GetSolutionVersion(TestPart testPart)
        // {
        //     switch (testPart)
        //     {
        //         case TestPart.One:
        //             return "1";
        //         case TestPart.Two:
        //             return "1";
        //         default:
        //             return base.GetSolutionVersion(testPart);
        //     }
        // }

        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "",
                RawInput =
@""
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }
        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}