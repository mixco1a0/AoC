using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day21 : Day
    {
        public Day21() { }
        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                case Part.One:
                    return "v1";
                // case Part.Two:
                //     return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }
        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }
        static string store =
@"Weapons:    Cost  Damage  Armor
Dagger        8     4       0
Shortsword   10     5       0
Warhammer    25     6       0
Longsword    40     7       0
Greataxe     74     8       0

Armor:      Cost  Damage  Armor
Leather      13     0       1
Chainmail    31     0       2
Splintmail   53     0       3
Bandedmail   75     0       4
Platemail   102     0       5

Rings:      Cost  Damage  Armor
Damage+1    25     1       0
Damage+2    50     2       0
Damage+3   100     3       0
Defense+1   20     0       1
Defense+2   40     0       2
Defense+3   80     0       3";

        enum ItemType
        {
            Invalid,
            Weapon,
            Armor,
            Ring
        }

        record Item(string Name, int Cost, int Damage, int Armor);

        private void ParseStore(out List<Item> weapons, out List<Item> armor, out List<Item> rings)
        {
            weapons = new List<Item>();
            armor = new List<Item>();
            rings = new List<Item>();

            ItemType cur = ItemType.Invalid;
            foreach (string line in store.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Contains(":"))
                {
                    ++cur;
                    continue;
                }

                string[] parts = line.Split("\n\r ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                switch (cur)
                {
                    case ItemType.Weapon:
                        weapons.Add(new Item(parts[0], int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3])));
                        break;
                    case ItemType.Armor:
                        armor.Add(new Item(parts[0], int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3])));
                        break;
                    case ItemType.Ring:
                        rings.Add(new Item(parts[0], int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3])));
                        break;
                }
            }
        }

        record Attacker(int HP, int Damage, int Armor);

        bool RunBattleSimulation(Attacker player, Attacker boss)
        {
            int playerHP = player.HP;
            int bossHP = boss.HP;
            bool playerTurn = true;
            while (playerHP > 0 && bossHP > 0)
            {
                if (playerTurn)
                {
                    bossHP -= (player.Damage - boss.Armor);
                }
                else
                {
                    playerHP -= (boss.Damage - player.Armor);
                }
                playerTurn = !playerTurn;
            }
            return playerHP > bossHP;
        }

        int GetBestPriceForRing(Item weapon, Item armor, Item ring, Attacker boss, List<Item> rings)
        {
            int bestPrice = int.MaxValue;
            if (RunBattleSimulation(new Attacker(100, weapon.Damage + ring.Damage, armor.Armor + ring.Armor), boss))
            {
                bestPrice = weapon.Cost + armor.Cost + ring.Cost;
                DebugWriteLine($"Successful battle! {weapon.Name} + {armor.Name} + {ring.Name} @ ${bestPrice}");
            }

            if (rings != null)
            {
                foreach (Item nextRing in rings)
                {
                    Item combinedRings = new Item($"{ring.Name}_{nextRing.Name}", ring.Cost + nextRing.Cost, ring.Damage + nextRing.Damage, ring.Armor + nextRing.Armor);
                    bestPrice = Math.Min(bestPrice, GetBestPriceForRing(weapon, armor, combinedRings, boss, null));
                }
            }
            return bestPrice;
        }

        int GetBestPriceForArmor(Item weapon, Item armor, Attacker boss, List<Item> rings)
        {
            bool armorEnough = false;
            int bestPrice = int.MaxValue;
            // get price for armor, if successful, ignore ring check with armor
            if (RunBattleSimulation(new Attacker(100, weapon.Damage, armor.Armor), boss))
            {
                armorEnough = true;
                bestPrice = weapon.Cost + armor.Cost;
                DebugWriteLine($"Successful battle! {weapon.Name} + {armor.Name} @ ${bestPrice}");
            }

            // get price without armor
            Item nullArmor = new Item("NA", 0, 0, 0);
            foreach (Item ring in rings)
            {
                if (!armorEnough)
                {
                    bestPrice = Math.Min(bestPrice, GetBestPriceForRing(weapon, armor, ring, boss, rings.Where(r => r.Name != ring.Name).ToList()));
                }
                bestPrice = Math.Min(bestPrice, GetBestPriceForRing(weapon, nullArmor, ring, boss, rings.Where(r => r.Name != ring.Name).ToList()));
            }
            return bestPrice;
        }

        int GetBestPriceForWeapon(Item weapon, Attacker boss, List<Item> armors, List<Item> rings)
        {
            int bestPrice = int.MaxValue;
            int d = weapon.Damage, a = 0;
            // if weapon is enough, dont spend any more
            if (RunBattleSimulation(new Attacker(100, d, a), boss))
            {
                DebugWriteLine($"Successful battle! {weapon.Name} + @ ${weapon.Cost}");
                return weapon.Cost;
            }
            else
            {
                foreach (Item armor in armors)
                {
                    bestPrice = Math.Min(bestPrice, GetBestPriceForArmor(weapon, armor, boss, rings));
                }
            }
            return bestPrice;
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<int> bossVals = inputs.Select(i => int.Parse(i.Split(" :".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last())).ToList();
            Attacker boss = new Attacker(bossVals[0], bossVals[1], bossVals[2]);

            List<Item> weapons, armors, rings;
            ParseStore(out weapons, out armors, out rings);
            int bestPrice = int.MaxValue;
            for (int w = 0; w < weapons.Count; ++w)
            {
                bestPrice = Math.Min(bestPrice, GetBestPriceForWeapon(weapons[w], boss, armors, rings));
            }
            return bestPrice.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            return "";
        }
    }
}