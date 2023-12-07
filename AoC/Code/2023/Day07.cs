using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2023
{
    class Day07 : Core.Day
    {
        public Day07() { }

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
                Output = "6440",
                RawInput =
@"32T3K 765
T55J5 684
KK677 28
KTJJT 220
QQQJA 483"
            });
//             testData.Add(new Core.TestDatum
//             {
//                 TestPart = Core.Part.One,
//                 Output = "6440",
//                 RawInput =
// @"23456 1
// 22345 2
// 22446 3
// 22256 4
// 22255 5
// 22226 6
// 22222 7"
//             });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        public class Hand : IComparable<Hand>
        {
            public enum Strength
            {
                FiveOfKind,
                FourOfKind,
                FullHouse,
                ThreeOfKind,
                TwoPair,
                OnePair,
                HighCard,
                Invalid
            }

            public Strength OverallStrength { get; set; }
            public string RawHand { get; set; }
            public string ConvertedHand { get; set; }
            public long Bid { get; set; }

            private static Dictionary<char, char> EasyCompare = new Dictionary<char, char>()
            {
                {'2', 'a'},
                {'3', 'b'},
                {'4', 'c'},
                {'5', 'd'},
                {'6', 'e'},
                {'7', 'f'},
                {'8', 'g'},
                {'9', 'h'},
                {'T', 'i'},
                {'J', 'j'},
                {'Q', 'k'},
                {'K', 'l'},
                {'A', 'm'},
            };

            public Hand()
            {
                OverallStrength = Strength.Invalid;
                RawHand = string.Empty;
                ConvertedHand = string.Empty;
                Bid = 0;
            }

            public static Hand Parse(string input)
            {
                Hand hand = new Hand();
                string[] split = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                hand.RawHand = split[0];
                hand.ConvertedHand = string.Join("", hand.RawHand.Select(rh => EasyCompare[rh]));
                hand.Bid = long.Parse(split[1]);
                hand.SetOverallStrength();
                return hand;
            }

            private void SetOverallStrength()
            {
                Dictionary<char, int> chars = RawHand.Select(_ => _).Distinct().ToDictionary(card => card, card => RawHand.Where(c => c == card).Count());
                switch (chars.Count)
                {
                    case 5:
                        OverallStrength = Strength.HighCard;
                        break;
                    case 4:
                        OverallStrength = Strength.OnePair;
                        break;
                    case 3:
                        // two pairs or three of a kind
                        if (chars.Values.Max() == 2)
                        {
                            OverallStrength = Strength.TwoPair;
                        }
                        else
                        {
                            OverallStrength = Strength.ThreeOfKind;
                        }
                        break;
                    case 2:
                        // four of a kind or full house
                        if (chars.Values.Max() == 3)
                        {
                            OverallStrength = Strength.FullHouse;
                        }
                        else
                        {
                            OverallStrength = Strength.FourOfKind;
                        }
                        break;
                    case 1:
                        OverallStrength = Strength.FiveOfKind;
                        break;
                }
            }

            int IComparable<Hand>.CompareTo(Hand other)
            {
                if (OverallStrength == other.OverallStrength)
                {
                    for (int i = 0; i < ConvertedHand.Length; ++i)
                    {
                        if (ConvertedHand[i] == other.ConvertedHand[i])
                        {
                            continue;
                        }
                        return ConvertedHand[i].CompareTo(other.ConvertedHand[i]);
                    }
                }

                return other.OverallStrength.CompareTo(OverallStrength);
            }

            public override string ToString()
            {
                return $"{RawHand} -> {OverallStrength}";
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<Hand> hands = inputs.Select(Hand.Parse).ToList();
            hands.Sort();
            return hands.Select((card, index) => card.Bid * (long)(index + 1)).Sum().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
            // 251029672 [too low]

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}