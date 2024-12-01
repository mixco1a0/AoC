using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day18 : Core.Day
    {
        public Day18() { }

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

            /*
                        // These are solely for parsing and reducing validation
                        testData.Add(new TestDatum
                        {
                            TestPart = Part.One,
                            Variables = new Dictionary<string, string>() { { "validationTest", "1" } },
                            Output = "[[[[0,9],2],3],4]",
                            RawInput =
            @"[[[[[9,8],1],2],3],4]"
                        });
                        testData.Add(new TestDatum
                        {
                            TestPart = Part.One,
                            Variables = new Dictionary<string, string>() { { "validationTest", "1" } },
                            Output = "[7,[6,[5,[7,0]]]]",
                            RawInput =
            @"[7,[6,[5,[4,[3,2]]]]]"
                        });
                        testData.Add(new TestDatum
                        {
                            TestPart = Part.One,
                            Variables = new Dictionary<string, string>() { { "validationTest", "1" } },
                            Output = "[[6,[5,[7,0]]],3]",
                            RawInput =
            @"[[6,[5,[4,[3,2]]]],1]"
                        });
                        testData.Add(new TestDatum
                        {
                            TestPart = Part.One,
                            Variables = new Dictionary<string, string>() { { "validationTest", "1" } },
                            Output = "[[3,[2,[8,0]]],[9,[5,[7,0]]]]",
                            RawInput =
            @"[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]"
                        });
                        testData.Add(new TestDatum
                        {
                            TestPart = Part.One,
                            Variables = new Dictionary<string, string>() { { "validationTest", "1" } },
                            Output = "[[[[0,7],4],[[7,8],[6,0]]],[8,1]]",
                            RawInput =
            @"[[[[[4,3],4],4],[7,[[8,4],9]]],[1,1]]"
                        });
            */

            // These are solely for addition validation
            /*
                        testData.Add(new TestDatum
                        {
                            TestPart = Part.One,
                            Variables = new Dictionary<string, string>() { { "validationTest", "2" } },
                            Output = "[[[[1,1],[2,2]],[3,3]],[4,4]]",
                            RawInput =
            @"[1,1]
            [2,2]
            [3,3]
            [4,4]"
                        });
                        testData.Add(new TestDatum
                        {
                            TestPart = Part.One,
                            Variables = new Dictionary<string, string>() { { "validationTest", "2" } },
                            Output = "[[[[3,0],[5,3]],[4,4]],[5,5]]",
                            RawInput =
            @"[1,1]
            [2,2]
            [3,3]
            [4,4]
            [5,5]"
                        });
                        testData.Add(new TestDatum
                        {
                            TestPart = Part.One,
                            Variables = new Dictionary<string, string>() { { "validationTest", "2" } },
                            Output = "[[[[5,0],[7,4]],[5,5]],[6,6]]",
                            RawInput =
            @"[1,1]
            [2,2]
            [3,3]
            [4,4]
            [5,5]
            [6,6]"
                        });
                        testData.Add(new TestDatum
                        {
                            TestPart = Part.One,
                            Variables = new Dictionary<string, string>() { { "validationTest", "2" } },
                            Output = "[[[[0,7],4],[[7,8],[6,0]]],[8,1]]",
                            RawInput =
            @"[[[[4,3],4],4],[7,[[8,4],9]]]
            [1,1]"
                        });
                        testData.Add(new TestDatum
                        {
                            TestPart = Part.One,
                            Variables = new Dictionary<string, string>() { { "validationTest", "2" } },
                            Output = "[[[[4,0],[5,4]],[[7,7],[6,0]]],[[8,[7,7]],[[7,9],[5,0]]]]",
                            RawInput =
            @"[[[0,[4,5]],[0,0]],[[[4,5],[2,6]],[9,5]]]
            [7,[[[3,7],[4,3]],[[6,3],[8,8]]]]"
                        });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Variables = new Dictionary<string, string>() { { "validationTest", "2" } },
                Output = "[[[[7,7],[7,8]],[[9,5],[8,7]]],[[[6,8],[0,8]],[[9,9],[9,0]]]]",
                RawInput =
@"[[[[7,0],[7,7]],[[7,7],[7,8]]],[[[7,7],[8,8]],[[7,7],[8,7]]]]
[7,[5,[[3,8],[1,4]]]]"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Variables = new Dictionary<string, string>() { { "validationTest", "2" } },
                Output = "[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]",
                RawInput =
@"[[[[7,7],[7,8]],[[9,5],[8,7]]],[[[6,8],[0,8]],[[9,9],[9,0]]]]
[[2,[2,2]],[8,[8,1]]]
[2,9]
[1,[[[9,3],9],[[9,0],[0,7]]]]
[[[5,[7,4]],7],1]
[[[[4,2],2],6],[8,7]]"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Variables = new Dictionary<string, string>() { { "validationTest", "2" } },
                Output = "[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]",
                RawInput =
@"[[[0,[4,5]],[0,0]],[[[4,5],[2,6]],[9,5]]]
[7,[[[3,7],[4,3]],[[6,3],[8,8]]]]
[[2,[[0,8],[3,4]]],[[[6,7],1],[7,[1,6]]]]
[[[[2,4],7],[6,[0,5]]],[[[6,8],[2,8]],[[2,1],[4,5]]]]
[7,[5,[[3,8],[1,4]]]]
[[2,[2,2]],[8,[8,1]]]
[2,9]
[1,[[[9,3],9],[[9,0],[0,7]]]]
[[[5,[7,4]],7],1]
[[[[4,2],2],6],[8,7]]"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Variables = new Dictionary<string, string>() { { "validationTest", "2" } },
                Output = "[[[[6,6],[7,6]],[[7,7],[7,0]]],[[[7,7],[7,7]],[[7,8],[9,9]]]]",
                RawInput =
@"[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]
[[[5,[2,8]],4],[5,[[9,9],0]]]
[6,[[[6,2],[5,6]],[[7,6],[4,7]]]]
[[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]
[[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]
[[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]
[[[[5,4],[7,7]],8],[[8,3],8]]
[[9,3],[[9,9],[6,[4,9]]]]
[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]
[[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]"
            });
            */

            // These are the real tests
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "143",
                RawInput =
@"[[1,2],[[3,4],5]]"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "1384",
                RawInput =
@"[[[[0,7],4],[[7,8],[6,0]]],[8,1]]"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "445",
                RawInput =
@"[[[[1,1],[2,2]],[3,3]],[4,4]]"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "791",
                RawInput =
@"[[[[3,0],[5,3]],[4,4]],[5,5]]"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "1137",
                RawInput =
@"[[[[5,0],[7,4]],[5,5]],[6,6]]"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "3488",
                RawInput =
@"[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "4140",
                RawInput =
@"[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]
[[[5,[2,8]],4],[5,[[9,9],0]]]
[6,[[[6,2],[5,6]],[[7,6],[4,7]]]]
[[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]
[[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]
[[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]
[[[[5,4],[7,7]],8],[[8,3],8]]
[[9,3],[[9,9],[6,[4,9]]]]
[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]
[[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "3993",
                RawInput =
@"[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]
[[[5,[2,8]],4],[5,[[9,9],0]]]
[6,[[[6,2],[5,6]],[[7,6],[4,7]]]]
[[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]
[[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]
[[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]
[[[[5,4],[7,7]],8],[[8,3],8]]
[[9,3],[[9,9],[6,[4,9]]]]
[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]
[[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]"
            });
            return testData;
        }

        private class Number
        {
            public enum EType
            {
                Blank,
                AllValues,
                FirstValue,
                FirstNested,
                AllNested
            }

            public Number Parent { get; set; }
            public EType Type { get; set; }
            public int[] Values { get; set; }
            public Number[] Nested { get; set; }
            private int Level { get => Parent != null ? Parent.Level + 1 : 0; }

            public Number()
            {
                Parent = null;
                Type = EType.Blank;
                Values = new int[2] { -1, -1 };
                Nested = new Number[2];
            }

            private Number(Number parent)
            {
                Parent = parent;
                Type = EType.Blank;
                Values = new int[2] { -1, -1 };
                Nested = new Number[2];
            }

            public static Number operator +(Number num1, Number num2)
            {
                if (num1.Type == EType.Blank)
                {
                    return num2;
                }

                if (num2.Type == EType.Blank)
                {
                    return num1;
                }

                Number sum = new Number();
                sum.Type = EType.AllNested;
                sum.Nested[0] = num1;
                sum.Nested[0].Parent = sum;
                sum.Nested[1] = num2;
                sum.Nested[1].Parent = sum;
                sum.Reduce();
                return sum;
            }

            public static Number Parse(string input)
            {
                Number num = new Number();
                num.InternalParse(input, 1);
                return num;
            }

            private int InternalParse(string input, int curIdx)
            {
                while (curIdx < input.Length)
                {
                    if (input[curIdx] == '[')
                    {
                        int nestedIdx = -1;
                        if (Type == EType.Blank)
                        {
                            nestedIdx = 0;
                            Type = EType.FirstNested;
                        }
                        else if (Type == EType.FirstNested)
                        {
                            nestedIdx = 1;
                            Type = EType.AllNested;
                        }
                        else if (Type == EType.FirstValue)
                        {
                            nestedIdx = 0;
                        }
                        Nested[nestedIdx] = new Number(this);
                        curIdx = Nested[nestedIdx].InternalParse(input, curIdx + 1);
                    }
                    else if (input[curIdx] == ',' || input[curIdx] == ']')
                    {
                        return curIdx + 1;
                    }
                    else
                    {
                        int valueIdx = -1;
                        char endChar = '0';
                        if (Type == EType.Blank)
                        {
                            valueIdx = 0;
                            Type = EType.FirstValue;
                            endChar = ',';
                        }
                        else if (Type == EType.FirstValue)
                        {
                            valueIdx = 1;
                            Type = EType.AllValues;
                            endChar = ']';
                        }
                        else if (Type == EType.FirstNested)
                        {
                            valueIdx = 0;
                            endChar = ']';
                        }
                        int endLocation = input.IndexOf(endChar, curIdx);
                        Values[valueIdx] = int.Parse(input.Substring(curIdx, endLocation - curIdx));
                        curIdx = endLocation + 1;
                    }

                    if (Type == EType.AllValues || Type == EType.AllNested)
                    {
                        return curIdx + 1;
                    }
                }
                return input.Length;
            }

            public void Reduce()
            {
                while (DidExplode() || DidSplit()) ;
            }

            private bool DidExplode()
            {
                if (Type == EType.AllValues)
                {
                    if (Level >= 4)
                    {
                        Parent.ExplodeChild(this);
                        return true;
                    }
                    return false;
                }

                bool didExplode = Nested[0].DidExplode();
                if (!didExplode && Type == EType.AllNested)
                {
                    return Nested[1].DidExplode();
                }
                return didExplode;
            }

            private bool DidSplit()
            {
                if (Type == EType.FirstNested || Type == EType.AllNested)
                {
                    if (Nested[0].DidSplit())
                    {
                        return true;
                    }
                }

                if (Values.Any(v => v >= 10))
                {
                    Split();
                    return true;
                }

                if (Type == EType.FirstValue)
                {
                    return Nested[0].DidSplit();
                }
                else if (Type == EType.AllNested)
                {
                    return Nested[1].DidSplit();
                }
                return false;
            }

            private void Split()
            {
                Func<int, Number> SplitValue = (value) =>
                {
                    int val1 = value / 2;
                    int val2 = value - val1;
                    Number num = new Number(this);
                    num.Values[0] = val1;
                    num.Values[1] = val2;
                    num.Type = EType.AllValues;
                    return num;
                };

                if (Values[0] >= 10)
                {
                    if (Type == EType.FirstNested)
                    {
                        Nested[1] = SplitValue(Values[0]);
                        Values[0] = -1;
                        Type = EType.AllNested;
                    }
                    else if (Type == EType.FirstValue)
                    {
                        Nested[1] = Nested[0];
                        Nested[0] = SplitValue(Values[0]);
                        Values[0] = -1;
                        Type = EType.AllNested;
                    }
                    else if (Type == EType.AllValues)
                    {
                        Nested[0] = SplitValue(Values[0]);
                        Values[0] = Values[1];
                        Values[1] = -1;
                        Type = EType.FirstNested;
                    }
                }
                else if (Values[1] >= 10)
                {
                    Nested[0] = SplitValue(Values[1]);
                    Values[1] = -1;
                    Type = EType.FirstValue;
                }
            }

            private void ExplodeChild(Number child)
            {
                if (child == Nested[0])
                {
                    if (Type == EType.FirstNested)
                    {
                        Values[1] = Values[0] + child.Values[1];
                        Values[0] = 0;
                        Type = EType.AllValues;
                        AddToLeftChild(child.Values[0]);
                        Nested[0] = null;
                    }
                    else if (Type == EType.FirstValue)
                    {
                        Values[0] += child.Values[0];
                        Values[1] = 0;
                        Type = EType.AllValues;
                        AddToRightChild(child.Values[1]);
                        Nested[0] = null;
                    }
                    else if (Type == EType.AllNested)
                    {
                        Values[0] = 0;
                        AddToFirstLeft(Nested[1], child.Values[1]);
                        AddToLeftChild(child.Values[0]);
                        Type = EType.FirstValue;
                        Nested[0] = Nested[1];
                        Nested[1] = null;
                    }
                }
                else if (child == Nested[1])
                {
                    Values[0] = 0;
                    AddToFirstRight(Nested[0], child.Values[0]);
                    AddToRightChild(child.Values[1]);
                    Type = EType.FirstNested;
                    Nested[1] = null;
                }
            }

            private void AddToLeftChild(int value)
            {
                Number curChild = this;
                Number curParent = Parent;
                while (curParent != null)
                {
                    if (curParent.Type == EType.AllNested && curParent.Nested[0] != curChild)
                    {
                        AddToFirstRight(curParent.Nested[0], value);
                        break;
                    }
                    else if (curParent.Type == EType.FirstValue)
                    {
                        AddToFirstLeft(curParent, value);
                        break;
                    }

                    curChild = curParent;
                    curParent = curChild.Parent;
                }
            }

            private void AddToRightChild(int value)
            {
                Number curChild = this;
                Number curParent = Parent;
                while (curParent != null)
                {
                    if (curParent.Type == EType.AllNested && curParent.Nested[1] != curChild)
                    {
                        AddToFirstLeft(curParent.Nested[1], value);
                        break;
                    }
                    else if (curParent.Type == EType.FirstNested)
                    {
                        AddToFirstRight(curParent, value);
                        break;
                    }

                    curChild = curParent;
                    curParent = curChild.Parent;
                }
            }

            private static void AddToFirstLeft(Number source, int value)
            {
                Action<Number, int> AddToLeft = (num, val) =>
                {
                    while (num.Type == EType.AllNested || num.Type == EType.FirstValue)
                    {
                        num = num.Nested[num.Type == EType.AllNested ? 1 : 0];
                    }
                    num.Values[num.Type == EType.AllValues ? 1 : 0] += value;
                };

                switch (source.Type)
                {
                    case EType.AllNested:
                    case EType.FirstNested:
                        AddToFirstLeft(source.Nested[0], value);
                        break;
                    case EType.FirstValue:
                    case EType.AllValues:
                        source.Values[0] += value;
                        break;
                }
            }

            private static void AddToFirstRight(Number source, int value)
            {
                Action<Number, int> AddToRight = (num, val) =>
                {
                    while (num.Type == EType.AllNested || num.Type == EType.FirstValue)
                    {
                        num = num.Nested[num.Type == EType.AllNested ? 1 : 0];
                    }
                    num.Values[num.Type == EType.AllValues ? 1 : 0] += value;
                };

                switch (source.Type)
                {
                    case EType.AllNested:
                        AddToFirstRight(source.Nested[1], value);
                        break;
                    case EType.FirstValue:
                        AddToFirstRight(source.Nested[0], value);
                        break;
                    case EType.AllValues:
                        source.Values[1] += value;
                        break;
                    case EType.FirstNested:
                        source.Values[0] += value;
                        break;
                }
            }


            public int Magnitude()
            {
                switch (Type)
                {
                    case EType.AllValues:
                        return 3 * Values[0] + 2 * Values[1];
                    case EType.FirstValue:
                        return 3 * Values[0] + 2 * Nested[0].Magnitude();
                    case EType.FirstNested:
                        return 3 * Nested[0].Magnitude() + 2 * Values[0];
                    case EType.AllNested:
                        return 3 * Nested[0].Magnitude() + 2 * Nested[1].Magnitude();
                }
                return 0;
            }

            public override string ToString()
            {
                string first = string.Empty, second = string.Empty;
                switch (Type)
                {
                    case EType.AllValues:
                        first = Values[0].ToString("D");
                        second = Values[1].ToString("D");
                        break;
                    case EType.FirstValue:
                        first = Values[0].ToString("D");
                        second = Nested[0].ToString();
                        break;
                    case EType.FirstNested:
                        first = Nested[0].ToString();
                        second = Values[0].ToString("D");
                        break;
                    case EType.AllNested:
                        first = Nested[0].ToString();
                        second = Nested[1].ToString();
                        break;
                }
                return $"[{first},{second}]";
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool totalSum)
        {
            Number[] numbers = inputs.Select(Number.Parse).ToArray();

            // parsing and reducing validation
            int validationTest;
            GetVariable(nameof(validationTest), 0, variables, out validationTest);
            if (validationTest == 1)
            {
                numbers[0].Reduce();
                return numbers[0].ToString();
            }

            if (totalSum)
            {
                Number finalSum = new Number();
                Queue<Number> pendingAdd = new Queue<Number>(numbers);
                while (pendingAdd.Count > 0)
                {
                    finalSum += pendingAdd.Dequeue();
                }

                if (validationTest == 2)
                {
                    return finalSum.ToString();
                }

                return finalSum.Magnitude().ToString();
            }

            int maxMagnitude = int.MinValue;
            for (int i = 0; i < numbers.Length; ++i)
            {
                for (int j = i + 1; j < numbers.Length; ++j)
                {
                    numbers[i] = Number.Parse(inputs[i]);
                    numbers[j] = Number.Parse(inputs[j]);
                    Number sum = numbers[i] + numbers[j];
                    maxMagnitude = Math.Max(maxMagnitude, sum.Magnitude());
                    numbers[i] = Number.Parse(inputs[i]);
                    numbers[j] = Number.Parse(inputs[j]);
                    sum = numbers[j] + numbers[i];
                    maxMagnitude = Math.Max(maxMagnitude, sum.Magnitude());
                }
            }
            // 4696 - wrong, too low
            return maxMagnitude.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}