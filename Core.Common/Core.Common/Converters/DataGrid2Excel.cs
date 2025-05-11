using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Core.Common.Converters
{
    /// <summary>
    ///     Class for generator of Excel file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public class ExportToCSV<T, U>
        where T : class
        where U : List<T>
    {
        public List<T> dataToPrint;
        // Excel object references.

        public List<PropertyInfo> IgnoreFields = new List<PropertyInfo>();

        /// <summary>
        ///     Set Header style as bold
        /// </summary>
        /// <summary>
        ///     Method to add an Excel rows
        /// </summary>
        /// <param name="startRange"></param>
        /// <param name="rowCount"></param>
        /// <param name="colCount"></param>
        /// <param name="values"></param>
        private readonly StringBuilder sb = new StringBuilder();


        /// <summary>
        ///     Generate report and sub functions
        /// </summary>
        public void GenerateReport()
        {
            try
            {
                if (dataToPrint != null)
                    if (dataToPrint.Count != 0)
                    {
                        Mouse.SetCursor(Cursors.Wait);
                        FillSheet();
                        OpenReport();
                        Mouse.SetCursor(Cursors.Arrow);
                        MessageBox.Show("Complete", "Asycuda Toolkit", MessageBoxButton.OK,
                            MessageBoxImage.Exclamation);
                    }
            }
            catch (Exception)
            {
                MessageBox.Show("Error while generating Excel report");
            }
            finally
            {
            }
        }

        public void SaveReport(string FileName)
        {
            try
            {
                if (dataToPrint != null)
                    if (dataToPrint.Count != 0)
                    {
                        Mouse.SetCursor(Cursors.Wait);

                        FillSheet();
                        SaveFile(FileName);

                        Mouse.SetCursor(Cursors.Arrow);
                    }
            }
            catch (Exception)
            {
                MessageBox.Show("Error while generating Excel report");
            }
            finally
            {
            }
        }

        public void StartUp()
        {
        }

        public void ShutDown()
        {
        }

        private void SaveFile(string fileName = "Data.csv")
        {
            using (var w = new StreamWriter(fileName))
            {
                w.WriteLine(sb.ToString());
                w.Flush();
            }
        }

        /// <summary>
        ///     Make MS Excel application visible
        /// </summary>
        private void OpenReport()
        {
            SaveFile();
            Process.Start(@"Data.csv");
        }

        /// <summary>
        ///     Populate the Excel sheet
        /// </summary>
        private void FillSheet()
        {
            sb.Clear();
            var header = CreateHeader();
            WriteData(header);
        }

        /// <summary>
        ///     Write data into the Excel sheet
        /// </summary>
        /// <param name="header"></param>
        private void WriteData(string[] header)
        {
            try
            {
                var header1 = header[0].Split(',');
                var objData = new string[dataToPrint.Count];

                for (var j = 0; j < dataToPrint.Count; j++)
                {
                    var item = dataToPrint[j];
                    if (item == null) continue;
                    var val = new string[header1.Length];
                    for (var i = 0; i < header1.Length; i++)
                    {
                        var y = typeof(T).InvokeMember(header1[i], BindingFlags.GetProperty, null, item, null);
                        val[i] = y == null ? "" : $@"""{y}""";
                    }

                    objData[j] = string.Join(",", val);
                }

                AddExcelRows(objData);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        ///     Create header from the properties
        /// </summary>
        /// <returns></returns>
        private string[] CreateHeader()
        {
            var headerInfo = typeof(T).GetProperties().Where(x => IgnoreFields.All(z => z.Name != x.Name)).ToArray();

            // Create an array for the headers and add it to the
            // worksheet starting at cell A1.
            var header = new string[headerInfo.Length];
            for (var n = 0; n < headerInfo.Length; n++) header[n] = headerInfo[n].Name;
            var objHeaders = new string[1];
            objHeaders[0] = string.Join(",", header);
            AddExcelRows(objHeaders);


            return objHeaders;
        }

        private void AddExcelRows(string[] values)
        {
            foreach (var row in values) sb.AppendLine(row);
        }
    }
}