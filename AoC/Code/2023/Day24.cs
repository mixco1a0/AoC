using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using AoC.Base;

namespace AoC._2023
{
    class Day24 : Core.Day
    {
        public Day24() { }

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

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Variables = new Dictionary<string, string> { { nameof(_MinRange), "7" }, { nameof(_MaxRange), "27" } },
                Output = "2",
                RawInput =
@"19, 13, 30 @ -2,  1, -2
18, 19, 22 @ -1, -1, -2
20, 25, 34 @ -2, -2, -4
12, 31, 28 @ -1, -2, -1
20, 19, 15 @  1, -5, -3"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "47",
                RawInput =
@"19, 13, 30 @ -2,  1, -2
18, 19, 22 @ -1, -1, -2
20, 25, 34 @ -2, -2, -4
12, 31, 28 @ -1, -2, -1
20, 19, 15 @  1, -5, -3"
            });
            return testData;
        }

        private long _MinRange { get; }
        private long _MaxRange { get; }

        private static string Count2DIntersections(List<Ray3L> hailstones, long minRange, long maxRange)
        {
            List<Ray2L> h2d = hailstones.Select(v3 => v3.DropZ()).ToList();
            // Log("Hailstones:");
            // h2d.ForEach(l => Log(l.ToString()));
            long count = 0;
            for (int i = 0; i < h2d.Count; ++i)
            {
                Ray2L iH = h2d[i];
                for (int j = i + 1; j < h2d.Count; ++j)
                {
                    Ray2L jH = h2d[j];
                    Ray.Intersection2D type = iH.GetIntersection(jH, out Vec2L pos2);
                    switch (type)
                    {
                        case Ray.Intersection2D.Parallel:
                            // Log($"{iH} is parallel to {jH}");
                            break;
                        case Ray.Intersection2D.Overlap:
                            // Log($"{iH} overlaps {jH}");
                            ++count;
                            break;
                        case Ray.Intersection2D.SinglePoint:
                            // char inside = 'X';
                            // char whenPath = 'P';
                            if (pos2.X >= minRange && pos2.X <= maxRange && pos2.Y >= minRange && pos2.Y <= maxRange)
                            {
                                // inside = 'O';
                                Vec2L iVel = iH.Vel;
                                Vec2L iNextVel = pos2 - iH.Pos;
                                Vec2L jVel = jH.Vel;
                                Vec2L jNextVel = pos2 - jH.Pos;
                                if ((iVel.X < 0 == iNextVel.X < 0) && (iVel.Y < 0 == iNextVel.Y < 0) && (jVel.X < 0 == jNextVel.X < 0) && (jVel.Y < 0 == jNextVel.Y < 0))
                                {
                                    // whenPath = 'F';
                                    ++count;
                                }
                            }
                            // Log($"{h2d[i]} intersects {h2d[j]} at {pos2} | [{inside}.{whenPath}]");
                            break;
                    }
                }
            }
            return count.ToString();
        }

        private static string FindRockValues(List<Ray3L> hailstones)
        {
            // rX + t * rDX = x + t * dx
            // rX - x = t * (dx - rDX)
            // t = (rX - x) / (dx - rDX)

            // rY + t * rDY = y + t * dy
            // t = (rY - y) / (dy - rDY)

            // using x and y
            // (X - x) / (dx - DX) = (Y - y) / (dy - DY)
            // (X - x) * (dy - DY) = (Y - y) * (dx - DX)
            // X * dy - x * dy - X * DY + x * DY = Y * dx - y * dx - Y * DX + y * DX
            // Y * DX - X * DY = Y * dx - y * dx + y * DX - X * dy + x * dy - x * DY
            // Y * DX - X * DY = (x * dy) - (y * dx) + (Y * dx) + (y * DX) - (x * DY) - (X * dy)
            // (x * dy) - (y * dx) + (Y * dx) + (y * DX) - (x * DY) - (X * dy) = (x' * dy') - (y' * dx') + (Y * dx') + (y' * DX) - (x' * DY) - (X * dy')
            // (Y * dx) + (y * DX) - (x * DY) - (X * dy) - (Y * dx') - (y' * DX) + (x' * DY) + (X * dy') = (x' * dy') - (y' * dx') - (x * dy) - (y * dx)
            // (X * dy') - (X * dy) + (Y * dx) - (Y * dx') + (y * DX) - (y' * DX) - (x * DY) + (x' * DY) = (x' * dy') - (y' * dx') - (x * dy) - (y * dx)
            // X * (dy'- dy) + Y * (dx - dx') + DX * (y - y') - DY * (x' - x) = (x' * dy') - (y' * dx') - (x * dy) - (y * dx)
            // X(dy'- dy) + Y(dx - dx') + DX(y - y') + DY(x' - x) = x'dy' - y'dx' - xdy + ydx

            // take first hailstone and get x, dx, y, dy
            // take second hailstone and get x', dx', y', dy'
            // Xhdy + Yhdx + DXhy - DYhx
            // hdy = dy' - dy
            // hdx = dx - dx'
            // hy = y - y'
            // hx = x' - x

            // solve for X, Y, DX, DY
            // generate 4 equations to solve the 4 unknowns
            double[,] aXY = new double[4, 5];
            for (int _r = 0; _r < 4; ++_r)
            {
                GetHailstoneXYValues(hailstones[_r], hailstones[_r + 1], ref aXY, _r);
            }
            double[] xydxdy = Algorithm.LinearSolver.Solve(aXY);

            // at this point we have solved for x, y, dx, and dy
            // get a reduced version of equations using x and z
            // X(dz'- dz) + Z(dx - dx') + DX(z - z') + DZ(x' - x) = x'dz' - y'dz' - xdz + zdx
            // Z(dx - dx') + DZ(x' - x) = x'dz' - y'dz' - xdz + zdx - X(dz'- dz) - DX(z - z')

            // solve for Z and DZ
            // generate 2 equations to solve 2 unknowns
            double[,] aZ = new double[2, 3];
            for (int _r = 0; _r < 2; ++_r)
            {
                GetHailstoneZValues(hailstones[_r], hailstones[_r + 1], xydxdy, ref aZ, _r);
            }
            double[] zdz = Algorithm.LinearSolver.Solve(aZ);

            double x = double.Round(xydxdy[0]);
            double dx = double.Round(xydxdy[2]);
            double y = double.Round(xydxdy[1]);
            double dy = double.Round(xydxdy[3]);
            double z = double.Round(zdz[0]);
            double dz = double.Round(zdz[1]);
            // Util.Log.WriteLine($"X={x} @ {dx}");
            // Util.Log.WriteLine($"Y={y} @ {dy}");
            // Util.Log.WriteLine($"Z={z} @ {dz}");
            
            return (x + y + z).ToString();
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool count2DIntersections)
        {
            GetVariable(nameof(_MinRange), 200_000_000_000_000, variables, out long minRange);
            GetVariable(nameof(_MaxRange), 400_000_000_000_000, variables, out long maxRange);
            List<Ray3L> hailstones = inputs.Select(Ray3L.ParseVel).ToList();
            if (count2DIntersections)
            {
                return Count2DIntersections(hailstones, minRange, maxRange);
            }
            else
            {
                return FindRockValues(hailstones);
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        private static void GetHailstoneXYValues(Ray3L h1, Ray3L h2, ref double[,] aXY, int row)
        {
            aXY[row, 0] = (double)h2.Vel.Y - h1.Vel.Y; // X
            aXY[row, 1] = (double)h1.Vel.X - h2.Vel.X; // Y
            aXY[row, 2] = (double)h1.Pos.Y - h2.Pos.Y; // DX
            aXY[row, 3] = (double)h2.Pos.X - h1.Pos.X; // DY
            aXY[row, 4] = (double)h2.Pos.X * h2.Vel.Y - (double)h2.Pos.Y * h2.Vel.X - (double)h1.Pos.X * h1.Vel.Y + (double)h1.Pos.Y * h1.Vel.X;
        }

        private static void GetHailstoneZValues(Ray3L h1, Ray3L h2, double[] xydxdy, ref double[,] aZ, int row)
        {
            aZ[row, 0] = (double)h1.Vel.X - h2.Vel.X; // Z
            aZ[row, 1] = (double)h2.Pos.X - h1.Pos.X; // DZ
            aZ[row, 2] = h2.Pos.X * h2.Vel.Z - (double)h2.Pos.Z * h2.Vel.X - (double)h1.Pos.X * h1.Vel.Z + (double)h1.Pos.Z * h1.Vel.X - xydxdy[0] * (h2.Vel.Z - h1.Vel.Z) - xydxdy[2] * (h1.Pos.Z - h2.Pos.Z);
        }

        private static void Print(double[,] a)
        {
            int maxLen = 0;
            for (int i = 0; i < a.GetLength(dimension: 0); ++i)
            {
                for (int j = 0; j < a.GetLength(dimension: 1); ++j)
                {
                    int len = a[i, j].ToString().Split('.').First().Length;
                    maxLen = int.Max(maxLen, len);
                }
            }

            string format = new('0', maxLen);

            Util.Log.WriteLine($"Matrix {a.GetLength(dimension: 0)}x{a.GetLength(dimension: 1)}");
            for (int i = 0; i < a.GetLength(dimension: 0); ++i)
            {
                StringBuilder sb = new($"a[{i}] = ");
                for (int j = 0; j < a.GetLength(dimension: 1); ++j)
                {
                    if (a[i, j] >= 0)
                    {
                        sb.Append(' ');
                    }
                    string tmp = $"0:{format}.00";
                    sb.Append(string.Format("   {" + tmp + "}", a[i, j]));
                }
                Util.Log.WriteLine(sb.ToString());
            }
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}