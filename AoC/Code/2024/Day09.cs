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
                Core.Part.One => "v1",
                Core.Part.Two => "v1",
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
                    Output = "2858",
                    RawInput =
@"2333133121414131402"
                },
            ];
            return testData;
        }

        private readonly int FreeSpaceId = -1;

        private record IdInfo(int Id, int Start, int Length);

        private void GetRawFileBlock(int[] ints, out List<int> rawFileBlock, out List<IdInfo> fileIdAndSize)
        {
            rawFileBlock = [];
            fileIdAndSize = [];
            bool isNumber = true;
            int fileId = 0;
            foreach (int i in ints)
            {
                int count = rawFileBlock.Count;
                if (isNumber)
                {
                    isNumber = false;
                    rawFileBlock.AddRange(Enumerable.Range(0, i).Select(num => fileId));
                    fileIdAndSize.Add(new IdInfo(fileId, count, i));
                    ++fileId;
                }
                else
                {
                    isNumber = true;
                    if (i > 0)
                    {
                        rawFileBlock.AddRange(Enumerable.Range(0, i).Select(num => FreeSpaceId));
                        // fileIdAndSize.Add(new IdInfo(FreeSpaceId, count, i));
                    }
                }
            }
        }

        private void GetCompressedFileBlock(List<int> rawFileBlock, out int[] compressedFileBlock)
        {
            int usedBlocks = rawFileBlock.Where(rfb => rfb != FreeSpaceId).Count();
            compressedFileBlock = new int[usedBlocks];
            int lastIndex = rawFileBlock.Count - 1;
            for (int i = 0; i < compressedFileBlock.Length; ++i)
            {
                if (rawFileBlock[i] != FreeSpaceId)
                {
                    compressedFileBlock[i] = rawFileBlock[i];
                }
                else
                {
                    while (rawFileBlock[lastIndex] == FreeSpaceId)
                    {
                        --lastIndex;
                    }
                    compressedFileBlock[i] = rawFileBlock[lastIndex--];
                }
            }
        }

        private void GetCompressedFileChunkBlock(List<int> rawFileBlock, List<IdInfo> fileIdAndSize, out int[] compressedFileBlock)
        {
            List<int> working = [.. rawFileBlock];
            fileIdAndSize.Reverse();
            // Util.Log.WriteLine($"Pre : {string.Join("", working.Select(w => w == -1 ? (char)'.' : (char)(w + '0')))}");
            while (fileIdAndSize.Count > 0)
            {
                IdInfo idInfo = fileIdAndSize.First();
                fileIdAndSize.RemoveAt(0);

                int startIdx = 0;
                while (true)
                {
                    startIdx = working.FindIndex(startIdx, id => id == FreeSpaceId);
                    if (startIdx >= 0 && startIdx < idInfo.Start)
                    {
                        int endIdx = working.FindIndex(startIdx, id => id != FreeSpaceId);
                        if (endIdx > 0)
                        {
                            if (endIdx - startIdx >= idInfo.Length)
                            {
                                for (int idx = 0; idx < idInfo.Length; ++idx)
                                {
                                    working[startIdx + idx] = idInfo.Id;
                                    working[idInfo.Start + idx] = FreeSpaceId;
                                }
                                break;
                            }
                            startIdx = endIdx;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                // Util.Log.WriteLine($"Step: {string.Join("", working.Select(w => w == -1 ? (char)'.' : (char)(w + '0')))}");
            }
            // Util.Log.WriteLine($"Step: {string.Join("", working.Select(w => w == -1 ? (char)'.' : (char)(w + '0')))}");
            compressedFileBlock = [.. working];
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool optimizeChunks)
        {
            int[] ints = inputs.First().ToCharArray().Select(a => a - '0').ToArray();
            if (!optimizeChunks)
            {
                GetRawFileBlock(ints, out List<int> rawFileBlock, out List<IdInfo> _);
                GetCompressedFileBlock(rawFileBlock, out int[] compressedFileBlock);
                return compressedFileBlock.Select((value, index) => (long)value * index).Sum().ToString();
            }
            else
            {
                GetRawFileBlock(ints, out List<int> rawFileBlock, out List<IdInfo> fileIdAndSize);
                GetCompressedFileChunkBlock(rawFileBlock, fileIdAndSize, out int[] compressedFileBlock);
                return compressedFileBlock.Select((value, index) => new {value, index}).Where(pair => pair.value != FreeSpaceId).Select(pair => (long)pair.value * pair.index).Sum().ToString();
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}