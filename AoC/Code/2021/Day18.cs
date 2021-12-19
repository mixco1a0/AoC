using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day18 : Day
    {
        public Day18() { }

        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                // case Part.One:
                //     return "v1";
                // case Part.Two:
                //     return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            //             testData.Add(new TestDatum
            //             {
            //                 TestPart = Part.One,
            //                 Output = "0",
            //                 RawInput =
            // @"[[[[[9,8],1],2],3],4]"
            //             });
            //             testData.Add(new TestDatum
            //             {
            //                 TestPart = Part.One,
            //                 Output = "0",
            //                 RawInput =
            // @"[7,[6,[5,[4,[3,2]]]]]"
            //             });
            //             testData.Add(new TestDatum
            //             {
            //                 TestPart = Part.One,
            //                 Output = "0",
            //                 RawInput =
            // @"[[6,[5,[4,[3,2]]]],1]"
            //             });
            //             testData.Add(new TestDatum
            //             {
            //                 TestPart = Part.One,
            //                 Output = "0",
            //                 RawInput =
            // @"[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]"
            //             });
            // ^ only for testing logic
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "0",
                RawInput =
@"[[[[[4,3],4],4],[7,[[8,4],9]]],[1,1]]"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "1384",
                RawInput =
@"[[[[0,7],4],[[7,8],[6,0]]],[8,1]]"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "445",
                RawInput =
@"[[[[1,1],[2,2]],[3,3]],[4,4]]"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "791",
                RawInput =
@"[[[[3,0],[5,3]],[4,4]],[5,5]]"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "1137",
                RawInput =
@"[[[[5,0],[7,4]],[5,5]],[6,6]]"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "3488",
                RawInput =
@"[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
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
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private class Number
        {
            //debug
            public static Number Top { get; set; }

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

            private Number()
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

            public Number Add(Number a, Number b)
            {
                Number num = new Number();
                num.Nested[0] = a;
                num.Nested[1] = b;
                num.Type = EType.AllNested;
                num.Reduce();
                return num;
            }

            public void Reduce()
            {
                while (ReduceInternal()) ;
            }

            public bool ReduceInternal()
            {
                if (Values.Any(v => v >= 10))
                {
                    Split();
                    return true;
                }

                if (Type == EType.AllValues)
                {
                    if (Level >= 4)
                    {
                        Parent.ExplodeChild(this);
                        return true;
                    }
                    return false;
                }

                bool didReduce = Nested[0].ReduceInternal();
                if (!didReduce && Type == EType.AllNested)
                {
                    return Nested[1].ReduceInternal();
                }
                return didReduce;
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

                if (Values[0] > 10)
                {
                    if (Type == EType.FirstNested)
                    {
                        Nested[1] = SplitValue(Values[0]);
                        Type = EType.AllNested;
                    }
                    else if (Type == EType.FirstValue)
                    {
                        Nested[1] = Nested[0];
                        Nested[0] = SplitValue(Values[0]);
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
                else if (Values[1] > 10)
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
                        AddToFirstRight(Nested[0], child.Values[0]);
                        AddToLeftChild(child.Values[1]);
                        Nested[0] = null;
                    }
                    else if (Type == EType.FirstValue)
                    {
                        Values[0] += child.Values[0];
                        Values[1] = 0;
                        Type = EType.AllValues;
                        AddToFirstLeft(Nested[0], child.Values[1]);
                        AddToRightChild(child.Values[1]);
                        Nested[0] = null;
                    }
                    else if (Type == EType.AllNested)
                    {
                        Values[0] = 0;
                        AddToFirstLeft(Nested[1], child.Values[1]);
                        AddToLeftChild(child.Values[1]);
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
                        AddToFirstRight(curParent.Nested[0], value);
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
                        AddToFirstLeft(curParent.Nested[0], value);
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

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            Number[] numbers = inputs.Select(Number.Parse).ToArray();
            Number.Top = numbers[0];
            DebugWriteLine($"Before = {Number.Top.ToString()}");
            while (Number.Top.ReduceInternal())
            {
                DebugWriteLine($"After = {Number.Top.ToString()}");
            }
            return string.Empty;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}