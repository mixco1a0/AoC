using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2015
{
    class Day22 : Core.Day
    {
        public Day22() { }

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
                Output = "",
                RawInput =
@""
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

        record Effect(int ID, int Turns, int Armor, int Damage, int Mana) { }
        static List<Effect> AllEffects = new List<Effect>()
        {
            new Effect(0, 6, 7, 0, 0),
            new Effect(1, 6, 0, 3, 0),
            new Effect(2, 5, 0, 0, 101)
        };

        record Spell(string Name, int Cost, int Damage, int Heal, Effect Effect) { }
        static List<Spell> AllSpells = new List<Spell>()
        {
            new Spell("Magic Missile", 53, 4, 0, null),
            new Spell("Drain", 73, 2, 2, null),
            new Spell("Shield", 113, 0, 0, AllEffects[0]),
            new Spell("Poison", 173, 0, 0, AllEffects[1]),
            new Spell("Recharge", 229, 0, 0, AllEffects[2])
        };

        class Boss
        {
            public Boss(Boss boss)
            {
                HP = boss.HP;
                Damage = boss.Damage;
            }
            public Boss(int hp, int damage)
            {
                HP = hp;
                Damage = damage;
            }

            public int HP { get; set; }
            public int Damage { get; set; }
        }

        class Player
        {
            public Player(Player player)
            {
                HP = player.HP;
                Mana = player.Mana;
                Armor = player.Armor;
                Effects = player.Effects.ToDictionary(p => p.Key, p => p.Value);
            }
            public Player(int hp, int mana, int armor, Dictionary<int, int> effects)
            {
                HP = hp;
                Mana = mana;
                Armor = armor;
                Effects = effects.ToDictionary(p => p.Key, p => p.Value);
            }
            public int HP { get; set; }
            public int Mana { get; set; }
            public int Armor { get; set; }
            public Dictionary<int, int> Effects { get; set; }
        }

        bool RunCombatSimulation(bool hardMode, Boss boss, Player player, int turnCount, int spentMana, ref int minMana)
        {
            string curTab = new string('-', turnCount * 2);

            if (hardMode && turnCount % 2 == 0)
            {
                player.HP -= 1;
            }

            if (player.HP <= 0)
            {
                // Core.Log.WriteLine(Core.Log.ELevel.Spam, $"{curTab}Player is dead!");
                return false;
            }

            if (boss.HP <= 0)
            {
                // Core.Log.WriteLine(Core.Log.ELevel.Spam, $"{curTab}Boss is dead!");
                minMana = Math.Min(minMana, spentMana);
                return true;
            }

            if (spentMana > minMana)
            {
                // Core.Log.WriteLine(Core.Log.ELevel.Spam, $"{curTab}Too much mana!");
                return false;
            }

            // Core.Log.WriteLine(Core.Log.ELevel.Spam, $"{curTab}Turn {turnCount} - Boss HP = {boss.HP} - Player HP = {player.HP}, Mana = {player.Mana}");

            List<KeyValuePair<int, int>> curEffects = player.Effects.Select(p => new KeyValuePair<int, int>(p.Key, p.Value)).ToList();
            // resolve effects now
            foreach (KeyValuePair<int, int> pair in player.Effects)
            {
                Effect curEffect = AllEffects[pair.Key];
                switch (curEffect.ID)
                {
                    case 0:
                        player.Armor += curEffect.Armor;
                        // Core.Log.WriteLine(Core.Log.ELevel.Spam, $"{curTab}Armor!");
                        break;
                    case 1:
                        boss.HP -= curEffect.Damage;
                        // Core.Log.WriteLine(Core.Log.ELevel.Spam, $"{curTab}Poison boss for {curEffect.Damage}!");
                        break;
                    case 2:
                        player.Mana += curEffect.Mana;
                        // Core.Log.WriteLine(Core.Log.ELevel.Spam, $"{curTab}Recover {curEffect.Mana} mana!");
                        break;
                }
            }
            // remove a remaining turn
            player.Effects = player.Effects.Where(p => p.Value > 1).ToDictionary(p => p.Key, p => p.Value - 1);

            if (turnCount % 2 == 0)
            {
                //reset armor
                player.Armor = 0;

                // player turn
                foreach (Spell spell in AllSpells)
                {
                    if (player.Mana >= spell.Cost)
                    {
                        Player nextPlayer = new Player(player);
                        if (spell.Effect != null && player.Effects.ContainsKey(spell.Effect.ID))
                        {
                            continue;
                        }
                        // Core.Log.WriteLine(Core.Log.ELevel.Spam, $"{curTab}Casting {spell.Name}!");

                        nextPlayer.Mana -= spell.Cost;
                        nextPlayer.HP += spell.Heal;
                        if (spell.Effect != null)
                        {
                            nextPlayer.Effects.Add(spell.Effect.ID, spell.Effect.Turns);
                        }

                        Boss nextBoss = new Boss(boss);
                        nextBoss.HP -= spell.Damage;

                        RunCombatSimulation(hardMode, nextBoss, nextPlayer, turnCount + 1, spentMana + spell.Cost, ref minMana);
                    }
                }

                // wasn't able to cast a spell!
                return false;
            }

            // Core.Log.WriteLine(Core.Log.ELevel.Spam, $"{curTab}Boss hitting for {Math.Max(1, boss.Damage - player.Armor)} damage!");
            // boss turn
            player.HP -= Math.Max(1, boss.Damage - player.Armor);

            // reset armor
            player.Armor = 0;

            return RunCombatSimulation(hardMode, boss, player, turnCount + 1, spentMana, ref minMana);
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int minMana = int.MaxValue;
            List<int> bossVals = inputs.Select(i => int.Parse(i.Split(" :".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last())).ToList();
            Boss boss = new Boss(bossVals[0], bossVals[1]);
            Player player = new Player(50, 500, 0, new Dictionary<int, int>());
            RunCombatSimulation(false, boss, player, 0, 0, ref minMana);
            return minMana.ToString();
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            int minMana = int.MaxValue;
            List<int> bossVals = inputs.Select(i => int.Parse(i.Split(" :".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Last())).ToList();
            Boss boss = new Boss(bossVals[0], bossVals[1]);
            Player player = new Player(50, 500, 0, new Dictionary<int, int>());
            RunCombatSimulation(true, boss, player, 0, 0, ref minMana);
            return minMana.ToString();
        }
    }
}