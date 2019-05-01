using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Contracts;
using CoreEntities.Client.DTO;

namespace CoreEntities.Client.Contracts
{
   
    public partial interface IAsycudaDocumentItemService
    {
        [OperationContract]
        Task RemoveSelectedItems(IEnumerable<int> lst);
        [OperationContract]
        Task SaveAsycudaDocumentItem(AsycudaDocumentItem asycudaDocumentItem);
    }
}

