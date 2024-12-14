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

        private record Machine(Base.Vec2 ButtonA, Base.Vec2 ButtonB, Base.Vec2 Prize);

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

        private int GetButtonPresses(Machine machine)
        {
            const int maxButtonPresses = 100;
            
            // time to math
            double prizeLength = Math.Sqrt(Math.Pow(machine.Prize.X, 2) + Math.Pow(machine.Prize.Y, 2));
            double prizeAngleX = Math.Pow(prizeLength, 2) + Math.Pow(machine.Prize.X, 2) - Math.Pow(machine.Prize.Y, 2);
            prizeAngleX /= 2 * prizeLength * machine.Prize.X;
            prizeAngleX = Math.Acos(prizeAngleX);
            // double prizeAngleY = Math.PI / 2 - prizeAngleX;
            // Log($"Prize -> {prizeLength} | {prizeAngleX} | {prizeAngleY}");
            // Log($"Prize -> {prizeLength} | {prizeAngleX}");

            double buttonASize = Math.Sqrt(Math.Pow(machine.ButtonA.X, 2) + Math.Pow(machine.ButtonA.Y, 2));
            double buttonAAngleX = Math.Pow(buttonASize, 2) + Math.Pow(machine.ButtonA.X, 2) - Math.Pow(machine.ButtonA.Y, 2);
            buttonAAngleX /= 2 * buttonASize * machine.ButtonA.X;
            buttonAAngleX = Math.Acos(buttonAAngleX);
            // dont need angle y
            // Log($"ButtonA -> {buttonASize} | {buttonAAngleX}");

            double buttonBSize = Math.Sqrt(Math.Pow(machine.ButtonB.X, 2) + Math.Pow(machine.ButtonB.Y, 2));
            double buttonBAngleX = Math.Pow(buttonBSize, 2) + Math.Pow(machine.ButtonB.X, 2) - Math.Pow(machine.ButtonB.Y, 2);
            buttonBAngleX /= 2 * buttonBSize * machine.ButtonB.X;
            buttonBAngleX = Math.Acos(buttonBAngleX);
            // dont need angle y
            // Log($"ButtonB -> {buttonBSize} | {buttonBAngleX}");

            double sizeC = prizeLength;
            double angleB = Math.Abs(prizeAngleX - buttonAAngleX);
            double angleA = Math.Abs(prizeAngleX - buttonBAngleX);
            double angleC = Math.PI - angleB - angleA;

            double sizeB = sizeC * Math.Sin(angleB) / Math.Sin(angleC);
            double sizeA = sizeC * Math.Sin(angleA) / Math.Sin(angleC);

            double buttonPressesA = sizeA / buttonASize;
            double buttonPressesB = sizeB / buttonBSize;
            
            int bpA = (int)Math.Round(buttonPressesA);
            int bpB = (int)Math.Round(buttonPressesB);
            if (bpA > maxButtonPresses || bpB > maxButtonPresses)
            {
                return 0;
            }

            // Log($"Machine -> {machine.Prize} | A @ {bpA} | B @ {bpB}");
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
            => SharedSolution(inputs, variables, true);
    }
}