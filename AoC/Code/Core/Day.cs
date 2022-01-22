using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AoC.Core
{
    public enum Part
    {
        One = 1,
        Two = 2
    }

    abstract public class Day
    {
        public static bool UseLogs { get; set; }
        private const char LogCorrect = '#';
        private const char LogUnknown = '?';
        private const char LogIncorrect = '!';
        private readonly string DefaultLogID = new string('.', 27);

        private enum RunType
        {
            Testing,
            Problem
        }

        #region Required Overrides
        protected abstract List<TestDatum> GetTestData();
        protected abstract string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables);
        protected abstract string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables);
        public virtual string GetSolutionVersion(Part part) => "v0";
        public virtual bool SkipTestData => false;
        #endregion


        public string Year { get; private set; }
        public string DayName { get; private set; }
        public Dictionary<Part, double> TimeResults { get; private set; }
        public Dictionary<Part, double> TimeWasted { get; private set; }
        private double TimeWaste { get; set; }

        private string LogID { get; set; }
        private Dictionary<string, List<TestDatum>> TestData { get; set; }
        private Dictionary<Part, Func<List<string>, Dictionary<string, string>, string>> PartSpecificFunctions { get; set; }

        protected Day()
        {
            try
            {
                TimeResults = new Dictionary<Part, double>();
                TimeWasted = new Dictionary<Part, double>();
                TimeWaste = 0.0f;

                Year = this.GetType().Namespace.ToString()[^4..];
                DayName = this.GetType().ToString()[^5..].ToLower();
                LogID = DefaultLogID;
                TestData = new Dictionary<string, List<TestDatum>>();
                TestData[DayName] = GetTestData();
                PartSpecificFunctions = new Dictionary<Part, Func<List<string>, Dictionary<string, string>, string>>
                {
                    {Part.One, RunPart1Solution},
                    {Part.Two, RunPart2Solution}
                };
            }
            catch (Exception e)
            {
                Core.Log.WriteException(e);
            }
        }

        public void Run()
        {
            // file input
            IEnumerable<string> input = GetInputFile();

            // file input
            IEnumerable<string> output = GetOutputFile();

            // run part 1
            RunAll(Part.One, input, output);

            // run part 2
            RunAll(Part.Two, input, output);

            // reset logging
            LogID = DefaultLogID;
        }

        public void RunProblem(Part part)
        {
            // run part
            RunAll(part, GetInputFile(), GetOutputFile());

            // reset logging
            LogID = DefaultLogID;
        }

        private IEnumerable<string> GetInputFile()
        {
            string fileName = string.Format("{0}.txt", DayName);
            string inputFile = Path.Combine(Core.WorkingDirectory.Get, "Data", Year, "In", fileName);
            return ConvertDayFileToList(File.ReadAllText(inputFile));
        }

        private IEnumerable<string> GetOutputFile()
        {
            string fileName = string.Format("{0}.txt", DayName);
            string outputFile = Path.Combine(Core.WorkingDirectory.Get, "Data", Year, "Out", fileName);
            List<string> output;
            if (File.Exists(outputFile))
            {
                output = ConvertDayFileToList(File.ReadAllText(outputFile)).ToList();
            }
            else
            {
                output = new List<string>();
            }
            output.AddRange(new string[2] { string.Empty, string.Empty });
            return output;
        }

        private void RunAll(Part part, IEnumerable<string> problemInput, IEnumerable<string> problemOutput)
        {
            // get test data if there is any
            IEnumerable<TestDatum> partSpecificTestData = TestData[DayName].Where(datum => datum.TestPart == part);

            if (!SkipTestData)
            {
                // run tests if there are any
                foreach (TestDatum datum in partSpecificTestData)
                {
                    // make sure the test has an output, and isn't just the default empty test set
                    if (!string.IsNullOrEmpty(datum.Output))
                    {
                        TimedRun(RunType.Testing, part, datum.Input.ToList(), datum.Output, datum.Variables);
                    }
                }
            }

            TimeResults[part] = TimedRun(RunType.Problem, part, problemInput.ToList(), problemOutput.ElementAt(((int)part) - 1), null);
            TimeWasted[part] = TimeWaste;
        }

        private double TimedRun(RunType runType, Part part, List<string> inputs, string expectedOuput, Dictionary<string, string> variables)
        {
            LogFiller();

            TimeWaste = 0.0f;
            Util.Timer timer = new Util.Timer();
            timer.Start();
            RunPart(runType, part, inputs, expectedOuput, variables);
            timer.Stop();

            Log(Core.Log.ELevel.Info, $"{timer.Print()}");
            return timer.GetElapsedMs();
        }

        private void RunPart(RunType runType, Part part, List<string> inputs, string expectedOuput, Dictionary<string, string> variables)
        {
            LogID = string.Format("{0}|{1}|{2}|part{3}|{4}", Year, DayName, runType.ToString().ToLower(), part == Part.One ? "1" : "2", GetSolutionVersion(part));
            try
            {
                string actualOutput = PartSpecificFunctions[part](inputs, variables);
                if (runType == RunType.Testing)
                {
                    if (actualOutput != expectedOuput)
                    {
                        LogAnswer($"[ERROR] Expected: {expectedOuput} - Actual: {actualOutput} [ERROR]", LogIncorrect, Core.Log.Negative);
                    }
                    else
                    {
                        LogAnswer(actualOutput, LogCorrect, Core.Log.Positive);
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(expectedOuput))
                    {
                        LogAnswer(actualOutput, LogUnknown, Core.Log.Neutral);
                    }
                    else if (actualOutput != expectedOuput)
                    {
                        LogAnswer($"[ERROR] Expected: {expectedOuput} - Actual: {actualOutput} [ERROR]", LogIncorrect, Core.Log.Negative);
                    }
                    else
                    {
                        LogAnswer(actualOutput, LogCorrect, Core.Log.Positive);
                    }
                }
            }
            catch (Exception e)
            {
                Core.Log.WriteException(e);
            }
        }

        protected void WasteTime(Action action)
        {
            Util.Timer timer = new Util.Timer();
            timer.Start();
            action();
            timer.Stop();
            TimeWaste += timer.GetElapsedMs();
        }

        private void Log(Core.Log.ELevel logLevel, string log)
        {
            if (UseLogs)
            {
                Core.Log.WriteLine(logLevel, $"[{LogID}] {log}");
            }
        }

        private void LogFiller()
        {
            if (UseLogs)
            {
                Core.Log.WriteLine(Core.Log.ELevel.Info, $"[{DefaultLogID}]");
            }
        }

        private void LogAnswer(string answer, char filler, Color color)
        {
            if (UseLogs && !string.IsNullOrWhiteSpace(answer))
            {
                string buffer = new string(filler, answer.Length);
                string empty = new string(' ', answer.Length);
                string bigFiller = new string(filler, 5);
                string smallFiller = new string(filler, 3);
                Core.Log.WriteLine(Core.Log.ELevel.Info, $"[{LogID}] {bigFiller}{buffer}{bigFiller}");
                Core.Log.WriteLine(Core.Log.ELevel.Info, $"[{LogID}] {smallFiller}  {empty }  {smallFiller}");
                Core.Log.WriteLine(Core.Log.ELevel.Info, $"[{LogID}] {smallFiller}  {Core.Log.ColorMarker}{answer}{Core.Log.ColorMarker}  {smallFiller}", new List<Color>() { color });
                Core.Log.WriteLine(Core.Log.ELevel.Info, $"[{LogID}] {smallFiller}  {empty }  {smallFiller}");
                Core.Log.WriteLine(Core.Log.ELevel.Info, $"[{LogID}] {bigFiller}{buffer}{bigFiller}");
            }
        }

        protected void DebugWrite(Log.ELevel level, string log)
        {
            if (UseLogs)
            {
                Core.Log.WriteLine(level, $"[{LogID}] \t{log}");
            }
        }

        protected void DebugWriteLine(Log.ELevel level, string log)
        {
            if (UseLogs)
            {
                Core.Log.WriteLine(level, $"[{LogID}] \t{log}");
            }
        }

        protected void DebugWriteLine(string log)
        {
            if (UseLogs)
            {
                Core.Log.WriteLine(Core.Log.ELevel.Info, $"[{LogID}] \t{log}");
            }
        }

        public static IEnumerable<string> ConvertDayFileToList(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new List<string>();
            }
            return input.Split('\n').Select(str => str.Trim('\r'));
        }

        public static void GetVariable(string variableName, int defaultValue, Dictionary<string, string> variables, out int value)
        {
            value = defaultValue;
            if (variables != null && variables.ContainsKey(variableName))
            {
                value = int.Parse(variables[variableName]);
            }
        }

        public static void GetVariable(string variableName, long defaultValue, Dictionary<string, string> variables, out long value)
        {
            value = defaultValue;
            if (variables != null && variables.ContainsKey(variableName))
            {
                value = long.Parse(variables[variableName]);
            }
        }

        public static void GetVariable(string variableName, char defaultValue, Dictionary<string, string> variables, out char value)
        {
            value = defaultValue;
            if (variables != null && variables.ContainsKey(variableName))
            {
                value = variables[variableName][0];
            }
        }

        public static void GetVariable(string variableName, string defaultValue, Dictionary<string, string> variables, out string value)
        {
            value = defaultValue;
            if (variables != null && variables.ContainsKey(variableName))
            {
                value = variables[variableName];
            }
        }
    }
}