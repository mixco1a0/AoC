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
                // Core.Part.One => "v1",
                // Core.Part.Two => "v1",
                _ => base.GetSolutionVersion(part),
            };
        }

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData =
            [
//                 new Core.TestDatum
//                 {
//                     TestPart = Core.Part.One,
//                     Output = "1",
//                     RawInput =
// @"Button A: X+19, Y+55
// Button B: X+55, Y+26
// Prize: X=4334, Y=4142"
//                 },
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

        private record Machine(Base.Vec2 ButtonA, Base.Vec2 ButtonB, Base.Vec2 Prize)
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

        private Machine[] GetMachines(List<string> inputs)
        {
            List<Machine> machines = [];
            Base.Vec2 a = default;
            Base.Vec2 b = default;

            MachineType mt = MachineType.ButtonA;
            foreach (string input in inputs)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                int[] split = Util.String.Split(input, "ButtonABPrize:+=, ").Where(i => int.TryParse(i, out _)).Select(int.Parse).ToArray();
                Base.Vec2 cur = new(split[0], split[1]);
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
                        machines.Add(new(a, b, cur));
                        mt = MachineType.ButtonA;
                        break;
                }
            }
            return [.. machines];
        }

        private static readonly double VARNAME = 0.0001;

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

            public Triangle(Base.Vec2 vec2)
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

        private int GetButtonPresses(Machine machine)
        {
            const int maxButtonPresses = 100;

            Triangle prize = new(machine.Prize);
            // Log(prize.ToString());

            Triangle buttonA = new(machine.ButtonA);
            // Log(buttonA.ToString());

            Triangle buttonB = new(machine.ButtonB);
            // Log(buttonB.ToString());

            Triangle solver = new();
            solver.S.B = prize.S.B;
            solver.A.A = Math.Abs(prize.A.A - buttonA.A.A);
            solver.A.C = Math.Abs(prize.A.C - buttonB.A.C);
            solver.Solve();
            // Log(solver.ToString());

            int bpA = (int)Math.Round(solver.S.C / buttonA.S.B);
            int bpB = (int)Math.Round(solver.S.A / buttonB.S.B);

            // verify the claw reached the targe
            Base.Vec2 claw = machine.ButtonA * bpA + machine.ButtonB * bpB;
            if (bpA > maxButtonPresses || bpB > maxButtonPresses || !claw.Equals(machine.Prize))
            {
                return 0;
            }

            return bpA * 3 + bpB;
        }

        private int GetButtonPresses(Machine machine, MachineType priorityButton, int curButtonPresses, Base.Vec2 claw, int maxButtonPresses)
        {
            if (curButtonPresses > maxButtonPresses)
            {
                return int.MaxValue;
            }

            if (claw.X > machine.Prize.X || claw.Y > machine.Prize.Y)
            {
                return int.MaxValue;
            }

            Base.Vec2 claw1 = claw + (priorityButton == MachineType.ButtonA ? machine.ButtonA : machine.ButtonB);
            Base.Vec2 claw2 = claw + (priorityButton == MachineType.ButtonA ? machine.ButtonB : machine.ButtonA);

            int b1Presses = GetButtonPresses(machine, priorityButton, curButtonPresses + 1, claw1, maxButtonPresses);
            int b2Presses = GetButtonPresses(machine, priorityButton, curButtonPresses + 1, claw2, maxButtonPresses);

            if (b1Presses == int.MaxValue && b2Presses == int.MaxValue)
            {
                return int.MaxValue;
            }

            return int.Min(b1Presses, b2Presses);
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool _)
        {
            Machine[] machines = GetMachines(inputs);
            int buttonPresses = 0;
            foreach (Machine machine in machines)
            {
                buttonPresses += GetButtonPresses(machine);
            }
            return buttonPresses.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
        // 34665 too high

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => "";//SharedSolution(inputs, variables, true);
    }
}