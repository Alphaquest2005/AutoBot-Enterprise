using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Utils
{
    public static class DirectoryUtils
    {
        public static void EnsureDirectoryExists(string testDirectory)
        {
            if (!Directory.Exists(testDirectory)) Directory.CreateDirectory(testDirectory);
        }

    
    }
}
