using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Core;

namespace AoC._2024
{
    class Day13 : Core.Day
    {
        public Day13() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            return part switch
            {
                Core.Part.One => "v2",
                Core.Part.Two => "v2",
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
                    Output = "480",
                    RawInput =
@"Button A: X+94, Y+34
Button B: X+22, Y+67
Prize: X=8400, Y=5400

Button A: X+26, Y+66
Button B: X+67, Y+21
Prize: X=12748, Y=12176

Button A: X+17, Y+86
Button B: X+84, Y+37
Prize: X=7870, Y=6450

Button A: X+69, Y+23
Button B: X+27, Y+71
Prize: X=18641, Y=10279"
                },
                new Core.TestDatum
                {
                    TestPart = Core.Part.Two,
                    Output = "",
                    RawInput =
@""
                },
            ];
            return testData;
        }

        private record Machine(Base.Vec2L ButtonA, Base.Vec2L ButtonB, Base.Vec2L Prize)
        {
            public void Log(Action<string> log)
            {
                log($"Button A: X+{ButtonA.X}, Y+{ButtonA.Y}");
                log($"Button B: X+{ButtonB.X}, Y+{ButtonB.Y}");
                log($"Prize: X={Prize.X}, Y={Prize.Y}");
                log("");
            }
        }

        private enum MachineType
        {
            ButtonA,
            ButtonB,
            Prize
        }

        private static Machine[] GetMachines(List<string> inputs, long prizeAdjustment)
        {
            List<Machine> machines = [];
            Base.Vec2L a = default;
            Base.Vec2L b = default;

            MachineType mt = MachineType.ButtonA;
            foreach (string input in inputs)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                long[] split = Util.String.Split(input, "ButtonABPrize:+=, ").Where(i => long.TryParse(i, out _)).Select(long.Parse).ToArray();
                Base.Vec2L cur = new(split[0], split[1]);
                switch (mt)
                {
                    case MachineType.ButtonA:
                        a = cur;
                        mt = MachineType.ButtonB;
                        break;
                    case MachineType.ButtonB:
                        b = cur;
                        mt = MachineType.Prize;
                        break;
                    case MachineType.Prize:
                        cur.X += prizeAdjustment;
                        cur.Y += prizeAdjustment;
                        machines.Add(new(a, b, cur));
                        mt = MachineType.ButtonA;
                        break;
                }
            }
            return [.. machines];
        }

        private class Triple : Base.Vec3D
        {
            public double A { get => X; set => X = value; }
            public double B { get => Y; set => Y = value; }
            public double C { get => Z; set => Z = value; }

            public Triple() : base() { }

            public Triple(double a, double b, double c) : base(a, b, c) { }

            public void SolveB()
            {
                B = Math.Sqrt(Math.Pow(C, 2) + Math.Pow(A, 2));
            }

            public override string ToString()
            {
                return $"({A:0.###},{B:0.###},{C:0.###})";
            }
        }

        private class Triangle
        {
            public Triple S { get; set; }
            public Triple A { get; set; }

            public Triangle()
            {
                S = new();
                A = new();
            }

            public Triangle(Base.Vec2L vec2)
            {
                S = new(a: vec2.Y, b: 0, c: vec2.X);
                S.SolveB();

                double a = Math.Pow(S.B, 2) + Math.Pow(S.C, 2) - Math.Pow(S.A, 2);
                a /= 2 * S.B * S.C;
                a = Math.Acos(a);

                double c = Math.Pow(S.B, 2) + Math.Pow(S.A, 2) - Math.Pow(S.C, 2);
                c /= 2 * S.B * S.A;
                c = Math.Acos(c);

                A = new(a: a, b: Math.PI / 2, c: c);
            }

            public override string ToString()
            {
                return $"S={S} | A={A}";
            }

            public void Solve()
            {
                A.B = Math.PI - A.C - A.A;
                S.A = S.B * Math.Sin(A.A) / Math.Sin(A.B);
                S.C = S.B * Math.Sin(A.C) / Math.Sin(A.B);
            }
        }

        private static long GetButtonPresses(Machine machine, long maxButtonPresses)
        {
            // Using Geometry to solve the equations...
            // Triangle prize = new(machine.Prize);
            // Triangle buttonA = new(machine.ButtonA);
            // Triangle buttonB = new(machine.ButtonB);

            // Triangle solver = new();
            // solver.S.B = prize.S.B;
            // solver.A.A = Math.Abs(prize.A.A - buttonA.A.A);
            // solver.A.C = Math.Abs(prize.A.C - buttonB.A.C);
            // solver.Solve();
            
            // long bpA = (long)Math.Round(solver.S.C / buttonA.S.B);
            // long bpB = (long)Math.Round(solver.S.A / buttonB.S.B);

            // Using Linear Algebra to solve the equations...
            double[,] abInput = new double[2, 3];
            abInput[0, 0] = machine.ButtonA.X;
            abInput[0, 1] = machine.ButtonB.X;
            abInput[0, 2] = machine.Prize.X;
            abInput[1, 0] = machine.ButtonA.Y;
            abInput[1, 1] = machine.ButtonB.Y;
            abInput[1, 2] = machine.Prize.Y;
            double[] ab = Algorithm.LinearSolver.Solve(abInput);

            long bpA = (long)Math.Round(ab[0]);
            long bpB = (long)Math.Round(ab[1]);

            // verify the claw reached the target
            Base.Vec2L claw = machine.ButtonA * bpA + machine.ButtonB * bpB;
            if (bpA > maxButtonPresses || bpB > maxButtonPresses || !claw.Equals(machine.Prize))
            {
                return 0;
            }

            return bpA * 3 + bpB;
        }

        private static string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool adjustPrizes)
        {
            long maxButtonPresses = adjustPrizes ? long.MaxValue : 100;
            long prizeAdjustment = adjustPrizes ? 10000000000000 : 0;
            Machine[] machines = GetMachines(inputs, prizeAdjustment);
            long buttonPresses = 0;
            foreach (Machine machine in machines)
            {
                buttonPresses += GetButtonPresses(machine, maxButtonPresses);
            }
            return buttonPresses.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}