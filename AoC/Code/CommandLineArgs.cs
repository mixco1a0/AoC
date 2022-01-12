using System.Collections.Generic;
using System.Linq;

using AoC.Core;

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
            RunWarmup,
            SkipLatest,
            ShowPerf,
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
                SupportedArgument.RunWarmup,
                new List<string>{"rw", "runwarmup"}
            },
            {
                SupportedArgument.SkipLatest,
                new List<string>{"sl", "skiplatest"}
            },
            {
                SupportedArgument.ShowPerf,
                new List<string>{"sp", "showperf"}
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

        public void Print()
        {
            if (Args.Count > 0)
            {
                Logger.WriteLine(Logger.ELogLevel.Info, "Command line arguments");
                foreach (KeyValuePair<SupportedArgument, string> argPair in Args)
                {
                    Logger.WriteLine(Logger.ELogLevel.Info, $"     -{string.Format("{0,-2}", SupportedArgs[argPair.Key].First())} {argPair.Value}");
                }
            }

            if (InvalidArgs.Count > 0)
            {
                Logger.WriteLine(Logger.ELogLevel.Error, "\nInvalid arguments");
                foreach (string invalidArg in InvalidArgs)
                {
                    Logger.WriteLine(Logger.ELogLevel.Info, $"     -{invalidArg}");
                }
            }

            if (DuplicateArgs.Count > 0)
            {
                Logger.WriteLine(Logger.ELogLevel.Warn, "\nDuplicate arguments");
                foreach (string duplicateArg in DuplicateArgs)
                {
                    Logger.WriteLine(Logger.ELogLevel.Info, $"     -{duplicateArg}");
                }
            }
            Logger.WriteLine(Logger.ELogLevel.Info, "");
        }

        public void PrintHelp()
        {
            Logger.WriteLine(Logger.ELogLevel.Info, "Supported command line arguments");
            foreach (KeyValuePair<SupportedArgument, List<string>> pair in SupportedArgs)
            {
                Logger.WriteLine(Logger.ELogLevel.Info, $"     {pair.Key.ToString()}");
                foreach (string val in pair.Value)
                {
                    Logger.WriteLine(Logger.ELogLevel.Info, $"          -{val}");
                }
                Logger.WriteLine(Logger.ELogLevel.Info, "");
            }
            Logger.WriteLine(Logger.ELogLevel.Info, "");
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