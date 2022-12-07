using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day07 : Core.Day
    {
        public Day07() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                // case Core.Part.One:
                //     return "v1";
                // case Core.Part.Two:
                //     return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        public override bool SkipTestData => false;

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "95437",
                RawInput =
@"$ cd /
$ ls
dir a
14848514 b.txt
8504156 c.dat
dir d
$ cd a
$ ls
dir e
29116 f
2557 g
62596 h.lst
$ cd e
$ ls
584 i
$ cd ..
$ cd ..
$ cd d
$ ls
4060174 j
8033020 d.log
5626152 d.ext
7214296 k"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        internal class File
        {
            public string Name { get; set; }
            public long Size { get; set; }

            public File()
            {
                Name = string.Empty;
                Size = 0;
            }

            static public File Parse(string input)
            {
                string[] split = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new File() { Name = split[1], Size = long.Parse(split[0]) };
            }

            public override string ToString()
            {
                return $"{Name} [{Size}]";
            }
        }

        internal class FilePath
        {
            public string Name { get; set; }
            public FilePath Parent { get; set; }
            public List<FilePath> Children { get; set; }
            public List<File> Files { get; set; }

            public FilePath(string name)
            {
                Name = name;
                Parent = null;
                Children = new List<FilePath>();
                Files = new List<File>();
            }

            public long GetSize()
            {
                long size = Files.Sum(f => f.Size);
                size += Children.Sum(c => c.GetSize());
                return size;
            }

            public override string ToString()
            {
                return $"{Name}";
            }
        }

        private bool SumDirectories(FilePath filePath, ref List<long> validSizes)
        {
            foreach (FilePath child in filePath.Children)
            {
                SumDirectories(child, ref validSizes);
            }

            long size = filePath.GetSize();
            if (size <= 100000)
            {
                validSizes.Add(size);
                return true;
            }

            return false;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            FilePath root = new FilePath("/");
            FilePath cur = root;
            foreach (string input in inputs)
            {
                if (input.StartsWith('$'))
                {
                    string[] split = input.Split("$ ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (split[0] == "ls")
                    {
                        continue;
                    }

                    if (split[1] == "..")
                    {
                        cur = cur.Parent;
                    }
                    else if (split[1] == "/")
                    {
                        cur = root;
                    }
                    else
                    {
                        if (cur.Children.Where(c => c.Name == split[1]).Any())
                        {
                            cur = cur.Children.Single(c => c.Name == split[1]);
                        }
                        else
                        {
                            cur.Children.Add(new FilePath(split[1]) { Parent = cur });
                            cur = cur.Children.Last();
                        }
                    }
                }
                else
                {
                    if (input.StartsWith("dir"))
                    {
                        cur.Children.Add(new FilePath(input.Substring(4) ){ Parent = cur });
                    }
                    else
                    {
                        cur.Files.Add(File.Parse(input));
                    }
                }
            }
            
            List<long> validSizes = new List<long>();
            SumDirectories(root, ref validSizes);
            return validSizes.Sum().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}