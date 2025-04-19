

using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DocumentDS.Business.Entities
{
    public partial class xcuda_Type
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


