using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterNut.DataLayer
{
    public partial class Document_Type
    {
        public string DisplayName
        {
            get
            {
                return Type_of_declaration + Declaration_gen_procedure_code;
            }
        }

        public Customs_Procedure DefaultCustoms_Procedure
        {
            get { return Customs_Procedures.FirstOrDefault(x => x.IsDefault == true); }
            set
            {
                var cp = value;
                var rcp = Customs_Procedures.FirstOrDefault(x => x.Customs_ProcedureId == cp.Customs_ProcedureId);


                if (rcp != null)
                {
                    ClearIsDefault();
                    rcp.IsDefault = true;
                }
                else
                {
                    ClearIsDefault();
                    cp.IsDefault = true;
                    Customs_Procedures.Add(cp);
                }
            }
        }

        private void ClearIsDefault()
        {
            foreach (var c in Customs_Procedures.Where(x => x.IsDefault == true))
            {
                c.IsDefault = false;
            }
        }
    }
}
