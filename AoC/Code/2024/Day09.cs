using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2024
{
    class Day09 : Core.Day
    {
        public Day09() { }

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
//                 new Core.TestDatum
//                 {
//                     TestPart = Core.Part.One,
//                     Output = "1",
//                     RawInput =
// @"12345"
//                 },
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "1928",
                    RawInput =
@"2333133121414131402"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "",
                    RawInput =
@""
                },
            ];
            return testData;
        }

        private readonly int FreeSpaceId = -1;

        private void GetRawFileBlock(int[] ints, out List<int> rawFileBlock)
        {
            rawFileBlock = [];
            bool isNumber = true;
            int fileId = 0;
            foreach (int i in ints)
            {
                if (isNumber)
                {
                    if (i == 0)
                    {
                        isNumber = false;
                    }
                    isNumber = false;
                    rawFileBlock.AddRange(Enumerable.Range(0, i).Select(num => fileId));
                    ++fileId;
                }
                else
                {
                    isNumber = true;
                    if (i > 0)
                    {
                        rawFileBlock.AddRange(Enumerable.Range(0, i).Select(num => FreeSpaceId));
                    }
                }
            }
        }

        private void GetCompressedFileBlock(List<int> rawFileBlock, out int[] compressedFileBlock)
        {
            int usedBlocks = rawFileBlock.Where(rfb => rfb != FreeSpaceId).Count();
            compressedFileBlock = new int[usedBlocks];
            for (int i = 0; i < compressedFileBlock.Length; ++i)
            {
                if (rawFileBlock[i] != FreeSpaceId)
                {
                    compressedFileBlock[i] = rawFileBlock[i];
                }
                else
                {
                    rawFileBlock.Reverse();
                    while (rawFileBlock[0] == FreeSpaceId)
                    {
                        rawFileBlock.RemoveAt(0);
                    }
                    int last = rawFileBlock[0];
                    rawFileBlock.RemoveAt(0);
                    compressedFileBlock[i] = last;
                    rawFileBlock.Reverse();
                }
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            int[] ints = inputs.First().ToCharArray().Select(a => a - '0').ToArray();
            GetRawFileBlock(ints, out List<int> rawFileBlock);
            GetCompressedFileBlock(rawFileBlock, out int[] compressedFileBlock);
            return compressedFileBlock.Select((value, index) => (long)value * index).Sum().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}