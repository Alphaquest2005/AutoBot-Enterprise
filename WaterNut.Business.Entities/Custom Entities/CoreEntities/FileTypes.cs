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

        [IgnoreDataMember]
        [NotMapped]
        public string EmailId { get; set; }

        [IgnoreDataMember]
        [NotMapped]
        public List<KeyValuePair<string, string>> Data { get; set; } = new List<KeyValuePair<string, string>>();

        [IgnoreDataMember]
        [NotMapped]
        public List<EmailInfoMappings> EmailInfoMappings { get; set; } = new List<EmailInfoMappings>();
    }
}
