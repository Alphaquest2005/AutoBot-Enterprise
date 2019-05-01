


using System.Threading.Tasks;
using EntryDataQS.Client.DTO;

namespace EntryDataQS.Client.Services
{

    public partial class ContainerExClient 
    {
        public async Task SaveContainer(ContainerEx container)
        {
            await Channel.SaveContainer(container).ConfigureAwait(false);
        }  
      
    }
}

