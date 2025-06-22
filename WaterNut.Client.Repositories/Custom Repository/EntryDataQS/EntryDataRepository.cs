
//using WaterNut.Client.Services;
//using WaterNut.Client.Repositories;
//using EntryDataQS.Client.Services;
//using EntryDataQS.Client.Entities;

//using System.Linq;
//using Core.Common;
//using System.ComponentModel;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Linq.Expressions;
//using System;
//using EntryDataQS.Client.DTO;
//using System.Reflection;

using System.Collections.Generic;
using System.Threading.Tasks;
using CoreEntities.Client.Entities;
using EntryDataQS.Client.Entities;
using EntryDataQS.Client.Services;

namespace EntryDataQS.Client.Repositories
{
   
    public partial class EntryDataExRepository 
    {
        public async Task AddDocToEntry(IEnumerable<string> lst, int docSetId, bool perInvoice, bool combineEntryDataInSameFile, bool checkPackages)
        {
            using (var ctx = new EntryDataExClient())
            {
                await ctx.AddDocToEntry(lst, docSetId, perInvoice, combineEntryDataInSameFile, checkPackages).ConfigureAwait(false);
            }
        }


        public async Task SaveCSV(string droppedFilePath, string fileType, int docSet,
            bool overWriteExisting, Serilog.ILogger log)
        {
            using (var ctx = new EntryDataExClient())
            {
                await ctx.SaveCSV(droppedFilePath, fileType, docSet, overWriteExisting).ConfigureAwait(false);
            }
        }

        public async Task SavePDF(string droppedFilePath, string fileType, int docSetId, bool overwrite)
        {
            using (var ctx = new EntryDataExClient())
            {
                await ctx.SavePDF(droppedFilePath, fileType, docSetId, overwrite).ConfigureAwait(false);
            }
        }


    }
}

