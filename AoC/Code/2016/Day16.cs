using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2016
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

        public override bool SkipTestData => true;

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Variables = new Dictionary<string, string>() {{"diskLength", "20"}},
                Output = "01100",
                RawInput =
@"10000"
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