using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC.Core
{
    public class CommandLine : Dictionary<CommandLine.ESupportedArgument, string>
    {
        public enum ESupportedArgument
        {
            Invalid,
            Help,
            LogLevel,
            Namespace,
            Day,
            RunWarmup,
            SkipLatest,
            ShowPerf,
            RunPerf,
            PerfRecordCount,
            PerfTimeout,
            End
        }

        private Dictionary<ESupportedArgument, List<string>> SupportedArgs;
        private List<string> DuplicateArgs { get; set; }
        private List<string> InvalidArgs { get; set; }
        private string Tab => "     ";

        public CommandLine(string[] args)
        {
            SupportedArgs = new Dictionary<ESupportedArgument, List<string>>();
            for (ESupportedArgument arg = ESupportedArgument.Help; arg != ESupportedArgument.End; ++arg)
            {
                SupportedArgs[arg] = new List<string>();
                SupportedArgs[arg].Add(string.Join("", arg.ToString().Where(a => char.IsUpper(a))).ToLower());
                SupportedArgs[arg].Add(arg.ToString().ToLower());
            }
            SupportedArgs[ESupportedArgument.Help].Add("?");

            DuplicateArgs = new List<string>();
            InvalidArgs = new List<string>();

            ESupportedArgument curSupportedArg = ESupportedArgument.Invalid;
            foreach (string arg in args)
            {
                if (arg[0] == '-')
                {
                    curSupportedArg = ESupportedArgument.Invalid;
                    string curArg = arg.Substring(1);
                    ESupportedArgument supportedArg = SupportedArgs.Where(pair => pair.Value.Contains(curArg)).Select(pair => pair.Key).FirstOrDefault();
                    if (supportedArg == ESupportedArgument.Invalid)
                    {
                        InvalidArgs.Add(curArg);
                    }
                    else if (ContainsKey(supportedArg))
                    {
                        DuplicateArgs.Add(curArg);
                    }
                    else
                    {
                        curSupportedArg = supportedArg;
                        this[supportedArg] = string.Empty;
                    }
                }
                else
                {
                    if (curSupportedArg != ESupportedArgument.Invalid)
                    {
                        this[curSupportedArg] = arg;
                    }
                    curSupportedArg = ESupportedArgument.Invalid;
                }
            }

            if (HasValue(ESupportedArgument.LogLevel) && Enum.TryParse<Logger.ELogLevel>(this[ESupportedArgument.LogLevel], true, out Logger.ELogLevel logLevel))
            {
                Logger.LogLevel = logLevel;
                Logger.WriteLine(Logger.ELogLevel.Debug, $"Setting logging level to {Logger.LogLevel}");
            }
        }

        public void Print()
        {
            if (Count > 0)
            {
                Logger.WriteLine(Logger.ELogLevel.Info, "Command line arguments");
                foreach (KeyValuePair<ESupportedArgument, string> argPair in this)
                {
                    Logger.WriteLine(Logger.ELogLevel.Info, $"{Tab}-{string.Format("{0,-3}", SupportedArgs[argPair.Key].First())} {argPair.Value}");
                }
            }

            if (InvalidArgs.Count > 0)
            {
                Logger.WriteLine(Logger.ELogLevel.Error, "\nInvalid arguments");
                foreach (string invalidArg in InvalidArgs)
                {
                    Logger.WriteLine(Logger.ELogLevel.Info, $"{Tab}-{invalidArg}");
                }
            }

            if (DuplicateArgs.Count > 0)
            {
                Logger.WriteLine(Logger.ELogLevel.Warn, "\nDuplicate arguments");
                foreach (string duplicateArg in DuplicateArgs)
                {
                    Logger.WriteLine(Logger.ELogLevel.Info, $"{Tab}-{duplicateArg}");
                }
            }
            Logger.WriteLine(Logger.ELogLevel.Info, "");
        }

        public void PrintHelp()
        {
            Logger.WriteLine(Logger.ELogLevel.Info, "Supported command line arguments");
            foreach (KeyValuePair<ESupportedArgument, List<string>> pair in SupportedArgs)
            {
                Logger.WriteLine(Logger.ELogLevel.Info, $"{Tab}{pair.Key.ToString()}");
                foreach (string val in pair.Value)
                {
                    Logger.WriteLine(Logger.ELogLevel.Info, $"{Tab}{Tab}-{val}");
                }
                Logger.WriteLine(Logger.ELogLevel.Info, "");
            }
            Logger.WriteLine(Logger.ELogLevel.Info, "");
        }

        public bool Has(ESupportedArgument argType)
        {
            return ContainsKey(argType);
        }

        public bool HasValue(ESupportedArgument argType)
        {
            return Has(argType) && !string.IsNullOrWhiteSpace(this[argType]);
        }
    }
}