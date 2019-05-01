

using System.ServiceModel;
using System.Threading.Tasks;
using EntryDataQS.Client.DTO;

namespace EntryDataQS.Client.Contracts
{

    public partial interface IEntryDataDetailsExService 
    {
		
  		[OperationContract]
       Task SaveEntryDataDetailsEX(EntryDataDetailsEx entryDataDetailsEx);
    }
}

