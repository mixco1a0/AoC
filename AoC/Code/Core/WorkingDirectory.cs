using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC.Core
{
    static class WorkingDirectory
    {
        private static string s_dir;
        static public string Get
        {
            get
            {
                if (string.IsNullOrEmpty(s_dir))
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
                        s_dir = curDir;
                    }
                    else
                    {
                        throw new DirectoryNotFoundException($"Unable to find base directory */{nameof(AoC)}/*");
                    }
                }
                return s_dir;
            }
        }
    }
}