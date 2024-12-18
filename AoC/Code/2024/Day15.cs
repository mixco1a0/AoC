using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Base;

namespace AoC._2024
{
    class Day15 : Core.Day
    {
        public Day15() { }

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
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "2028",
                    RawInput =
@"########
#..O.O.#
##@.O..#
#...O..#
#.#.O..#
#...O..#
#......#
########

<^^>>>vv
<v>>v<<"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "10092",
                    RawInput =
@"##########
#..O..O.O#
#......O.#
#.OO..O.O#
#..O@..O.#
#O#..O...#
#O..O..O.#
#.OO.O.OO#
#....O...#
##########

<vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
<<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
>^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
<><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "1",
                    RawInput =
@"#######
#...#.#
#.....#
#..OO@#
#..O..#
#.....#
#######

<vv<<^^<<^^"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "9021",
                    RawInput =
@"##########
#..O..O.O#
#......O.#
#.OO..O.O#
#..O@..O.#
#O#..O...#
#O..O..O.#
#.OO.O.OO#
#....O...#
##########

<vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
<<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
>^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
<><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^"
                },
            ];
            return testData;
        }

        private const char Wall = '#';
        private const char Box = 'O';
        private const char LeftBox = '[';
        private const char RightBox = ']';
        private const char Robot = '@';
        private const char Empty = '.';


        private static void PrintGrid(Grid2Char grid, Vec2 robot, HashSet<Vec2> boxes)
        {
            Grid2Char temp = new(grid);
            foreach (Base.Vec2 vec2 in grid)
            {
                if (vec2.Equals(robot))
                {
                    temp[vec2] = Robot;
                }
                else if (boxes.Contains(vec2))
                {
                    temp[vec2] = Box;
                }
            }
            temp.Print(Core.Log.ELevel.Spam);
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int maxInput = 0;
            for (int i = 0; i < inputs.Count; ++i)
            {
                maxInput = i;
                if (string.IsNullOrWhiteSpace(inputs[i]))
                {
                    break;
                }
            }

            Util.Grid2.Dir[] instructions = string.Join("", inputs.Skip(maxInput)).Select(i => Util.Grid2.Map.SimpleArrowFlipped[i]).ToArray();

            Base.Grid2Char grid = new(inputs.Take(maxInput).ToList());
            HashSet<Base.Vec2> walls = [];
            HashSet<Base.Vec2> boxes = [];
            Base.Vec2 robot = new();
            foreach (Base.Vec2 vec2 in grid)
            {
                switch (grid[vec2])
                {
                    case Wall:
                        walls.Add(vec2);
                        break;
                    case Box:
                        boxes.Add(vec2);
                        grid[vec2] = Empty;
                        break;
                    case Robot:
                        robot = vec2;
                        grid[vec2] = Empty;
                        break;
                }
            }
            // PrintGrid(grid, robot, boxes);

            foreach (Util.Grid2.Dir dir in instructions)
            {
                // Log($"Moving: {Util.Grid2.Map.Arrow[dir]}");
                Base.Vec2 next = robot + Util.Grid2.Map.Neighbor[dir];

                // no wall, can potentially move
                if (!walls.Contains(next))
                {
                    // check for boxes
                    if (boxes.Contains(next))
                    {
                        Base.Vec2 finalBox = next;
                        while (boxes.Contains(finalBox))
                        {
                            finalBox += Util.Grid2.Map.Neighbor[dir];
                        }

                        if (!walls.Contains(finalBox))
                        {
                            // slide first box into new location
                            boxes.Remove(next);
                            boxes.Add(finalBox);
                            robot = next;
                        }
                    }
                    // no box, just move
                    else
                    {
                        robot = next;
                    }
                }
                // PrintGrid(grid, robot, boxes);
            }

            return boxes.Select(b => 100 * b.Y + b.X).Sum().ToString();
        }

        private static void PrintBigGrid(Grid2Char grid, Vec2 robot, HashSet<Vec2> leftBoxes, HashSet<Vec2> rightBoxes)
        {
            Grid2Char temp = new(grid);
            foreach (Base.Vec2 vec2 in grid)
            {
                if (vec2.Equals(robot))
                {
                    temp[vec2] = Robot;
                }
                else if (leftBoxes.Contains(vec2))
                {
                    temp[vec2] = LeftBox;
                }
                else if (rightBoxes.Contains(vec2))
                {
                    temp[vec2] = RightBox;
                }
            }
            temp.Print(Core.Log.ELevel.Spam);
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int maxInput = 0;
            for (int i = 0; i < inputs.Count; ++i)
            {
                maxInput = i;
                if (string.IsNullOrWhiteSpace(inputs[i]))
                {
                    break;
                }
            }

            Util.Grid2.Dir[] instructions = string.Join("", inputs.Skip(maxInput)).Select(i => Util.Grid2.Map.SimpleArrowFlipped[i]).ToArray();
            Base.Grid2Char grid = new(inputs.Take(maxInput).ToList());
            HashSet<Base.Vec2> walls = [];
            HashSet<Base.Vec2> leftBoxes = [];
            HashSet<Base.Vec2> rightBoxes = [];
            Base.Vec2 robot = new();
            Base.Grid2Char bigGrid = new(grid.MaxCol * 2, grid.MaxRow, Empty);
            foreach (Base.Vec2 vec2 in grid)
            {
                Base.Vec2 left = new(vec2.X * 2, vec2.Y);
                Base.Vec2 right = new(vec2.X * 2 + 1, vec2.Y);
                switch (grid[vec2])
                {
                    case Wall:
                        walls.Add(left);
                        walls.Add(right);
                        bigGrid[left] = Wall;
                        bigGrid[right] = Wall;
                        break;
                    case Box:
                        leftBoxes.Add(left);
                        rightBoxes.Add(right);
                        break;
                    case Robot:
                        robot = left;
                        break;
                }
            }

            PrintBigGrid(bigGrid, robot, leftBoxes, rightBoxes);

            foreach (Util.Grid2.Dir dir in instructions)
            {
                bool isNS = dir == Util.Grid2.Dir.North || dir == Util.Grid2.Dir.South;
                Log($"Moving: {Util.Grid2.Map.Arrow[dir]}");
                Base.Vec2 next = robot + Util.Grid2.Map.Neighbor[dir];

                // no wall, can potentially move
                if (!walls.Contains(next))
                {
                    // check for boxes
                    if (isNS)
                    {

                    }
                    else
                    {
                        HashSet<Vec2> newLeft = [];
                        HashSet<Vec2> oldLeft = [];
                        HashSet<Vec2> newRight = [];
                        HashSet<Vec2> oldRight = [];
                        if (leftBoxes.Contains(next) || rightBoxes.Contains(next))
                        {
                            Base.Vec2 finalBox = next;
                            while (leftBoxes.Contains(finalBox) || rightBoxes.Contains(finalBox))
                            {
                                bool isLeft = leftBoxes.Contains(finalBox);
                                if (isLeft)
                                {
                                    oldLeft.Add(finalBox);
                                }
                                else
                                {
                                    oldRight.Add(finalBox);
                                }
                                finalBox += Util.Grid2.Map.Neighbor[dir];
                                if (isLeft)
                                {
                                    newLeft.Add(finalBox);
                                }
                                else
                                {
                                    newRight.Add(finalBox);
                                }

                            }

                            if (!walls.Contains(finalBox))
                            {
                                leftBoxes.RemoveWhere(oldLeft.Contains);
                                rightBoxes.RemoveWhere(oldRight.Contains);
                                foreach (Base.Vec2 left in newLeft)
                                {
                                    leftBoxes.Add(left);
                                }
                                foreach (Base.Vec2 right in newRight)
                                {
                                    rightBoxes.Add(right);
                                }
                                robot = next;
                            }
                        }
                        // no box, just move
                        else
                        {
                            robot = next;
                        }
                    }
                }
                PrintBigGrid(bigGrid, robot, leftBoxes, rightBoxes);
            }

            // return boxes.Select(b => 100 * b.Y + b.X).Sum().ToString();
            return string.Empty;
        }
    }
}