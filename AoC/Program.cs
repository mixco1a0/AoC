using System.IO;
using System;
using Newtonsoft.Json;

namespace AoC
{
    class EntryPoint
    {
        static void Main(string[] args)
        {            
            new Program();
        }
    }

    class Program
    {
        Json.RunData m_runData;
        string m_workingDirectory;

        public Program()
        {
            SetWorkingDirectory();
            LoadRunData();
            Run2020();
            //Run2015();
        }

        private void SetWorkingDirectory()
        {
            string curDir = Directory.GetCurrentDirectory();
            string dirRoot = Path.GetPathRoot(curDir);
            while (true)
            {
                if (curDir == dirRoot)
                    break;
                if (Path.GetFileName(curDir) == nameof(AoC))
                    break;
                curDir = Path.GetDirectoryName(curDir);
            }
            if (curDir != dirRoot)
            {
                m_workingDirectory = curDir;
            }
            else
            {
                throw new DirectoryNotFoundException($"Unable to find base directory */{nameof(AoC)}/*");
            }
        }

        private void LoadRunData()
        {
            if (File.Exists(Path.Combine(m_workingDirectory, "rundata.json")))
            {
                string rawJson = File.ReadAllText(Path.Combine(m_workingDirectory, "rundata.json"));
                m_runData = JsonConvert.DeserializeObject<Json.RunData>(rawJson);
            }
            else
            {
                m_runData = new Json.RunData();
            }
        }

        private void Run2020()
        {
            Console.WriteLine("");
            Console.WriteLine("Running 2020 Advent of Code");
            Console.WriteLine("");

            // new _2020.Day08();
            // new _2020.Day07();
            // new _2020.Day06();
            // new _2020.Day05();
            // new _2020.Day04();
            // new _2020.Day03();
            // new _2020.Day02();
            new _2020.Day01{WorkingDirectory=m_workingDirectory};
            
        }

        private void Run2015()
        {
            Console.WriteLine("");
            Console.WriteLine("Running 2015 Advent of Code");
            Console.WriteLine("");

            // new _2015.Day05();
            // new _2015.Day04();
            // new _2015.Day03();
            // new _2015.Day02();
            // new _2015.Day01();
        }
    }
}
