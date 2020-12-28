using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC
{
    public class CommandLineArgs
    {
        public enum SupportedArgument
        {
            Invalid,
            Help,
            Namespace,
            Day,
            SkipLatest,
            RunPerf,
            PerfRecordCount,
            PerfTimeout
        }

        private static Dictionary<SupportedArgument, List<string>> SupportedArgs = new Dictionary<SupportedArgument, List<string>>()
        {
            {
                SupportedArgument.Help,
                new List<string>{"?", "h", "help"}
            },
            {
                SupportedArgument.Namespace,
                new List<string>{"n", "namespace"}
            },
            {
                SupportedArgument.Day,
                new List<string>{"d", "day"}
            },
            {
                SupportedArgument.SkipLatest,
                new List<string>{"sl", "skiplatest"}
            },
            {
                SupportedArgument.RunPerf,
                new List<string>{"rp", "runperf"}
            },
            {
                SupportedArgument.PerfRecordCount,
                new List<string>{"prc", "recordcount"}
            },
            {
                SupportedArgument.PerfTimeout,
                new List<string>{"t", "timeout"}
            }
        };

        public Dictionary<SupportedArgument, string> Args { get; private set; }
        private List<string> DuplicateArgs { get; set; }
        private List<string> InvalidArgs { get; set; }

        public CommandLineArgs(string[] args)
        {
            Args = new Dictionary<SupportedArgument, string>();
            DuplicateArgs = new List<string>();
            InvalidArgs = new List<string>();

            SupportedArgument curSupportedArg = SupportedArgument.Invalid;
            foreach (string arg in args)
            {
                if (arg[0] == '-')
                {
                    curSupportedArg = SupportedArgument.Invalid;
                    string curArg = arg[1..];
                    SupportedArgument supportedArg = SupportedArgs.Where(pair => pair.Value.Contains(curArg)).Select(pair => pair.Key).FirstOrDefault();
                    if (supportedArg == SupportedArgument.Invalid)
                    {
                        InvalidArgs.Add(curArg);
                    }
                    else if (Args.ContainsKey(supportedArg))
                    {
                        DuplicateArgs.Add(curArg);
                    }
                    else
                    {
                        curSupportedArg = supportedArg;
                        Args[supportedArg] = string.Empty;
                    }
                }
                else
                {
                    if (curSupportedArg != SupportedArgument.Invalid)
                    {
                        Args[curSupportedArg] = arg;
                    }
                    curSupportedArg = SupportedArgument.Invalid;
                }
            }
        }

        public void Print(Action<string> PrintFunc)
        {
            if (Args.Count > 0)
            {
                PrintFunc("Command line arguments");
                foreach (KeyValuePair<SupportedArgument, string> argPair in Args)
                {
                    PrintFunc($"     -{string.Format("{0,-2}", SupportedArgs[argPair.Key].First())} {argPair.Value}");
                }
            }

            if (InvalidArgs.Count > 0)
            {
                PrintFunc("");
                PrintFunc("Invalid arguments");
                foreach (string invalidArg in InvalidArgs)
                {
                    PrintFunc($"     -{invalidArg}");
                }
            }

            if (DuplicateArgs.Count > 0)
            {
                PrintFunc("");
                PrintFunc("Duplicate arguments");
                foreach (string duplicateArg in DuplicateArgs)
                {
                    PrintFunc($"     -{duplicateArg}");
                }
            }
            PrintFunc("");
        }

        public void PrintHelp(Action<string> PrintFunc)
        {
            PrintFunc("Supported command line arguments");
            foreach (KeyValuePair<SupportedArgument, List<string>> pair in SupportedArgs)
            {
                PrintFunc($"     {pair.Key.ToString()}");
                foreach (string val in pair.Value)
                {
                    PrintFunc($"          -{val}");
                }
                PrintFunc("");
            }
            PrintFunc("");
        }

        public bool HasArg(SupportedArgument argType)
        {
            return Args.ContainsKey(argType);
        }

        public bool HasArgValue(SupportedArgument argType)
        {
            return HasArg(argType) && !string.IsNullOrWhiteSpace(Args[argType]);
        }
    }
}