using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2024
{
    class Day15 : Core.Day
    {
        public Day15() { }

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
//                 new Core.TestDatum
//                 {
//                     TestPart = Core.Part.Two,
//                     Output = "1",
//                     RawInput =
// @"#######
// #...#.#
// #.....#
// #..OO@#
// #..O..#
// #.....#
// #######

// <vv<<^^<<^^"
//                 },
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


        private static void PrintGrid(Base.Grid2Char grid, Base.Vec2 robot, HashSet<Base.Vec2> boxes)
        {
            Base.Grid2Char temp = new(grid);
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

        private record BigBox(Base.Vec2 Left, Base.Vec2 Right)
        {
            public bool CanMove(Util.Grid2.Dir dir, HashSet<Base.Vec2> walls, HashSet<BigBox> boxes, ref HashSet<Base.Vec2> movedBoxes)
            {
                // only keep track of the left side
                movedBoxes.Add(Left);

                bool canMove = true;
                Base.Vec2 lNext = Left + Util.Grid2.Map.Neighbor[dir];
                Base.Vec2 rNext = Right + Util.Grid2.Map.Neighbor[dir];
                if (dir == Util.Grid2.Dir.East)
                {
                    // right is a wall
                    if (walls.Contains(rNext))
                    {
                        return false;
                    }

                    // check next box to the right
                    IEnumerable<BigBox> nextBoxes = boxes.Where(b => b.Left.Equals(rNext));
                    if (nextBoxes.Any())
                    {
                        canMove = nextBoxes.First().CanMove(dir, walls, boxes, ref movedBoxes);
                    }
                }
                else if (dir == Util.Grid2.Dir.West)
                {
                    // left is a wall
                    if (walls.Contains(lNext))
                    {
                        return false;
                    }

                    // check next box to the left
                    IEnumerable<BigBox> nextBoxes = boxes.Where(b => b.Right.Equals(lNext));
                    if (nextBoxes.Any())
                    {
                        canMove = nextBoxes.First().CanMove(dir, walls, boxes, ref movedBoxes);
                    }
                }
                else
                {
                    // either a wall
                    if (walls.Contains(lNext) || walls.Contains(rNext))
                    {
                        return false;
                    }

                    IEnumerable<BigBox> nextBoxes = boxes.Where(b => b.Left.Equals(lNext) || b.Right.Equals(lNext) || b.Left.Equals(rNext) || b.Right.Equals(rNext));
                    foreach (BigBox bigBox in nextBoxes)
                    {
                        canMove &= bigBox.CanMove(dir, walls, boxes, ref movedBoxes);
                    }
                    // if (nextBoxes.Any())
                    // {
                    //     BigBox nextBigBox = nextBoxes.First();
                    //     canMove = nextBoxes.First().CanMove(dir, walls, boxes, ref movedBoxes);

                    //     if (canMove)
                    //     {
                    //         // box is not aligned on top
                    //         nextBoxes = boxes.Where(b => b.Left.Equals(rNext) || b.Right.Equals(rNext));
                    //         if (nextBoxes.Any())
                    //         {
                    //             canMove &= nextBoxes.First().CanMove(dir, walls, boxes, ref movedBoxes);
                    //         }
                    //     }

                    // }
                }
                return canMove;
            }

            public BigBox Move(Util.Grid2.Dir dir)
            {
                return new(Left + Util.Grid2.Map.Neighbor[dir], Right + Util.Grid2.Map.Neighbor[dir]);
            }
        }

        private static void PrintBigGrid(Base.Grid2Char grid, Base.Vec2 robot, Util.Grid2.Dir dir, HashSet<BigBox> boxes)
        {
            Base.Grid2Char temp = new(grid);
            HashSet<Base.Vec2> leftBoxes = boxes.Select(b => b.Left).ToHashSet();
            HashSet<Base.Vec2> rightBoxes = boxes.Select(b => b.Right).ToHashSet();
            foreach (Base.Vec2 vec2 in grid)
            {
                if (vec2.Equals(robot))
                {
                    if (dir == Util.Grid2.Dir.None)
                    {
                        temp[vec2] = Robot;
                    }
                    else
                    {
                        temp[vec2] = Util.Grid2.Map.Arrow[dir];
                    }
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
            HashSet<BigBox> boxes = [];
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
                        boxes.Add(new(left, right));
                        break;
                    case Robot:
                        robot = left;
                        break;
                }
            }

            // PrintBigGrid(bigGrid, robot, Util.Grid2.Dir.None, boxes);

            int index = 0;
            foreach (Util.Grid2.Dir dir in instructions)
            {
                ++index;
                // Log($"Moving: {Util.Grid2.Map.Arrow[dir]}");
                Base.Vec2 next = robot + Util.Grid2.Map.Neighbor[dir];

                // no wall, can potentially move
                if (!walls.Contains(next))
                {
                    bool canMove = true;
                    IEnumerable<BigBox> potentialBox = boxes.Where(b => b.Left.Equals(next) || b.Right.Equals(next));
                    if (potentialBox.Any())
                    {
                        HashSet<Base.Vec2> movedBoxes = [];
                        if (potentialBox.First().CanMove(dir, walls, boxes, ref movedBoxes))
                        {
                            List<BigBox> moved = boxes.Where(b => movedBoxes.Contains(b.Left)).ToList();
                            boxes.RemoveWhere(moved.Contains);
                            foreach (BigBox bb in moved)
                            {
                                boxes.Add(bb.Move(dir));
                            }
                        }
                        else
                        {
                            canMove = false;
                        }
                    }

                    if (canMove)
                    {
                        robot = next;
                        // Log($"Step {index} | Moving: {Util.Grid2.Map.Arrow[dir]}");
                        // PrintBigGrid(bigGrid, robot, dir, boxes);
                    }
                    else
                    {
                        // Log($"Step {index} | Blocked by Box: {Util.Grid2.Map.Arrow[dir]}");
                        // PrintBigGrid(bigGrid, robot, Util.Grid2.Dir.None, boxes);
                    }
                }
                else
                {
                    // Log($"Step {index} | Blocked by Wall: {Util.Grid2.Map.Arrow[dir]}");
                    // PrintBigGrid(bigGrid, robot, Util.Grid2.Dir.None, boxes);
                }
            }

            // PrintBigGrid(bigGrid, robot, Util.Grid2.Dir.None, boxes);

            return boxes.Select(b => 100 * b.Left.Y + b.Left.X).Sum().ToString();
        }
    }
}