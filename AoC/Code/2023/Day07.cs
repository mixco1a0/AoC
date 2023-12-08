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
                case Core.Part.One:
                    return "v1";
                case Core.Part.Two:
                    return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        public override bool SkipTestData => true;

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
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "5905",
                RawInput =
@"32T3K 765
T55J5 684
KK677 28
KTJJT 220
QQQJA 483"
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

            public static bool UseJokers { get; set; }

            public Strength OverallStrength { get; set; }
            public Strength JokerStrength { get; set; }
            public string RawHand { get; set; }
            public string ComparableHand { get; set; }
            public string SortedHand { get; set; }
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

            private static Dictionary<char, char> JokerCompare = new Dictionary<char, char>()
            {
                {'J', 'a'},
                {'2', 'b'},
                {'3', 'c'},
                {'4', 'd'},
                {'5', 'e'},
                {'6', 'f'},
                {'7', 'g'},
                {'8', 'h'},
                {'9', 'i'},
                {'T', 'j'},
                {'Q', 'k'},
                {'K', 'l'},
                {'A', 'm'},
            };

            public Hand()
            {
                OverallStrength = Strength.Invalid;
                JokerStrength = Strength.Invalid;
                RawHand = string.Empty;
                ComparableHand = string.Empty;
                Bid = 0;
            }

            public static Hand Parse(string input)
            {
                Hand hand = new Hand();
                string[] split = Util.String.Split(input, ' ');
                hand.RawHand = split[0];
                if (UseJokers)
                {
                    hand.ComparableHand = string.Join("", hand.RawHand.Select(rh => JokerCompare[rh]));
                }
                else
                {
                    hand.ComparableHand = string.Join("", hand.RawHand.Select(rh => EasyCompare[rh]));
                }
                hand.SortedHand = string.Concat(hand.RawHand.OrderBy(c => c));
                hand.Bid = long.Parse(split[1]);
                hand.SetOverallStrength();
                if (UseJokers)
                {
                    hand.SetJokerStrength();
                }
                return hand;
            }

            private void SetOverallStrength()
            {
                Dictionary<char, int> chars = RawHand.Distinct().ToDictionary(card => card, card => RawHand.Where(c => c == card).Count());
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

            private void SetJokerStrength()
            {
                if (!UseJokers)
                {
                    JokerStrength = Strength.Invalid;
                    return;
                }

                if (!RawHand.Contains('J'))
                {
                    JokerStrength = OverallStrength;
                    return;
                }

                int jokerCount = RawHand.Where(card => card == 'J').Count();
                Dictionary<char, int> chars = RawHand.Where(card => card != 'J').Distinct().ToDictionary(card => card, card => RawHand.Where(c => c == card).Count());
                switch (chars.Count)
                {
                    case 4:
                        // overall strength -> high card
                        JokerStrength = Strength.OnePair;
                        break;
                    case 3:
                        // overall strength -> pair (joker), pair (non joker)
                        JokerStrength = Strength.ThreeOfKind;
                        break;
                    case 2:
                        if (jokerCount == 1)
                        {
                            if (chars.Values.Max() == 2)
                            {
                                JokerStrength = Strength.FullHouse;
                            }
                            else
                            {
                                JokerStrength = Strength.FourOfKind;
                            }
                        }
                        else
                        {
                            JokerStrength = Strength.FourOfKind;
                        }
                        break;
                    case 1:
                        // n non jokers
                        JokerStrength = Strength.FiveOfKind;
                        break;
                    case 0:
                        // 5 jokers
                        JokerStrength = Strength.FiveOfKind;
                        break;
                }
            }

            int IComparable<Hand>.CompareTo(Hand other)
            {
                Strength s = UseJokers ? JokerStrength : OverallStrength;
                Strength os = UseJokers ? other.JokerStrength : other.OverallStrength;
                if (s == os)
                {
                    for (int i = 0; i < ComparableHand.Length; ++i)
                    {
                        if (ComparableHand[i] == other.ComparableHand[i])
                        {
                            continue;
                        }
                        return ComparableHand[i].CompareTo(other.ComparableHand[i]);
                    }
                }

                return os.CompareTo(s);
            }

            public override string ToString()
            {
                if (UseJokers)
                {
                    string noJokers = string.Concat(SortedHand.Where(s => s != 'J'));
                    return $"{noJokers} -> {JokerStrength}";
                }
                return $"{SortedHand} -> {OverallStrength}";
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool useJokers)
        {
            Hand.UseJokers = useJokers;
            List<Hand> hands = inputs.Select(Hand.Parse).ToList();
            hands.Sort();
            return hands.Select((card, index) => card.Bid * (long)(index + 1)).Sum().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}