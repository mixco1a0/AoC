using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC
{
    class Util
    {
        static public IEnumerable<string> ConvertInputToList(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new List<string>();
            }
            return input.Split('\n').Select(str => str.Trim('\r'));
        }

        static public string GetLogTimeStamp()
        {
            return $"|{DateTime.Now.ToString("hh:mm:ss.fff")}| ";
        }

        private static string s_workingDirectory;
        static public string WorkingDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(s_workingDirectory))
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
                        s_workingDirectory = curDir;
                    }
                    else
                    {
                        throw new DirectoryNotFoundException($"Unable to find base directory */{nameof(AoC)}/*");
                    }
                }
                return s_workingDirectory;
            }
        }

    }
    
    public class MinMax
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public bool GTE_LTE(int val) { return Min <= val && val <= Max; }
        public bool GT_LTE(int val) { return Min < val && val <= Max; }
        public bool GTE_LT(int val) { return Min <= val && val < Max; }
        public bool GT_LT(int val) { return Min < val && val < Max; }
    }
}