using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC
{
    abstract class Day
    {
        private enum RunType
        {
            Testing,
            Problem
        }

        #region Required Overrides
        protected abstract List<TestDatum> GetTestData();
        protected abstract string RunPart1Solution(List<string> inputs);
        protected abstract string RunPart2Solution(List<string> inputs);
        protected virtual string GetSolutionVersion(TestPart testPart)
        {
            return "0";
        }
        #endregion

        private string DefaultLogID { get { return new string('.', 24); } }
        public string WorkingDirectory {get;set;}
        public Dictionary<TestPart, double> TestTiming { get; private set; }

        private string m_year;
        private string m_dayName;
        private string m_logID;
        private Dictionary<string, List<TestDatum>> m_testData;
        private Dictionary<TestPart, Func<List<string>, string>> m_partSpecificFunctions;

        protected Day()
        {
            try
            {
                TestTiming = new Dictionary<TestPart, double>();

                m_year = this.GetType().Namespace.ToString()[^4..];
                m_dayName = this.GetType().ToString()[^5..].ToLower();
                m_logID = DefaultLogID;
                m_testData = new Dictionary<string, List<TestDatum>>();
                m_testData[m_dayName] = GetTestData();
                m_partSpecificFunctions = new Dictionary<TestPart, Func<List<string>, string>>
                {
                    {TestPart.One, RunPart1Solution},
                    {TestPart.Two, RunPart2Solution}
                };
                Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        private void Run()
        {
            // file input
            string fileName = string.Format("{0}.txt", m_dayName);
            string inputFile = Path.Combine(WorkingDirectory, "Data", m_year, fileName);
            IEnumerable<string> rawFileRead = Util.ConvertInputToList(File.ReadAllText(inputFile));

            // run part 1
            RunAll(TestPart.One, rawFileRead);

            // run part 2
            RunAll(TestPart.Two, rawFileRead);

            // reset logging
            m_logID = DefaultLogID;
        }

        private void RunAll(TestPart testPart, IEnumerable<string> problemInput)
        {
            // get test data if there is any
            IEnumerable<KeyValuePair<IEnumerable<string>, string>> partSpecificTestData = m_testData[m_dayName]
                .Where(datum => datum.TestPart == testPart)
                .Select(datum => new KeyValuePair<IEnumerable<string>, string>(datum.Input, datum.Output));

            // run tests if there any
            foreach (var pair in partSpecificTestData)
            {
                RunPart(RunType.Testing, testPart, pair.Key.ToList(), pair.Value);
                LogFiller();
            }

            // run problem
            Timer timer = new Timer();
            timer.Start();
            RunPart(RunType.Problem, testPart, problemInput.ToList(), "");
            timer.Stop();
            TestTiming[testPart] = timer.GetElapsedMs();

            // print time
            Log($"{timer.Print()}");
            LogFiller();
        }

        private void RunPart(RunType runType, TestPart testPart, List<string> inputs, string expectedOuput)
        {
            m_logID = string.Format("{0}|{1}|{2}|part{3}", m_year, m_dayName, runType == RunType.Problem ? "problem" : "testing", testPart == TestPart.One ? "1" : "2");
            try
            {
                string actualOutput = m_partSpecificFunctions[testPart](inputs);
                if (runType == RunType.Testing && actualOutput != expectedOuput)
                {
                    LogAnswer($"[ERROR] Expected: {expectedOuput} - Actual: {actualOutput} [ERROR]");
                }
                else
                {
                    LogAnswer(actualOutput);
                }
            }
            catch (Exception e)
            {
                Log($"{e.Message}");
                Log($"{e.StackTrace.Split('\r').FirstOrDefault()}");
            }
        }

        private void Log(string log)
        {
            Console.WriteLine($"[{m_logID}] {log}");
        }

        private void LogFiller()
        {
            Console.WriteLine($"[{DefaultLogID}]");
        }

        private void LogAnswer(string answer)
        {
            string buffer = new string('*', answer.Length);
            Console.WriteLine($"[{m_logID}] *****{buffer}*****");
            Console.WriteLine($"[{m_logID}] ***  {answer}  ***");
            Console.WriteLine($"[{m_logID}] *****{buffer}*****");
        }

        protected void Debug(string log)
        {
            Console.WriteLine($"[{m_logID}] \t{log}");
        }
    }
}