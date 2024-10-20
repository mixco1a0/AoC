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
                // case Core.Part.One:
                //     return "v1";
                // case Core.Part.Two:
                //     return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        public override bool SkipTestData => false;

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
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private long _MinRange { get; }
        private long _MaxRange { get; }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            GetVariable(nameof(_MinRange), 200_000_000_000_000, variables, out long minRange);
            GetVariable(nameof(_MaxRange), 400_000_000_000_000, variables, out long maxRange);
            List<Vec3L> hailstones = inputs.Select(Vec3L.ParseVel).ToList();
            List<Vec2L> h2d = hailstones.Select(v3 => v3.DropZ()).ToList();
            // Log("Hailstones:");
            // h2d.ForEach(l => Log(l.ToString()));
            long count = 0;
            for (int i = 0; i < h2d.Count; ++i)
            {
                Vec2L iH = h2d[i];
                for (int j = i + 1; j < h2d.Count; ++j)
                {
                    Vec2L jH = h2d[j];
                    Vec.Intersection2D type = iH.GetIntersection(jH, out Pos2L pos2);
                    switch (type)
                    {
                        case Vec.Intersection2D.Parallel:
                            // Log($"{iH} is parallel to {jH}");
                            break;
                        case Vec.Intersection2D.Overlap:
                            // Log($"{iH} overlaps {jH}");
                            ++count;
                            break;
                        case Vec.Intersection2D.SinglePoint:
                            // char inside = 'X';
                            // char whenPath = 'P';
                            if (pos2.X >= minRange && pos2.X <= maxRange && pos2.Y >= minRange && pos2.Y <= maxRange)
                            {
                                // inside = 'O';
                                Pos2L iVel = iH.Vel;
                                Pos2L iNextVel = pos2 - iH.Pos;
                                Pos2L jVel = jH.Vel;
                                Pos2L jNextVel = pos2 - jH.Pos;
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

        private void GetHailstoneXYValues(Vec3L h1, Vec3L h2, out List<double> values)
        {
            values = new()
            {
                (double)h2.Vel.Y - (double)h1.Vel.Y, // X
                (double)h1.Vel.X - (double)h2.Vel.X, // Y
                (double)h1.Pos.Y - (double)h2.Pos.Y, // DX
                (double)h2.Pos.X - (double)h1.Pos.X, // DY
                (double)h2.Pos.X * (double)h2.Vel.Y - (double)h2.Pos.Y * (double)h2.Vel.X - (double)h1.Pos.X * (double)h1.Vel.Y + (double)h1.Pos.Y * (double)h1.Vel.X
            };
        }

        private void GetHailstoneZValues(Vec3L h1, Vec3L h2, double[] xydxdy, out List<double> values)
        {
            values = new()
            {
                (double)h1.Vel.X - (double)h2.Vel.X, // Z
                (double)h2.Pos.Z - (double)h1.Pos.Z, // DZ
                (double)h2.Pos.X * (double)h2.Vel.Z - (double)h2.Pos.Z * (double)h2.Vel.X - (double)h1.Pos.X * (double)h1.Vel.Z + (double)h1.Pos.Z * (double)h1.Vel.X - xydxdy[0] * (double)(h2.Vel.Z - h1.Vel.Z) - xydxdy[2] * (double)(h1.Pos.Z - h2.Pos.Z)
            };
        }

        private static double[] Solve(double[,] a)
        {
            int m = a.GetLength(0);
            int n = a.GetLength(1);
            return GaussianElimination(a, m, n);
        }

        private static double[] GaussianElimination(double[,] a, int rMax, int cMax)
        {
            int r = 0;
            int c = 0;
            while (r < rMax && c < cMax)
            {
                for (int _c = c + 1; _c < cMax; ++_c)
                {
                    a[r, _c] /= a[r, c];
                }
                a[r, c] = 1;

                for (int _r = 0; _r < rMax; ++_r)
                {
                    if (_r == r)
                    {
                        continue;
                    }

                    double factor = a[_r, c] * -1;
                    a[_r, c] = 0;
                    for (int _c = c + 1; _c < cMax; ++_c)
                    {
                        a[_r, _c] += a[r, _c] * factor;
                    }
                }

                ++r;
                ++c;
            }

            double[] solve = new double[rMax];
            for (int _r = 0; _r < rMax; ++_r)
            {
                solve[_r] = a[_r, cMax - 1];
            }
            return solve;
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

            Core.TempLog.WriteLine($"Matrix {a.GetLength(dimension: 0)}x{a.GetLength(dimension: 1)}");
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
                Core.TempLog.WriteLine(sb.ToString());
            }
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<Vec3L> hailstones = inputs.Select(Vec3L.ParseVel).ToList();

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
            List<List<double>> equations = new();
            for (int h = 0; h < 4; ++h)
            {
                GetHailstoneXYValues(hailstones[h], hailstones[h + 1], out List<double> curEquationVals);
                equations.Add(curEquationVals);
            }
            // 4 equations with unknowns

            for (int y = 0; y < equations[0].Count; ++y)
            {
                for (int x = 0; x < equations.Count; ++x)
                {
                    aXY[x, y] = equations[x][y];
                }
            }

            // at this point we have solved for x, y, dx, and dy
            double[] xydxdy = Solve(aXY);

            // get a reduced version of equations using x and z
            // X(dz'- dz) + Z(dx - dx') + DX(z - z') + DZ(x' - x) = x'dz' - y'dz' - xdz + zdx
            // Z(dx - dx') + DZ(x' - x) = x'dz' - y'dz' - xdz + zdx - X(dz'- dz) - DX(z - z')

            // solve for Z and DZ
            // generate 2 equations to solve 2 unknowns
            equations = new();
            for (int h = 0; h < 2; ++h)
            {
                GetHailstoneZValues(hailstones[h], hailstones[h + 1], xydxdy, out List<double> curEquationVals);
                equations.Add(curEquationVals);
            }

            double[,] aZ = new double[2, 3];
            for (int y = 0; y < equations[0].Count; ++y)
            {
                for (int x = 0; x < equations.Count; ++x)
                {
                    aZ[x, y] = equations[x][y];
                }
            }

            double[] zdz = Solve(aZ);

            Core.TempLog.WriteLine($"X={xydxdy[0]}");
            Core.TempLog.WriteLine($"Y={xydxdy[1]}");
            Core.TempLog.WriteLine($"Z={zdz[0]}");

            // 277 - too low
            // 1210438009750217 - too high
            // -9222974158435048377 - wrong
            return ((long)xydxdy[0] + (long)xydxdy[1] + (long)zdz[0]).ToString();
        }
    }
}