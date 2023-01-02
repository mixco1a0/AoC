using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day22 : Core.Day
    {
        public Day22() { }

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
                Output = "6032",
                RawInput =
@"        ...#
        .#..
        #...
        ....
...#.......#
........#...
..#....#....
..........#.
        ...#....
        .....#..
        .#......
        ......#.

4x4
0,0,1,0
2,3,4,0
0,0,5,6
0,0,0,0

1|T:2.T,L:3.T,R:6.R,B:4.T
2|T:1.T,L:6.B,R:3.L,B:5.B
3|T:1.L,L:2.R,R:4.L,B:5.L
4|T:1.B,L:3.R,R:6.T,B:5.T
5|T:4.B,L:3.B,R:6.L,B:2.B
6|T:4.R,L:5.R,R:1.R,B:2.L

10R5L5R10L4R5L5"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "5031",
                RawInput =
@"        ...#
        .#..
        #...
        ....
...#.......#
........#...
..#....#....
..........#.
        ...#....
        .....#..
        .#......
        ......#.

4x4
0,0,1,0
2,3,4,0
0,0,5,6
0,0,0,0

1|T:2.T,L:3.T,R:6.R,B:4.T
2|T:1.T,L:6.B,R:3.L,B:5.B
3|T:1.L,L:2.R,R:4.L,B:5.L
4|T:1.B,L:3.R,R:6.T,B:5.T
5|T:4.B,L:3.B,R:6.L,B:2.B
6|T:4.R,L:5.R,R:1.R,B:2.L

10R5L5R10L4R5L5"
            });
//             testData.Add(new Core.TestDatum
//             {
//                 TestPart = Core.Part.Two,
//                 Output = "5031",
//                 RawInput =
// @"        ....
//         .##.
//         .##.
//         ....
// ............
// .##..##..##.
// .##..##..##.
// ............
//         ........
//         .##..##.
//         .##.....
//         ......#.

// 4x4
// 0,0,1,0
// 2,3,4,0
// 0,0,5,6
// 0,0,0,0

// 1|T:2.T,L:3.T,R:6.R,B:4.T
// 2|T:1.T,L:6.B,R:3.L,B:5.B
// 3|T:1.L,L:2.R,R:4.L,B:5.L
// 4|T:1.B,L:3.R,R:6.T,B:5.T
// 5|T:4.B,L:3.B,R:6.L,B:2.B
// 6|T:4.R,L:5.R,R:1.R,B:2.L

// 10R5L5R10L4R5L5"
//             });
            return testData;
        }

        private static char NoneChar { get { return ' '; } }
        private static char WallChar { get { return '#'; } }
        private static char PathChar { get { return '.'; } }
        private static char WalkChar { get { return '@'; } }

        private int MaxX { get; set; }
        private int MaxY { get; set; }
        private int FaceX { get; set; }
        private int FaceY { get; set; }

        private class FaceConfig
        {
            public enum EDirection : int
            {
                Top,
                Left,
                Right,
                Bottom,
                Count
            }
            public enum ERotation : int
            {
                None,
                Left,
                Right,
                Twice,
                Count
            }

            public int Id { get; set; }
            public int[] DirectionIds { get; set; }
            public EDirection[] TargetSide { get; set; }

            public int RawX { get; set; }
            public int RawY { get; set; }

            public int MinX { get; set; }
            public int MaxX { get; set; }
            public int MinY { get; set; }
            public int MaxY { get; set; }

            public FaceConfig(int id, int rawX, int rawY)
            {
                Id = id;
                RawX = rawX;
                RawY = rawY;
                MinX = -1;
                MaxX = -1;
                MinY = -1;
                MaxY = -1;
                DirectionIds = new int[(int)EDirection.Count];
                TargetSide = new EDirection[(int)EDirection.Count];
            }

            public static EDirection GetDirection(char c)
            {
                switch (c)
                {
                    case 'T':
                        return EDirection.Top;
                    case 'L':
                        return EDirection.Left;
                    case 'R':
                        return EDirection.Right;
                    case 'B':
                        return EDirection.Bottom;
                }
                return EDirection.Count;
            }
        }

        private void Parse(List<string> inputs, out char[,] grid, out Dictionary<int, FaceConfig> faceConfigs, out List<string> instructions)
        {
            MaxY = inputs.Select((value, index) => (value, index)).FirstOrDefault(pair => string.IsNullOrWhiteSpace(pair.value)).index;
            //maxY = inputs.Count - 2;
            MaxX = inputs.Select(i => i.Length).Max();
            grid = new char[MaxX, MaxY];
            for (int y = 0; y < MaxY; ++y)
            {
                for (int x = 0; x < MaxX; ++x)
                {
                    if (x >= inputs[y].Length)
                    {
                        grid[x, y] = NoneChar;
                    }
                    else
                    {
                        grid[x, y] = inputs[y][x];
                    }
                }
            }

            // parse out special instructions
            // cube face size
            string sideSize = inputs[MaxY + 1];
            string[] splitSide = sideSize.Split('x', StringSplitOptions.RemoveEmptyEntries);
            FaceX = int.Parse(splitSide[0]);
            FaceY = int.Parse(splitSide[1]);

            // cube face ids
            List<List<int>> locations = new List<List<int>>();
            int index = 0;
            for (int i = MaxY + 2; i < inputs.Count; ++i)
            {
                if (string.IsNullOrWhiteSpace(inputs[i]))
                {
                    break;
                }

                locations.Add(new List<int>());
                string[] split = inputs[i].Split(',', StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < split.Length; ++j)
                {
                    locations[index].Add(int.Parse(split[j]));
                }
                ++index;
            }

            // cube face translations
            faceConfigs = new Dictionary<int, FaceConfig>();
            for (int i = 1; i <= 6; ++i)
            {
                int curX = -1;
                int curY = -1;
                for (int y = 0; y < locations.Count; ++y)
                {
                    for (int x = 0; x < locations[y].Count; ++x)
                    {
                        if (locations[y][x] == i)
                        {
                            curX = x;
                            curY = y;
                            break;
                        }
                    }
                }

                FaceConfig curConfig = new FaceConfig(i, curX, curY);
                curConfig.MinX = curX * FaceX;
                curConfig.MaxX = (curX + 1) * FaceX - 1;
                curConfig.MinY = curY * FaceY;
                curConfig.MaxY = (curY + 1) * FaceY - 1;

                string faceDirections = inputs.Where(input => input.StartsWith($"{i}|")).First();
                string[] split = faceDirections.Split("|:,.".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                curConfig.DirectionIds[(int)FaceConfig.EDirection.Top] = int.Parse(split[2]);
                curConfig.TargetSide[(int)FaceConfig.EDirection.Top] = FaceConfig.GetDirection(split[3][0]);
                curConfig.DirectionIds[(int)FaceConfig.EDirection.Left] = int.Parse(split[5]);
                curConfig.TargetSide[(int)FaceConfig.EDirection.Left] = FaceConfig.GetDirection(split[6][0]);
                curConfig.DirectionIds[(int)FaceConfig.EDirection.Right] = int.Parse(split[8]);
                curConfig.TargetSide[(int)FaceConfig.EDirection.Right] = FaceConfig.GetDirection(split[9][0]);
                curConfig.DirectionIds[(int)FaceConfig.EDirection.Bottom] = int.Parse(split[11]);
                curConfig.TargetSide[(int)FaceConfig.EDirection.Bottom] = FaceConfig.GetDirection(split[12][0]);
                faceConfigs[i] = curConfig;
            }

            // // populate missing faces
            // for (int i = 1; i <= 6; ++i)
            // {
            //     SolveFace(i, ref faceConfigs, locations);
            // }

            instructions = new List<string>();
            string completeInstruction = inputs.Last();
            string[] numbers = completeInstruction.Split("LR".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[] directions = completeInstruction.Split("0123456789".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < numbers.Length || i < directions.Length; ++i)
            {
                if (i < numbers.Length)
                {
                    instructions.Add(numbers[i]);
                }
                if (i < directions.Length)
                {
                    instructions.Add(directions[i]);
                }
            }
        }

        private class GridState
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Face { get; set; }
            Dictionary<int, FaceConfig> FaceConfigs { get; set; }

            // 0 -> R
            // 1 -> U
            // 2 -> L
            // 3 -> D
            public const int DRight = 0;
            public const int DUp = 1;
            public const int DLeft = 2;
            public const int DDown = 3;
            public int Direction { get; set; }
            private int MaxDirection { get { return 4; } }

            public int RealX { get { return X + FaceConfigs[Face].MinX; } }
            public int RealY { get { return Y + FaceConfigs[Face].MinY; } }
            public int GetX(int localX)
            {
                return localX + FaceConfigs[Face].MinX;
            }
            public int GetY(int localY)
            {
                return localY + FaceConfigs[Face].MinY;
            }

            public GridState(Dictionary<int, FaceConfig> faceConfigs)
            {
                FaceConfigs = faceConfigs;
                X = 0;
                Y = 0;
                Face = 1;
                Direction = 0;
            }

            public void Move(char[,] grid, int maxX, int maxY, string instruction)
            {
                if (int.TryParse(instruction, out int steps))
                {
                    int direction = DUp;
                    if (Direction == DDown || Direction == DLeft)
                    {
                        direction = -1;
                    }

                    if (Direction == DRight || Direction == DLeft)
                    {
                        // side ways
                        for (int x = 0; x < steps; ++x)
                        {
                            int curX = X + direction;
                            if (curX < 0 || curX >= maxX || grid[curX, Y] == NoneChar)
                            {
                                // wrap around
                                if (curX < 0)
                                {
                                    curX = maxX - 1;
                                }
                                else if (curX >= maxX)
                                {
                                    curX = 0;
                                }

                                while (grid[curX, Y] == NoneChar)
                                {
                                    curX += direction;
                                    if (curX < 0)
                                    {
                                        curX = maxX - 1;
                                    }
                                    else if (curX >= maxX)
                                    {
                                        curX = 0;
                                    }
                                }
                                if (grid[curX, Y] == WallChar)
                                {
                                    break;
                                }
                                X = curX;
                            }
                            else if (grid[curX, Y] == WallChar)
                            {
                                break;
                            }
                            else if (grid[curX, Y] == PathChar)
                            {
                                X = curX;
                            }
                        }
                    }
                    else
                    {
                        // up down
                        for (int y = 0; y < steps; ++y)
                        {
                            int curY = Y + direction;
                            if (curY < 0 || curY >= maxY || grid[X, curY] == NoneChar)
                            {
                                // wrap around
                                if (curY < 0)
                                {
                                    curY = maxY - 1;
                                }
                                else if (curY >= maxY)
                                {
                                    curY = 0;
                                }

                                while (grid[X, curY] == NoneChar)
                                {
                                    curY += direction;
                                    if (curY < 0)
                                    {
                                        curY = maxY - 1;
                                    }
                                    else if (curY >= maxY)
                                    {
                                        curY = 0;
                                    }
                                }
                                if (grid[X, curY] == WallChar)
                                {
                                    break;
                                }
                                Y = curY;
                            }
                            else if (grid[X, curY] == WallChar)
                            {
                                break;
                            }
                            else if (grid[X, curY] == PathChar)
                            {
                                Y = curY;
                            }
                        }
                    }
                }
                else
                {
                    if (instruction[0] == 'R')
                    {
                        Direction = (Direction + 1) % MaxDirection;
                    }
                    else
                    {
                        Direction = (Direction + MaxDirection - 1) % MaxDirection;
                    }
                }
            }

            public string MoveCube(char[,] grid, int maxX, int maxY, string instruction)
            {
                int maxXVal = maxX - 1;
                int maxYVal = maxY - 1;
                if (int.TryParse(instruction, out int steps))
                {
                    int direction = 1;
                    if (Direction == DUp || Direction == DLeft)
                    {
                        direction = -1;
                    }
                    else if (Direction == DDown || Direction == DRight)
                    {
                        direction = 1;
                    }

                    if (Direction == DRight || Direction == DLeft)
                    {
                        // side ways
                        for (int x = 0; x < steps; ++x)
                        {
                            int curX = X + direction;
                            if (curX < 0 || curX >= maxX)
                            {
                                // move to the next face
                                curX = (curX + maxX) % maxX;

                                // need to swap to a different face
                                int targetFace = 0;
                                FaceConfig.EDirection targetDir;
                                if (Direction == DRight)
                                {
                                    targetFace = FaceConfigs[Face].DirectionIds[(int)FaceConfig.EDirection.Right];
                                    targetDir = FaceConfigs[Face].TargetSide[(int)FaceConfig.EDirection.Right];
                                }
                                else
                                {
                                    targetFace = FaceConfigs[Face].DirectionIds[(int)FaceConfig.EDirection.Left];
                                    targetDir = FaceConfigs[Face].TargetSide[(int)FaceConfig.EDirection.Left];
                                }

                                int tempX = X;
                                int tempY = Y;
                                int tempD = Direction;
                                int tempF = Face;

                                // perform rotations
                                switch (targetDir)
                                {
                                    case FaceConfig.EDirection.Top:
                                        if (Direction == DRight)
                                        {
                                            X = maxYVal - Y;
                                            Y = curX;
                                        }
                                        else
                                        {
                                            X = Y;
                                            Y = maxXVal - curX;
                                        }
                                        Direction = DDown;
                                        break;
                                    case FaceConfig.EDirection.Left:
                                        if (Direction == DRight)
                                        {
                                            X = curX;
                                        }
                                        else
                                        {
                                            X = maxXVal - curX;
                                            Y = maxYVal - Y;
                                        }
                                        Direction = DRight;
                                        break;
                                    case FaceConfig.EDirection.Right:
                                        if (Direction == DRight)
                                        {
                                            X = maxXVal - curX;
                                            Y = maxYVal - Y;
                                        }
                                        else
                                        {
                                            X = curX;
                                        }
                                        Direction = DLeft;
                                        break;
                                    case FaceConfig.EDirection.Bottom:
                                        if (Direction == DRight)
                                        {
                                            X = Y;
                                            Y = maxXVal - curX;
                                        }
                                        else
                                        {
                                            X = maxYVal - Y;
                                            Y = curX;
                                        }
                                        Direction = DUp;
                                        break;
                                }

                                Face = targetFace;
                                if (grid[RealX, RealY] == WallChar)
                                {
                                    X = tempX;
                                    Y = tempY;
                                    Direction = tempD;
                                    Face = tempF;
                                    break;
                                }

                                return (steps - x - 1).ToString();
                            }
                            else if (grid[GetX(curX), RealY] == WallChar)
                            {
                                break;
                            }
                            else if (grid[GetX(curX), RealY] == PathChar)
                            {
                                X = curX;
                            }
                        }
                    }
                    else
                    {
                        // up down
                        for (int y = 0; y < steps; ++y)
                        {
                            int curY = Y + direction;
                            if (curY < 0 || curY >= maxY)
                            {
                                // move to the next face
                                curY = (curY + maxY) % maxY;

                                // need to swap to a different face
                                int targetFace = 0;
                                FaceConfig.EDirection targetDir;
                                if (Direction == DUp)
                                {
                                    targetFace = FaceConfigs[Face].DirectionIds[(int)FaceConfig.EDirection.Top];
                                    targetDir = FaceConfigs[Face].TargetSide[(int)FaceConfig.EDirection.Top];
                                }
                                else
                                {
                                    targetFace = FaceConfigs[Face].DirectionIds[(int)FaceConfig.EDirection.Bottom];
                                    targetDir = FaceConfigs[Face].TargetSide[(int)FaceConfig.EDirection.Bottom];
                                }

                                int tempX = X;
                                int tempY = Y;
                                int tempD = Direction;
                                int tempF = Face;

                                // perform rotations
                                switch (targetDir)
                                {
                                    case FaceConfig.EDirection.Top:
                                        if (Direction == DUp)
                                        {
                                            Y = maxYVal - curY;
                                            X = maxXVal - X;
                                        }
                                        else
                                        {
                                            Y = curY;
                                        }
                                        Direction = DDown;
                                        break;
                                    case FaceConfig.EDirection.Left:
                                        if (Direction == DUp)
                                        {
                                            Y = X;
                                            X = maxYVal - curY;
                                        }
                                        else
                                        {
                                            Y = maxYVal - X;
                                            X = curY;
                                        }
                                        Direction = DRight;
                                        break;
                                    case FaceConfig.EDirection.Right:
                                        if (Direction == DUp)
                                        {
                                            Y = maxYVal - X;
                                            X = curY;
                                        }
                                        else
                                        {
                                            Y = X;
                                            X = maxYVal - curY;
                                        }
                                        Direction = DLeft;
                                        break;
                                    case FaceConfig.EDirection.Bottom:
                                        if (Direction == DUp)
                                        {
                                            Y = curY;
                                        }
                                        else
                                        {
                                            Y = maxYVal - curY;
                                            X = maxXVal - X;
                                        }
                                        Direction = DUp;
                                        break;
                                }

                                Face = targetFace;
                                if (grid[RealX, RealY] == WallChar)
                                {
                                    X = tempX;
                                    Y = tempY;
                                    Direction = tempD;
                                    Face = tempF;
                                    break;
                                }

                                return (steps - y - 1).ToString();
                            }
                            else if (grid[RealX, GetY(curY)] == WallChar)
                            {
                                break;
                            }
                            else if (grid[RealX, GetY(curY)] == PathChar)
                            {
                                Y = curY;
                            }
                        }
                    }
                }
                else
                {
                    if (instruction[0] == 'R')
                    {
                        switch (Direction)
                        {
                            case DRight:
                                Direction = DDown;
                                break;
                            case DUp:
                                Direction = DRight;
                                break;
                            case DLeft:
                                Direction = DUp;
                                break;
                            case DDown:
                                Direction = DLeft;
                                break;
                        }
                        //Direction = (Direction + 1) % MaxDirection;
                    }
                    else
                    {
                        switch (Direction)
                        {
                            case DRight:
                                Direction = DUp;
                                break;
                            case DUp:
                                Direction = DLeft;
                                break;
                            case DLeft:
                                Direction = DDown;
                                break;
                            case DDown:
                                Direction = DRight;
                                break;
                        }
                        //Direction = (Direction + MaxDirection - 1) % MaxDirection;
                    }
                }

                return string.Empty;
            }

            public int GetPassword(bool useRealValues)
            {
                if (useRealValues)
                {
                    int val = ((RealY + 1) * 1000) + ((RealX + 1) * 4);
                    if (Direction == DDown)
                    {
                        val += 1;
                    }
                    else if (Direction == DLeft)
                    {
                        val += 2;
                    }
                    else if (Direction == DUp)
                    {
                        val += 3;
                    }
                    return val;
                }
                return ((Y + 1) * 1000) + ((X + 1) * 4) + Direction;
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool traverseCube)
        {
            Parse(inputs, out char[,] grid, out Dictionary<int, FaceConfig> faceConfigs, out List<string> instructions);
            char[,] printGrid = new char[MaxX, MaxY];
            for (int x = 0; x < MaxX; ++x)
            {
                for (int y = 0; y < MaxY; ++y)
                {
                    printGrid[x, y] = grid[x, y];
                }
            }

            GridState gridState = new GridState(faceConfigs);
            int testX = inputs.First().IndexOfAny(new char[] { '.', '#' });
            gridState.Face = 1;
            printGrid[gridState.RealX, gridState.RealY] = WalkChar;
            //Util.Grid.PrintGrid(printGrid, Core.Log.ELevel.Debug);
            instructions.Reverse();
            Stack<string> instructionSet = new Stack<string>(instructions);
            while (instructionSet.Count > 0)
            {
                string instruction = instructionSet.Pop();
                if (traverseCube)
                {
                    if (char.IsDigit(instruction[0]))
                    {
                        switch (gridState.Direction)
                        {
                            case 0:
                                printGrid[gridState.RealX, gridState.RealY] = '>';
                                break;
                            case 1:
                                printGrid[gridState.RealX, gridState.RealY] = '^';
                                break;
                            case 2:
                                printGrid[gridState.RealX, gridState.RealY] = '<';
                                break;
                            case 3:
                                printGrid[gridState.RealX, gridState.RealY] = 'v';
                                break;
                        }
                    }
                    // DebugWriteLine($"Running... {instruction}");

                    string additionalSteps = gridState.MoveCube(grid, FaceX, FaceY, instruction);
                    if (!string.IsNullOrWhiteSpace(additionalSteps))
                    {
                        instructionSet.Push(additionalSteps);
                    }
                    printGrid[gridState.RealX, gridState.RealY] = WalkChar;
                    // Util.Grid.PrintGrid(printGrid, Core.Log.ELevel.Debug);
                }
                else
                {
                    printGrid[gridState.X, gridState.Y] = WalkChar;
                    gridState.Move(grid, MaxX, MaxY, instruction);
                }
            }

            if (traverseCube)
            {
                // Util.Grid.PrintGrid(printGrid, Core.Log.ELevel.Debug);
            }
            return gridState.GetPassword(traverseCube).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
            // 148185 [TOO HIGH]
    }
}

// TODO: parse the actual cube without help?

// private void SolveFace(int face, ref Dictionary<int, FaceConfig> faceConfigs, List<List<int>> locations)
// {
//     FaceConfig curFace = faceConfigs[face];
//     FaceConfig.EDirection curDirection = FaceConfig.EDirection.Top;
//     while (curDirection != FaceConfig.EDirection.Count)
//     {
//         if (curFace.DirectionIds[(int)curDirection] != 0)
//         {
//             ++curDirection;
//             continue;
//         }

//         switch (curDirection)
//         {
//             case FaceConfig.EDirection.Top:
//                 SolveFaceTop(ref curFace, faceConfigs, locations);
//                 break;
//             case FaceConfig.EDirection.Left:
//                 break;
//             case FaceConfig.EDirection.Right:
//                 break;
//             case FaceConfig.EDirection.Bottom:
//                 break;
//         }
//     }
// }

// static readonly Dictionary<FaceConfig.EDirection, List<NeighborFinder>> NeighborMap = new Dictionary<FaceConfig.EDirection, List<NeighborFinder>>()
// {
//     {FaceConfig.EDirection.Top, new List<NeighborFinder>()
//         {
//             new NeighborFinder( 0, -1, FaceConfig.ERotation.None),
//             new NeighborFinder(-1, -1, FaceConfig.ERotation.Left),
//             new NeighborFinder( 1, -1, FaceConfig.ERotation.Right),
//             new NeighborFinder(-2,  1, FaceConfig.ERotation.Twice),
//             new NeighborFinder( 2,  1, FaceConfig.ERotation.Twice)
//         }
//     },
//     {FaceConfig.EDirection.Bottom, new List<NeighborFinder>()
//         {
//             new NeighborFinder( 0,  1, FaceConfig.ERotation.None),
//             new NeighborFinder(-1,  1, FaceConfig.ERotation.Left),
//             new NeighborFinder( 1,  1, FaceConfig.ERotation.Right),
//             new NeighborFinder(-2, -1, FaceConfig.ERotation.Twice),
//             new NeighborFinder( 2, -1, FaceConfig.ERotation.Twice)
//         }
//     },
//     {FaceConfig.EDirection.Left, new List<NeighborFinder>()
//         {
//             new NeighborFinder(-1,  0, FaceConfig.ERotation.None),
//             new NeighborFinder(-1,  1, FaceConfig.ERotation.Left),
//             new NeighborFinder(-1, -1, FaceConfig.ERotation.Right),
//             new NeighborFinder( 1, -2, FaceConfig.ERotation.Twice),
//             new NeighborFinder( 1,  2, FaceConfig.ERotation.Twice)
//         }
//     },
//     {FaceConfig.EDirection.Right, new List<NeighborFinder>()
//         {
//             new NeighborFinder( 1,  0, FaceConfig.ERotation.None),
//             new NeighborFinder( 1, -1, FaceConfig.ERotation.Left),
//             new NeighborFinder( 1,  1, FaceConfig.ERotation.Right),
//             new NeighborFinder(-1, -2, FaceConfig.ERotation.Twice),
//             new NeighborFinder(-1,  2, FaceConfig.ERotation.Twice)
//         }
//     },
// };

// private record NeighborFinder(int XOffset, int YOffset, FaceConfig.ERotation Rotation);

// private void SolveFaceTop(ref FaceConfig curFace, Dictionary<int, FaceConfig> faceConfigs, List<List<int>> locations)
// {
//     // not directly above, try top neighbors

//     // up, left
//     // X--
//     // -@-
//     int tX = (curFace.RawX - 1 + locations[0].Count) % locations[0].Count;
//     int tY = (curFace.RawY - 1 + locations.Count) % locations.Count;
//     if (locations[tY][tX] != 0)
//     {
//         curFace.DirectionIds[(int)FaceConfig.EDirection.Top] = locations[tY][tX];
//         curFace.TargetSide[(int)FaceConfig.EDirection.Top] = FaceConfig.ERotation.Left;
//         return;
//     }

//     // up, right
//     // --X
//     // -@-
//     tX = (curFace.RawX + 1 + locations[0].Count) % locations[0].Count;
//     if (locations[tY][tX] != 0)
//     {
//         curFace.DirectionIds[(int)FaceConfig.EDirection.Top] = locations[tY][tX];
//         curFace.TargetSide[(int)FaceConfig.EDirection.Top] = FaceConfig.ERotation.Right;
//         return;
//     }

//     // try bottom far neighbors

//     // down, left two
//     // --@-
//     // X---
//     tX = (curFace.RawX - 2 + locations[0].Count) % locations[0].Count;
//     tY = (curFace.RawY + 1 + locations.Count) % locations.Count;
//     if (locations[tY][tX] != 0)
//     {
//         curFace.DirectionIds[(int)FaceConfig.EDirection.Top] = locations[tY][tX];
//         curFace.TargetSide[(int)FaceConfig.EDirection.Top] = FaceConfig.ERotation.Twice;
//         return;
//     }

//     // down, right two
//     // --@-
//     // X---
//     tX = (curFace.RawX + 2 + locations[0].Count) % locations[0].Count;
//     if (locations[tY][tX] != 0)
//     {
//         curFace.DirectionIds[(int)FaceConfig.EDirection.Top] = locations[tY][tX];
//         curFace.TargetSide[(int)FaceConfig.EDirection.Top] = FaceConfig.ERotation.Twice;
//         return;
//     }

//     DebugWriteLine($"Couldn't solve TOP for {curFace.Id}!");
// }