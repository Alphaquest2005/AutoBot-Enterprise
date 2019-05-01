

namespace EntryDataQS.Client.Entities
{
       public partial class EntryDataEx
    {
           public bool TotalsEqual
           {
                get { return this.Total == this.ImportedTotal; }
           }

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


