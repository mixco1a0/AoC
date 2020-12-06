using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace _2020
{
    class Day04 : Day
    {
        public Day04() : base() {}

        protected override string GetDay() { return nameof(Day04).ToLower(); }

        protected override void RunPart1Solution(List<string> inputs)
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

            LogAnswer($"{validPassports} valid passports");
        }

        protected override void RunPart2Solution(List<string> inputs)
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

            LogAnswer($"{validPassports} valid passports");
        }
        
        private bool CheckIsValid(string passportData)
        {
            List<string> requiredFields = new List<string> {"byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid"};
            Dictionary<string, string> fields = passportData.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToDictionary(str => str.Substring(0, 3));
            foreach(string requiredField in requiredFields)
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
            requiredFieldChecks["byr"] = CheckIsValidBYR;
            requiredFieldChecks["iyr"] = CheckIsValidIYR;
            requiredFieldChecks["eyr"] = CheckIsValidEYR;
            requiredFieldChecks["hgt"] = CheckIsValidHGT;
            requiredFieldChecks["hcl"] = CheckIsValidHCL;
            requiredFieldChecks["ecl"] = CheckIsValidECL;
            requiredFieldChecks["pid"] = CheckIsValidPID;

            Dictionary<string, string> fields = passportData.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToDictionary(str => str.Substring(0, 3), str => str.Substring(4));
            foreach(string requiredField in requiredFieldChecks.Keys)
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

        private bool CheckIsValidBYR(string val)
        {
            int year;
            if (val.Length != 4 || !int.TryParse(val, out year))
            {
                return false;
            }
            return year >= 1920 && year <= 2002;
        }

        private bool CheckIsValidIYR(string val)
        {
            int year;
            if (val.Length != 4 || !int.TryParse(val, out year))
            {
                return false;
            }
            return year >= 2010 && year <= 2020;
        }

        private bool CheckIsValidEYR(string val)
        {
            int year;
            if (val.Length != 4 || !int.TryParse(val, out year))
            {
                return false;
            }
            return year >= 2020 && year <= 2030;
        }

        struct MinMax
        {
            public int min;
            public int max;
            public bool InRange(int val) { return val >= min && val <= max;}
        };

        private bool CheckIsValidHGT(string val)
        {
            Dictionary<string, MinMax> valids = new Dictionary<string, MinMax>{{"cm", new MinMax{min=150, max=193}},{"in", new MinMax{min=59, max=76}}};
            string height = val.Substring(0, val.Length-2);
            string unit = val.Substring(val.Length-2);
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

            string code = val.Substring(1);
            int hex;
            if (code.Length != 6 || !int.TryParse(code, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out hex))
            {
                return false;
            }
            return true;
        }

        private bool CheckIsValidECL(string val)
        {
            List<string> valids = new List<string> {"amb", "blu", "brn", "gry", "grn", "hzl", "oth"};
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