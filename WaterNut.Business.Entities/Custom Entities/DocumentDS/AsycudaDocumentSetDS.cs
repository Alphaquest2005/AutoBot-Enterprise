using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;


namespace DocumentDS.Business.Entities
{

    public partial class AsycudaDocumentSet//: IHasEntryTimeStamp
    {



        [IgnoreDataMember]
        [NotMapped]
        public IEnumerable<xcuda_ASYCUDA> Documents
        {
            get
            {
                var alist = from a in xcuda_ASYCUDA_ExtendedProperties.Where(x => x.xcuda_ASYCUDA != null)
                            select a.xcuda_ASYCUDA;
                return new List<xcuda_ASYCUDA>(alist);
            }
        }

        
    }
}
