using System.Collections.Generic;

namespace _2020
{
    class Day01 : Day
    {
        public Day01() : base() { }

        protected override string GetDay() { return nameof(Day01).ToLower(); }

        protected override string GetPart1ExampleInput()
        {
            return
@"1721
979
366
299
675
1456";
        }
        protected override string GetPart1ExampleAnswer() { return "514579"; }
        protected override string RunPart1Solution(List<string> inputs)
        {
            inputs.Sort();
            for (int i = 0; i < inputs.Count; ++i)
            {
                int inputI;
                if (!int.TryParse(inputs[i], out inputI))
                {
                    continue;
                }

                for (int j = inputs.Count - 1; j >= 0; --j)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    int inputJ;
                    if (!int.TryParse(inputs[j], out inputJ))
                    {
                        continue;
                    }

                    if (inputI + inputJ == 2020)
                    {
                        return $"{inputI * inputJ}";
                    }
                }
            }

            return "";
        }

        protected override string GetPart2ExampleInput()
        {
            return GetPart1ExampleInput();
        }
        protected override string GetPart2ExampleAnswer() { return "241861950"; }
        protected override string RunPart2Solution(List<string> inputs)
        {
            inputs.Sort();
            for (int i = 0; i < inputs.Count; ++i)
            {
                int inputI;
                if (!int.TryParse(inputs[i], out inputI))
                {
                    continue;
                }

                for (int j = i + 1; j < inputs.Count; ++j)
                {
                    int inputJ;
                    if (!int.TryParse(inputs[j], out inputJ))
                    {
                        continue;
                    }

                    for (int k = j + 1; k < inputs.Count; ++k)
                    {
                        int inputK;
                        if (!int.TryParse(inputs[k], out inputK))
                        {
                            continue;
                        }

                        if (inputI + inputJ + inputK == 2020)
                        {
                            return $"{inputI * inputJ * inputK}";
                        }
                    }
                }
            }

            return "";
        }
    }
}