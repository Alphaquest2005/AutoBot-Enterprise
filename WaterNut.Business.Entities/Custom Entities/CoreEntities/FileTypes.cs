using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace CoreEntities.Business.Entities
{
   
    public partial class FileTypes 
    {
        [IgnoreDataMember]
        [NotMapped]
        public string DocReference { get; set; }
    }
}
