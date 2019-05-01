
using System.Collections.ObjectModel;
using CoreEntities.Client.DTO;
using TrackableEntities.Client;
using Core.Common.Validation;

namespace CoreEntities.Client.Entities
{
       public partial class Customs_Procedure
    {
           public string DisplayName
           {
               get { return $"{Extended_customs_procedure}-{National_customs_procedure}"; }
           }
    }
}


