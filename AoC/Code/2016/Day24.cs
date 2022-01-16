using System;
using System.Collections.Generic;

using AoC.Core;

namespace AoC._2016
{
    class Day24 : Day
    {
        public Day24() { }

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
                Output = "14",
                RawInput =
@"###########
#0.1.....2#
#.#######.#
#4.......3#
###########"
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

        private char WallChar = '#';
        private char OpenChar = '.';
        private char StartChar = '0';

        private record Path(Base.Point Coords, int Steps) { }

        private int PathTo(char[][] grid, Base.Point max, Base.Point start, char target)
        {
            Queue<Path> pendingChecks = new Queue<Path>();
            pendingChecks.Enqueue(new Path(start, 0));

            HashSet<Base.Point> history = new HashSet<Base.Point>();
            history.Add(start);

            while (pendingChecks.Count > 0)
            {
                Path cur = pendingChecks.Dequeue();
                if (grid[cur.Coords.Y][cur.Coords.X] == target)
                {
                    return cur.Steps;
                }

                Base.Point[] movements = new Base.Point[] { new Base.Point(0, -1), new Base.Point(-1, 0), new Base.Point(0, 1), new Base.Point(1, 0) };
                foreach (Base.Point movement in movements)
                {
                    Base.Point nextMove = cur.Coords + movement;
                    if (nextMove.X >= 0 && nextMove.X < max.X && nextMove.Y >= 0 && nextMove.Y < max.Y)
                    {
                        if (grid[nextMove.Y][nextMove.X] == WallChar || history.Contains(nextMove))
                        {
                            continue;
                        }

                        pendingChecks.Enqueue(new Path(nextMove, cur.Steps + 1));
                        history.Add(nextMove);
                    }
                }
            }

            return (int)short.MaxValue;
        }

        private int[] GeneratePaths(char[][] grid, Base.Point max, char target, int totalTargets)
        {
            // get starting position
            Base.Point start = new Base.Point(-1, -1);
            for (int y = 0; y < max.Y && start.Y < 0; ++y)
            {
                for (int x = 0; x < max.X; ++x)
                {
                    if (grid[y][x] == target)
                    {
                        start = new Base.Point(x, y);
                        break;
                    }
                }
            }

            // get starting target to every other target
            int[] fromTargetTo = new int[totalTargets];
            for (int curTarget = 0; curTarget < totalTargets; ++curTarget)
            {
                char curTargetChar = curTarget.ToString()[0];
                if (curTargetChar == target)
                {
                    fromTargetTo[curTarget] = 0;
                    continue;
                }

                fromTargetTo[curTarget] = PathTo(grid, max, start, curTargetChar);
            }
            return fromTargetTo;
        }

        private int FindShortestPath(int[][] toAllTargets, int totalTargets, int prevTarget, string usedTargets, int pathLength, bool returnHome)
        {
            if (usedTargets.Length == totalTargets)
            {
                if (returnHome)
                {
                    return pathLength + toAllTargets[prevTarget][0];
                }
                else
                {
                    return pathLength;
                }
            }

            int shortestPath = int.MaxValue;
            for (int curTarget = 0; curTarget < totalTargets; ++curTarget)
            {
                char curTargetChar = curTarget.ToString()[0];
                if (usedTargets.Contains(curTargetChar))
                {
                    continue;
                }

                int newLength = pathLength + toAllTargets[prevTarget][curTarget];
                shortestPath = Math.Min(shortestPath, FindShortestPath(toAllTargets, totalTargets, curTarget, usedTargets + curTarget.ToString(), newLength, returnHome));
            }

            return shortestPath;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool returnHome)
        {
            Base.Point max = new Base.Point(inputs[0].Length, inputs.Count);
            char[][] grid = new char[inputs.Count][];
            Base.Point start = new Base.Point();
            for (int i = 0; i < inputs.Count; ++i)
            {
                grid[i] = inputs[i].ToCharArray();
                int idx = inputs[i].IndexOf(StartChar);
                if (idx >= 0)
                {
                    start.Y = i;
                    start.X = idx;
                }
            }

            string allTargets = string.Join(string.Empty, inputs).Replace($"{WallChar}", string.Empty).Replace($"{OpenChar}", string.Empty);
            int[][] toAllTargets = new int[allTargets.Length][];
            foreach (char target in allTargets)
            {
                toAllTargets[int.Parse($"{target}")] = GeneratePaths(grid, max, target, allTargets.Length);
            }
            return FindShortestPath(toAllTargets, allTargets.Length, 0, "0", 0, returnHome).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}