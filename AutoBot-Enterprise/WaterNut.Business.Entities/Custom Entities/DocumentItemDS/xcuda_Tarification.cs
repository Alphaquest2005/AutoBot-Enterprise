using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace DocumentItemDS.Business.Entities
{
    public partial class xcuda_Tarification
    {
        [IgnoreDataMember]
        [NotMapped]
        [JsonIgnore]
        public ObservableCollection<xcuda_Supplementary_unit> xcuda_Supplementary_unit
        {
            get
            {
                return new ObservableCollection<xcuda_Supplementary_unit>(Unordered_xcuda_Supplementary_unit.OrderBy(x => x.Supplementary_unit_Id));
            }
        }
    }
}
