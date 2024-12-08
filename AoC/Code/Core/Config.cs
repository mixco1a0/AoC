using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AoC.Base;
using Newtonsoft.Json;

namespace AoC.Core
{
    internal class ConfigFile
    {
        public Dictionary<string, Base.KeyVal<bool, string>> Values { get; set; }
        public ConfigFile()
        {
            Values = [];
        }
    }

    public class Config : Dictionary<Config.ESupportedArgument, string>
    {
        public enum ESupportedArgument
        {
            Invalid,
            Help,
            LogLevel,

            // general options
            Namespace,
            Day,
            RunWarmup,
            SkipLatest,
            ForceTests,
            RunNamespace,
            RunAll,

            // performance options
            ShowPerf,
            RunPerf,
            PerfRecordCount,
            PerfTimeout,
            CompactPerf,

            // config file options
            ConfigFile,
            IgnoreConfigFile,

            End
        }

        private readonly Dictionary<ESupportedArgument, List<string>> SupportedArgs;
        private List<string> DuplicateArgs { get; set; }
        private List<string> InvalidArgs { get; set; }
        private List<string> IgnoredArgs { get; set; }
        private static string Tab => "     ";

        public Config()
        {
            SupportedArgs = new Dictionary<ESupportedArgument, List<string>>();
            for (ESupportedArgument arg = ESupportedArgument.Help; arg != ESupportedArgument.End; ++arg)
            {
                SupportedArgs[arg] =
                [
                    string.Join("", arg.ToString().Where(char.IsUpper)).ToLower(),
                    arg.ToString().ToLower(),
                ];
            }
            SupportedArgs[ESupportedArgument.Help].Add("?");
            DuplicateArgs = [];
            InvalidArgs = [];
        }

        /// <summary>
        /// Initialize the config from command line and config file
        /// </summary>
        /// <param name="args"></param>
        public void Init(string[] args)
        {
            ParseCommandLine(args, out Dictionary<ESupportedArgument, string> cliArgs);

            Dictionary<ESupportedArgument, string> cfgArgs = [];
            if (!cliArgs.ContainsKey(ESupportedArgument.IgnoreConfigFile))
            {
                // add cfg args
                ParseConfigFile(ref cfgArgs);
                foreach (var pair in cfgArgs)
                {
                    this[pair.Key] = pair.Value;
                }
            }

            // add cli args
            foreach (var pair in cliArgs)
            {
                this[pair.Key] = pair.Value;
            }

            if (HasValue(ESupportedArgument.LogLevel) && Enum.TryParse<Log.ELevel>(this[ESupportedArgument.LogLevel], true, out Log.ELevel logLevel))
            {
                Log.LogLevel = logLevel;
                Log.WriteLine(Log.ELevel.Debug, $"Setting logging level to {Log.LogLevel}");
            }

            Print(cfgArgs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usedArgs"></param>
        private void ParseConfigFile(ref Dictionary<ESupportedArgument, string> usedArgs)
        {
            string defaultConfigFile = Path.Combine(WorkingDirectory.Get, "Data", "default_config.json");
            if (!File.Exists(defaultConfigFile))
            {
                GenerateConfigFile(defaultConfigFile);
            }

            string configFile = defaultConfigFile;
            if (HasValue(ESupportedArgument.ConfigFile))
            {
                if (File.Exists(this[ESupportedArgument.ConfigFile]))
                {
                    configFile = this[ESupportedArgument.ConfigFile];
                }
            }

            string rawJson = File.ReadAllText(configFile);
            ConfigFile cf = JsonConvert.DeserializeObject<ConfigFile>(rawJson);
            foreach (var pair in cf.Values)
            {
                string curArg = pair.Key.ToLower();
                ESupportedArgument supportedArg = ESupportedArgument.Invalid;
                supportedArg = SupportedArgs.Where(pair => pair.Value.Contains(curArg)).Select(pair => pair.Key).FirstOrDefault();
                if (supportedArg == ESupportedArgument.Invalid || supportedArg == ESupportedArgument.ConfigFile)
                {
                    // do nothing, can't support these through config file
                }
                else if (supportedArg == ESupportedArgument.Invalid)
                {
                    InvalidArgs.Add($"[cfg] {curArg}");
                }
                else if (usedArgs.ContainsKey(supportedArg))
                {
                    DuplicateArgs.Add($"[cfg] {curArg}");
                }
                else if (pair.Value.Key)
                {
                    usedArgs[supportedArg] = pair.Value.Val;
                }
                else
                {
                    // IgnoredArgs.Add($"[cfg] {curArg}");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="usedArgs"></param>
        private void ParseCommandLine(string[] args, out Dictionary<ESupportedArgument, string> usedArgs)
        {
            usedArgs = [];

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
                        InvalidArgs.Add($"[cli] {curArg}");
                    }
                    else if (usedArgs.ContainsKey(supportedArg))
                    {
                        DuplicateArgs.Add($"[cli] {curArg}");
                    }
                    else
                    {
                        curSupportedArg = supportedArg;
                        usedArgs[supportedArg] = string.Empty;
                    }
                }
                else
                {
                    if (curSupportedArg != ESupportedArgument.Invalid)
                    {
                        usedArgs[curSupportedArg] = arg;
                    }
                    else
                    {
                        InvalidArgs.Add($"[cli] {arg}");
                    }
                    curSupportedArg = ESupportedArgument.Invalid;
                }
            }

            // make sure the config file can be used
            if (usedArgs.TryGetValue(ESupportedArgument.ConfigFile, out string value))
            {
                this[ESupportedArgument.ConfigFile] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cfgArgs"></param>
        public void Print(Dictionary<ESupportedArgument, string> cfgArgs)
        {
            if (Count > 0)
            {
                Log.WriteLine(Log.ELevel.Info, "Command line arguments");
                foreach (KeyValuePair<ESupportedArgument, string> argPair in this)
                {
                    string cfg = cfgArgs.TryGetValue(argPair.Key, out string value) && value == argPair.Value ? "cfg" : "cli";
                    Log.WriteLine(Log.ELevel.Info, $"{Tab}[{cfg}] -{string.Format("{0,-3}", SupportedArgs[argPair.Key].Last())} {argPair.Value}");
                }
            }

            if (InvalidArgs.Count > 0)
            {
                Log.WriteLine(Log.ELevel.Error, "\nInvalid arguments");
                foreach (string invalidArg in InvalidArgs)
                {
                    Log.WriteLine(Log.ELevel.Error, $"{Tab}-{invalidArg}");
                }
            }

            if (DuplicateArgs.Count > 0)
            {
                Log.WriteLine(Log.ELevel.Warn, "\nDuplicate arguments");
                foreach (string duplicateArg in DuplicateArgs)
                {
                    Log.WriteLine(Log.ELevel.Info, $"{Tab}-{duplicateArg}");
                }
            }
            Log.WriteLine(Log.ELevel.Info, "");
        }

        /// <summary>
        /// 
        /// </summary>
        public void PrintHelp()
        {
            Log.WriteLine(Log.ELevel.Info, "Supported command line arguments");
            foreach (KeyValuePair<ESupportedArgument, List<string>> pair in SupportedArgs)
            {
                Log.WriteLine(Log.ELevel.Info, $"{Tab}{pair.Key.ToString()}");
                foreach (string val in pair.Value)
                {
                    Log.WriteLine(Log.ELevel.Info, $"{Tab}{Tab}-{val}");
                }
                Log.WriteLine(Log.ELevel.Info, "");
            }
            Log.WriteLine(Log.ELevel.Info, "");
        }

        /// <summary>
        /// Generate a fresh config file
        /// </summary>
        /// <param name="configFileName"></param>
        private static void GenerateConfigFile(string configFileName)
        {
            ConfigFile cf = new();
            for (ESupportedArgument arg = ESupportedArgument.Help; arg != ESupportedArgument.End; ++arg)
            {
                cf.Values.Add(arg.ToString(), new Base.KeyVal<bool, string>(false, ""));
            }
            string rawJson = JsonConvert.SerializeObject(cf, Formatting.Indented);
            using (StreamWriter sWriter = new(configFileName))
            {
                sWriter.Write(rawJson);
            }
        }

        /// <summary>
        /// Check if config has a specific argument
        /// </summary>
        /// <param name="argType"></param>
        /// <returns></returns>
        public bool Has(ESupportedArgument argType)
        {
            return ContainsKey(argType);
        }

        /// <summary>
        /// Check if config has a specific argument value
        /// </summary>
        /// <param name="argType"></param>
        /// <returns></returns>
        public bool HasValue(ESupportedArgument argType)
        {
            return Has(argType) && !string.IsNullOrWhiteSpace(this[argType]);
        }
    }
}