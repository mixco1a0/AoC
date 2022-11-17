using System.Collections.Generic;

namespace AoC._2020
{
    class Day25 : Core.Day
    {
        public Day25() { }

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
                Output = "14897079",
                RawInput =
@"5764801
17807724"
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

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            long sn1 = long.Parse(inputs[0]);
            int loop1 = 0;
            long transform1 = 1;
            while (true)
            {
                ++loop1;

                transform1 *= 7;
                transform1 = transform1 % 20201227;
                if (transform1 == sn1)
                {
                    break;
                }
            }

            long sn2 = long.Parse(inputs[1]);
            long transformE = 1;
            for (int i = 0; i < loop1; ++i)
            {
                transformE *= sn2;
                transformE = transformE % 20201227;
            }

            return transformE.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        { RunPart1Solution(inputs, variables); return "50"; }
    }
}