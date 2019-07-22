using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;

namespace WaterNut.Business.Entities
{
    public class DocumentCT
    {

        public DocumentCT()
        {
            DocumentItems = new List<xcuda_Item>();
            Document = new xcuda_ASYCUDA(true);
            EntryDataDetails= new List<EntryDataDetails>();
           
        }
        [IgnoreDataMember]
        [NotMapped]
        public xcuda_ASYCUDA Document { get; set; }
        [IgnoreDataMember]
        [NotMapped]
        public List<xcuda_Item> DocumentItems { get; set; }
        [IgnoreDataMember]
        [NotMapped]
        public List<EntryDataDetails> EntryDataDetails { get; set; }

        public List<int?> EmailIds { get; set; } = new List<int?>();
    }
}
