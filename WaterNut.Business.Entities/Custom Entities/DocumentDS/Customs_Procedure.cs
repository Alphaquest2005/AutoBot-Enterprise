using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DocumentDS.Business.Entities
{
    public partial class Customs_Procedure
    {    
        [IgnoreDataMember]
        [NotMapped]
        public string DisplayName
        {
            get
            {
                return $"{Extended_customs_procedure}-{National_customs_procedure}";
            }
        }

        
    }




}
