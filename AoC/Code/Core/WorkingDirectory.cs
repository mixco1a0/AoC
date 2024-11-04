using System.IO;

namespace AoC.Core
{
    public static class WorkingDirectory
    {
        private static string s_baseDir;
        public static string Get
        {
            get
            {
                if (string.IsNullOrEmpty(s_baseDir))
                {
                    string curDir = Directory.GetCurrentDirectory();
                    string dirRoot = Path.GetPathRoot(curDir);
                    while (true)
                    {
                        if (curDir == dirRoot)
                        {
                            break;
                        }

                        if (Path.GetFileName(curDir) == nameof(AoC))
                        {
                            break;
                        }

                        curDir = Path.GetDirectoryName(curDir);
                    }

                    if (curDir != dirRoot)
                    {
                        s_baseDir = curDir;
                    }
                    else
                    {
                        throw new DirectoryNotFoundException($"Unable to find base directory */{nameof(AoC)}/*");
                    }
                }
                return s_baseDir;
            }
        }
    }
}