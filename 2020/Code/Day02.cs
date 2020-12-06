using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace _2020
{
    class Day02 : Day
    {
        public Day02() : base() {}
        
        protected override string GetDay() { return nameof(Day02).ToLower(); }

        protected override void RunPart1Solution(List<string> inputs)
        {
            int validPasswords = 0;
            foreach(string input in inputs)
            {
                string[] split = input.Split("-: ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 4)
                {
                    continue;
                }

                int min;
                if (!int.TryParse(split[0], out min))
                {
                    continue;
                }

                int max;
                if (!int.TryParse(split[1], out max))
                {
                    continue;
                }

                string removedLetters = split[3].Replace(split[2], "");
                int diff = split[3].Length - removedLetters.Length;
                if (diff >= min && diff <= max)
                {
                    ++validPasswords;
                    //Debug($"Valid password found: {input} [{split[2]} was found {diff} times]");
                }
            }
            
            LogAnswer($"{validPasswords} valid passwords");
        }

        protected override void RunPart2Solution(List<string>inputs)
        {
            int validPasswords = 0;
            foreach(string input in inputs)
            {
                string[] split = input.Split("-: ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 4)
                {
                    continue;
                }

                int idx1;
                if (!int.TryParse(split[0], out idx1))
                {
                    continue;
                }
                --idx1;

                int idx2;
                if (!int.TryParse(split[1], out idx2))
                {
                    continue;
                }
                --idx2;

                if (split[2].Length > 1)
                {
                    continue;
                }

                char letter = split[2][0];
                string password = split[3];

                char loc1 = password.ElementAtOrDefault(idx1);
                char loc2 = password.ElementAtOrDefault(idx2);
                if (loc1 == letter && loc2 != letter)
                {
                    ++validPasswords;
                    //Debug($"Valid password found: {input} [{letter} was found at index {idx1+1}]");
                }
                else if (loc1 != letter && loc2 == letter)
                {
                    ++validPasswords;
                    //Debug($"Valid password found: {input} [{letter} was found at index {idx2+1}]");
                }
            }
            
            LogAnswer($"{validPasswords} valid passwords");
        }
    }
}