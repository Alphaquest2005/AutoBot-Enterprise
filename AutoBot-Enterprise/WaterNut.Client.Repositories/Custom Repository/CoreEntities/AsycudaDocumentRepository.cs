

using CoreEntities.Client.Services;


using System.Threading.Tasks;
using AsycudaDocument = CoreEntities.Client.Entities.AsycudaDocument;

namespace CoreEntities.Client.Repositories 
{
   
    public partial class AsycudaDocumentRepository 
    {
        public async Task SaveDocument(AsycudaDocument doc)
        {
            if(doc == null) return ;
            using (var ctx = new AsycudaDocumentClient())
            {
                await ctx.SaveDocument(doc.DTO).ConfigureAwait(false);
            }
        }

        public async Task SaveDocumentCT(AsycudaDocument doc)
        {
            if (doc == null) return;
            using (var ctx = new AsycudaDocumentClient())
            {
                await ctx.SaveDocumentCT(doc.DTO).ConfigureAwait(false);
            }
        }

        public async Task DeleteDocument(int asycudaDocumentId)
        {
            using (var ctx = new AsycudaDocumentClient())
            {
                await ctx.DeleteDocument(asycudaDocumentId).ConfigureAwait(false);
            }
        }

        public async Task ExportDocument(string fileName, int asycudaDocumentId)
        {
           
            using (var ctx = new AsycudaDocumentClient())
            {
                await ctx.ExportDocument(fileName, asycudaDocumentId).ConfigureAwait(false);
            }
        }


        public async Task IM72Ex9Document(string fileName)
        {

            using (var ctx = new AsycudaDocumentClient())
            {
                ctx.IM72Ex9Document(fileName);
            }
        }

        public async Task<AsycudaDocument> NewDocument(int docSetId)
        {
            using (var ctx = new AsycudaDocumentClient())
            {
                var dto = await ctx.NewDocument(docSetId).ConfigureAwait(false);
                return new AsycudaDocument(dto);
            }
        }
    }
}

