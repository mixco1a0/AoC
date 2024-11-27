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
                case Core.Part.One:
                    return "v1";
                case Core.Part.Two:
                    return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

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
                Output = "24933642",
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
            private long Size { get; set; }

            public FilePath(string name)
            {
                Name = name;
                Parent = null;
                Children = new List<FilePath>();
                Files = new List<File>();
            }

            public long GetSize()
            {
                if (Size == 0)
                {
                    Size = Files.Sum(f => f.Size);
                    Size += Children.Sum(c => c.GetSize());
                }
                return Size;
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

            if (filePath.GetSize() <= 100000)
            {
                validSizes.Add(filePath.GetSize());
                return true;
            }

            return false;
        }

        private void FindDelete(FilePath filePath, long unusedSpace, ref List<FilePath> validDeletes)
        {
            foreach (FilePath child in filePath.Children)
            {
                FindDelete(child, unusedSpace, ref validDeletes);
            }

            const long requiredSpace = 30000000;
            if (unusedSpace + filePath.GetSize() >= requiredSpace)
            {
                validDeletes.Add(filePath);
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool deleteFiles)
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
                        cur = cur.Children.Single(c => c.Name == split[1]);
                    }
                }
                else
                {
                    if (input.StartsWith("dir"))
                    {
                        cur.Children.Add(new FilePath(input.Substring(4)) { Parent = cur });
                    }
                    else
                    {
                        cur.Files.Add(File.Parse(input));
                    }
                }
            }

            if (!deleteFiles)
            {
                List<long> validSizes = new List<long>();
                SumDirectories(root, ref validSizes);
                return validSizes.Sum().ToString();
            }
            else
            {
                List<FilePath> validDeletes = new List<FilePath>();
                const long totalSpace = 70000000;
                FindDelete(root, totalSpace - root.GetSize(), ref validDeletes);
                return validDeletes.Min(f => f.GetSize()).ToString();
            }
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}