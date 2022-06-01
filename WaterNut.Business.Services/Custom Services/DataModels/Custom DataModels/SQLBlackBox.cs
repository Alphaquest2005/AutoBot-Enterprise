using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using CoreEntities.Business.Entities;

namespace WaterNut.DataSpace
{
    public class SQLBlackBox
    {
        public static void RunSqlBlackBox()
        {
            try
            {
                var scripts = new List<string>()
                {
                    "CleanBackupHistory",
                    "SmatIndexRebuild",
                    "UpdateStats",
                    "AdhocChange",
                    "dropIndexDupes",

                };
                using (var ctx = new CoreEntitiesContext())
                {
                    var cncStr = ctx.Database.Connection.ConnectionString;
                    var oDatabase = ctx.Database.Connection.Database;
                    SqlConnection nConn = new SqlConnection(cncStr);
                   // List<string> Databases = GetDatabaseList(cncStr);

                    foreach (var sName in scripts)
                    {



                        String script = File.ReadAllText($@"SQLBlackBox\{sName}.sql");
                        SqlCommand myCommand = new SqlCommand(script, nConn);
                        int i = 0;
                        //foreach (string db in Databases)
                        //{
                            try
                            {
                                //var database = db;
                                
                                ++i;

                                //cncStr = cncStr.Replace(oDatabase, db);
                                nConn = new SqlConnection(cncStr);
                                myCommand = new SqlCommand(script, nConn);
                                nConn.Open();
                                myCommand.CommandTimeout = 0;
                                int aa = myCommand.ExecuteNonQuery();
                                myCommand.CommandTimeout = 0;
                                nConn.Close();

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error in Db: " + oDatabase + "\nError Statement:" + ex.Message);
                            }
                            finally
                            {
                                nConn.Close();
                                // Console.WriteLine("----------------------------------------------------------");
                            }
                     //   }

                    }

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static List<string> GetDatabaseList(string cncstr)
        {
            List<string> list = new List<string>();
            try
            {
                // Open connection to the database
                string conString = cncstr;
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    // Set up a command with the given query and associate
                    // this with the current connection.
                    using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases where name <> 'master' and name <> 'model' and name <> 'msdb' and name <> 'tempdb'", con))
                    {
                        using (IDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                list.Add(dr[0].ToString());
                            }
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
    }
}