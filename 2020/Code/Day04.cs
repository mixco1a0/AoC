using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2020
{
    class Day04 : Day
    {
        public Day04() : base() { }

        protected override string GetDay() { return nameof(Day04).ToLower(); }

        protected override string GetPart1ExampleInput()
        {
            return
@"ecl:gry pid:860033327 eyr:2020 hcl:#fffffd
byr:1937 iyr:2017 cid:147 hgt:183cm

iyr:2013 ecl:amb cid:350 eyr:2023 pid:028048884
hcl:#cfa07d byr:1929

hcl:#ae17e1 iyr:2013
eyr:2024
ecl:brn pid:760753108 byr:1931
hgt:179cm

hcl:#cfa07d eyr:2025 pid:166559648
iyr:2011 ecl:brn hgt:59in";
        }
        protected override string GetPart1ExampleAnswer() { return "2"; }
        protected override string RunPart1Solution(List<string> inputs)
        {
            int validPassports = 0;
            string curInfo = "";
            foreach (string input in inputs)
            {
                curInfo += $"{input} ";
                if (string.IsNullOrWhiteSpace(input))
                {
                    if (CheckIsValid(curInfo))
                    {
                        ++validPassports;
                    }
                    curInfo = "";
                }
            }

            if (CheckIsValid(curInfo))
            {
                ++validPassports;
            }

            return validPassports.ToString();
        }

        // TODO: this is getting the wrong answer
        protected override string GetPart2ExampleInput()
        {
            return
@"eyr:1972 cid:100
hcl:#18171d ecl:amb hgt:170 pid:186cm iyr:2018 byr:1926

iyr:2019
hcl:#602927 eyr:1967 hgt:170cm
ecl:grn pid:012533040 byr:1946

hcl:dab227 iyr:2012
ecl:brn hgt:182cm pid:021572410 eyr:2020 byr:1992 cid:277

hgt:59cm ecl:zzz
eyr:2038 hcl:74454a iyr:2023
pid:3556412378 byr:2007

pid:087499704 hgt:74in ecl:grn iyr:2012 eyr:2030 byr:1980
hcl:#623a2f

eyr:2029 ecl:blu cid:129 byr:1989
iyr:2014 pid:896056539 hcl:#a97842 hgt:165cm

hcl:#888785
hgt:164cm byr:2001 iyr:2015 cid:88
pid:545766238 ecl:hzl
eyr:2022

iyr:2010 hgt:158cm hcl:#b6652a ecl:blu byr:1944 eyr:2021 pid:093154719";
        }
        protected override string GetPart2ExampleAnswer() { return "4"; }
        protected override string RunPart2Solution(List<string> inputs)
        {
            int validPassports = 0;
            string curInfo = "";
            foreach (string input in inputs)
            {
                curInfo += $"{input} ";
                if (string.IsNullOrWhiteSpace(input))
                {
                    if (CheckIsValidStringent(curInfo))
                    {
                        ++validPassports;
                    }
                    curInfo = "";
                }
            }

            if (CheckIsValidStringent(curInfo))
            {
                ++validPassports;
            }

            return validPassports.ToString();
        }

        private bool CheckIsValid(string passportData)
        {
            List<string> requiredFields = new List<string> { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };
            Dictionary<string, string> fields = passportData.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToDictionary(str => str[0..3]);
            foreach (string requiredField in requiredFields)
            {
                if (!fields.ContainsKey(requiredField))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckIsValidStringent(string passportData)
        {
            Dictionary<string, Func<string, bool>> requiredFieldChecks = new Dictionary<string, Func<string, bool>>();
            requiredFieldChecks["byr"] = val => CheckIsValid(val, 4, 1920, 2002);
            requiredFieldChecks["iyr"] = val => CheckIsValid(val, 4, 2010, 2020);
            requiredFieldChecks["eyr"] = val => CheckIsValid(val, 4, 2020, 2030);
            requiredFieldChecks["hgt"] = CheckIsValidHGT;
            requiredFieldChecks["hcl"] = CheckIsValidHCL;
            requiredFieldChecks["ecl"] = CheckIsValidECL;
            requiredFieldChecks["pid"] = CheckIsValidPID;

            Dictionary<string, string> fields = passportData.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToDictionary(str => str.Substring(0, 3), str => str.Substring(4));
            foreach (string requiredField in requiredFieldChecks.Keys)
            {
                if (!fields.ContainsKey(requiredField))
                {
                    return false;
                }
                else if (!requiredFieldChecks[requiredField](fields[requiredField]))
                {
                    return false;
                }
            }
            return true;
        }

        struct MinMax
        {
            public int min;
            public int max;
            public bool InRange(int val) { return val >= min && val <= max; }
        };

        private bool CheckIsValid(string val, int len, int min, int max)
        {
            int valInt;
            if (val.Length != len || !int.TryParse(val, out valInt))
            {
                return false;
            }
            return new MinMax { min = min, max = max }.InRange(valInt);
        }

        private bool CheckIsValidHGT(string val)
        {
            Dictionary<string, MinMax> valids = new Dictionary<string, MinMax> { { "cm", new MinMax { min = 150, max = 193 } }, { "in", new MinMax { min = 59, max = 76 } } };
            string height = val[0..^2];
            string unit = val[^2..];
            int heightVal;
            if (!valids.Keys.Contains(unit) || !int.TryParse(height, out heightVal))
            {
                return false;
            }
            return valids[unit].InRange(heightVal);
        }

        private bool CheckIsValidHCL(string val)
        {
            if (val.ElementAt(0) != '#')
            {
                return false;
            }

            string code = val[1..];
            int hex;
            if (code.Length != 6 || !int.TryParse(code, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out hex))
            {
                return false;
            }
            return true;
        }

        private bool CheckIsValidECL(string val)
        {
            HashSet<string> valids = new HashSet<string> { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };
            return valids.Contains(val);
        }

        private bool CheckIsValidPID(string val)
        {
            int id;
            if (val.Length != 9 || !int.TryParse(val, out id))
            {
                return false;
            }
            return true;
        }
    }
}