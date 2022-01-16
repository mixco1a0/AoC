using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AoC.Core;

namespace AoC._2016
{
    class Day16 : Day
    {
        public Day16() { }
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
                Variables = new Dictionary<string, string>() {{"diskLength", "20"}},
                Output = "01100",
                RawInput =
@"10000"
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

        private string FillDisk(string a)
        {
            string b = string.Join("", a.Replace('0', '#').Replace('1', '0').Replace('#', '1').Reverse());
            StringBuilder sb = new StringBuilder(a);
            sb.Append('0');
            return sb.Append(b).ToString();
        }

        private string GetChecksum(string disk)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i + 1 < disk.Length; i += 2)
            {
                sb.Append(disk[i] == disk[i+1] ? '1' : '0');
            }
            return sb.ToString();
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int diskLength)
        {
            if (variables != null && variables.ContainsKey(nameof(diskLength)))
            {
                diskLength = int.Parse(variables[nameof(diskLength)]);
            }

            string disk = inputs.First();
            while (disk.Length < diskLength)
            {
                disk = FillDisk(disk);
            }

            string checkSum = disk.Substring(0, diskLength);
            while (checkSum.Length % 2 == 0)
            {
                checkSum = GetChecksum(checkSum);
            }

            return checkSum;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 272);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 35651584);
    }
}