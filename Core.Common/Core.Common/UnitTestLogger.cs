using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Core.Common
{
    public static class UnitTestLogger
    {

        public static void Log(List<string> nameOfCallingClass, string dataFolder, dynamic data)
        {
            var unitTestFolder = "UnitTests";
            try
            {
                var serializer = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                var yaml = serializer.Serialize(data);
                
                var names = nameOfCallingClass
                    .Where(x => !string.IsNullOrEmpty(x))
                    .DefaultIfEmpty("")
                    .Aggregate((o, n) => o + "\\" + n);

                var log = serializer.Serialize(new { DateTime = DateTime.Now, LogPath = names });
               
                var file = new FileInfo(Path.Combine(dataFolder, unitTestFolder,
                    names, "UnitTestsData.yaml"));

                var currentUnitTest = Path.Combine(dataFolder,unitTestFolder, nameOfCallingClass.First(), "UnitTests.yaml");
                var UnitTestHistory = Path.Combine(dataFolder,unitTestFolder, nameOfCallingClass.First(), "UnitTestHistory.yaml");
                 
                if (!Directory.Exists(file.Directory.FullName)) Directory.CreateDirectory(file.Directory.FullName);

                File.AppendAllText(file.FullName, yaml);
                File.AppendAllText(currentUnitTest, log);
                File.AppendAllText(UnitTestHistory, log);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public class Logs
        {
            public DateTime DateTime { get; set; }
            public string LogPath { get; set; }
        }
    }
}
