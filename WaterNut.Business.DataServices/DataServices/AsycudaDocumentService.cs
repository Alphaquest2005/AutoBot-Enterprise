
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using CoreEntities.Business.Contracts;
using System.ComponentModel.Composition;
using CoreEntities.Business.Entities;
using Core.Common.Contracts;

namespace WaterNut.Business.DataServices

{
   [Export (typeof(IAsycudaDocumentService))]
   [Export(typeof(IBusinessService))]
   [PartCreationPolicy(CreationPolicy.NonShared)]
   [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
                    ConcurrencyMode = ConcurrencyMode.Multiple)]
   
    public partial class AsycudaDocumentDataService : IAsycudaDocumentService, IDisposable
    {

        #region IAsycudaDocumentService Members

       public async Task SaveDocument(AsycudaDocument entity)
       {
           await DataSpace.BaseDataModel.Instance.SaveAsycudaDocument(entity).ConfigureAwait(false);
       }

       public async Task ExportDocument(int asycudaDocumentId)
       {
           
       }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
           // throw new NotImplementedException();
        }

        #endregion
    }
}



