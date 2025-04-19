

using System;

namespace EntryDataQS.Client.Entities
{
       public partial class EntryDataEx
    {
           public bool TotalsEqual => Math.Abs(ExpectedTotal - (InvoiceTotal?? ExpectedTotal)) < .009;

        public int MissingLines
           {
               get
               {
                   if (ImportedLines.HasValue)
                   {
                       return  (int) (ImportedLines - TotalLines);
                   }
                   return 0;
               }
           }
    }
}


