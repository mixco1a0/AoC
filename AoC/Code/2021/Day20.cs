using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2021
{
    class Day20 : Core.Day
    {
        public Day20() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v2";
                case Core.Part.Two:
                    return "v2";
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
                Output = "35",
                RawInput =
@"..#.#..#####.#.#.#.###.##.....###.##.#..###.####..#####..#....#..#..##..###..######.###...####..#..#####..##..#.#####...##.#.#..#.##..#.#......#.###.######.###.####...#.##.##..#..#..#####.....#.#....###..#.##......#.....#..#..#..##..#...##.######.####.####.#.#...#.......#..#.#.#...####.##.#......#..#...##.#.##..#...##.#.##..###.#......#.#.......#.#.#.####.###.##...#.....####.#..#..#.##.#....##..#.####....##...##..#...#......#.#.......#.......##..####..#...#.#.#...##..#.#..###..#####........#..####......#..#

#..#.
#....
##..#
..#..
..###"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "3351",
                RawInput =
@"..#.#..#####.#.#.#.###.##.....###.##.#..###.####..#####..#....#..#..##..###..######.###...####..#..#####..##..#.#####...##.#.#..#.##..#.#......#.###.######.###.####...#.##.##..#..#..#####.....#.#....###..#.##......#.....#..#..#..##..#...##.######.####.####.#.#...#.......#..#.#.#...####.##.#......#..#...##.#.##..#...##.#.##..###.#......#.#.......#.#.#.####.###.##...#.....####.#..#..#.##.#....##..#.####....##...##..#...#......#.#.......#.......##..####..#...#.#.#...##..#.#..###..#####........#..####......#..#

#..#.
#....
##..#
..#..
..###"
            });
            return testData;
        }

        static readonly char LightPixel = '#';
        static readonly char DarkPixel = '.';
        static readonly Base.Vec2[] PixelCheck = new Base.Vec2[] { new Base.Vec2(-1, -1), new Base.Vec2(0, -1), new Base.Vec2(1, -1),
                                                                     new Base.Vec2(-1, 0), new Base.Vec2(0, 0), new Base.Vec2(1, 0),
                                                                     new Base.Vec2(-1, 1), new Base.Vec2(0, 1), new Base.Vec2(1, 1) };

        private char EnhancePixel(List<string> pixels, string algorithm, int x, int y, char defaultPixel)
        {
            StringBuilder sb = new StringBuilder();
            Base.Vec2 curPixel = new Base.Vec2(x, y);
            foreach (Base.Vec2 gridPixel in PixelCheck.Select(p => curPixel + p))
            {
                if (gridPixel.Y < 0 || gridPixel.Y >= pixels.Count || gridPixel.X < 0 || gridPixel.X >= pixels[y].Length)
                {
                    sb.Append(defaultPixel);
                }
                else
                {
                    sb.Append(pixels[gridPixel.Y][gridPixel.X]);
                }
            }

            string binary = sb.Replace(DarkPixel, '0').Replace(LightPixel, '1').ToString();
            int key = Convert.ToInt32(binary, 2);
            return algorithm[key];
        }

        private string EnhancePixels(List<string> pixels, string algorithm, int y, char defaultPixel)
        {
            StringBuilder oldPixels = new StringBuilder(pixels[y]);
            StringBuilder newPixels = new StringBuilder(new string(DarkPixel, oldPixels.Length));
            for (int x = 0; x < pixels.First().Length; ++x)
            {
                newPixels[x] = EnhancePixel(pixels, algorithm, x, y, defaultPixel);
            }
            return newPixels.ToString();
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, int enhancementCount)
        {
            string algorithm = inputs.First();

            List<string> pixels = new List<string>();
            pixels.AddRange(inputs.Skip(2));

            char[] defaultPixels = new char[2] { DarkPixel, DarkPixel };
            if (algorithm[0] == LightPixel)
            {
                defaultPixels[0] = DarkPixel;
                defaultPixels[1] = LightPixel;
            }

            for (int i = 0; i < enhancementCount; ++i)
            {
                // expand
                int oldXSize = pixels.First().Length;
                int oldYSize = pixels.Count;
                for (int y = 0; y < oldYSize; ++y)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(defaultPixels[i % 2]);
                    sb.Append(pixels[y]);
                    sb.Append(defaultPixels[i % 2]);
                    pixels[y] = sb.ToString();
                }
                int newXSize = oldXSize + 2;
                int newYSize = oldYSize + 2;
                pixels.Insert(0, new string(defaultPixels[i % 2], newXSize));
                pixels.Add(new string(defaultPixels[i % 2], newXSize));

                // ehnance
                List<string> newPixels = new List<string>();
                for (int y = 0; y < newYSize; ++y)
                {
                    newPixels.Add(EnhancePixels(pixels, algorithm, y, defaultPixels[i % 2]));
                }
                pixels = newPixels;
            }
            return string.Join(string.Empty, pixels).Count(c => c == LightPixel).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 2);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, 50);
    }
}