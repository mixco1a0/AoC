using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Util;

namespace AoC._2024
{
    class Day12 : Core.Day
    {
        public Day12() { }

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
                    Output = "140",
                    RawInput =
@"AAAA
BBCD
BBCC
EEEC"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "772",
                    RawInput =
@"OOOOO
OXOXO
OOOOO
OXOXO
OOOOO"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.One,
                    Output = "1930",
                    RawInput =
@"RRRRIICCFF
RRRRIICCCF
VVRRRCCFFF
VVRCCCJFFF
VVVVCJJCFE
VVIVCCJJEE
VVIIICJJEE
MIIIIIJJEE
MIIISIJEEE
MMMISSJEEE"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "80",
                    RawInput =
@"AAAA
BBCD
BBCC
EEEC"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "436",
                    RawInput =
@"OOOOO
OXOXO
OOOOO
OXOXO
OOOOO"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "236",
                    RawInput =
@"EEEEE
EXXXX
EEEEE
EXXXX
EEEEE"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "368",
                    RawInput =
@"AAAAAA
AAABBA
AAABBA
ABBAAA
ABBAAA
AAAAAA"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "1206",
                    RawInput =
@"RRRRIICCFF
RRRRIICCCF
VVRRRCCFFF
VVRCCCJFFF
VVVVCJJCFE
VVIVCCJJEE
VVIIICJJEE
MIIIIIJJEE
MIIISIJEEE
MMMISSJEEE"
                },
            ];
            return testData;
        }

        private class Value(int area, int perimeter, int faces) : Base.Pair<int, int>(area, perimeter)
        {
            public int Area { get => First; set => First = value; }
            public int Perimeter { get => Last; set => Last = value; }
            public int Faces { get; set; } = faces;

            public override string ToString()
            {
                return $"[A={Area}, P={Perimeter}, F={Faces}]";
            }
        }

        private record MoveDir(Base.Vec2 Vec2, Util.Grid2.Dir Dir);

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool useDiscount)
        {
            Base.Grid2Char grid = new(inputs);
            Base.Grid2Bool flags = new(inputs);
            Queue<Base.Vec2> differentNodes = [];
            Dictionary<Base.Vec2, Value> values = [];
            differentNodes.Enqueue(new(0, 0));
            while (differentNodes.Count > 0)
            {
                Base.Vec2 curOrigin = differentNodes.Dequeue();
                if (flags[curOrigin])
                {
                    continue;
                }

                // get the current initial state
                values[curOrigin] = new(0, 0, 0);
                char curRegion = grid[curOrigin];

                // walk out the contiguous region 
                Queue<Base.Vec2> sameNodes = [];
                List<Base.Vec2> curRegionNodes = [];
                sameNodes.Enqueue(curOrigin);
                while (sameNodes.Count > 0)
                {
                    Base.Vec2 curNode = sameNodes.Dequeue();
                    if (flags[curNode])
                    {
                        continue;
                    }
                    flags[curNode] = true;
                    curRegionNodes.Add(curNode);

                    int nodePerimeter = 0;
                    foreach (Util.Grid2.Dir dir in Util.Grid2.Iter.Cardinal)
                    {
                        Base.Vec2 nextNode = curNode + Util.Grid2.Map.Neighbor[dir];
                        if (!grid.Contains(nextNode))
                        {
                            ++nodePerimeter;
                            continue;
                        }

                        if (grid[nextNode] != curRegion)
                        {
                            differentNodes.Enqueue(nextNode);
                            ++nodePerimeter;
                            continue;
                        }

                        sameNodes.Enqueue(nextNode);
                    }

                    values[curOrigin].Area += 1;
                    values[curOrigin].Perimeter += nodePerimeter;
                }

                // calculate the faces
                if (useDiscount)
                {
                    if (curRegionNodes.Count == 1 || curRegionNodes.Count == 2)
                    {
                        values[curOrigin].Faces = 4;
                    }
                    else
                    {
                        Base.Grid2Bool tempFlags = new(inputs);

                        // find a safe point to start moving around from
                        Base.Vec2 faceOrigin = curOrigin;

                        // TODO: just sort by X, then Y, always choosing min
                        bool moved = true;
                        while (moved)
                        {
                            moved = false;
                            while (true)
                            {
                                Base.Vec2 prevFace = faceOrigin - Util.Grid2.Map.Neighbor[Util.Grid2.Dir.West];
                                if (curRegionNodes.Contains(prevFace))
                                {
                                    moved = true;
                                    faceOrigin = prevFace;
                                }
                                break;
                            }

                            while (true)
                            {
                                Base.Vec2 prevFace = faceOrigin - Util.Grid2.Map.Neighbor[Util.Grid2.Dir.North];
                                if (curRegionNodes.Contains(prevFace))
                                {
                                    moved = true;
                                    faceOrigin = prevFace;
                                }
                                break;
                            }
                        }

                        // goal node
                        MoveDir moveDirOrigin = new(curOrigin, Util.Grid2.Dir.East);
                        MoveDir prevMoveDir = moveDirOrigin;

                        Base.Vec2 curNode = curOrigin;
                        // Util.Grid2.Dir curDir = Util.Grid2.Dir.East;
                        int faceCount = 0;

                        Queue<MoveDir> wallWalk = [];
                        wallWalk.Enqueue(moveDirOrigin);
                        while (wallWalk.Count > 0)
                        {
                            // tempFlags.Print(Core.Log.ELevel.Spam);

                            MoveDir moveDir = wallWalk.Dequeue();
                            // grid.PrintNextArrow(Core.Log.ELevel.Spam, moveDir.Vec2, moveDir.Dir);
                            if (faceCount > 1 && prevMoveDir.Equals(moveDirOrigin))
                            {
                                break;
                            }

                            tempFlags[moveDir.Vec2] = true;

                            Util.Grid2.Dir ccwDir = Util.Grid2.Map.RotateCCW[moveDir.Dir];
                            Base.Vec2 ccwNode = moveDir.Vec2 + Util.Grid2.Map.Neighbor[ccwDir];
                            if (grid.Contains(ccwNode) && curRegionNodes.Contains(ccwNode))
                            {
                                ++faceCount;
                                prevMoveDir = new(moveDir.Vec2, ccwDir);
                                wallWalk.Enqueue(new(ccwNode, ccwDir));
                                continue;
                            }

                            Base.Vec2 nextWall = moveDir.Vec2 + Util.Grid2.Map.Neighbor[moveDir.Dir];
                            if (grid.Contains(nextWall) && curRegionNodes.Contains(nextWall))
                            {
                                prevMoveDir = new(nextWall, moveDir.Dir);
                                wallWalk.Enqueue(new(nextWall, moveDir.Dir));
                                continue;
                            }

                            Util.Grid2.Dir cwDir = Util.Grid2.Map.RotateCW[moveDir.Dir];
                            Base.Vec2 cwNode = moveDir.Vec2 + Util.Grid2.Map.Neighbor[cwDir];
                            if (grid.Contains(cwNode) && curRegionNodes.Contains(cwNode))
                            {
                                ++faceCount;
                                prevMoveDir = new(moveDir.Vec2, cwDir);
                                wallWalk.Enqueue(new(cwNode, cwDir));
                                continue;
                            }

                            // turn in place
                            ++faceCount;
                            prevMoveDir = new(moveDir.Vec2, cwDir);
                            wallWalk.Enqueue(new(moveDir.Vec2, cwDir));
                        }

                        values[curOrigin].Faces = faceCount;
                    }

                    // Log(Core.Log.ELevel.Spam, $"Region: {curRegion} @ {curOrigin} = {values[curOrigin]}");
                    // grid.Print(Core.Log.ELevel.Spam);
                    // flags.Print(Core.Log.ELevel.Spam);
                }
            }

            if (!useDiscount)
            {
                return values.Select(v => v.Value.Area * v.Value.Perimeter).Sum().ToString();
            }


            return values.Select(v => v.Value.Area * v.Value.Faces).Sum().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}