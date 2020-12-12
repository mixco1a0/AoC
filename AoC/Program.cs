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
        private string m_runDataFile;
        private Json.RunData m_runData;

        public Program()
        {
            LoadRunData();
            Run2020();
            Run2015();
            SaveRunData();
        }

        private void LoadRunData()
        {
            string workingDir = Util.WorkingDirectory;
            m_runDataFile = Path.Combine(workingDir, "rundata.json");
            if (File.Exists(m_runDataFile))
            {
                string rawJson = File.ReadAllText(m_runDataFile);
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

            new _2020.Day12();
            // m_runData.AddData(new _2020.Day11());
            // m_runData.AddData(new _2020.Day10());
            // m_runData.AddData(new _2020.Day09());
            // m_runData.AddData(new _2020.Day08());
            // m_runData.AddData(new _2020.Day07());
            // m_runData.AddData(new _2020.Day06());
            // m_runData.AddData(new _2020.Day05());
            // m_runData.AddData(new _2020.Day04());
            // m_runData.AddData(new _2020.Day03());
            // m_runData.AddData(new _2020.Day02());
            //m_runData.AddData(new _2020.Day01());

        }

        private void Run2015()
        {
            Console.WriteLine("");
            Console.WriteLine("Running 2015 Advent of Code");
            Console.WriteLine("");

            // m_runData.AddData(new _2015.Day07());
            // m_runData.AddData(new _2015.Day06());
            // m_runData.AddData(new _2015.Day05());
            // m_runData.AddData(new _2015.Day04());
            // m_runData.AddData(new _2015.Day03());
            // m_runData.AddData(new _2015.Day02());
            // m_runData.AddData(new _2015.Day01());
        }

        private void SaveRunData()
        {
            string rawJson = JsonConvert.SerializeObject(m_runData, Formatting.Indented);
            using (StreamWriter sWriter = new StreamWriter(m_runDataFile))
            {
                sWriter.Write(rawJson);
            }
        }
    }
}
