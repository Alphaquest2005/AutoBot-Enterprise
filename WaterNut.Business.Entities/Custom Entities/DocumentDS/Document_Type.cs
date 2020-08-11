using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DocumentDS.Business.Entities
{
    public partial class Document_Type
    {
        [IgnoreDataMember]
        [NotMapped]
        public string DisplayName
        {
            get
            {
                return Type_of_declaration + Declaration_gen_procedure_code;
            }
        }

     
      
    }
}
