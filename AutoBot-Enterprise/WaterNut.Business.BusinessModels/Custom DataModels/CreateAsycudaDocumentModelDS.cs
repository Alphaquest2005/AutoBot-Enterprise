using System.Threading.Tasks;

using System.Linq;


using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using DocumentDS.Business.Services;
using SimpleMvvmToolkit;
using TrackableEntities;

using Customs_Procedure = DocumentDS.Business.Entities.Customs_Procedure;

namespace WaterNut.DataSpace
{
	public class CreateAsycudaDocumentModel 
	{
       private static readonly CreateAsycudaDocumentModel instance;
        static CreateAsycudaDocumentModel()
        {
            instance = new CreateAsycudaDocumentModel();
        }

        public static CreateAsycudaDocumentModel Instance
        {
            get { return instance; }
        }

	    public async Task SaveAsycudaDocument(xcuda_ASYCUDA doc, DocInfo docInfo)
	    {
	        if (doc != null &&
	            doc.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate == false)
	        {
	            UpdateDocument(doc, docInfo);
	        }
	    }

	    public async Task SaveAsycudaDocument(AsycudaDocumentSet docSet, DocInfo docInfo)
        {
            if (docSet != null)
                {
                    if (docSet.Declarant_Reference_Number == "Search Results")
                    {
                        return;
                        //  SaveSearchResults();
                    }

                    UpdateDocSet(docSet, docInfo);
                    foreach (var cdoc in docSet.Documents.Where(x => x.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate == true))
                    {

                        cdoc.SetupProperties();

                        BaseDataModel.Instance.IntCdoc(cdoc, docSet.Document_Type, docSet);
                    
                    }
                }
                else
                {
                    //Create New AsycudaDocumentset
                    docSet = await CreateNewAsycudaDocumentSet().ConfigureAwait(false);
                    
                }


            using (var ctx = new AsycudaDocumentSetService())
            {
                await ctx.UpdateAsycudaDocumentSet(docSet).ConfigureAwait(false);
            }


           
        }

        //private void SaveSearchResults()
        //{
        //    //foreach (var item in AsycudaDocumentSummaryListModel._asycudaDocuments.ToList())
        //    //{
        //    //   Document_Type doc = db.Document_Type.FirstOrDefault();

        //    //    doc.LoadAllChild();

        //    //    Document_Type ndoc = Common.EntityClone<Document_Type>(doc);

        //    //    DetachAll(doc, true);
        //    //    //Clear Entity Values of New Object 

        //    //    ndoc.ClearEntityReference(true);
        //    //    //detach the Load empbasic from Context

        //    //    ndoc.AsycudaDocumentSet.Clear();
              
        //    //    //Add new Clone Object and save it DB 
        //    //    db.Document_Type.AddObject(ndoc);

        //    //    //xcuda_ASYCUDA doc = db.xcuda_ASYCUDA.Where(x => x.ASYCUDA_Id == item.ASYCUDA_Id).FirstOrDefault();

        //    //    //doc.LoadAllChild();

        //    //    //xcuda_ASYCUDA ndoc = Common.EntityClone<xcuda_ASYCUDA>(doc);

        //    //    //DetachAll(doc,true);
        //    //    ////Clear Entity Values of New Object 
                
        //    //    //ndoc.ClearEntityReference(true);
        //    //    ////detach the Load empbasic from Context
                
                
        //    //    ////Add new Clone Object and save it DB 
        //    //    //db.xcuda_ASYCUDA.AddObject(ndoc);
        //    //    //ndoc.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet = BaseDataModel.Instance.CurrentAsycudaDocumentSet;
        //    //    SaveDatabase();
        //    //}

        //}


       

        //private AsycudaDocumentSet CreateNewAsycudaDocumentSet()
        //{
        //   return CreateNewAsycudaDocumentSet(db);
        //}
        async Task<AsycudaDocumentSet> CreateNewAsycudaDocumentSet()
        {
            using (var ctx = new AsycudaDocumentSetService())
            {
                //var dset = (await ctx.AsycudaDocumentSets().ConfigureAwait(false)).FirstOrDefault(ads => ads.Declarant_Reference_Number == docInfo.DeclarantReferenceNumber);
                //if (dset == null)
                //{
                   var dset = new AsycudaDocumentSet(){TrackingState = TrackingState.Added};
                   // dset = await ctx.CreateAsycudaDocumentSet(dset).ConfigureAwait(false);
                  //  UpdateDocSet(dset);
                    await ctx.UpdateAsycudaDocumentSet(dset).ConfigureAwait(false);
                //}
                return dset;
            }
        }

         void UpdateDocSet(AsycudaDocumentSet dset, DocInfo docInfo = null)
         {
             if (docInfo != null)
             {
                 dset.Declarant_Reference_Number = docInfo.DeclarantReferenceNumber;
                 dset.Customs_ProcedureId = docInfo.Customs_ProcedureId;
                 if (docInfo.Description != null) dset.Description = docInfo.Description;
                 if (docInfo.Currency_Code != null) dset.Currency_Code = docInfo.Currency_Code;
                 dset.Document_TypeId = docInfo.Document_TypeId;
                 dset.Exchange_Rate = docInfo.Exchange_Rate;
                 if (docInfo.Country_of_origin_code != null) dset.Country_of_origin_code = docInfo.Country_of_origin_code;
                 if (docInfo.ManifestNumber != null) dset.Manifest_Number = docInfo.ManifestNumber;
                 if (docInfo.BlNumber != null) dset.BLNumber = docInfo.BlNumber;
                 // dset.TotalGrossWeight = docInfo.TotalGrossWeight;
             }

         }

        private  void UpdateDocument(xcuda_ASYCUDA doc, DocInfo docInfo)
        {
            if (doc != null ||doc.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate == false)
            {
                doc.SetupProperties();
                doc.xcuda_Declarant.Number = docInfo.DeclarantReferenceNumber;

                if (docInfo.Description != null)
                    doc.xcuda_ASYCUDA_ExtendedProperties.Description = docInfo.Description;
                if (docInfo.Currency_Code != null)
                    doc.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code = docInfo.Currency_Code;
                doc.xcuda_ASYCUDA_ExtendedProperties.Document_TypeId = docInfo.Document_TypeId;
                var dt = BaseDataModel.Instance.Document_Types.FirstOrDefault(x => x.Document_TypeId == docInfo.Document_TypeId);
                if (dt != null)
                {
                    doc.xcuda_Identification.xcuda_Type.Declaration_gen_procedure_code =
                        dt.Declaration_gen_procedure_code;
                    doc.xcuda_Identification.xcuda_Type.Type_of_declaration = dt.Type_of_declaration;
                }

                doc.xcuda_Valuation.xcuda_Gs_Invoice.Currency_rate = (float)docInfo.Exchange_Rate;
                if (docInfo.Country_of_origin_code != null)
                    doc.xcuda_General_information.xcuda_Country.Country_first_destination = docInfo.Country_of_origin_code;

                if (docInfo.ManifestNumber != null)
                    doc.xcuda_Identification.Manifest_reference_number = docInfo.ManifestNumber;
                if (docInfo.BlNumber != null)
                    doc.xcuda_ASYCUDA_ExtendedProperties.BLNumber = docInfo.BlNumber;

                //if (doc.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure == null || doc.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.Customs_ProcedureId != CustomsProcedureId)
                //{

                    var c = new Customs_Procedure();
                    c = BaseDataModel.Instance.Customs_Procedures.FirstOrDefault(x => x != null && x.Customs_ProcedureId == docInfo.Customs_ProcedureId);
                           if (c == null && docInfo.Document_TypeId != null)
                        {
                            c = BaseDataModel.Instance.Document_Types.FirstOrDefault(x => x.Document_TypeId == docInfo.Document_TypeId).DefaultCustoms_Procedure;
                        }
                        if (c != null)
                        {
                            //TODO: implement this
                            //foreach (var item in doc.PreviousDocumentItem
                            //    .Where(x => x.xcuda_Tarification.Extended_customs_procedure != c.Extended_customs_procedure 
                            //            || x.xcuda_Tarification.National_customs_procedure != c.National_customs_procedure).ToList())
                            //{
                            //    item.xcuda_Tarification.Extended_customs_procedure = c.Extended_customs_procedure;
                            //    item.xcuda_Tarification.National_customs_procedure = c.National_customs_procedure;
                            //}
                        }
                    doc.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure = c;
                    doc.xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId = docInfo.Customs_ProcedureId;
               
                    var b = docInfo.BlNumber;
                        //TODO: Implement this
                        //foreach (var item in doc.PreviousDocumentItem.Where(x => x.xcuda_Previous_doc.Summary_declaration != b).ToList())
                        //{
                        //    item.xcuda_Previous_doc.Summary_declaration = BlNumber;
                        //}

                    doc.xcuda_ASYCUDA_ExtendedProperties.BLNumber = docInfo.BlNumber;
          

            }
        }

        //public async Task DeleteAsycudaDocument(xcuda_ASYCUDA doc)
        //{
        //    await BaseDataModel.Instance.DeleteAsycudaDocument(doc).ConfigureAwait(false);
        //}

        //public async Task AddNewAsycudaDocument()
        //{
            
        //    BaseDataModel.Instance.CurrentAsycudaDocumentSet = await CreateNewAsycudaDocumentSet().ConfigureAwait(false);
        //    var newdoc = BaseDataModel.Instance.CreateNewAsycudaDocument();
        //    //TODO:
        //    //CurrentAsycudaDocument = newdoc;

        //    //AsycudaDocuments.Add(CurrentAsycudaDocument);
            
        //    MessageBus.Default.BeginNotify<int>(MessageToken.CurrentAsycudaDocumentSetExIDChanged, null,
        //            new NotificationEventArgs<int>(MessageToken.CurrentAsycudaDocumentSetExIDChanged, BaseDataModel.Instance.CurrentAsycudaDocumentSet.AsycudaDocumentSetId));

        //    MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentsChanged, null,
        //            new NotificationEventArgs(MessageToken.AsycudaDocumentsChanged));

        //    MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentSetExsChanged, null,
        //            new NotificationEventArgs(MessageToken.AsycudaDocumentSetExsChanged));

            
        //}



        //public async Task NewAsycudaDocumentSet()
        //{
        //    BaseDataModel.Instance.CurrentAsycudaDocumentSet = await BaseDataModel.Instance.CreateAsycudaDocumentSet().ConfigureAwait(false);
            
        //}

        //public  void AssessAllinSet()
        //{
        //    if (BaseDataModel.Instance.CurrentAsycudaDocument != null)
        //    {
        //        foreach (var doc in BaseDataModel.Instance.CurrentAsycudaDocumentSet.Documents)//.Where(x => x != CurrentAsycudaDocument && x.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed != CurrentAsycudaDocument.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed))
        //        {
        //            doc.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed = BaseDataModel.Instance.CurrentAsycudaDocument.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed;
        //            doc.xcuda_ASYCUDA_ExtendedProperties.CNumber = BaseDataModel.Instance.CurrentAsycudaDocumentSet.Declarant_Reference_Number;
        //            doc.xcuda_ASYCUDA_ExtendedProperties.RegistrationDate = BaseDataModel.Instance.CurrentAsycudaDocumentSet.RegistrationDate;
        //            doc.xcuda_ASYCUDA_ExtendedProperties.ReferenceNumber = BaseDataModel.Instance.CurrentAsycudaDocumentSet.Declarant_Reference_Number + "-F" + doc.xcuda_ASYCUDA_ExtendedProperties.FileNumber.ToString(); 
        //        }
               
        //        MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentsChanged, null,
        //                new NotificationEventArgs(MessageToken.AsycudaDocumentsChanged));

        //        MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentSetExsChanged, null,
        //                new NotificationEventArgs(MessageToken.AsycudaDocumentSetExsChanged));
              
        //        MessageBox.Show("Complete");
        //    }
        //    else
        //    {
        //        MessageBox.Show("Please select an Asycuda Document before continuing");
        //    }
        //}

       
    }

    public class DocInfo
    {
        public string Currency_Code { get; set; }
        public string DeclarantReferenceNumber { get; set; }
        public int? Customs_ProcedureId { get; set; }
        public string Description { get; set; }
        public int? Document_TypeId { get; set; }
        public double Exchange_Rate { get; set; }
        public string Country_of_origin_code { get; set; }
        public string ManifestNumber { get; set; }
        public string BlNumber { get; set; }
        public decimal TotalGrossWeight { get; set; }
    }
}