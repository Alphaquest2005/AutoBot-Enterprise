

using System;

namespace EntryDataQS.Client.Entities
{
       public partial class EntryDataEx
    {
           public bool TotalsEqual => Math.Abs(ExpectedTotal - (InvoiceTotal?? ExpectedTotal)) < .005;

        public int MissingLines
           {
               get
               {
                   if (ImportedLines.HasValue)
                   {
                       return ImportedLines.GetValueOrDefault() - TotalLines.GetValueOrDefault();
                   }
                   return 0;
               }
           }
    }
}


