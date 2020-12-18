using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
{
    class Day17 : Day
    {
        public Day17() { }
        public override string GetSolutionVersion(TestPart testPart)
        {
            switch (testPart)
            {
                case TestPart.One:
                    return "v1";
                case TestPart.Two:
                    return "v1";
                default:
                    return base.GetSolutionVersion(testPart);
            }
        }
        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = TestPart.One,
                Output = "112",
                RawInput =
@".#.
..#
###"
            });
            testData.Add(new TestDatum
            {
                TestPart = TestPart.Two,
                Output = "848",
                RawInput =
@".#.
..#
###"
            });
            return testData;
        }

        private void Print(List<List<List<char>>> matrix)
        {
            // DebugWriteLine("Print() - START");
            for (int i = 0; i < matrix.Count; ++i)
            {
                Console.WriteLine($"Layer {i}");
                for (int j = 0; j < matrix[i].Count; ++j)
                {
                    for (int k = 0; k < matrix[i][j].Count; ++k)
                    {
                        Console.Write(matrix[i][j][k]);
                    }
                    Console.WriteLine("");
                }
            }
            // DebugWriteLine("Print() - END");
        }

        private List<List<List<char>>> Grow(List<List<List<char>>> matrix)
        {
            List<List<List<char>>> newMatrix = new List<List<List<char>>>();
            int zCount = matrix.Count + 2;
            int xCount = matrix[0].Count + 2;
            int yCount = matrix[0][0].Count + 2;

            // grow
            for (int z = 0; z < zCount; ++z)
            {
                newMatrix.Add(new List<List<char>>());
                for (int x = 0; x < xCount; ++x)
                {
                    newMatrix[z].Add(new List<char>());
                    for (int y = 0; y < yCount; ++y)
                    {
                        newMatrix[z][x].Add('.');
                    }
                }
            }

            // copy
            for (int z = 1; z < zCount - 1; ++z)
            {
                for (int x = 1; x < xCount - 1; ++x)
                {
                    for (int y = 1; y < yCount - 1; ++y)
                    {
                        newMatrix[z][x][y] = matrix[z - 1][x - 1][y - 1];
                    }
                }
            }

            return newMatrix;
        }


        private List<List<List<char>>> Update(List<List<List<char>>> matrix)
        {
            // deep copy
            List<List<List<char>>> newMatrix = new List<List<List<char>>>();
            for (int z = 0; z < matrix.Count; ++z)
            {
                newMatrix.Add(new List<List<char>>());
                for (int x = 0; x < matrix[0].Count; ++x)
                {
                    newMatrix[z].Add(new List<char>());
                    for (int y = 0; y < matrix[0][0].Count; ++y)
                    {
                        newMatrix[z][x].Add(' ');
                        newMatrix[z][x][y] = GetCubeInfo(z, x, y, matrix);
                    }
                }
            }
            return newMatrix;
        }

        private char GetCubeInfo(int zIdx, int xIdx, int yIdx, List<List<List<char>>> matrix)
        {
            int activeCount = 0;
            for (int z = zIdx - 1; z <= zIdx + 1; ++z)
            {
                // Debug($"Checking matrix[{z}]");
                if (z < 0 || z >= matrix.Count)
                {
                    continue;
                }

                for (int x = xIdx - 1; x <= xIdx + 1; ++x)
                {
                    // Debug($"Checking matrix[{z}][{x}]");
                    if (x < 0 || x >= matrix[z].Count)
                    {
                        continue;
                    }

                    for (int y = yIdx - 1; y <= yIdx + 1; ++y)
                    {
                        // Debug($"Checking matrix[{z}][{x}][{y}]");
                        if (z == zIdx && x == xIdx && y == yIdx)
                        {
                            continue;
                        }

                        if (y < 0 || y >= matrix[z][x].Count)
                        {
                            continue;
                        }

                        if (matrix[z][x][y] == '#')
                        {
                            ++activeCount;
                        }
                    }
                }
            }

            switch (matrix[zIdx][xIdx][yIdx])
            {
                case '.':
                    return activeCount == 3 ? '#' : '.';
                case '#':
                    return activeCount >= 2 && activeCount <= 3 ? '#' : '.';
            }

            return '!';
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<List<List<char>>> matrix = new List<List<List<char>>>();
            int originalSize = inputs.Count;
            matrix.Add(new List<List<char>>());
            for (int i = 0; i < originalSize; ++i)
            {
                matrix[0].Add(new List<char>());
                for (int j = 0; j < originalSize; ++j)
                {
                    matrix[0][i].Add(inputs[i].ElementAt(j));
                }
            }

            // Print(matrix);
            for (int i = 0; i < 6; ++i)
            {
                matrix = Grow(matrix);
                matrix = Update(matrix);
                // Print(matrix);
            }

            return string.Join("", matrix.Select(x => string.Join("", x.Select(y => string.Join("", y))))).Replace(".", "").Count().ToString();
        }



        private List<List<List<List<char>>>> Grow(List<List<List<List<char>>>> matrix)
        {
            List<List<List<List<char>>>> newMatrix = new List<List<List<List<char>>>>();
            int wCount = matrix.Count + 2;
            int zCount = matrix[0].Count + 2;
            int xCount = matrix[0][0].Count + 2;
            int yCount = matrix[0][0][0].Count + 2;

            // grow
            for (int w = 0; w < wCount; ++w)
            {
                newMatrix.Add(new List<List<List<char>>>());
                for (int z = 0; z < zCount; ++z)
                {
                    newMatrix[w].Add(new List<List<char>>());
                    for (int x = 0; x < xCount; ++x)
                    {
                        newMatrix[w][z].Add(new List<char>());
                        for (int y = 0; y < yCount; ++y)
                        {
                            newMatrix[w][z][x].Add('.');
                        }
                    }
                }
            }

            // copy
            for (int w = 1; w < wCount - 1; ++w)
            {
                for (int z = 1; z < zCount - 1; ++z)
                {
                    for (int x = 1; x < xCount - 1; ++x)
                    {
                        for (int y = 1; y < yCount - 1; ++y)
                        {
                            newMatrix[w][z][x][y] = matrix[w - 1][z - 1][x - 1][y - 1];
                        }
                    }
                }
            }

            return newMatrix;
        }


        private List<List<List<List<char>>>> Update(List<List<List<List<char>>>> matrix)
        {
            // deep copy
            List<List<List<List<char>>>> newMatrix = new List<List<List<List<char>>>>();
            for (int w = 0; w < matrix.Count; ++w)
            {
                newMatrix.Add(new List<List<List<char>>>());
                for (int z = 0; z < matrix[w].Count; ++z)
                {
                    newMatrix[w].Add(new List<List<char>>());
                    for (int x = 0; x < matrix[w][z].Count; ++x)
                    {
                        newMatrix[w][z].Add(new List<char>());
                        for (int y = 0; y < matrix[w][z][x].Count; ++y)
                        {
                            newMatrix[w][z][x].Add(' ');
                            newMatrix[w][z][x][y] = GetHyperCubeInfo(w, z, x, y, matrix);
                        }
                    }
                }
            }
            return newMatrix;
        }

        private char GetHyperCubeInfo(int wIdx, int zIdx, int xIdx, int yIdx, List<List<List<List<char>>>> matrix)
        {
            int activeCount = 0;
            for (int w = wIdx - 1; w <= wIdx + 1; ++w)
            {
                if (w < 0 || w >= matrix.Count)
                {
                    continue;
                }

                for (int z = zIdx - 1; z <= zIdx + 1; ++z)
                {
                    // Debug($"Checking matrix[{z}]");
                    if (z < 0 || z >= matrix[w].Count)
                    {
                        continue;
                    }

                    for (int x = xIdx - 1; x <= xIdx + 1; ++x)
                    {
                        // Debug($"Checking matrix[{z}][{x}]");
                        if (x < 0 || x >= matrix[w][z].Count)
                        {
                            continue;
                        }

                        for (int y = yIdx - 1; y <= yIdx + 1; ++y)
                        {
                            // Debug($"Checking matrix[{z}][{x}][{y}]");
                            if (w == wIdx && z == zIdx && x == xIdx && y == yIdx)
                            {
                                continue;
                            }

                            if (y < 0 || y >= matrix[w][z][x].Count)
                            {
                                continue;
                            }

                            if (matrix[w][z][x][y] == '#')
                            {
                                ++activeCount;
                            }
                        }
                    }
                }
            }

            switch (matrix[wIdx][zIdx][xIdx][yIdx])
            {
                case '.':
                    return activeCount == 3 ? '#' : '.';
                case '#':
                    return activeCount >= 2 && activeCount <= 3 ? '#' : '.';
            }

            return '!';
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<List<List<List<char>>>> matrix = new List<List<List<List<char>>>>();
            int originalSize = inputs.Count;
            matrix.Add(new List<List<List<char>>>());
            matrix[0].Add(new List<List<char>>());
            for (int i = 0; i < originalSize; ++i)
            {
                matrix[0][0].Add(new List<char>());
                for (int j = 0; j < originalSize; ++j)
                {
                    matrix[0][0][i].Add(inputs[i].ElementAt(j));
                }
            }

            // Print(matrix);
            for (int i = 0; i < 6; ++i)
            {
                matrix = Grow(matrix);
                matrix = Update(matrix);
                // Print(matrix);
            }

            return string.Join("", matrix.Select(z => string.Join("", z.Select(x => string.Join("", x.Select(y => string.Join("", y))))))).Replace(".", "").Count().ToString();
        }


        private void Print(List<List<List<List<char>>>> matrix)
        {
            // DebugWriteLine("Print() - START");
            for (int w = 0; w < matrix.Count; ++w)
            {
                for (int z = 0; z < matrix[w].Count; ++z)
                {
                    Console.WriteLine($"Layer {w}.{z}");
                    for (int x = 0; x < matrix[w][z].Count; ++x)
                    {
                        for (int y = 0; y < matrix[w][z][x].Count; ++y)
                        {
                            Console.Write(matrix[w][z][x][y]);
                        }
                        Console.WriteLine("");
                    }
                }
            }
            // DebugWriteLine("Print() - END");
        }
    }
}