namespace AoC.Algorithm
{
    public static class LinearSolver
    {
        public static double[] Solve(double[,] a)
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
    }
}