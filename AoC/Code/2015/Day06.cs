using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day06 : Day
    {
        public Day06() { }
        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "4",
                RawInput =
@"turn on 499,499 through 500,500"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "1",
                RawInput =
@"turn on 0,0 through 0,0"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "2000000",
                RawInput =
@"toggle 0,0 through 999,999"
            });
            return testData;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            bool[,] array = new bool[1000, 1000];
            for (int i = 0; i < 1000; ++i)
            {
                for (int j = 0; j < 1000; ++j)
                {
                    array[i, j] = false;
                }
            }

            List<string> instructions = inputs.Select(str => str.Replace("turn ", "").Replace("through ", "")).ToList();
            foreach (string instruction in instructions)
            {
                List<int> splits = instruction.Split(new char[] { ' ', ',' }).Skip(1).Select(int.Parse).ToList();
                for (int i = splits[0]; i <= splits[2]; ++i)
                {
                    for (int j = splits[1]; j <= splits[3]; ++j)
                    {
                        switch (instruction[0..2])
                        {
                            case "on":
                                array[i, j] = true;
                                break;
                            case "of":
                                array[i, j] = false;
                                break;
                            case "to":
                                array[i, j] = !array[i, j];
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            int count = 0;
            for (int i = 0; i < 1000; ++i)
            {
                for (int j = 0; j < 1000; ++j)
                {
                    count += (array[i, j] ? 1 : 0);
                }
            }

            return count.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int[,] array = new int[1000, 1000];
            for (int i = 0; i < 1000; ++i)
            {
                for (int j = 0; j < 1000; ++j)
                {
                    array[i, j] = 0;
                }
            }

            List<string> instructions = inputs.Select(str => str.Replace("turn ", "").Replace("through ", "")).ToList();
            foreach (string instruction in instructions)
            {
                List<int> splits = instruction.Split(new char[] { ' ', ',' }).Skip(1).Select(int.Parse).ToList();
                for (int i = splits[0]; i <= splits[2]; ++i)
                {
                    for (int j = splits[1]; j <= splits[3]; ++j)
                    {
                        switch (instruction[0..2])
                        {
                            case "on":
                                ++array[i, j];
                                break;
                            case "of":
                                if (array[i, j] > 0)
                                {
                                    --array[i, j];
                                }
                                break;
                            case "to":
                                array[i, j] += 2;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            int count = 0;
            for (int i = 0; i < 1000; ++i)
            {
                for (int j = 0; j < 1000; ++j)
                {
                    count += array[i, j];
                }
            }

            return count.ToString();
        }
    }
}