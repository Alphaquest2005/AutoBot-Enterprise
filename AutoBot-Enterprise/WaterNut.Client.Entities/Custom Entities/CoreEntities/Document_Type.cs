
using System;

using System.Collections.ObjectModel;

using CoreEntities.Client.DTO;
using TrackableEntities.Client;
using Core.Common.Validation;

namespace CoreEntities.Client.Entities
{
       public partial class Document_Type
    {
           public string DisplayName
           {
               get { return Type_of_declaration + Declaration_gen_procedure_code; }
           }
    }
}


