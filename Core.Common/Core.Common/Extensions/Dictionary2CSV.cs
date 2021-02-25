using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Extensions
{
    public static class Dictionary2CSV
    {
       

        //public static void Dictionary2CSV(this List<IDictionary<string, object>> rawLst, string fileName)
        //{
        //    try
        //    {


        //        foreach (var file in files)
        //        {
        //            var dfile = new FileInfo($@"{file.DirectoryName}\{file.Name.Replace(file.Extension, ".csv")}");
        //            if (dfile.Exists && dfile.LastWriteTime >= file.LastWriteTime.AddMinutes(5)) return;
        //            // Reading from a binary Excel file (format; *.xlsx)
        //            FileStream stream = File.Open(file.FullName, FileMode.Open, FileAccess.Read);
        //            var excelReader = ExcelReaderFactory.CreateReader(stream);
        //            var result = excelReader.AsDataSet();
        //            excelReader.Close();


        //            int row_no = 0;

        //            result.Tables[0].Columns.Add("LineNumber", typeof(int));

        //            var rows = new List<DataRow>();
        //            ///insert linenumber
        //            while (row_no < result.Tables[0].Rows.Count)
        //            {

        //                var dataRow = result.Tables[0].Rows[row_no];
        //                dataRow["LineNumber"] = row_no;
        //                rows.Add(dataRow);
        //                row_no++;
        //            }

        //            var table = new ConcurrentDictionary<int, string>();
        //            Parallel.ForEach(rows, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount * 1 },
        //                row =>
        //                {
        //                    StringBuilder a = new StringBuilder();

        //                    if (fileType.FileTypeMappings.Any() && fileType.FileTypeMappings.Select(x => x.OriginalName)
        //                            .All(x => row.ItemArray.Contains(x)))
        //                    {
        //                        //if(dic.ContainsKey())
        //                        a.Append("");
        //                    }

        //                    for (int i = 0; i < result.Tables[0].Columns.Count - 1; i++)
        //                    {

        //                        a.Append(StringToCSVCell(row[i].ToString()) + ",");
        //                    }


        //                    a.Append("\n");
        //                    table.GetOrAdd(Convert.ToInt32(row["LineNumber"]), a.ToString());
        //                });




        //            string output = Path.ChangeExtension(file.FullName, ".csv");
        //            StreamWriter csv = new StreamWriter(output, false);
        //            csv.Write(table.OrderBy(x => x.Key).Select(x => x.Value).Aggregate((a, x) => a + x));
        //            csv.Close();

        //            FixCsv(new FileInfo(output), fileType, dic);

        //        }

        //    }
        //    catch (Exception e)
        //    {

        //        throw;
        //    }
        //}
    }
}
