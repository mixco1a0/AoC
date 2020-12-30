using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2020
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
                case Part.Two:
                    return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }
        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "5",
                RawInput =
@"mxmxvkd kfcds sqjhc nhms (contains dairy, fish)
trh fvjkl sbzzf mxmxvkd (contains dairy)
sqjhc fvjkl (contains soy)
sqjhc mxmxvkd sbzzf (contains fish)"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "mxmxvkd,sqjhc,fvjkl",
                RawInput =
@"mxmxvkd kfcds sqjhc nhms (contains dairy, fish)
trh fvjkl sbzzf mxmxvkd (contains dairy)
sqjhc fvjkl (contains soy)
sqjhc mxmxvkd sbzzf (contains fish)"
            });
            return testData;
        }

        class Food
        {
            public string AllIngredients { get; set; }
            public List<string> Ingredients { get; set; }
            public string AllAllergens { get; set; }
            public List<string> Allergens { get; set; }

            public void Parse()
            {
                Ingredients = AllIngredients.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                Allergens = AllAllergens.Replace("contains", "").Split(" ,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            public override string ToString()
            {
                return $"{AllIngredients} ({AllAllergens})";
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<string> ingredients = new List<string>();
            List<string> allergens = new List<string>();
            List<Food> foods = new List<Food>();
            foreach (string input in inputs)
            {
                string[] split = input.Split("()".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foods.Add(new Food { AllIngredients = split[0], AllAllergens = split[1] });
                foods.Last().Parse();

                ingredients.AddRange(foods.Last().Ingredients);
                allergens.AddRange(foods.Last().Allergens);
            }

            ingredients = ingredients.Distinct().ToList();
            allergens = allergens.Distinct().ToList();

            List<string> knownBads = new List<string>();
            foreach (string allergen in allergens)
            {
                IEnumerable<Food> tempFoods = foods.Where(f => f.Allergens.Contains(allergen));
                if (tempFoods.Count() > 0)
                {
                    List<List<string>> possibilities = tempFoods.Select(f => f.Ingredients).Distinct().ToList();
                    List<string> trimmed = null;
                    foreach (List<string> curPos in possibilities)
                    {
                        if (trimmed == null)
                        {
                            trimmed = new List<string>(curPos);
                        }
                        else
                        {
                            trimmed = trimmed.Intersect(curPos).ToList();
                        }
                    }
                    knownBads.AddRange(trimmed);
                }
            }
            List<string> possibleGoods = ingredients.Where(i => !knownBads.Contains(i)).ToList();
            long sum = 0;
            foreach (Food food in foods)
            {
                sum += food.Ingredients.Where(i => possibleGoods.Contains(i)).Count();
            }
            return sum.ToString();
        }


        class KnownAllergen
        {
            public string Ingredient { get; set; }
            public string Allergen { get; set; }
            public override string ToString()
            {
                return $"{Ingredient} = {Allergen}";
            }
        }

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<string> ingredients = new List<string>();
            List<string> allergens = new List<string>();
            List<Food> foods = new List<Food>();
            foreach (string input in inputs)
            {
                string[] split = input.Split("()".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foods.Add(new Food { AllIngredients = split[0], AllAllergens = split[1] });
                foods.Last().Parse();

                ingredients.AddRange(foods.Last().Ingredients);
                allergens.AddRange(foods.Last().Allergens);
            }

            ingredients = ingredients.Distinct().ToList();
            allergens = allergens.Distinct().ToList();

            List<string> knownBads = new List<string>();
            Dictionary<string, List<string>> allergenToIngredients = new Dictionary<string, List<string>>();
            foreach (string allergen in allergens)
            {
                IEnumerable<Food> tempFoods = foods.Where(f => f.Allergens.Contains(allergen));
                if (tempFoods.Count() > 0)
                {
                    List<List<string>> possibilities = tempFoods.Select(f => f.Ingredients).Distinct().ToList();
                    List<string> trimmed = null;
                    foreach (List<string> curPos in possibilities)
                    {
                        if (trimmed == null)
                        {
                            trimmed = new List<string>(curPos);
                        }
                        else
                        {
                            trimmed = trimmed.Intersect(curPos).ToList();
                        }
                    }
                    knownBads.AddRange(trimmed);
                    allergenToIngredients[allergen] = trimmed;
                }
            }

            List<KnownAllergen> knownAllergens = new List<KnownAllergen>();
            while (true)
            {
                if (allergenToIngredients.Count == 0)
                {
                    break;
                }

                var solved = allergenToIngredients.Where(pair => pair.Value.Count == 1).ToList();
                foreach (var pair in solved)
                {
                    knownAllergens.Add(new KnownAllergen { Allergen = pair.Key, Ingredient = pair.Value.First() });
                    allergenToIngredients.Remove(pair.Key);
                    foreach (var curPair in allergenToIngredients)
                    {
                        curPair.Value.Remove(knownAllergens.Last().Ingredient);
                    }
                }
            }

            return string.Join(',', knownAllergens.OrderBy(ka => ka.Allergen).Select(ka => ka.Ingredient));
        }
    }
}