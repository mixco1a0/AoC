using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace _2020
{
    abstract class Day
    {
        private enum RunType
        {
            Example,
            Problem
        }

        private string m_logID;

        protected abstract string GetDay();
        protected abstract string GetPart1ExampleInput();
        protected abstract string GetPart1ExampleAnswer();
        protected abstract string RunPart1Solution(List<string> inputs);
        protected abstract string GetPart2ExampleInput();
        protected abstract string GetPart2ExampleAnswer();
        protected abstract string RunPart2Solution(List<string> inputs);

        private string DefaultLogID { get { return new string('.', 19); } }

        protected Day()
        {
            try
            {
                m_logID = DefaultLogID;
                Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        private delegate void RunPartX(RunType runType, List<string> inputs);

        private void Run()
        {
            // file input
            string fileName = string.Format("{0}.txt", GetDay());
            string inputFile = Path.Combine(Directory.GetCurrentDirectory(), "Data", fileName);
            IEnumerable<string> rawFileRead = File.ReadAllText(inputFile).Split('\n').Select(str => str.Trim('\r'));

            // run part 1
            IEnumerable<string> part1ExampleInput = GetPart1ExampleInput().Split('\n').Select(str => str.Trim('\r'));
            SharedRun(RunPart1, part1ExampleInput, rawFileRead);

            // run part 2
            IEnumerable<string> part2ExampleInput = GetPart1ExampleInput().Split('\n').Select(str => str.Trim('\r'));
            SharedRun(RunPart2, part2ExampleInput, rawFileRead);

            // reset logging
            m_logID = DefaultLogID;
        }

        private void SharedRun(RunPartX runPartX, IEnumerable<string> exampleInput, IEnumerable<string> problemInput)
        {
            runPartX(RunType.Example, exampleInput.ToList());
            LogFiller();
            Timer timer = new Timer();
            timer.Start();
            runPartX(RunType.Problem, problemInput.ToList());
            timer.Stop();
            Log($"{timer.Print()}");
            LogFiller();
        }

        private void RunPart1(RunType runType, List<string> inputs)
        {
            m_logID = string.Format("{0}|{1}|part1", GetDay(), runType == RunType.Problem ? "problem" : "example");
            try
            {
                string answer = RunPart1Solution(inputs);
                if (runType == RunType.Example && answer != GetPart1ExampleAnswer())
                {
                    LogAnswer($"[ERROR] Expected: {GetPart1ExampleAnswer()} - Actual: {answer} [ERROR]");
                }
                else
                {
                    LogAnswer(answer);
                }
            }
            catch (Exception e)
            {
                Log($"{e.Message}");
                Log($"{e.StackTrace.Split('\r').FirstOrDefault()}");
            }
        }

        private void RunPart2(RunType runType, List<string> inputs)
        {
            m_logID = string.Format("{0}|{1}|part2", GetDay(), runType == RunType.Problem ? "problem" : "example");
            try
            {
                string answer = RunPart2Solution(inputs);
                if (runType == RunType.Example && answer != GetPart2ExampleAnswer())
                {
                    LogAnswer($"[ERROR] Expected: {GetPart2ExampleAnswer()} - Actual: {answer} [ERROR]");
                }
                else
                {
                    LogAnswer(answer);
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

/*
using System.Collections.Generic;

namespace _2020
{
    class DayXX : Day
    {
        public DayXX() : base() {}
        
        protected override string GetDay() { return nameof(DayXX).ToLower(); }

        protected override void RunPart1Solution(List<string> inputs)
        {
            LogAnswer("NA");
        }

        protected override void RunPart2Solution(List<string>inputs)
        {
            LogAnswer("NA");
        }
    }
}

*/