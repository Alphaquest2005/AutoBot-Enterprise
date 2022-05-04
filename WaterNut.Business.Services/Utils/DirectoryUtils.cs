using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Utils
{
    public static class DirectoryUtils
    {
        public static string GetDirectory(List<string> folderPath)
        {
            folderPath.Insert(0, BaseDataModel.Instance.CurrentApplicationSettings.DataFolder);
            var directory = Path.Combine(folderPath.ToArray());
            Core.Common.Utils.DirectoryUtils.EnsureDirectoryExists(directory);
            return directory;
        }
    }
}
