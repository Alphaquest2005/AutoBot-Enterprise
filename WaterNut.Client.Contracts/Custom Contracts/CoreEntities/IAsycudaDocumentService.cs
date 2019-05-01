
using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Contracts;
using CoreEntities.Client.DTO;

namespace CoreEntities.Client.Contracts
{
   public partial interface IAsycudaDocumentService
   {
       [OperationContract]
       Task SaveDocument(AsycudaDocument entity);

       [OperationContract]
       Task SaveDocumentCT(AsycudaDocument entity);

       [OperationContract]
       Task DeleteDocument(int asycudaDocumentId);

       [OperationContract]
       Task ExportDocument(string fileName, int asycudaDocumentId);
       
       [OperationContract]
       Task<AsycudaDocument> NewDocument(int docSetId);

        [OperationContract]
        void IM72Ex9Document(string filename);
    }
}

