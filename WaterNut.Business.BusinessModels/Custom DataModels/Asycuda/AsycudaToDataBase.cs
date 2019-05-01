using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using DocumentItemDS.Business.Services;
using InventoryDS.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
//using WaterNut.DataLayer;
using TrackableEntities;
using WaterNut.Asycuda;
using WaterNut.Business.Entities;
using WaterNut.Interfaces;

using Customs_Procedure = DocumentDS.Business.Entities.Customs_Procedure;
using xcuda_Item = DocumentItemDS.Business.Entities.xcuda_Item;
using xcuda_PreviousItem = DocumentItemDS.Business.Entities.xcuda_PreviousItem;
using xcuda_PreviousItemService = DocumentItemDS.Business.Services.xcuda_PreviousItemService;
using DIBaseDataModel = WaterNut.DataSpace.DocumentItemDS.DataModels.BaseDataModel;
using DBaseDataModel = WaterNut.DataSpace.DocumentDS.DataModels.BaseDataModel;
using Document_Type = DocumentDS.Business.Entities.Document_Type;
using EntryPreviousItems = DocumentItemDS.Business.Entities.EntryPreviousItems;
using xcuda_Goods_description = DocumentItemDS.Business.Entities.xcuda_Goods_description;
using xcuda_HScode = DocumentItemDS.Business.Entities.xcuda_HScode;
using xcuda_Item_Invoice = DocumentItemDS.Business.Entities.xcuda_Item_Invoice;
using xcuda_Supplementary_unit = DocumentItemDS.Business.Entities.xcuda_Supplementary_unit;
using xcuda_Tarification = DocumentItemDS.Business.Entities.xcuda_Tarification;
using xcuda_Taxation = DocumentItemDS.Business.Entities.xcuda_Taxation;
using xcuda_Taxation_line = DocumentItemDS.Business.Entities.xcuda_Taxation_line;
using xcuda_Valuation_item = DocumentItemDS.Business.Entities.xcuda_Valuation_item;
using xcuda_Weight = DocumentDS.Business.Entities.xcuda_Weight;
using xcuda_Weight_itm = DocumentItemDS.Business.Entities.xcuda_Weight_itm;


namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase
    {
        private static readonly AsycudaToDataBase instance;
        static AsycudaToDataBase()
        {
            instance = new AsycudaToDataBase();
           
       
        }

         public static AsycudaToDataBase Instance
        {
            get { return instance; }
        }

        private DocumentCT da;
        private ASYCUDA a;

       
        //private static DataCache<AsycudaDocumentItem> _asycudaDocumentItemCache;
        //private static DataCache<AsycudaDocument> _asycudaDocumentCache;

        private bool updateItemsTariffCode = false;
        private bool importOnlyRegisteredDocuments = true;

        [XmlIgnore()]
        public bool UpdateItemsTariffCode
        {
            get { return updateItemsTariffCode; }
            set { updateItemsTariffCode = value; }
        }

        [XmlIgnore()]
        public bool ImportOnlyRegisteredDocuments
        {
            get { return importOnlyRegisteredDocuments; }
            set { importOnlyRegisteredDocuments = value; }
        }
        public bool OverwriteExisting { get; set; }
        public bool NoMessages { get; set; }
        public async Task SaveToDatabase(ASYCUDA adoc, AsycudaDocumentSet docSet)
        {

            try
            {

                // db = new WaterNutDBEntities();
                a = adoc;
                xcuda_ASYCUDA doc;
                if (await DeleteExistingDocument().ConfigureAwait(false)) return;

                var ads = docSet; //await GetAsycudaDocumentSet().ConfigureAwait(false);
                //}
                da = await CreateDocumentCt(ads).ConfigureAwait(false);


                await SaveGeneralInformation().ConfigureAwait(false);
                await SaveDeclarant().ConfigureAwait(false);
                await SaveTraders().ConfigureAwait(false);
                await SaveProperty().ConfigureAwait(false);
                await SaveIdentification().ConfigureAwait(false);
                await SaveTransport().ConfigureAwait(false);
                await SaveFinancial().ConfigureAwait(false);
                await Save_Warehouse().ConfigureAwait(false);
                await Save_Valuation().ConfigureAwait(false);
                await SaveContainer().ConfigureAwait(false);

                await Save_Items().ConfigureAwait(false);
                
                if (!da.DocumentItems.Any() == true)
                {
                    await BaseDataModel.Instance.DeleteAsycudaDocument(da.Document.ASYCUDA_Id).ConfigureAwait(false);
                    return;
                }
                
                await LinkExistingPreviousItems().ConfigureAwait(false);

                await SavePreviousItem().ConfigureAwait(false);

                await Save_Suppliers_Documents().ConfigureAwait(false);

                if(!da.DocumentItems.Any(x => x.ImportComplete == false))
                    da.Document.xcuda_ASYCUDA_ExtendedProperties.ImportComplete = true;

                //await
                    //DBaseDataModel.Instance.Savexcuda_ASYCUDA_ExtendedProperties(
                    //    da.Document.xcuda_ASYCUDA_ExtendedProperties).ConfigureAwait(false);

                await BaseDataModel.Instance.SaveDocumentCT(da).ConfigureAwait(false);

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (da != null && da.Document != null && !da.Document.xcuda_ASYCUDA_ExtendedProperties.ImportComplete)
                {
                   BaseDataModel.Instance.DeleteDocumentCt(da);
                }
            }

        }

        private  async Task<DocumentCT> CreateDocumentCt(AsycudaDocumentSet ads)
        {
            DocumentCT da = await BaseDataModel.Instance.CreateDocumentCt(ads).ConfigureAwait(false);
            da.Document.xcuda_ASYCUDA_ExtendedProperties.ImportComplete = false;
            da.Document.id = a.id;
            da.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = false;
            da.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = ads.AsycudaDocumentSetId;
            da.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet = ads;
            //await BaseDataModel.Instance.SaveDocumentCT(da).ConfigureAwait(false);
            
            return da;
        }

        private async Task<AsycudaDocumentSet> GetAsycudaDocumentSet(int docSetId = -1)
        {
            AsycudaDocumentSet ads;
            //db.AsycudaDocumentSet.FirstOrDefault(
            //    x =>
            //    x.Declarant_Reference_Number.Replace(" ", "")
            //     .Contains(refstr.Substring(0, refstr.Length - refstr.IndexOf("-F"))));
            //if (ads == null)
            //{
            if (docSetId == -1)
            {
                ads = await NewAsycudaDocumentSet(a).ConfigureAwait(false);
            }
            else
            {
                ads =
                    await
                        BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId,
                            new List<string>()
                            {
                                "ASYCUDA_ExtendedProperties"
                            }).ConfigureAwait(false);
            }
            return ads;
        }

        private async Task<bool> DeleteExistingDocument()
        {
            xcuda_ASYCUDA doc = null;
            if (string.IsNullOrEmpty(a.Identification.Registration.Number))
            {
                if (ImportOnlyRegisteredDocuments) return true;
                a.Identification.Registration.Number = "0";
            }
            if (a.Identification.Registration.Date == "")
                a.Identification.Registration.Date = DateTime.MinValue.ToShortDateString();

            //docs =  db.xcuda_ASYCUDA.Where(x => x.id == a.id).ToList();
            doc = (await DBaseDataModel.Instance.Searchxcuda_ASYCUDA(new List<string>()
            {
                string.Format("id == \"{0}\"",a.id)
            
            }, new List<string>()
            {
                "xcuda_Identification ",
                "xcuda_ASYCUDA_ExtendedProperties",
                "xcuda_Identification.xcuda_Registration",
                "xcuda_Identification.xcuda_Office_segment",
                "xcuda_Declarant"
            }).ConfigureAwait(false)).FirstOrDefault();

            ////if (doc == null)
            ////{
            ////    using (var ctx = new xcuda_ASYCUDAService())
            ////    {
            ////        doc = (await ctx.Getxcuda_ASYCUDAByExpressionLst(new List<string>()
            ////        {
            ////            string.Format("xcuda_Identification.xcuda_Registration.Number == \"{0}\"",
            ////                a.Identification.Registration.Number),
            ////            string.Format("xcuda_Identification.xcuda_Registration.Date != null && Convert.ToDateTime(xcuda_Identification.xcuda_Registration.Date).Year == {0}",
            ////                Convert.ToDateTime(a.Identification.Registration.Date).Year),
            ////            string.Format(
            ////                "xcuda_Identification.xcuda_Office_segment.Customs_clearance_office_code == \"{0}\"",
            ////                a.Identification.Office_segment.Customs_clearance_office_code.Text)
            ////        }, new List<string>()
            ////        {
            ////            "xcuda_Identification ",
            ////            "xcuda_ASYCUDA_ExtendedProperties",
            ////            "xcuda_Identification.xcuda_Registration",
            ////            "xcuda_Identification.xcuda_Office_segment",
            ////            "xcuda_Declarant"
            ////        }).ConfigureAwait(false)).FirstOrDefault();
            ////    }
            ////}
            //// check the declarant reference number
            //if (doc == null)
            //{
            //    doc =
            //        db.xcuda_ASYCUDA.Where(x => x.xcuda_Identification.xcuda_Registration.Number == null
            //                                    && x.xcuda_Declarant != null
            //                                    && x.xcuda_Declarant.Number != null
            //                                    && x.xcuda_Declarant.Number.Replace(" ", "")
            //                                    == a.Declarant.Reference.Number.Replace(" ", ""))
            //            .AsEnumerable()
            //            .Where(c => c.RegistrationDate == DateTime.MinValue
            //                        || (c.RegistrationDate != DateTime.MinValue
            //                            &&
            //                            c.RegistrationDate.Year ==
            //                            Convert.ToDateTime(a.Identification.Registration.Date).Year)
            //                        &&
            //                        c.xcuda_Identification.xcuda_Office_segment.Customs_clearance_office_code ==
            //                        a.Identification.Office_segment.Customs_clearance_office_code.Text
            //                            .FirstOrDefault()
            //            ).Distinct().ToList();


            //}

            if (doc != null)
            {
                if (!OverwriteExisting && doc.xcuda_ASYCUDA_ExtendedProperties.ImportComplete) return true;
                await BaseDataModel.Instance.DeleteAsycudaDocument(doc.ASYCUDA_Id).ConfigureAwait(false);
            }
            return false;
        }

        private async Task LinkExistingPreviousItems()
        {
            //get all previous items for this document
            var year = Convert.ToDateTime(da.Document.xcuda_Identification.xcuda_Registration.Date).Year.ToString();
            var plst = await DIBaseDataModel.Instance.Searchxcuda_PreviousItem(new List<string>()
            {
                string.Format("Prev_reg_nbr == \"{0}\"",da.Document.xcuda_Identification.xcuda_Registration.Number),
                string.Format("Prev_reg_dat == \"{0}\"",year),
                string.Format("Prev_reg_cuo == \"{0}\"",da.Document.xcuda_Identification.xcuda_Office_segment.Customs_clearance_office_code)

            }).ConfigureAwait(false);

            //Where(x => x.Prev_reg_nbr == da.xcuda_Identification.xcuda_Registration.Number
            //                                            && x.Prev_reg_dat == year
            //                                            && x.Prev_reg_cuo == da.xcuda_Identification.xcuda_Office_segment.Customs_clearance_office_code).ToList();

            if (plst.Any() == false) return;
            foreach (var itm in da.DocumentItems)
            {
                var pplst = plst.Where(x => x.Previous_item_number == itm.LineNumber.ToString());
                //if(pplst.Any == false) MessageBox.Show("Please Import 
                foreach (var p in pplst)
                {
                    var ep = new EntryPreviousItems(){Item_Id = itm.Item_Id, PreviousItem_Id = p.PreviousItem_Id, TrackingState = TrackingState.Added};
                    itm.xcuda_PreviousItems.Add(ep);
                   //await DIBaseDataModel.Instance.SaveEntryPreviousItems(ep).ConfigureAwait(false);
                }
                    
                
            }

        }

  

        private async Task SavePreviousItem()
        {

            try
            {

                for (var i = 0; i < a.PreviousItem.Count; i++)
                {


                    var ai = a.PreviousItem.ElementAt(i);
                    if (ai == null) continue;
                    var pi = await GetExistingPreviousItem(ai).ConfigureAwait(false);


                  


                    if (pi == null)
                    {

                        pi = new global::DocumentItemDS.Business.Entities.xcuda_PreviousItem() { PreviousItem_Id = da.DocumentItems.ElementAt(i).Item_Id, TrackingState = TrackingState.Added };
                        da.DocumentItems.ElementAt(i).xcuda_PreviousItem = pi;
                    }

                   
                        var bl = String.Format("{0} {1} C {2} art. {3}", ai.Prev_reg_cuo,
                            ai.Prev_reg_dat,
                            ai.Prev_reg_nbr, ai.Previous_item_number);

                        pi.xcuda_Item = (await SearchDocumentItems(new List<string>()
                            {
                                string.Format("Item_Id == {0}", da.DocumentItems.ElementAt(i).Item_Id )
                            }, new List<string>()
                            {
                                "xcuda_Tarification.xcuda_HScode",
                                "xcuda_PreviousItems.xcuda_PreviousItem"
                            }).ConfigureAwait(false)).FirstOrDefault();

                      
                        await LinkPi2Item(ai, pi, bl).ConfigureAwait(false);

                        pi.Commodity_code = ai.Commodity_code;
                        pi.Current_item_number = ai.Current_item_number;
                        pi.Current_value = Convert.ToSingle(Math.Round(Convert.ToDouble(ai.Current_value), 2));
                        pi.Goods_origin = ai.Goods_origin;
                        pi.Hs_code = ai.Hs_code;
                        pi.Net_weight = Convert.ToSingle(ai.Net_weight);
                        pi.Packages_number = ai.Packages_number;
                        pi.Prev_net_weight = Convert.ToSingle(ai.Prev_net_weight);
                        pi.Prev_reg_cuo = ai.Prev_reg_cuo;
                        pi.Prev_reg_dat = ai.Prev_reg_dat;
                        pi.Prev_reg_nbr = ai.Prev_reg_nbr;
                        pi.Prev_reg_ser = ai.Prev_reg_ser;
                        pi.Preveious_suplementary_quantity = Convert.ToSingle(ai.Preveious_suplementary_quantity);
                        pi.Previous_item_number = ai.Previous_item_number;
                        pi.Previous_Packages_number = ai.Previous_Packages_number;
                        if (ai.Previous_value != null)
                            pi.Previous_value = (float)Math.Round(Convert.ToDouble(ai.Previous_value), 2);
                        pi.Suplementary_Quantity = Convert.ToSingle(ai.Suplementary_Quantity);

                      //await DataSpace.DocumentItemDS.ViewModels.BaseDataModel.Instance.Savexcuda_PreviousItem(pi).ConfigureAwait(false);

                }
            }
            catch (Exception Ex)
            {
                throw;
            }
        }

        private async Task<IEnumerable<xcuda_Item>> SearchDocumentItems(List<string> explst, List<string> includeLst = null)
        {
            using (var ctx = new xcuda_ItemService())
            {

                return await ctx.Getxcuda_ItemByExpressionLst(explst, includeLst).ConfigureAwait(false);
            }
        }

        private async Task<xcuda_PreviousItem> GetExistingPreviousItem(ASYCUDAPreviousItem ai)
        {
            using (var ctx = new xcuda_PreviousItemService())
            {
                return (await ctx.Getxcuda_PreviousItemByExpressionLst(new List<string>()
                {
                    string.Format("Ex.RndCurrent_Value == {0}", ai.Current_value),
                    string.Format("Prev_reg_cuo == \"{0}\"", ai.Prev_reg_cuo),
                    string.Format("Prev_reg_dat == \"{0}\"", ai.Prev_reg_dat),
                    string.Format("Prev_reg_nbr == \"{0}\"", ai.Prev_reg_nbr),
                    string.Format("Previous_item_number == \"{0}\"", ai.Previous_item_number),
                    string.Format("Current_item_number == \"{0}\"", ai.Current_item_number),
                    string.Format("Suplementary_Quantity == {0}", ai.Suplementary_Quantity)
                }, new List<string>()
                {
                    
                    "xcuda_Items.xcuda_Item",
                    "xcuda_Item.xcuda_Tarification.xcuda_HScode"
                }).ConfigureAwait(false)).FirstOrDefault();

                //string.Format("Prev_reg_cuo == \"{0}\" && " +
                //                  "Prev_reg_dat == \"{1}\" &&" +
                //                  "Prev_reg_nbr == \"{2}\" &&" +
                //                  "Previous_item_number == \"{3}\" &&" +
                //                  "Current_item_number == \"{4}\" &&" +
                //                  "Suplementary_Quantity == {5}", ai.Prev_reg_cuo
                //                  ,ai.Prev_reg_dat, ai.Prev_reg_nbr,ai.Previous_item_number,ai.Current_item_number,ai.Suplementary_Quantity)
                //,new Dictionary<string, string>()
                //{
                //    {"Ex", string.Format("RndCurrent_Value == {0}", ai.Current_value)}
                //}).ConfigureAwait(false)).FirstOrDefault();

                //.xcuda_PreviousItem.Where(x =>
                //    Math.Round(x.Current_value, 2).ToString() == ai.Current_value
                //    && x.Prev_reg_cuo == ai.Prev_reg_cuo
                //    && x.Prev_reg_dat == ai.Prev_reg_dat
                //    && x.Prev_reg_nbr == ai.Prev_reg_nbr
                //    && x.Previous_item_number == ai.Previous_item_number
                //    && x.Current_item_number == ai.Current_item_number
                //    && x.Suplementary_Quantity.ToString() == ai.Suplementary_Quantity);
            }
        }

        private async Task LinkPi2Item(ASYCUDAPreviousItem ai, xcuda_PreviousItem pi, string bl)
        {
            // find original row
            if (pi.xcuda_Item != null)
            {
                var pLineNo = Convert.ToInt32(ai.Previous_item_number);
                // get document

                var pdoc = (await DBaseDataModel.Instance.Searchxcuda_ASYCUDA(new List<string>()
                {
                    string.Format("xcuda_Identification.xcuda_Registration.Number == \"{0}\"", ai.Prev_reg_nbr),
                    string.Format("Ex.RegistrationYear == {0}",
                        ai.Prev_reg_dat)

                }, new List<string>()
                {
                    "xcuda_Identification.xcuda_Registration",
                    "xcuda_ASYCUDA_ExtendedProperties"
                })).FirstOrDefault();
                //.Where(
                //    x => x.xcuda_Identification.xcuda_Registration.Number == ai.Prev_reg_nbr)
                //    .AsEnumerable()
                //    .FirstOrDefault(
                //        x =>
                //            DateTime.Parse(x.xcuda_Identification.xcuda_Registration.Date).Year.ToString() ==
                //            ai.Prev_reg_dat);
                if (pdoc == null)
                {
                    if (!NoMessages)
                    {
                        throw new ApplicationException(
                            string.Format("You need to import Previous Document '{0}' before importing this Ex9",
                                ai.Prev_reg_nbr));
                    }
                    return;
                }
                if (pi.xcuda_Item.xcuda_Tarification == null || pi.xcuda_Item.xcuda_Tarification.xcuda_HScode == null)
                    return;

                var originalItm = (await DIBaseDataModel.Instance.Searchxcuda_Item(new List<string>()
                {
                    string.Format("xcuda_Tarification.xcuda_HScode.Precision_4.Contains(\"{0}\")",pi.xcuda_Item.xcuda_Tarification.xcuda_HScode.Precision_4),
                    string.Format("LineNumber == {0}", pLineNo),
                    string.Format("ASYCUDA_Id == {0}",pdoc.ASYCUDA_Id)
                }, new List<string>()
                {
                    "xcuda_PreviousItems.xcuda_PreviousItem"
                }).ConfigureAwait(false)).FirstOrDefault();
                    //db.xcuda_Item.Where(
                    //    x =>
                    //        x.xcuda_Tarification.xcuda_HScode.Precision_4.Contains(
                    //            pi.xcuda_Item.xcuda_Tarification.xcuda_HScode.Precision_4)
                    //            && x.LineNumber == pLineNo
                    //            && x.ASYCUDA_Id == pdoc.ASYCUDA_Id)
                    //            .AsEnumerable()
                    
                    //            .FirstOrDefault();



                if (originalItm != null)
                {
                    if (pi.xcuda_Items.Select(x => x.xcuda_Item).Contains(originalItm) == false && originalItm.xcuda_PreviousItems.Select(x => x.xcuda_PreviousItem).Contains(pi) == false)
                    {
                        var ep = new EntryPreviousItems()
                        {
                            Item_Id = originalItm.Item_Id,
                            PreviousItem_Id = pi.PreviousItem_Id,
                            TrackingState = TrackingState.Added
                        };
                       // //await DIBaseDataModel.Instance.SaveEntryPreviousItems(ep).ConfigureAwait(false);
                        pi.xcuda_Items.Add(ep);
                        //originalItm.xcuda_PreviousItems.Add(ep);
                    }

                }
                else
                {
                    if(!NoMessages)
                        throw new ApplicationException(
                        string.Format("Item Not found {0} line: {1} PrevCNumber: {2} CNumber: {3}",
                            pi.xcuda_Item.xcuda_Tarification.xcuda_HScode.Precision_4, pLineNo, pdoc.CNumber,
                            da.Document.CNumber));
                }
            }
            else
            {
                if (!NoMessages)
                    throw new ApplicationException(string.Format("Item Not found {0}, LineNo:-{1}",
                    bl, ai.Current_item_number));
            }
        }



        public static async Task<AsycudaDocumentSet> NewAsycudaDocumentSet(ASYCUDA a)
        {
            var ads = (await DBaseDataModel.Instance.SearchAsycudaDocumentSet(new List<string>()
            {
                string.Format("Declarant_Reference_Number == \"{0}\"", a.Declarant.Reference.Number)
            }).ConfigureAwait(false)).FirstOrDefault();//.AsycudaDocumentSet.FirstOrDefault(d => d.Declarant_Reference_Number == a.Declarant.Reference.Number);
            if (ads == null)
            {
                ads = new AsycudaDocumentSet
                    {
                        TrackingState = TrackingState.Added,
                        Declarant_Reference_Number = a.Declarant.Reference.Number,
                        Currency_Code = a.Valuation.Gs_Invoice.Currency_code.Text.FirstOrDefault(),
                        Document_Type =
                            BaseDataModel.Instance.Document_Types
                              .FirstOrDefault(
                                  d =>
                                  d.Type_of_declaration == a.Identification.Type.Type_of_declaration &&
                                  d.Declaration_gen_procedure_code == a.Identification.Type.Declaration_gen_procedure_code)
                    };


                if (ads.Document_Type == null)
                {
                    var dt = new Document_Type
                        {
                            Declaration_gen_procedure_code = a.Identification.Type.Declaration_gen_procedure_code,
                            Type_of_declaration = a.Identification.Type.Type_of_declaration,
                            TrackingState = TrackingState.Added
                        };
                    //await DBaseDataModel.Instance.SaveDocument_Type(dt).ConfigureAwait(false);
                    ads.Document_Type = dt;
                }

                ads.Customs_Procedure = BaseDataModel.Instance.Customs_Procedures
                                       .FirstOrDefault(cp => cp.National_customs_procedure == a.Item.FirstOrDefault().Tarification.National_customs_procedure 
                                                    && cp.Extended_customs_procedure == a.Item.FirstOrDefault().Tarification.Extended_customs_procedure);
                if (ads.Customs_Procedure == null)
                {
                    var cp = new Customs_Procedure
                        {
                            Extended_customs_procedure = a.Item[0].Tarification.Extended_customs_procedure,
                            National_customs_procedure = a.Item[0].Tarification.National_customs_procedure,
                            Document_Type = ads.Document_Type,
                            TrackingState = TrackingState.Added
                        };
                    //await DBaseDataModel.Instance.SaveCustoms_Procedure(cp).ConfigureAwait(false);
                    ads.Customs_Procedure = cp;
                }

                ads.Exchange_Rate = Convert.ToSingle(a.Valuation.Gs_Invoice.Currency_rate);

                //await DBaseDataModel.Instance.SaveAsycudaDocumentSet(ads).ConfigureAwait(false);

                return ads;
            }
            else
            {
                return ads;
            }

        }

        private async Task Save_Suppliers_Documents()
        {
            if (a.Suppliers_documents.Count > 0 && a.Suppliers_documents[0] == null) return;
            for (int i = 0; i < a.Suppliers_documents.Count; i++)
            {
                var asd = a.Suppliers_documents.ElementAt(i);  
            
                var s = da.Document.xcuda_Suppliers_documents.ElementAtOrDefault(i);
                if (s == null)
                {
                    s = new xcuda_Suppliers_documents()
                    {
                        ASYCUDA_Id = da.Document.ASYCUDA_Id,
                        TrackingState = TrackingState.Added
                    };
                    da.Document.xcuda_Suppliers_documents.Add(s);
                }
                // var asd = a.Suppliers_documents[0];
                if (asd.Suppliers_document_city.Text.Count > 0)
                    s.Suppliers_document_city = asd.Suppliers_document_city.Text[0];

                s.Suppliers_document_date = asd.Suppliers_document_date;

                if (asd.Suppliers_document_country.Text.Count > 0)
                    s.Suppliers_document_country = asd.Suppliers_document_country.Text[0];

                if (asd.Suppliers_document_fax.Text.Count > 0)
                    s.Suppliers_document_fax = asd.Suppliers_document_fax.Text[0];

                if (asd.Suppliers_document_name.Text.Count > 0)
                    s.Suppliers_document_name = asd.Suppliers_document_name.Text[0];

                if (asd.Suppliers_document_street.Text.Count > 0)
                    s.Suppliers_document_street = asd.Suppliers_document_street.Text[0];

                if (asd.Suppliers_document_telephone.Text.Count > 0)
                    s.Suppliers_document_telephone = asd.Suppliers_document_telephone.Text[0];

                if (asd.Suppliers_document_type_code.Text.Count > 0)
                    s.Suppliers_document_type_code = asd.Suppliers_document_type_code.Text[0];
                if (asd.Suppliers_document_zip_code.Text.Count > 0)
                    s.Suppliers_document_zip_code = asd.Suppliers_document_zip_code.Text[0];

                //await DBaseDataModel.Instance.Savexcuda_Suppliers_documents(s).ConfigureAwait(false);

            }
        }



        private async Task Save_Items()
        {
            try
            {

          
          for (var i = 0; i < a.Item.Count; i++)
           // Parallel.For(0, a.Item.Count, i =>
            {
                var ai = a.Item.ElementAt(i);
                xcuda_Item di;

                /////*******************remove check because of duplicate on im7 to make this standard procedure
                //if (da.xcuda_Identification.xcuda_Type.DisplayName == "Ex9" || da.xcuda_Identification.xcuda_Type.DisplayName == "IM4")
                //{
                di = da.DocumentItems.ElementAtOrDefault(i);
                //}
                //else
                //{
                //    di = da.xcuda_Item.FirstOrDefault(x => x.ItemNumber == ai.Tarification.HScode.Precision_4
                //                                                  && x.ItemQuantity.ToString() == ai.Tarification.Supplementary_unit.FirstOrDefault().Suppplementary_unit_quantity
                //                                                  && Math.Round(x.xcuda_Tarification.Item_price, 2).ToString() == ai.Tarification.Item_price);
                //}                                                 
                if (di == null)
                {
                    di = new xcuda_Item() {ASYCUDA_Id = da.Document.ASYCUDA_Id, ImportComplete = false, TrackingState = TrackingState.Added};
                        // db.xcuda_Item.CreateObject();//
                    //DIBaseDataModel.Instance.Savexcuda_Item(di);
                    da.DocumentItems.Add(di);

                }

                if (!string.IsNullOrEmpty(a.Identification.Registration.Number))
                {
                    di.IsAssessed = true;
                }



                di.LineNumber = i + 1;

                if (ai.Licence_number.Text.Count > 0)
                {
                    di.Licence_number = ai.Licence_number.Text[0];
                    di.Amount_deducted_from_licence = ai.Amount_deducted_from_licence;
                    di.Quantity_deducted_from_licence = ai.Quantity_deducted_from_licence;
                }
               

                //DIBaseDataModel.Instance.Savexcuda_Item(di).Wait();
                //await DIBaseDataModel.Instance.Savexcuda_Item(di).ConfigureAwait(false);

                await Save_Item_Suppliers_link(di, ai).ConfigureAwait(false);
               
                await Save_Item_Attached_documents(di, ai).ConfigureAwait(false);

                await Save_Item_Packages(di, ai).ConfigureAwait(false);

                // SaveInventoryItem(ai);


                await Save_Item_Tarification(di, ai).ConfigureAwait(false);
                //Save_Item_Tarification(di, ai).Wait();
                await Save_Item_Goods_description(di, ai).ConfigureAwait(false);
                await Save_Item_Previous_doc(di, ai).ConfigureAwait(false);
                await Save_Item_Taxation(di, ai).ConfigureAwait(false);
                //Save_Item_Taxation(di, ai).Wait();
               await Save_Item_Valuation_item(di, ai).ConfigureAwait(false);
               // Save_Item_Valuation_item(di, ai).Wait();

                di.ImportComplete = true;
               //await DIBaseDataModel.Instance.Savexcuda_Item(di).ConfigureAwait(false);

            }
            //    );
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<InventoryItem> SaveInventoryItem(ASYCUDAItem ai)
        {
            var iv =
                 BaseDataModel.Instance.InventoryCache.GetSingle(x => x.ItemNumber == ai.Tarification.HScode.Precision_4.Text.FirstOrDefault());
            ////    (await DataSpace.InventoryDS.ViewModels.BaseDataModel.Instance.SearchInventoryItem(new List<string>()
            ////{
            ////    string.Format("ItemNumber == \"{0}\"",ai.Tarification.HScode.Precision_4.Text.FirstOrDefault())
            ////}).ConfigureAwait(false)).FirstOrDefault();
                //InventoryItems.FirstOrDefault(i => i.ItemNumber == ai.Tarification.HScode.Precision_4.Text.FirstOrDefault());
            if (iv == null && ai.Tarification.HScode.Precision_4.Text.FirstOrDefault() != null)
            {

                iv = new InventoryItem()
                {
                    ItemNumber = ai.Tarification.HScode.Precision_4.Text.FirstOrDefault(),
                    Description = ai.Goods_description.Commercial_Description,
                    TrackingState = TrackingState.Added
                };
                

                    var tc = BaseDataModel.Instance.TariffCodeCache.GetSingle(x => x.TariffCodeName == ai.Tarification.HScode.Commodity_code);
                    //(await DataSpace.InventoryDS.ViewModels.BaseDataModel.Instance.SearchTariffCode(new List<string>()
                    //{
                    //    string.Format("TariffCodeName == \"{0}\"", ai.Tarification.HScode.Commodity_code)
                    //}).ConfigureAwait(false)).FirstOrDefault();
                    if (tc != null)
                        iv.TariffCode = ai.Tarification.HScode.Commodity_code;

                    // DataSpace.InventoryDS.ViewModels.BaseDataModel.Instance.SaveInventoryItem(iv).Wait();
                    await DataSpace.InventoryDS.DataModels.BaseDataModel.Instance.SaveInventoryItem(iv).ConfigureAwait(false);
                    BaseDataModel.Instance.InventoryCache.AddItem(iv);
               

            }
            //include tarrifcode
            return iv;
        }

        private async Task Save_Item_Valuation_item(xcuda_Item di, ASYCUDAItem ai)
        {
            var vi = di.xcuda_Valuation_item;//.FirstOrDefault();
            if (vi == null)
            {
                vi = new xcuda_Valuation_item(){Item_Id = di.Item_Id, TrackingState = TrackingState.Added};
              //  DIBaseDataModel.Instance.Savexcuda_Valuation_item(vi);
                di.xcuda_Valuation_item = vi;//di.xcuda_Valuation_item.Add(vi);
            }
            if (ai.Valuation_item.Alpha_coeficient_of_apportionment != "")
                vi.Alpha_coeficient_of_apportionment = ai.Valuation_item.Alpha_coeficient_of_apportionment;
            if (ai.Valuation_item.Rate_of_adjustement != "")
                vi.Rate_of_adjustement = Convert.ToDouble(ai.Valuation_item.Rate_of_adjustement);
            if (ai.Valuation_item.Statistical_value != "")
                vi.Statistical_value = Convert.ToSingle(ai.Valuation_item.Statistical_value);
            if (ai.Valuation_item.Total_CIF_itm != "")
                vi.Total_CIF_itm = Convert.ToSingle(ai.Valuation_item.Total_CIF_itm);
            if (ai.Valuation_item.Total_cost_itm != "")
                vi.Total_cost_itm = Convert.ToSingle(ai.Valuation_item.Total_cost_itm);

            Save_Item_Invoice(vi, ai);
            Save_item_External_freight(vi, ai);
            Save_Weight_Item(vi, ai);

           //await DIBaseDataModel.Instance.Savexcuda_Valuation_item(vi).ConfigureAwait(false);
        }

        private void Save_Weight_Item(xcuda_Valuation_item vi, ASYCUDAItem ai)
        {
            var wi = vi.xcuda_Weight_itm;
            if (wi == null)
            {
                wi = new xcuda_Weight_itm() { Valuation_item_Id = vi.Item_Id, TrackingState = TrackingState.Added };
                vi.xcuda_Weight_itm = wi;
            }
            if (ai.Valuation_item.Weight_itm.Gross_weight_itm != "")
                wi.Gross_weight_itm = Convert.ToSingle(ai.Valuation_item.Weight_itm.Gross_weight_itm);

            if (ai.Valuation_item.Weight_itm.Net_weight_itm != "")
                wi.Net_weight_itm = Convert.ToSingle(ai.Valuation_item.Weight_itm.Net_weight_itm);

        }

        private void Save_item_External_freight(xcuda_Valuation_item vi, ASYCUDAItem ai)
        {
            var i = vi.xcuda_item_external_freight;
            if (i == null)
            {
                i = new xcuda_item_external_freight() { Valuation_item_Id = vi.Item_Id, TrackingState = TrackingState.Added };
                vi.xcuda_item_external_freight = i;
            }
            if (ai.Valuation_item.item_external_freight.Amount_foreign_currency != "")
                i.Amount_foreign_currency = Convert.ToSingle(ai.Valuation_item.item_external_freight.Amount_foreign_currency);
            if (ai.Valuation_item.item_external_freight.Amount_national_currency != "")
                i.Amount_national_currency = Convert.ToSingle(ai.Valuation_item.item_external_freight.Amount_national_currency);

            i.Currency_code = ai.Valuation_item.item_external_freight.Currency_code.Text.FirstOrDefault();

            if (ai.Valuation_item.item_external_freight.Currency_rate != "")
                i.Currency_rate = Convert.ToSingle(ai.Valuation_item.item_external_freight.Currency_rate);

        }

        private void Save_Item_Invoice(xcuda_Valuation_item vi, ASYCUDAItem ai)
        {
            var i = vi.xcuda_Item_Invoice;
            if (i == null)
            {
                i = new xcuda_Item_Invoice(){Valuation_item_Id = vi.Item_Id, TrackingState = TrackingState.Added};
                vi.xcuda_Item_Invoice = i;
            }
            if (ai.Valuation_item.Item_Invoice.Amount_foreign_currency != "")
                i.Amount_foreign_currency = Convert.ToSingle(ai.Valuation_item.Item_Invoice.Amount_foreign_currency);
            if (ai.Valuation_item.Item_Invoice.Amount_national_currency != "")
                i.Amount_national_currency = Convert.ToSingle(ai.Valuation_item.Item_Invoice.Amount_national_currency);
            if (ai.Valuation_item.Item_Invoice.Currency_code != "")
                i.Currency_code = ai.Valuation_item.Item_Invoice.Currency_code;
            if (ai.Valuation_item.Item_Invoice.Currency_rate != "")
                i.Currency_rate = Convert.ToSingle(ai.Valuation_item.Item_Invoice.Currency_rate);

        }

        private async Task Save_Item_Taxation(xcuda_Item di, ASYCUDAItem ai)
        {
            var t = di.xcuda_Taxation.FirstOrDefault();
            if (t == null)
            {

                t = new xcuda_Taxation(){Item_Id = di.Item_Id, TrackingState = TrackingState.Added};
                di.xcuda_Taxation.Add(t);
               
            }

            //t.Counter_of_normal_mode_of_payment = ai.Taxation.Counter_of_normal_mode_of_payment
            //t.Displayed_item_taxes_amount = ai.Taxation.Displayed_item_taxes_amount;
            if (ai.Taxation.Item_taxes_amount != "")
                t.Item_taxes_amount = Convert.ToSingle(ai.Taxation.Item_taxes_amount);
            //t.Item_taxes_guaranted_amount = ai.Taxation.Item_taxes_guaranted_amount;
            if (ai.Taxation.Item_taxes_mode_of_payment.Text.Count > 0)
                t.Item_taxes_mode_of_payment = ai.Taxation.Item_taxes_mode_of_payment.Text[0];


            Save_Taxation_line(t, ai);

            //await DIBaseDataModel.Instance.Savexcuda_Taxation(t).ConfigureAwait(false);
        }

        private void Save_Taxation_line(xcuda_Taxation t, ASYCUDAItem ai)
        {
            for (var i = 0; i < ai.Taxation.Taxation_line.Count; i++)
            {
                var au = ai.Taxation.Taxation_line.ElementAt(i);

                if (au.Duty_tax_code.Text.Count == 0) break;

                var tl = t.xcuda_Taxation_line.ElementAtOrDefault(i);
                if (tl == null)
                {
                    tl = new xcuda_Taxation_line(){TrackingState = TrackingState.Added};
                    t.xcuda_Taxation_line.Add(tl);
                    
                }

                tl.Duty_tax_amount = Convert.ToDouble(au.Duty_tax_amount);
                tl.Duty_tax_Base = au.Duty_tax_Base;
                tl.Duty_tax_code = au.Duty_tax_code.Text[0];

                if (au.Duty_tax_MP.Text.Count > 0)
                    tl.Duty_tax_MP = au.Duty_tax_MP.Text[0];

                tl.Duty_tax_rate = Convert.ToDouble(au.Duty_tax_rate);

            }
        }

        private async Task Save_Item_Previous_doc(xcuda_Item di, ASYCUDAItem ai)
        {
            var pd = di.xcuda_Previous_doc;
            if (pd == null)
            {
                pd = new xcuda_Previous_doc(){Item_Id = di.Item_Id,TrackingState = TrackingState.Added};
                // di.xcuda_Previous_doc.Add(pd);
                di.xcuda_Previous_doc = pd;
            }
            pd.Summary_declaration = ai.Previous_doc.Summary_declaration.Text.FirstOrDefault();
            if (da.Document.xcuda_ASYCUDA_ExtendedProperties.BLNumber == null && ai.Previous_doc.Summary_declaration != null)
                da.Document.xcuda_ASYCUDA_ExtendedProperties.BLNumber = ai.Previous_doc.Summary_declaration.Text.FirstOrDefault();

            //await DIBaseDataModel.Instance.Savexcuda_Previous_doc(pd).ConfigureAwait(false);
        }

        private async Task Save_Item_Goods_description(xcuda_Item di, ASYCUDAItem ai)
        {
            var g = di.xcuda_Goods_description;//.FirstOrDefault();
            if (g == null)
            {
                g = new xcuda_Goods_description(){Item_Id = di.Item_Id, TrackingState = TrackingState.Added};
                di.xcuda_Goods_description = g;
            }
            g.Commercial_Description = ai.Goods_description.Commercial_Description;
            g.Country_of_origin_code = ai.Goods_description.Country_of_origin_code;
            g.Description_of_goods = ai.Goods_description.Description_of_goods.Text.FirstOrDefault();

            //await DIBaseDataModel.Instance.Savexcuda_Goods_description(g).ConfigureAwait(false);
        }

        private async Task Save_Item_Tarification(xcuda_Item di, ASYCUDAItem ai)
        {
            var t = di.xcuda_Tarification;//.FirstOrDefault();
            if (t == null)
            {
                t = new xcuda_Tarification(){Item_Id = di.Item_Id, TrackingState = TrackingState.Added};
                di.xcuda_Tarification = t;

            }

            t.Extended_customs_procedure = ai.Tarification.Extended_customs_procedure;
            t.National_customs_procedure = ai.Tarification.National_customs_procedure;
            if (ai.Tarification.Item_price != "")
                t.Item_price = Convert.ToSingle(ai.Tarification.Item_price);
            if (ai.Tarification.Value_item.Text.Count > 0)
                t.Value_item = ai.Tarification.Value_item.Text[0];

            Save_Supplementary_unit(t, ai);
            
            if (ai.Tarification.Attached_doc_item.Text.Count > 0)
                t.Attached_doc_item = ai.Tarification.Attached_doc_item.Text[0];
            
            await SaveCustomsProcedure(t).ConfigureAwait(false);

            await Save_HScode(t, di,ai).ConfigureAwait(false);

            //await DIBaseDataModel.Instance.Savexcuda_Tarification(t).ConfigureAwait(false);
        }

        private async Task<Customs_Procedure> SaveCustomsProcedure(xcuda_Tarification t)
        {
            var cp = BaseDataModel.Instance.Customs_ProcedureCache.GetSingle(x => x.Extended_customs_procedure == t.Extended_customs_procedure
                                                            &&
                                                            x.National_customs_procedure == t.National_customs_procedure);
            //    (await DBaseDataModel.Instance.SearchCustoms_Procedure(new List<string>()
            //{
            //    string.Format("Extended_customs_procedure == \"{0}\"", t.Extended_customs_procedure),
            //    string.Format("National_customs_procedure == \"{0}\"", t.National_customs_procedure)
            //}).ConfigureAwait(false)).FirstOrDefault();
            if (cp == null)
            {
                cp = new Customs_Procedure()
                {
                    Extended_customs_procedure = t.Extended_customs_procedure,
                    National_customs_procedure = t.National_customs_procedure,
                    TrackingState = TrackingState.Added
                };
                
                    if (da.Document.xcuda_ASYCUDA_ExtendedProperties.Document_Type != null)
                    {
                        //if (da.Document.xcuda_ASYCUDA_ExtendedProperties.Document_Type
                        //    .DefaultCustoms_Procedure == null)
                        //    da.Document.xcuda_ASYCUDA_ExtendedProperties.Document_Type
                        //        .DefaultCustoms_Procedure = cp;

                        cp.Document_TypeId = da.Document.xcuda_ASYCUDA_ExtendedProperties.Document_Type.Document_TypeId;
                    }

                   // DBaseDataModel.Instance.SaveCustoms_Procedure(cp).Wait();
                    //await DBaseDataModel.Instance.SaveCustoms_Procedure(cp).ConfigureAwait(false);
                    BaseDataModel.Instance.Customs_ProcedureCache.AddItem(cp);
                
            }
            da.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId = cp.Customs_ProcedureId;
            da.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure = cp;
            await
                DBaseDataModel.Instance.Savexcuda_ASYCUDA_ExtendedProperties(
                    da.Document.xcuda_ASYCUDA_ExtendedProperties).ConfigureAwait(false);
            return cp;
        }

        private void Save_Supplementary_unit(xcuda_Tarification t, ASYCUDAItem ai)
        {
            for (var i = 0; i < ai.Tarification.Supplementary_unit.Count; i++)
            {
                var au = ai.Tarification.Supplementary_unit.ElementAt(i);

                if (au.Suppplementary_unit_code.Text.Count == 0) break;

                var su = t.xcuda_Supplementary_unit.ElementAtOrDefault(i);
                if (su == null)
                {
                    su = new xcuda_Supplementary_unit(){Tarification_Id = t.Item_Id, TrackingState = TrackingState.Added};
                    t.Unordered_xcuda_Supplementary_unit.Add(su);
                }

                su.Suppplementary_unit_quantity = Convert.ToDouble(string.IsNullOrEmpty(au.Suppplementary_unit_quantity) ? "0" : au.Suppplementary_unit_quantity);

                if (au.Suppplementary_unit_code.Text.Count > 0)
                    su.Suppplementary_unit_code = au.Suppplementary_unit_code.Text[0];

                if (au.Suppplementary_unit_name.Text.Count > 0)
                    su.Suppplementary_unit_name = au.Suppplementary_unit_name.Text[0];

            }
        }

        private async Task Save_HScode(xcuda_Tarification t,xcuda_Item di, ASYCUDAItem ai)
        {
            var h = t.xcuda_HScode;//.FirstOrDefault();
            if (h == null)
            {
                h = new xcuda_HScode(){Item_Id = t.Item_Id, TrackingState = TrackingState.Added};
                t.xcuda_HScode = h;
            }

            h.Commodity_code = ai.Tarification.HScode.Commodity_code;
            h.Precision_1 = ai.Tarification.HScode.Precision_1;
            if (ai.Tarification.HScode.Precision_4.Text.FirstOrDefault() != null)
            {
                h.Precision_4 = ai.Tarification.HScode.Precision_4.Text.FirstOrDefault();
            }
            else
            {
                if (!NoMessages)
                    throw new ApplicationException(string.Format("Null Product Code on Line{0}", di.LineNumber));
            }


            //InventoryItems inv = db.InventoryItems.Where(x => x.ItemNumber == h.Precision_4).FirstOrDefault();
            //if(inv == null)
            //{
            //    inv = new InventoryItems(){ ItemNumber = h.Precision_4, Description = ai.Goods_description.Commercial_Description};
            //    db.InventoryItems.AddObject(inv);
            //}

           

            var i = await SaveInventoryItem(ai).ConfigureAwait(false);
            if (i != null)
            {
                h.InventoryItems = i as IInventoryItem;

                if (h.InventoryItems.TariffCode == null || (updateItemsTariffCode == true && h.InventoryItems.TariffCode != h.Commodity_code))
                {
                    var tc =
                        (await
                            DataSpace.InventoryDS.DataModels.BaseDataModel.Instance.SearchTariffCode(new List<string>()
                            {
                                string.Format("TariffCodeName == \"{0}\"", h.Commodity_code)
                            }).ConfigureAwait(false)).FirstOrDefault();
                    if(tc != null)
                        h.InventoryItems.TariffCode = h.Commodity_code;
                }
            }
        }

        private async Task Save_Item_Packages(xcuda_Item di, ASYCUDAItem ai)
        {
            var p = di.xcuda_Packages.FirstOrDefault();
            if (p == null)
            {
                p = new xcuda_Packages(){Item_Id = di.Item_Id,TrackingState = TrackingState.Added};
                di.xcuda_Packages.Add(p);
            }
            p.Kind_of_packages_code = ai.Packages.Kind_of_packages_code;
            p.Kind_of_packages_name = ai.Packages.Kind_of_packages_name;
            p.Number_of_packages = Convert.ToSingle(ai.Packages.Number_of_packages);

            if (ai.Packages.Marks1_of_packages.Text.Count > 0)
                p.Marks1_of_packages = ai.Packages.Marks1_of_packages.Text[0];

            if (ai.Packages.Marks2_of_packages.Text.Count > 0)
                p.Marks2_of_packages = ai.Packages.Marks2_of_packages.Text[0];

            //await DIBaseDataModel.Instance.Savexcuda_Packages(p).ConfigureAwait(false);
        }

        private async Task Save_Item_Suppliers_link(xcuda_Item di, ASYCUDAItem ai)
        {
            var sl = di.xcuda_Suppliers_link.FirstOrDefault();
            if (sl == null)
            {
                sl = new xcuda_Suppliers_link(){Item_Id = di.Item_Id, TrackingState = TrackingState.Added};
                di.xcuda_Suppliers_link.Add(sl);
            }

            sl.Suppliers_link_code = ai.Suppliers_link.Suppliers_link_code;
            //await DIBaseDataModel.Instance.Savexcuda_Suppliers_link(sl).ConfigureAwait(false);
        }

        private async Task Save_Item_Attached_documents(xcuda_Item di, ASYCUDAItem ai)
        {
            for (var i = 0; i < ai.Attached_documents.Count; i++)
            {
                if (ai.Attached_documents[i].Attached_document_code.Text.Count == 0) break;

                var ad = di.xcuda_Attached_documents.ElementAtOrDefault(i);
                if (ad == null)
                {
                    ad = new xcuda_Attached_documents(){Item_Id = di.Item_Id, TrackingState = TrackingState.Added};
                    di.xcuda_Attached_documents.Add(ad);
                }

                ad.Attached_document_date = ai.Attached_documents[i].Attached_document_date;

                if (ai.Attached_documents[i].Attached_document_code.Text.Count != 0)
                    ad.Attached_document_code = ai.Attached_documents[i].Attached_document_code.Text[0];

                if (ai.Attached_documents[i].Attached_document_from_rule.Text.Count != 0)
                    ad.Attached_document_from_rule = Convert.ToInt32(ai.Attached_documents[i].Attached_document_from_rule.Text[0]);

                if (ai.Attached_documents[i].Attached_document_name.Text.Count != 0)
                    ad.Attached_document_name = ai.Attached_documents[i].Attached_document_name.Text[0];

                if (ai.Attached_documents[i].Attached_document_reference.Text.Count != 0)
                    ad.Attached_document_reference = ai.Attached_documents[i].Attached_document_reference.Text[0];

                //await DIBaseDataModel.Instance.Savexcuda_Attached_documents(ad).ConfigureAwait(false);

            }
        }

        private async Task SaveContainer()
        {
            try
            {
                var c = da.Document.xcuda_Container.FirstOrDefault();
                if (c == null)
                {
                    c = new xcuda_Container(){ASYCUDA_Id = da.Document.ASYCUDA_Id, TrackingState = TrackingState.Added};
                    da.Document.xcuda_Container.Add(c);
                }
                if (a.Container != null)
                {
                    c.Container_identity = a.Container.Container_identity;
                    c.Container_type = a.Container.Container_type;
                    c.Goods_description = a.Container.Goods_description;
                    c.Gross_weight = Convert.ToSingle(a.Container.Gross_weight.Text.FirstOrDefault());
                    c.Item_Number = a.Container.Item_Number;
                    c.Packages_number = a.Container.Packages_number;
                    c.Packages_type = a.Container.Packages_type;
                    c.Packages_weight = Convert.ToSingle(a.Container.Packages_weight);
                }
                //await DBaseDataModel.Instance.Savexcuda_Container(c).ConfigureAwait(false);
            }
            catch (Exception Ex)
            {
                throw;
            }
        }

        private async Task Save_Valuation()
        {
            var v = da.Document.xcuda_Valuation;
            if (v == null)
            {
                v = new xcuda_Valuation(){ASYCUDA_Id = da.Document.ASYCUDA_Id, TrackingState = TrackingState.Added};
                da.Document.xcuda_Valuation = v;
            }
            v.Calculation_working_mode = a.Valuation.Calculation_working_mode;
            v.Total_CIF = Convert.ToSingle(a.Valuation.Total_CIF);
            v.Total_cost = Convert.ToSingle(a.Valuation.Total_cost);

            Save_Valuation_Weight(v);
            Save_Gs_Invoice(v);
            Save_Gs_External_freight(v);
            Save_Total(v);
            //await DBaseDataModel.Instance.Savexcuda_Valuation(v).ConfigureAwait(false);
        }

        private void Save_Total(xcuda_Valuation v)
        {
            var t = v.xcuda_Total;
            if (t == null)
            {
                t = new xcuda_Total() { Valuation_Id = v.ASYCUDA_Id, TrackingState = TrackingState.Added };
                v.xcuda_Total = t;
            }
            t.Total_invoice = Convert.ToSingle(a.Valuation.Total.Total_invoice);
            t.Total_weight = Convert.ToSingle(a.Valuation.Total.Total_weight);
        }

        private void Save_Gs_External_freight(xcuda_Valuation v)
        {
            var gf = v.xcuda_Gs_external_freight;
            if (gf == null)
            {
                gf = new xcuda_Gs_external_freight(){Valuation_Id = v.ASYCUDA_Id, TrackingState = TrackingState.Added};
                v.xcuda_Gs_external_freight = gf;
            }

            gf.Amount_foreign_currency = Convert.ToSingle(a.Valuation.Gs_external_freight.Amount_foreign_currency);
            gf.Amount_national_currency = Convert.ToSingle(a.Valuation.Gs_external_freight.Amount_national_currency);
            gf.Currency_code = a.Valuation.Gs_external_freight.Currency_code.Text.FirstOrDefault();
            gf.Currency_name = a.Valuation.Gs_external_freight.Currency_name;
            gf.Currency_rate = Convert.ToSingle(a.Valuation.Gs_external_freight.Currency_rate);


        }

        private void Save_Gs_Invoice(xcuda_Valuation v)
        {
            var gi = v.xcuda_Gs_Invoice;
            if (gi == null)
            {
                gi = new xcuda_Gs_Invoice() { Valuation_Id = v.ASYCUDA_Id, TrackingState = TrackingState.Added };
                v.xcuda_Gs_Invoice = gi;
            }

            gi.Amount_foreign_currency = Convert.ToSingle(a.Valuation.Gs_Invoice.Amount_foreign_currency);
            gi.Amount_national_currency = Convert.ToSingle(a.Valuation.Gs_Invoice.Amount_national_currency);
            gi.Currency_code = a.Valuation.Gs_Invoice.Currency_code.Text.FirstOrDefault();
            gi.Currency_rate = Convert.ToSingle(a.Valuation.Gs_Invoice.Currency_rate);
            if (a.Valuation.Gs_Invoice.Currency_name.Text.Count != 0)
                gi.Currency_name = a.Valuation.Gs_Invoice.Currency_name.Text[0];
        }

        private void Save_Valuation_Weight(xcuda_Valuation v)
        {
            var w = v.xcuda_Weight;
            if (w == null)
            {
                w = new xcuda_Weight() { Valuation_Id = v.ASYCUDA_Id, TrackingState = TrackingState.Added };
                v.xcuda_Weight = w;
            }
            // w.Gross_weight = a.Valuation.Weight.Gross_weight
        }

        private async Task Save_Warehouse()
        {
            var w = da.Document.xcuda_Warehouse.FirstOrDefault();
            if (w == null)
            {
                w = new xcuda_Warehouse(){ASYCUDA_Id =da.Document.ASYCUDA_Id, TrackingState = TrackingState.Added};
                da.Document.xcuda_Warehouse.Add(w);
            }
            w.Identification = a.Warehouse.Identification.Text.FirstOrDefault();
            w.Delay = a.Warehouse.Delay;
            //await DBaseDataModel.Instance.Savexcuda_Warehouse(w).ConfigureAwait(false);
        }

        private async Task SaveFinancial()
        {
            var f = da.Document.xcuda_Financial.FirstOrDefault();
            if (f == null)
            {
                f = new xcuda_Financial(){ASYCUDA_Id = da.Document.ASYCUDA_Id, TrackingState = TrackingState.Added};
               // await DBaseDataModel.Instance.Savexcuda_Financial(f).ConfigureAwait(false); 
               // da.Document.xcuda_Financial.Add(f);
            }
            if (a.Financial.Deffered_payment_reference.Text.Count != 0)
                f.Deffered_payment_reference = a.Financial.Deffered_payment_reference.Text[0];

            f.Mode_of_payment = a.Financial.Mode_of_payment;

            Save_Amounts(f);
            Save_Guarantee(f);
                //await DBaseDataModel.Instance.Savexcuda_Financial(f).ConfigureAwait(false); 

        }

        public ASYCUDA A
        {
            get { return a; }
            set { a = value; }
        }

        private void Save_Guarantee(xcuda_Financial f)
        {
            var g = f.xcuda_Financial_Guarantee.FirstOrDefault();
            if (g == null)
            {
                g = new xcuda_Financial_Guarantee() { Financial_Id = f.Financial_Id, TrackingState = TrackingState.Added };
                f.xcuda_Financial_Guarantee.Add(g);
            }
            if (a.Financial.Guarantee.Amount != "")
                g.Amount = Convert.ToDecimal(a.Financial.Guarantee.Amount);
            //  g.Date = a.Financial.Guarantee.Date;
        }

        private void Save_Amounts(xcuda_Financial f)
        {
            var fa = f.xcuda_Financial_Amounts.FirstOrDefault();
            if (fa == null)
            {
                fa = new xcuda_Financial_Amounts() {Financial_Id = f.Financial_Id, TrackingState = TrackingState.Added };
                f.xcuda_Financial_Amounts.Add(fa);
            }
            if (a.Financial.Amounts.Global_taxes != "")
                fa.Global_taxes = Convert.ToDecimal(a.Financial.Amounts.Global_taxes);
            // fa.Total_manual_taxes = a.Financial.Amounts.Total_manual_taxes;
            if (a.Financial.Amounts.Totals_taxes != "")
                fa.Totals_taxes = Convert.ToDecimal(a.Financial.Amounts.Totals_taxes);
        }

        private async Task SaveTransport()
        {
            var t = da.Document.xcuda_Transport.FirstOrDefault();
            if (t == null)
            {
                t = new xcuda_Transport() {ASYCUDA_Id = da.Document.ASYCUDA_Id, TrackingState = TrackingState.Added };
                da.Document.xcuda_Transport.Add(t);
            }
            t.Container_flag = a.Transport.Container_flag;
            t.Single_waybill_flag = a.Transport.Single_waybill_flag;
            if (a.Transport.Location_of_goods.Text.Count != 0)
            {
                t.Location_of_goods = a.Transport.Location_of_goods.Text[0];
            }
            SaveMeansofTransport(t);
            Save_Delivery_terms(t);
            Save_Border_office(t);
            //await DBaseDataModel.Instance.Savexcuda_Transport(t).ConfigureAwait(false);
        }

        private void Save_Border_office(xcuda_Transport t)
        {
            var bo = t.xcuda_Border_office.FirstOrDefault();
            if (bo == null)
            {
                bo = new xcuda_Border_office() {Transport_Id = t.Transport_Id, TrackingState = TrackingState.Added };
                t.xcuda_Border_office.Add(bo);
            }
            if (a.Transport.Border_office.Code.Text.Count != 0)
                bo.Code = a.Transport.Border_office.Code.Text[0];

            if (a.Transport.Border_office.Name.Text.Count != 0)
                bo.Name = a.Transport.Border_office.Name.Text[0];

        }

        private void Save_Delivery_terms(xcuda_Transport t)
        {
            var d = t.xcuda_Delivery_terms.FirstOrDefault();
            if (d == null)
            {
                d = new xcuda_Delivery_terms() {Transport_Id = t.Transport_Id, TrackingState = TrackingState.Added };
                t.xcuda_Delivery_terms.Add(d);
            }
            if (a.Transport.Delivery_terms.Code.Text.Count != 0)
                d.Code = a.Transport.Delivery_terms.Code.Text[0];
            //d.Place = a.Transport.Delivery_terms.Place
        }

        private void SaveMeansofTransport(xcuda_Transport t)
        {
            var m = t.xcuda_Means_of_transport.FirstOrDefault();
            if (m == null)
            {
                m = new xcuda_Means_of_transport() { Transport_Id = t.Transport_Id, TrackingState = TrackingState.Added };
                t.xcuda_Means_of_transport.Add(m);

            }

            SaveDepartureArrivalInformation(m);
            SaveBorderInformation(m);
            //m.Inland_mode_of_transport = a.Transport.Means_of_transport.Inland_mode_of_transport.

        }



        private void SaveBorderInformation(xcuda_Means_of_transport m)
        {
            var d = m.xcuda_Border_information.FirstOrDefault();
            if (d == null)
            {
                d = new xcuda_Border_information() { Means_of_transport_Id = m.Means_of_transport_Id, TrackingState = TrackingState.Added };
                m.xcuda_Border_information.Add(d);
            }
            //if (a.Transport.Means_of_transport.Border_information.Nationality.ToString() != null)
            //    d.Nationality = a.Transport.Means_of_transport.Departure_arrival_information.Nationality.Text[0];

            //if (a.Transport.Means_of_transport.Departure_arrival_information.Identity.Text.Count != 0)
            //    d.Identity = a.Transport.Means_of_transport.Departure_arrival_information.Identity.Text[0];
            if (a.Transport.Means_of_transport.Border_information.Mode.Text.Count != 0)
                d.Mode = Convert.ToInt32(a.Transport.Means_of_transport.Border_information.Mode.Text[0]);
        }

        private void SaveDepartureArrivalInformation(xcuda_Means_of_transport m)
        {
            var d = m.xcuda_Departure_arrival_information.FirstOrDefault();
            if (d == null)
            {
                d = new xcuda_Departure_arrival_information(){Means_of_transport_Id = m.Means_of_transport_Id, TrackingState = TrackingState.Added};
                m.xcuda_Departure_arrival_information.Add(d);
            }
            if (a.Transport.Means_of_transport.Departure_arrival_information.Nationality.Text.Count != 0)
                d.Nationality = a.Transport.Means_of_transport.Departure_arrival_information.Nationality.Text[0];

            if (a.Transport.Means_of_transport.Departure_arrival_information.Identity.Text.Count != 0)
                d.Identity = a.Transport.Means_of_transport.Departure_arrival_information.Identity.Text[0];
        }

        private async Task SaveGeneralInformation()
        {
            var gi = da.Document.xcuda_General_information;
            if (gi == null)
            {
                gi = new xcuda_General_information() {ASYCUDA_Id = da.Document.ASYCUDA_Id, TrackingState = TrackingState.Added };
                da.Document.xcuda_General_information = gi;
            }
            gi.Value_details = a.General_information.Value_details;

            SaveCountry(gi);
            //await DBaseDataModel.Instance.Savexcuda_General_information(gi).ConfigureAwait(false);
        }

        private void SaveCountry(xcuda_General_information gi)
        {
            var c = gi.xcuda_Country;
            if (c == null)
            {
                c = new xcuda_Country() {Country_Id = gi.ASYCUDA_Id, TrackingState = TrackingState.Added };
                gi.xcuda_Country = c;
            }
            c.Country_first_destination = a.General_information.Country.Country_first_destination.Text.FirstOrDefault();
            c.Country_of_origin_name = a.General_information.Country.Country_of_origin_name;
            c.Trading_country = a.General_information.Country.Trading_country.Text.FirstOrDefault();
            SaveExport(c);
            SaveDestination(c);
        }

        private void SaveDestination(xcuda_Country c)
        {
            var des = c.xcuda_Destination;
            if (des == null)
            {
                des = new xcuda_Destination() {Country_Id = c.Country_Id, TrackingState = TrackingState.Added };
                c.xcuda_Destination = des;
                des.xcuda_Country = c;
                //await BaseDataModel.Instance.SaveDocumentCT(da).ConfigureAwait(false);
            }
            des.Destination_country_code = a.General_information.Country.Destination.Destination_country_code;
            des.Destination_country_name = a.General_information.Country.Destination.Destination_country_name;
            //Exp.Export_country_region = a.General_information.Country.Export.Export_country_region.;
        }

        private void SaveExport(xcuda_Country c)
        {
            var Exp = c.xcuda_Export;
            if (Exp == null)
            {
                Exp = new xcuda_Export() {Country_Id = c.Country_Id, TrackingState = TrackingState.Added };
                c.xcuda_Export = Exp;
            }
            Exp.Export_country_code = a.General_information.Country.Export.Export_country_code;
            Exp.Export_country_name = a.General_information.Country.Export.Export_country_name;
            //Exp.Export_country_region = a.General_information.Country.Export.Export_country_region.;
        }

        private async Task SaveDeclarant()
        {
            try
            {
                var d = da.Document.xcuda_Declarant;//.FirstOrDefault();
                if (d == null)
                {
                    da.Document.xcuda_Declarant = new xcuda_Declarant() {ASYCUDA_Id = da.Document.ASYCUDA_Id, TrackingState = TrackingState.Added };
                    d = da.Document.xcuda_Declarant;
                    //da.xcuda_Declarant.Add(d);
                }

                d.Declarant_name = a.Declarant.Declarant_name;
                d.Declarant_representative = a.Declarant.Declarant_representative;
                d.Declarant_code = a.Declarant.Declarant_code;

                //if(a.Declarant.Reference.Number.Text.Count > 0)
                d.Number = a.Declarant.Reference.Number;
                //await DBaseDataModel.Instance.Savexcuda_Declarant(d).ConfigureAwait(false);
            }
            catch (Exception Ex)
            {
                throw new Exception("Declarant fail to import - " + a.Declarant.Reference.Number);
            }

        }

        private async Task SaveTraders()
        {
            var t = da.Document.xcuda_Traders;
            if (t == null)
            {
                t = new xcuda_Traders() {Traders_Id = da.Document.ASYCUDA_Id, TrackingState = TrackingState.Added };
                da.Document.xcuda_Traders = t;
            }
            SaveExporter(t);
            SaveConsignee(t);
            SaveTradersFinancial(t);
            //await DBaseDataModel.Instance.Savexcuda_Traders(t).ConfigureAwait(false);
        }

        private void SaveTradersFinancial(xcuda_Traders t)
        {
            if (a.Traders.Financial.Financial_code.Text.Count == 0) return;
            var f = t.xcuda_Traders_Financial;
            if (f == null)
            {
                f = new xcuda_Traders_Financial() {Traders_Id = t.Traders_Id, TrackingState = TrackingState.Added };
                t.xcuda_Traders_Financial = f;
            }
            if (a.Traders.Financial.Financial_code.Text.Count != 0)
            {
                f.Financial_code = a.Traders.Financial.Financial_code.Text[0];
            }
            if (a.Traders.Financial.Financial_name.Text.Count != 0)
            {
                f.Financial_name = a.Traders.Financial.Financial_name.Text[0];
            }
        }

        private void SaveConsignee(xcuda_Traders t)
        {
            var c = t.xcuda_Consignee;
            if (c == null)
            {
                c = new xcuda_Consignee() {Traders_Id = t.Traders_Id, TrackingState = TrackingState.Added };
                t.xcuda_Consignee = c;
            }
            if (a.Traders.Consignee.Consignee_code.Text.Count != 0)
            {
                c.Consignee_code = a.Traders.Consignee.Consignee_code.Text[0];
            }
            if (a.Traders.Consignee.Consignee_name.Text.Count != 0)
            {
                c.Consignee_name = a.Traders.Consignee.Consignee_name.Text[0];
            }
        }

        private void SaveExporter(xcuda_Traders t)
        {
            var e = t.xcuda_Exporter;
            if (e == null)
            {
                e = new xcuda_Exporter() {Traders_Id = t.Traders_Id, TrackingState = TrackingState.Added };
                t.xcuda_Exporter = e;
            }

            if (a.Traders.Exporter.Exporter_name.Text.Count != 0)
            {
                e.Exporter_code = a.Traders.Exporter.Exporter_name.Text[0];
            }

            if (a.Traders.Exporter.Exporter_code.Text.Count != 0)
            {
                e.Exporter_code = a.Traders.Exporter.Exporter_code.Text[0];
            }
        }

        private async Task SaveProperty()
        {
            var p = da.Document.xcuda_Property;//.FirstOrDefault();

            if (p == null)
            {
                p = new xcuda_Property() { TrackingState = TrackingState.Added };
                da.Document.xcuda_Property = p;
                // da.xcuda_Property.Add(p);
            }
            // p.Date_of_declaration = a.Property.Date_of_declaration.ToString();
            SaveNbers(p);
            //await DBaseDataModel.Instance.Savexcuda_Property(p).ConfigureAwait(false);
        }

        private void SaveNbers(xcuda_Property p)
        {

            var n = p.xcuda_Nbers;//.FirstOrDefault();
            if (n == null)
            {
                n = new xcuda_Nbers() {ASYCUDA_Id = p.ASYCUDA_Id, TrackingState = TrackingState.Added };
                p.xcuda_Nbers = n;
                //  p.xcuda_Nbers.Add(n);
            }
            n.Number_of_loading_lists = a.Property.Nbers.Number_of_loading_lists;
            n.Total_number_of_packages = Convert.ToSingle(a.Property.Nbers.Total_number_of_packages);
            n.Total_number_of_items = a.Property.Nbers.Total_number_of_items;

        }

        private async Task SaveIdentification()
        {
            var di = da.Document.xcuda_Identification;//.FirstOrDefault();
            if (di == null)
            {
                di = new xcuda_Identification() { TrackingState = TrackingState.Added };
                da.Document.xcuda_Identification = di;
                // da.xcuda_Identification.Add(di);
            }

            SaveManifestReferenceNumber(di);
            SaveOfficeSegment(di);
            SaveRegistration(di);
            await SaveType(di).ConfigureAwait(false);

            //await DBaseDataModel.Instance.Savexcuda_Identification(di).ConfigureAwait(false);

        }

        private async Task SaveType(xcuda_Identification di)
        {
            var t = di.xcuda_Type;
            if (t == null)
            {
                t = new xcuda_Type() { TrackingState = TrackingState.Added };
                di.xcuda_Type = t;
            }

            t.Declaration_gen_procedure_code = a.Identification.Type.Declaration_gen_procedure_code;
            t.Type_of_declaration = a.Identification.Type.Type_of_declaration;

            var dt = await GetDocumentType(t).ConfigureAwait(false);
            da.Document.xcuda_ASYCUDA_ExtendedProperties.Document_TypeId = dt.Document_TypeId;
            da.Document.xcuda_ASYCUDA_ExtendedProperties.Document_Type = dt;

            await
                DBaseDataModel.Instance.Savexcuda_ASYCUDA_ExtendedProperties(
                    da.Document.xcuda_ASYCUDA_ExtendedProperties).ConfigureAwait(false);
        }

        private async Task<Document_Type> GetDocumentType(xcuda_Type t)
        {
            var dt =
                 BaseDataModel.Instance.Document_TypeCache.GetSingle(x => x.Declaration_gen_procedure_code == t.Declaration_gen_procedure_code
                                                   && x.Type_of_declaration == t.Type_of_declaration);
            ////    (await DBaseDataModel.Instance.SearchDocument_Type(new List<string>()
            ////{
            ////    string.Format("Declaration_gen_procedure_code == \"{0}\"", t.Declaration_gen_procedure_code),
            ////    string.Format("Type_of_declaration == \"{0}\"", t.Type_of_declaration)
            ////}).ConfigureAwait(false)).FirstOrDefault();

            if (dt == null)
            {

                dt = new Document_Type()
                {
                    Type_of_declaration = t.Type_of_declaration,
                    Declaration_gen_procedure_code = t.Declaration_gen_procedure_code,
                    TrackingState = TrackingState.Added
                };
               
                   //await DBaseDataModel.Instance.SaveDocument_Type(dt).ConfigureAwait(false);
                BaseDataModel.Instance.Document_TypeCache.AddItem(dt);
               
            }
            return dt;
        }

        private void SaveManifestReferenceNumber(xcuda_Identification di)
        {
            //xcuda_Manifest_reference_number r = di.xcuda_Manifest_reference_number;//.FirstOrDefault();
            //if (r == null)
            //{
            //    r = new xcuda_Manifest_reference_number();
            //    di.xcuda_Manifest_reference_number = r;
            //    //di.xcuda_Manifest_reference_number.Add(r);
            //}
            if (a.Identification.Manifest_reference_number.Text.Count != 0)
                di.Manifest_reference_number = a.Identification.Manifest_reference_number.Text[0];

        }

        private void SaveOfficeSegment(xcuda_Identification di)
        {
            var o = di.xcuda_Office_segment;//.FirstOrDefault();
            if (o == null)
            {
                o = new xcuda_Office_segment(){ASYCUDA_Id = di.ASYCUDA_Id, TrackingState = TrackingState.Added};
                di.xcuda_Office_segment = o;
                // di.xcuda_Office_segment.Add(o);
            }
            o.Customs_clearance_office_code = a.Identification.Office_segment.Customs_clearance_office_code.Text.FirstOrDefault();
            o.Customs_Clearance_office_name = a.Identification.Office_segment.Customs_Clearance_office_name.Text.FirstOrDefault();

        }

        private void SaveRegistration(xcuda_Identification di)
        {
            var r = di.xcuda_Registration;
            if (r == null)
            {
                r = new xcuda_Registration(){ASYCUDA_Id = di.ASYCUDA_Id, TrackingState = TrackingState.Added};
                di.xcuda_Registration = r;
                // di.xcuda_Registration.Add(r);
            }
            if (a.Identification.Registration.Date != "1/1/0001")
                r.Date = a.Identification.Registration.Date;
            if (a.Identification.Registration.Number != "")
                r.Number = a.Identification.Registration.Number;

        }
    }
}
