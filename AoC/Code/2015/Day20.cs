using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day20 : Core.Day
    {
        public Day20() { }

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

        private long GetStart(long lhn)
        {
            long temp = lhn;
            for (long l = 1; ; ++l)
            {
                if (temp - l <= 0)
                {
                    return l;
                }
                temp -= l;
            }
        }

        private long Sum(long end)
        {
            long sum = 0;
            for (long i = 1; i <= end; ++i)
            {
                if (end % i == 0)
                {
                    sum += 10 * i;
                }
            }
            return sum;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            long lhn = inputs.Select(long.Parse).First();
            long house = GetStart(lhn);
            long max = 0, sum = 0;
            while (true)
            {
                long now = Sum(house);
                if (now > max)
                {
                    Core.Log.WriteLine(Core.Log.ELevel.Spam, $"{house} -> {now}");
                    max = now;
                }
                sum = now;
                if (sum >= lhn)
                {
                    return house.ToString();
                }
                ++house;
            }
        }

        private long Sum2(long end)
        {
            long sum = 0;
            for (long i = 1; i <= end; ++i)
            {
                if (end % i == 0 && end / i <= 50)
                {
                    sum += 11 * i;
                }
            }
            return sum;
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            long lhn = inputs.Select(long.Parse).First();
            long house = 776160;
            long max = 0, sum = 0;
            while (true)
            {
                long now = Sum2(house);
                if (now > max)
                {
                    Core.Log.WriteLine(Core.Log.ELevel.Spam, $"{house} -> {now}");
                    max = now;
                }
                sum = now;
                if (sum >= lhn)
                {
                    return house.ToString();
                }
                ++house;
            }
        }
    }
}