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

        private void GetHailstoneXYValues(Vec3L h1, Vec3L h2, out List<decimal> values)
        {
            values = new()
            {
                (decimal)h2.Pos.X - (decimal)h1.Pos.X,
                (decimal)h1.Pos.Y - (decimal)h2.Pos.Y,
                (decimal)h1.Vel.X - (decimal)h2.Vel.X,
                (decimal)h2.Vel.Y - (decimal)h1.Vel.Y,
                (decimal)h2.Pos.X * (decimal)h2.Vel.Y - (decimal)h2.Pos.Y * (decimal)h2.Vel.X - (decimal)h1.Pos.X * (decimal)h1.Vel.Y + (decimal)h1.Pos.Y * (decimal)h1.Vel.X
            };
        }

        private decimal[] Solve(decimal[,] a)
        {
            int m = a.GetLength(0);
            int n = a.GetLength(1);
            Pivot(a, m, n);
            return BackSubstitue(a, m, n);
        }

        private static void Pivot(decimal[,] a, int m, int n)
        {
            int h = 0;
            int k = 0;
            while (h < m && k < n)
            {
                Core.TempLog.WriteLine("Processing...");
                Print(a);
                Core.TempLog.WriteLine(".");
                Core.TempLog.WriteLine("..");
                Core.TempLog.WriteLine("...");
                int iMax = h;
                for (int i = h + 1; i < m; ++i)
                {
                    if (decimal.Abs(a[i, k]) > decimal.Abs(a[iMax, k]))
                    {
                        iMax = i;
                    }
                }

                if (a[iMax, k] == 0)
                {
                    ++k;
                }
                else
                {
                    if (iMax != h)
                    {
                        for (int _n = 0; _n < n; ++_n)
                        {
                            (a[h, _n], a[iMax, _n]) = (a[iMax, _n], a[h, _n]);
                        }
                    }

                    for (int i = h + 1; i < m; ++i)
                    {
                        decimal f = a[i, k] / a[h, k];
                        a[i, k] = 0;
                        for (int j = k + 1; j < n; ++j)
                        {
                            a[i, j] -= a[h, j] * f;
                        }
                    }

                    ++h;
                    ++k;
                }
            }
            Print(a);
        }

        private decimal[] BackSubstitue(decimal[,] a, int m, int n)
        {
            decimal[] solve = new decimal[m];
            for (int i = m - 1; i >= 0; --i)
            {
                decimal sum = decimal.Zero;
                for (int j = i + 1; j < m; ++j)
                {
                    sum += a[i, j] * solve[j];
                }

                solve[i] = (a[i, m] - sum) / a[i, i];
            }
            return solve;
        }

        private static void Print(decimal[,] a)
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
                    if (a[i,j] >= 0)
                    {
                        sb.Append(' ');
                    }
                    string tmp = $"0:{format}.00";
                    sb.Append(string.Format("   {" + tmp + "}", a[i,j]));
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
            decimal[,] aXY = new decimal[4, 5];
            List<List<decimal>> equations = new();
            for (int h1 = 0; h1 < 4; ++h1)
            {
                GetHailstoneXYValues(hailstones[h1], hailstones[h1 + 1], out List<decimal> curEquationVals);
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
            Print(aXY);
            Solve(aXY);

            return string.Empty;
        }
    }
}