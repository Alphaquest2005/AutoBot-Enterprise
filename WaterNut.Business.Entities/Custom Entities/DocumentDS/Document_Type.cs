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

        private Customs_Procedure _defaultCustoms_Procedure = null;
        [IgnoreDataMember]
        [NotMapped]
        public Customs_Procedure DefaultCustoms_Procedure
        {
            get
            {
                return _defaultCustoms_Procedure ??
                       (_defaultCustoms_Procedure = Customs_Procedure.FirstOrDefault(x => x.IsDefault == true) ??
                                                    Customs_Procedure.FirstOrDefault());
            }
            set
            {
                _defaultCustoms_Procedure = value;
            }
        }
    }
}
