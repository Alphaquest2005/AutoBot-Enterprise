using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterNut.DataLayer
{
    public partial class xcuda_Type
    {
        

        public string DisplayName
        {
            get
            {
                return Type_of_declaration + Declaration_gen_procedure_code;
            }
        }
    }
}
