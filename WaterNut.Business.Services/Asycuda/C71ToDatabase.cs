using Asycuda421;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
//using WaterNut.DataLayer;
using TrackableEntities;
using TrackableEntities.EF6;
using ValuationDS.Business.Entities;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;


namespace WaterNut.DataSpace.Asycuda
{
    public partial class C71ToDataBase
    {
        private static readonly C71ToDataBase instance;
        static C71ToDataBase()
        {
            instance = new C71ToDataBase();
           
       
        }

         public static C71ToDataBase Instance
        {
            get { return instance; }
        }

        private ValuationDS.Business.Entities.Registered da;
        private Value_declaration_form a;

       
        
        public void SaveToDatabase(Value_declaration_form adoc, AsycudaDocumentSet docSet, FileInfo file)
        {

            try
            {
                var mat = Regex.Match(file.FullName,
                    @"[A-Z\\ -.]*(?<RegNumber>[0-9]+)-C71.*.xml",
                    RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (!mat.Success) return;
                var regNumber = mat.Groups[1].Captures[0].Value;

                //}
                da =  CreateNewC71(docSet, file, regNumber, adoc.id);

                SaveValuationForm(adoc, da);
                SaveIdentification(adoc.Identification_segment, da.xC71_Identification_segment);
                SaveDocumentRef(da);
                SaveItems(adoc.Item, da);
                AttachC71ToDocset(docSet, file, da);
                using (var ctx = new ValuationDSContext())
                {
                    ctx.ApplyChanges(da);
                    ctx.SaveChanges();
                }

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
               
            }

        }

        private static void SaveDocumentRef(Registered da)
        {
            var match = Regex.Match(da.xC71_Identification_segment.xC71_Seller_segment.Address,
                @"((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))", RegexOptions.IgnoreCase);
            if (match.Success)
                da.DocumentReference = match.Groups["Value"].Value.Trim().Replace(",","");
        }

        public static bool GetRegNumber(FileInfo file, out string regNumber)
        {
            using (var ctx = new ValuationDSContext())
            {
                var c71 = ctx.xC71_Value_declaration_form.OfType<Registered>()
                    .Include(x => x.xC71_Identification_segment)
                    .FirstOrDefault(x => x.SourceFile == file.FullName);
                if (c71 != null)
                {
                    regNumber = c71.RegNumber;
                    return true;
                }
                else
                {
                    regNumber = null;
                    return false;
                }

            }

        }

        private void SaveItems(ObservableCollection<Value_declaration_formItem> aItems,
            xC71_Value_declaration_form xC71ValueDeclarationForm)
        {
            try
            {
                foreach (var aItem in aItems)
                {
                    var dItem = da.xC71_Item.FirstOrDefault(x => x.Invoice_Number == aItem.Invoice_Number.Text.FirstOrDefault());
                    if (dItem == null)
                    {
                        dItem = new xC71_Item(true)
                        {
                            TrackingState = TrackingState.Added
                        };
                        da.xC71_Item.Add(dItem);
                    }

                    dItem.Terms_of_Delivery_Code = aItem.Terms_of_Delivery_Code.Text.FirstOrDefault();
                    dItem.Terms_of_Delivery_Desc = aItem.Terms_of_Delivery_Desc.Text.FirstOrDefault();
                    dItem.Invoice_Number = aItem.Invoice_Number.Text.FirstOrDefault();
                    dItem.Invoice_Date = aItem.Invoice_Date;
                    dItem.Currency_Rate_ind = aItem.Currency_Rate_ind;
                    dItem.Net_Price = aItem.Net_Price;
                    dItem.Currency_code_net = aItem.Currency_code_net.Text.FirstOrDefault();
                    dItem.Currency_Name_net = aItem.Currency_Name_net.Text.FirstOrDefault();
                    dItem.Currency_Rate_net = aItem.Currency_Rate_net.Text.FirstOrDefault();
                    dItem.Indirect_Payments = aItem.Indirect_Payments;
                    dItem.Currency_Rate_com = aItem.Currency_Rate_com;
                    dItem.Commissions = aItem.Commissions;
                    dItem.Currency_Rate_brg = aItem.Currency_Rate_brg;
                    dItem.Brokerage = aItem.Brokerage;
                    dItem.Currency_Rate_cap = aItem.Currency_Rate_cap;
                    dItem.Containers_Packaging = aItem.Containers_Packaging;
                    dItem.Currency_Rate_mcp = aItem.Currency_Rate_mcp;
                    dItem.Material_Components = aItem.Material_Components;
                    dItem.Currency_Rate_tls = aItem.Currency_Rate_tls;
                    dItem.Tool_Dies = aItem.Tool_Dies;
                    dItem.Currency_Rate_mcg = aItem.Currency_Rate_mcg;
                    dItem.Materials_Consumed = aItem.Materials_Consumed;
                    dItem.Currency_Rate_eng = aItem.Currency_Rate_eng;
                    dItem.Engineering_Development = aItem.Engineering_Development;
                    dItem.Currency_Rate_roy = aItem.Currency_Rate_roy;
                    dItem.Royalties_licence_fees = aItem.Royalties_licence_fees;
                    dItem.Currency_Rate_pro = aItem.Currency_Rate_pro.Text.FirstOrDefault();
                    dItem.Proceeds = aItem.Proceeds.Text.FirstOrDefault();
                    dItem.Currency_code_tpt = aItem.Currency_code_tpt.Text.FirstOrDefault();
                    dItem.Currency_Name_tpt = aItem.Currency_Name_tpt.Text.FirstOrDefault();
                    dItem.Currency_Rate_tpt = aItem.Currency_Rate_tpt.Text.FirstOrDefault();
                    dItem.Transport = aItem.Transport.Text.FirstOrDefault();
                    dItem.Currency_Rate_lhc = aItem.Currency_Rate_lhc.Text.FirstOrDefault();
                    dItem.Loading_handling = aItem.Loading_handling.Text.FirstOrDefault();
                    dItem.Currency_Rate_ins = aItem.Currency_Rate_ins;
                    dItem.Insurance = aItem.Insurance;
                    dItem.Currency_Rate_aim = aItem.Currency_Rate_aim;
                    dItem.Transport_after_import = aItem.Transport_after_import;
                    dItem.Currency_Rate_cfc = aItem.Currency_Rate_cfc;
                    dItem.Construction = aItem.Construction;
                    dItem.Currency_Rate_oth = aItem.Currency_Rate_oth;
                    dItem.Other_charges = aItem.Other_charges;
                    dItem.Currency_Rate_txs = aItem.Currency_Rate_txs;
                    dItem.Customs_duties_taxes = aItem.Customs_duties_taxes;
                    dItem.Currency_Name_com = aItem.Currency_Name_com.Text.FirstOrDefault();
                    dItem.Currency_code_ind = aItem.Currency_code_ind.Text.FirstOrDefault();
                    dItem.Currency_code_mcp = aItem.Currency_code_mcp.Text.FirstOrDefault();
                    dItem.Currency_code_ins = aItem.Currency_code_ins.Text.FirstOrDefault();
                    dItem.Currency_Name_ind = aItem.Currency_Name_ind.Text.FirstOrDefault();
                    dItem.Currency_Name_mcg = aItem.Currency_Name_mcg.Text.FirstOrDefault();
                  //  dItem.Other_specify = aItem.Other_specify.Text.FirstOrDefault();
                    dItem.Currency_Name_mcp = aItem.Currency_Name_mcp.Text.FirstOrDefault();
                    dItem.Currency_Name_brg = aItem.Currency_Name_brg.Text.FirstOrDefault();
                    dItem.Currency_code_tls = aItem.Currency_code_tls.Text.FirstOrDefault();
                    dItem.Currency_code_txs = aItem.Currency_code_txs.Text.FirstOrDefault();
                    dItem.Currency_code_oth = aItem.Currency_code_oth.Text.FirstOrDefault();
                    dItem.Currency_Name_eng = aItem.Currency_Name_eng.Text.FirstOrDefault();
                    dItem.Currency_Name_cap = aItem.Currency_Name_cap.Text.FirstOrDefault();
                    dItem.Currency_Name_aim = aItem.Currency_Name_aim.Text.FirstOrDefault();
                    dItem.Currency_code_eng = aItem.Currency_code_eng.Text.FirstOrDefault();
                    dItem.Currency_code_com = aItem.Currency_code_com.Text.FirstOrDefault();
                    dItem.Currency_Name_lhc = aItem.Currency_Name_lhc.Text.FirstOrDefault();
                    dItem.Currency_code_roy = aItem.Currency_code_roy.Text.FirstOrDefault();
                    dItem.Currency_code_aim = aItem.Currency_code_aim.Text.FirstOrDefault();
                    dItem.Currency_Name_tls = aItem.Currency_Name_tls.Text.FirstOrDefault();
                    dItem.Currency_code_mcg = aItem.Currency_code_mcg.Text.FirstOrDefault();
                    dItem.Currency_code_pro = aItem.Currency_code_pro.Text.FirstOrDefault();
                    dItem.Currency_Name_cfc = aItem.Currency_Name_cfc.Text.FirstOrDefault();
                    dItem.Currency_Name_roy = aItem.Currency_Name_roy.Text.FirstOrDefault();
                    dItem.Currency_code_brg = aItem.Currency_code_brg.Text.FirstOrDefault();
                    dItem.Currency_code_cap = aItem.Currency_code_cap.Text.FirstOrDefault();
                    dItem.Currency_Name_ins = aItem.Currency_Name_ins.Text.FirstOrDefault();
                    dItem.Currency_Name_pro = aItem.Currency_Name_pro.Text.FirstOrDefault();
                    dItem.Currency_code_cfc = aItem.Currency_code_cfc.Text.FirstOrDefault();
                    dItem.Currency_Name_txs = aItem.Currency_Name_txs.Text.FirstOrDefault();
                    dItem.Currency_code_lhc = aItem.Currency_code_lhc.Text.FirstOrDefault();
                    dItem.Currency_Name_oth = aItem.Currency_Name_oth.Text.FirstOrDefault();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void SaveIdentification(Value_declaration_formIdentification_segment aId, xC71_Identification_segment dId)
        {
            try
            {
                
                dId.Customs_Decision_Date = aId.Customs_Decision_Date.Text.FirstOrDefault();
                dId.Contract_Date = aId.Contract_Date.Text.FirstOrDefault();
                dId.No_7A = ToBool(aId.No_7A);
                dId.No_7B = ToBool(aId.No_7B);
                dId.No_7C = ToBool(aId.No_7C);
                dId.No_8A = ToBool(aId.No_8A);
                dId.No_8B = ToBool(aId.No_8B);
                dId.No_9A = ToBool(aId.No_9A);
                dId.No_9B = ToBool(aId.No_9B);
                dId.Yes_7A = ToBool(aId.Yes_7A);
                dId.Yes_7B = ToBool(aId.Yes_7B);
                dId.Yes_7C = ToBool(aId.Yes_7C);
                dId.Yes_8A = ToBool(aId.Yes_8A);
                dId.Yes_8B = ToBool(aId.Yes_8B);
                dId.Yes_9A = ToBool(aId.Yes_9A);
                dId.Yes_9B = ToBool(aId.Yes_9B);

                SaveBuyer(aId.Buyer_segment, dId.xC71_Buyer_segment);
                SaveDeclarant(aId.Declarant_segment, dId.xC71_Declarant_segment);
                SaveSeller(aId.Seller_segment, dId.xC71_Seller_segment);


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static bool ToBool(string val)
        {
            return !string.IsNullOrEmpty(val) && Convert.ToBoolean(val);
        }

        private void SaveSeller(Value_declaration_formIdentification_segmentSeller_segment aseller, xC71_Seller_segment dseller)
        {
            try
            {
                dseller.Address = aseller.Address.Text.FirstOrDefault();
                dseller.Name = aseller.Name.Text.FirstOrDefault();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private void SaveBuyer(Value_declaration_formIdentification_segmentBuyer_segment abuyer, xC71_Buyer_segment dbuyer)
        {
            try
            {
                dbuyer.Address = abuyer.Address.Text.FirstOrDefault();
                dbuyer.Code = abuyer.Code.Text.FirstOrDefault();
                dbuyer.Name = abuyer.Name.Text.FirstOrDefault();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void SaveDeclarant(Value_declaration_formIdentification_segmentDeclarant_segment aDecl, xC71_Declarant_segment dDecl)
        {
            try
            {
                dDecl.Address = aDecl.Address.Text.FirstOrDefault();
                dDecl.Code = aDecl.Code.Text.FirstOrDefault();
                dDecl.Name = aDecl.Name.Text.FirstOrDefault();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void SaveValuationForm(Value_declaration_form adoc, ValuationDS.Business.Entities.Registered da)
        {
            try
            {
                da.id = adoc.id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private ValuationDS.Business.Entities.Registered CreateNewC71(AsycudaDocumentSet docSet, FileInfo file,string regNumber ,string id)
        {
            ValuationDS.Business.Entities.Registered  ndoc;// 
            using (var ctx = new ValuationDSContext())
            {
                ndoc = ctx.xC71_Value_declaration_form.OfType<ValuationDS.Business.Entities.Registered>()
                    .Include(x => x.xC71_Identification_segment)
                    .Include(x => x.xC71_Identification_segment.xC71_Buyer_segment)
                    .Include(x => x.xC71_Identification_segment.xC71_Declarant_segment)
                    .Include(x => x.xC71_Identification_segment.xC71_Seller_segment)
                    .Include(x => x.xC71_Item)
                    .FirstOrDefault(x => x.id == id);
                if (ndoc == null)
                {
                    ndoc = (ValuationDS.Business.Entities.Registered) CreateNewRegisteredC71();
                    ndoc.RegNumber = regNumber;
                    ndoc.SourceFile = file.FullName;
                    ndoc.ApplicationSettingsId =
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId;

                }
            }



            //ndoc.SetupProperties();
            

            return ndoc;
        }

        private Registered CreateNewRegisteredC71()
        {
            return new Registered(true)
            {

                xC71_Identification_segment = new xC71_Identification_segment(true)
                {
                    xC71_Buyer_segment = new xC71_Buyer_segment(true) { TrackingState = TrackingState.Added },
                    xC71_Seller_segment = new xC71_Seller_segment(true) { TrackingState = TrackingState.Added },
                    xC71_Declarant_segment = new xC71_Declarant_segment(true) { TrackingState = TrackingState.Added },
                    TrackingState = TrackingState.Added

                },
                xC71_Item = new List<xC71_Item>(),
                TrackingState = TrackingState.Added
            };
        }

        private static void AttachC71ToDocset(AsycudaDocumentSet docSet, FileInfo file, Registered ndoc = null)
        {
            using (var cctx = new CoreEntitiesContext())
            {
                try
                {
                    var fileType = cctx.FileTypes.First(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                        x.Type == "C71");

                    var elst = ndoc?.xC71_Item.Select(x => x.Invoice_Number).ToList();

                    if (ndoc != null && ndoc.xC71_Item.Any() && !cctx.AsycudaDocumentSetEntryDataEx.Any(z =>
                            z.AsycudaDocumentSetId == docSet.AsycudaDocumentSetId &&
                            elst.Any(x => x == z.EntryDataId))) return;

                    var attachments =
                        cctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachments).Where(x =>
                            x.Attachments.FilePath == file.FullName &&
                            x.AsycudaDocumentSetId == docSet.AsycudaDocumentSetId).ToList();
                    if (attachments.Any())
                    {
                        cctx.Attachments.RemoveRange(attachments.Select(x => x.Attachments));
                        cctx.AsycudaDocumentSet_Attachments.RemoveRange(attachments);
                        cctx.SaveChanges();
                    }



                    var c71s = cctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachments)
                        .Where(x => x.AsycudaDocumentSetId == docSet.AsycudaDocumentSetId &&
                                    x.FileTypeId == fileType.Id && x.Attachments.Reference != "C71")
                        .ToList(); //OrderByDescending(x => x.AttachmentId).Skip(1).ToList();

                    var rc71s = cctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachments)
                        .Where(x => x.AsycudaDocumentSetId == docSet.AsycudaDocumentSetId &&
                                    x.FileTypeId == fileType.Id && x.Attachments.Reference == "C71" &&
                                    !x.Attachments.FilePath.Contains("\\C71.xml")).ToList();

                    cctx.Attachments.RemoveRange(c71s.Select(x => x.Attachments));
                    cctx.AsycudaDocumentSet_Attachments.RemoveRange(c71s);
                    cctx.Attachments.RemoveRange(rc71s.Select(x => x.Attachments));
                    cctx.AsycudaDocumentSet_Attachments.RemoveRange(rc71s);
                    cctx.SaveChanges();




                    var res = new AsycudaDocumentSet_Attachments(true)
                    {
                        AsycudaDocumentSetId = docSet.AsycudaDocumentSetId,
                        DocumentSpecific = true,
                        FileDate = file.LastWriteTime,
                        EmailUniqueId = null,
                        FileTypeId = fileType.Id,
                        TrackingState = TrackingState.Added,
                        Attachments = new CoreEntities.Business.Entities.Attachments(true)
                        {
                            FilePath = file.FullName,
                            DocumentCode = fileType.DocumentCode,
                            Reference = ndoc?.RegNumber ?? "C71",
                            TrackingState = TrackingState.Added,
                        }
                    };
                    cctx.AsycudaDocumentSet_Attachments.Add(res);
                    cctx.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }
        }

        private static xC71_Value_declaration_form CreateNewC71()
        {
            return new xC71_Value_declaration_form(true)
            {
                
                xC71_Identification_segment = new xC71_Identification_segment(true)
                {
                    xC71_Buyer_segment = new xC71_Buyer_segment(true) {TrackingState = TrackingState.Added},
                    xC71_Seller_segment = new xC71_Seller_segment(true) {TrackingState = TrackingState.Added},
                    xC71_Declarant_segment = new xC71_Declarant_segment(true) {TrackingState = TrackingState.Added},
                    TrackingState = TrackingState.Added

                },
                xC71_Item = new List<xC71_Item>(),
                TrackingState = TrackingState.Added
            };
        }

        public xC71_Value_declaration_form CreateC71(Suppliers supplier, List<TODO_C71ToXML> lst,
            string docRef)
        {
            var c71 = CreateNewC71();
            c71.xC71_Identification_segment.xC71_Seller_segment.Name = supplier.SupplierName ?? supplier.SupplierCode;
            c71.xC71_Identification_segment.xC71_Seller_segment.Address = $"Ref:{docRef},\r\n{supplier.Street}" ;
            c71.xC71_Identification_segment.xC71_Seller_segment.CountryCode = supplier.CountryCode;
            

            c71.xC71_Identification_segment.xC71_Buyer_segment.Code = BaseDataModel.Instance.CurrentApplicationSettings.DeclarantCode;
            c71.xC71_Identification_segment.xC71_Declarant_segment.Code = BaseDataModel.Instance.CurrentApplicationSettings.DeclarantCode;
            c71.xC71_Identification_segment.No_7A = true;
            c71.xC71_Identification_segment.No_8A = true;
            c71.xC71_Identification_segment.No_9A = true;
            c71.xC71_Identification_segment.No_9B = true;

            foreach (var item in lst)
            {
                c71.xC71_Item.Add(new xC71_Item(true)
                {
                    Terms_of_Delivery_Code = item.Code ?? "FOB",
                    Invoice_Number = item.InvoiceNo,
                    Invoice_Date = item.InvoiceDate.ToShortDateString(),
                    Currency_code_net = item.Currency,
                    Net_Price = item.InvoiceTotal.ToString(CultureInfo.InvariantCulture),
                    
                    TrackingState = TrackingState.Added
                });
            }

            return c71;
        }

        public bool ExportC71(int docSetId,xC71_Value_declaration_form c71, string fileName)
        {
            try
            {
                var docSet = BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).Result;
                var adoc = DatabaseToC71(c71);
                adoc.SaveToFile(fileName);
                var fileInfo = new FileInfo(fileName);
                AttachC71ToDocset(docSet, fileInfo);
                var instructionsFile = Path.Combine(fileInfo.DirectoryName, "C71-Instructions.txt");
                var results = new FileInfo(Path.Combine(fileInfo.DirectoryName, "C71-Results.txt"));
                if (File.Exists(results.FullName)) File.Delete(results.FullName);
                if (File.Exists(instructionsFile)) File.Delete(instructionsFile);
                
                File.WriteAllText(instructionsFile,
                    $"File\t{fileInfo.FullName}\t{c71.xC71_Identification_segment.xC71_Seller_segment.Address.Replace("\r\n","|")}\t{c71.xC71_Identification_segment.xC71_Seller_segment.CountryCode}\r\n");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }

        private Value_declaration_form DatabaseToC71(xC71_Value_declaration_form c71)
        {
            Value_declaration_form adoc = new Value_declaration_form();
            ExportIdentification(adoc.Identification_segment, c71.xC71_Identification_segment);
            ExportItems(adoc.Item, c71);
            return adoc;
        }

        private void ExportItems(ObservableCollection<Value_declaration_formItem> adocItem, xC71_Value_declaration_form c71)
        {
            try
            {
                foreach (var dItem in c71.xC71_Item)
                {
                    //var aItem = adocItem.FirstOrDefault(x => x.Invoice_Number.Text.FirstOrDefault() == dItem.Invoice_Number);
                    //if (aItem == null)
                    //{
                        var aItem = new Value_declaration_formItem();
                        adocItem.Add(aItem);
                   // }

                    aItem.Terms_of_Delivery_Code.Text.Add(dItem.Terms_of_Delivery_Code);
                    if (dItem.Terms_of_Delivery_Desc == null) aItem.Terms_of_Delivery_Desc = new Value_declaration_formItemTerms_of_Delivery_Desc() { @null = new object() }; else aItem.Terms_of_Delivery_Desc.Text.Add(dItem.Terms_of_Delivery_Desc);
                    aItem.Invoice_Number.Text.Add(dItem.Invoice_Number);
                    aItem.Invoice_Date = dItem.Invoice_Date;
                    if (dItem.Currency_code_ind == null) aItem.Currency_code_ind = new Value_declaration_formItemCurrency_code_ind() { @null = new object() }; else aItem.Currency_code_ind.Text.Add(dItem.Currency_code_ind);
                    if (dItem.Currency_Name_ind == null) aItem.Currency_Name_ind = new Value_declaration_formItemCurrency_Name_ind() { @null = new object() }; else aItem.Currency_Name_ind.Text.Add(dItem.Currency_Name_ind);
                    aItem.Currency_Rate_ind = dItem.Currency_Rate_ind??"0";
                    aItem.Net_Price = dItem.Net_Price;
                    if (dItem.Currency_code_net == null) aItem.Currency_code_net = new Value_declaration_formItemCurrency_code_net() { @null = new object() }; else aItem.Currency_code_net.Text.Add(dItem.Currency_code_net);
                    if (dItem.Currency_Name_net == null) aItem.Currency_Name_net = new Value_declaration_formItemCurrency_Name_net() { @null = new object() }; else aItem.Currency_Name_net.Text.Add(dItem.Currency_Name_net);
                    if (dItem.Currency_Rate_net == null) aItem.Currency_Rate_net = new Value_declaration_formItemCurrency_Rate_net() { @null = new object() }; else aItem.Currency_Rate_net.Text.Add(dItem.Currency_Rate_net);
                    
                    aItem.Indirect_Payments = dItem.Indirect_Payments??"0";
                    aItem.Currency_Rate_com = dItem.Currency_Rate_com ?? "0";
                    aItem.Commissions = dItem.Commissions ?? "0";
                    aItem.Currency_Rate_brg = dItem.Currency_Rate_brg ?? "0";
                    aItem.Brokerage = dItem.Brokerage ?? "0";
                    aItem.Currency_Rate_cap = dItem.Currency_Rate_cap ?? "0";
                    aItem.Containers_Packaging = dItem.Containers_Packaging??"0";
                    aItem.Currency_Rate_mcp = dItem.Currency_Rate_mcp ?? "0";
                    aItem.Material_Components = dItem.Material_Components??"0";
                    aItem.Currency_Rate_tls = dItem.Currency_Rate_tls ?? "0";
                    aItem.Tool_Dies = dItem.Tool_Dies ?? "0";
                    aItem.Currency_Rate_mcg = dItem.Currency_Rate_mcg ?? "0";
                    aItem.Materials_Consumed = dItem.Materials_Consumed??"0";
                    aItem.Currency_Rate_eng = dItem.Currency_Rate_eng ?? "0";
                    aItem.Engineering_Development = dItem.Engineering_Development??"0";
                    aItem.Currency_Rate_roy = dItem.Currency_Rate_roy ?? "0";
                    aItem.Royalties_licence_fees = dItem.Royalties_licence_fees??"0";
                    aItem.Currency_Rate_ins = dItem.Currency_Rate_ins ?? "0";
                    aItem.Insurance = dItem.Insurance??"0";
                    aItem.Currency_Rate_aim = dItem.Currency_Rate_aim ?? "0";
                    aItem.Transport_after_import = dItem.Transport_after_import ?? "0";
                    aItem.Currency_Rate_cfc = dItem.Currency_Rate_cfc ?? "0";
                    aItem.Construction = dItem.Construction ?? "0";
                    aItem.Currency_Rate_oth = dItem.Currency_Rate_oth ?? "0";
                    aItem.Other_charges = dItem.Other_charges ?? "0";
                    aItem.Currency_Rate_txs = dItem.Currency_Rate_txs ?? "0";
                    aItem.Customs_duties_taxes = dItem.Customs_duties_taxes ?? "0";
                    if (dItem.Currency_Name_com == null) aItem.Currency_Name_com = new Value_declaration_formItemCurrency_Name_com() { @null = new object() }; else aItem.Currency_Name_com.Text.Add(dItem.Currency_Name_com);
                    if (dItem.Currency_Rate_pro == null) aItem.Currency_Rate_pro = new Value_declaration_formItemCurrency_Rate_pro() { @null = new object() }; else aItem.Currency_Rate_pro.Text.Add(dItem.Currency_Rate_pro);
                    if (dItem.Proceeds == null) aItem.Proceeds = new Value_declaration_formItemProceeds() { @null = new object() }; else aItem.Proceeds.Text.Add(dItem.Proceeds);
                    if (dItem.Currency_code_tpt == null) aItem.Currency_code_tpt = new Value_declaration_formItemCurrency_code_tpt() { @null = new object() }; else aItem.Currency_code_tpt.Text.Add(dItem.Currency_code_tpt);
                    if (dItem.Currency_Name_tpt == null) aItem.Currency_Name_tpt = new Value_declaration_formItemCurrency_Name_tpt() { @null = new object() }; else aItem.Currency_Name_tpt.Text.Add(dItem.Currency_Name_tpt);
                    if (dItem.Currency_Rate_tpt == null) aItem.Currency_Rate_tpt = new Value_declaration_formItemCurrency_Rate_tpt() { @null = new object() }; else aItem.Currency_Rate_tpt.Text.Add(dItem.Currency_Rate_tpt);
                    if (dItem.Transport == null) aItem.Transport = new Value_declaration_formItemTransport() { @null = new object() }; else aItem.Transport.Text.Add(dItem.Transport);
                    if (dItem.Currency_Rate_lhc == null) aItem.Currency_Rate_lhc = new Value_declaration_formItemCurrency_Rate_lhc() { @null = new object() }; else aItem.Currency_Rate_lhc.Text.Add(dItem.Currency_Rate_lhc);
                    if (dItem.Loading_handling == null) aItem.Loading_handling = new Value_declaration_formItemLoading_handling() { @null = new object() }; else aItem.Loading_handling.Text.Add(dItem.Loading_handling);

                    if (dItem.Currency_code_mcp == null) aItem.Currency_code_mcp = new Value_declaration_formItemCurrency_code_mcp() { @null = new object() }; else aItem.Currency_code_mcp.Text.Add(dItem.Currency_code_mcp);
                    if (dItem.Currency_code_ins == null) aItem.Currency_code_ins = new Value_declaration_formItemCurrency_code_ins() { @null = new object() }; else aItem.Currency_code_ins.Text.Add(dItem.Currency_code_ins);

                    if (dItem.Currency_Name_mcg == null) aItem.Currency_Name_mcg = new Value_declaration_formItemCurrency_Name_mcg() { @null = new object() }; else aItem.Currency_Name_mcg.Text.Add(dItem.Currency_Name_mcg);
                    //if (dItem.Other_specify == null) aItem.Other_specify = new Value_declaration_formItemOther_specify() { @null = new object() }; else aItem.Other_specify.Text.Add(dItem.Other_specify);


                    aItem.Other_specify = null;


                    if (dItem.Currency_Name_mcp == null) aItem.Currency_Name_mcp = new Value_declaration_formItemCurrency_Name_mcp() { @null = new object() }; else aItem.Currency_Name_mcp.Text.Add(dItem.Currency_Name_mcp);
                    if (dItem.Currency_Name_brg == null) aItem.Currency_Name_brg = new Value_declaration_formItemCurrency_Name_brg() { @null = new object() }; else aItem.Currency_Name_brg.Text.Add(dItem.Currency_Name_brg);
                    if (dItem.Currency_code_tls == null) aItem.Currency_code_tls = new Value_declaration_formItemCurrency_code_tls() { @null = new object() }; else aItem.Currency_code_tls.Text.Add(dItem.Currency_code_tls);
                    if (dItem.Currency_code_txs == null) aItem.Currency_code_txs = new Value_declaration_formItemCurrency_code_txs() { @null = new object() }; else aItem.Currency_code_txs.Text.Add(dItem.Currency_code_txs);
                    if (dItem.Currency_code_oth == null) aItem.Currency_code_oth = new Value_declaration_formItemCurrency_code_oth() { @null = new object() }; else aItem.Currency_code_oth.Text.Add(dItem.Currency_code_oth);
                    if (dItem.Currency_Name_eng == null) aItem.Currency_Name_eng = new Value_declaration_formItemCurrency_Name_eng() { @null = new object() }; else aItem.Currency_Name_eng.Text.Add(dItem.Currency_Name_eng);
                    if (dItem.Currency_Name_cap == null) aItem.Currency_Name_cap = new Value_declaration_formItemCurrency_Name_cap() { @null = new object() }; else aItem.Currency_Name_cap.Text.Add(dItem.Currency_Name_cap);
                    if (dItem.Currency_Name_aim == null) aItem.Currency_Name_aim = new Value_declaration_formItemCurrency_Name_aim() { @null = new object() }; else aItem.Currency_Name_aim.Text.Add(dItem.Currency_Name_aim);
                    if (dItem.Currency_code_eng == null) aItem.Currency_code_eng = new Value_declaration_formItemCurrency_code_eng() { @null = new object() }; else aItem.Currency_code_eng.Text.Add(dItem.Currency_code_eng);
                    if (dItem.Currency_code_com == null) aItem.Currency_code_com = new Value_declaration_formItemCurrency_code_com() { @null = new object() }; else aItem.Currency_code_com.Text.Add(dItem.Currency_code_com);
                    if (dItem.Currency_Name_lhc == null) aItem.Currency_Name_lhc = new Value_declaration_formItemCurrency_Name_lhc() { @null = new object() }; else aItem.Currency_Name_lhc.Text.Add(dItem.Currency_Name_lhc);
                    if (dItem.Currency_code_roy == null) aItem.Currency_code_roy = new Value_declaration_formItemCurrency_code_roy() { @null = new object() }; else aItem.Currency_code_roy.Text.Add(dItem.Currency_code_roy);
                    if (dItem.Currency_code_aim == null) aItem.Currency_code_aim = new Value_declaration_formItemCurrency_code_aim() { @null = new object() }; else aItem.Currency_code_aim.Text.Add(dItem.Currency_code_aim);
                    if (dItem.Currency_Name_tls == null) aItem.Currency_Name_tls = new Value_declaration_formItemCurrency_Name_tls() { @null = new object() }; else aItem.Currency_Name_tls.Text.Add(dItem.Currency_Name_tls);
                    if (dItem.Currency_code_mcg == null) aItem.Currency_code_mcg = new Value_declaration_formItemCurrency_code_mcg() { @null = new object() }; else aItem.Currency_code_mcg.Text.Add(dItem.Currency_code_mcg);
                    if (dItem.Currency_code_pro == null) aItem.Currency_code_pro = new Value_declaration_formItemCurrency_code_pro() { @null = new object() }; else aItem.Currency_code_pro.Text.Add(dItem.Currency_code_pro);
                    if (dItem.Currency_Name_cfc == null) aItem.Currency_Name_cfc = new Value_declaration_formItemCurrency_Name_cfc() { @null = new object() }; else aItem.Currency_Name_cfc.Text.Add(dItem.Currency_Name_cfc);
                    if (dItem.Currency_Name_roy == null) aItem.Currency_Name_roy = new Value_declaration_formItemCurrency_Name_roy() { @null = new object() }; else aItem.Currency_Name_roy.Text.Add(dItem.Currency_Name_roy);
                    if (dItem.Currency_code_brg == null) aItem.Currency_code_brg = new Value_declaration_formItemCurrency_code_brg() { @null = new object() }; else aItem.Currency_code_brg.Text.Add(dItem.Currency_code_brg);
                    if (dItem.Currency_code_cap == null) aItem.Currency_code_cap = new Value_declaration_formItemCurrency_code_cap() { @null = new object() }; else aItem.Currency_code_cap.Text.Add(dItem.Currency_code_cap);
                    if (dItem.Currency_Name_ins == null) aItem.Currency_Name_ins = new Value_declaration_formItemCurrency_Name_ins() { @null = new object() }; else aItem.Currency_Name_ins.Text.Add(dItem.Currency_Name_ins);
                    if (dItem.Currency_Name_pro == null) aItem.Currency_Name_pro = new Value_declaration_formItemCurrency_Name_pro() { @null = new object() }; else aItem.Currency_Name_pro.Text.Add(dItem.Currency_Name_pro);
                    if (dItem.Currency_code_cfc == null) aItem.Currency_code_cfc = new Value_declaration_formItemCurrency_code_cfc() { @null = new object() }; else aItem.Currency_code_cfc.Text.Add(dItem.Currency_code_cfc);
                    if (dItem.Currency_Name_txs == null) aItem.Currency_Name_txs = new Value_declaration_formItemCurrency_Name_txs() { @null = new object() }; else aItem.Currency_Name_txs.Text.Add(dItem.Currency_Name_txs);
                    if (dItem.Currency_code_lhc == null) aItem.Currency_code_lhc = new Value_declaration_formItemCurrency_code_lhc() { @null = new object() }; else aItem.Currency_code_lhc.Text.Add(dItem.Currency_code_lhc);
                    if (dItem.Currency_Name_oth == null) aItem.Currency_Name_oth = new Value_declaration_formItemCurrency_Name_oth() { @null = new object() }; else aItem.Currency_Name_oth.Text.Add(dItem.Currency_Name_oth);




                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void ExportIdentification(Value_declaration_formIdentification_segment aId, xC71_Identification_segment dId)
        {
            try
            {
                if(aId == null) aId = new Value_declaration_formIdentification_segment();
                //aId.Customs_Decision_Date = dId.Customs_Decision_Date;
                //aId.Contract_Date = dId.Contract_Date;
                aId.No_7A = dId.No_7A.ToString();
                aId.No_7B = dId.No_7B.ToString();
                aId.No_7C = dId.No_7C.ToString();
                aId.No_8A = dId.No_8A.ToString();
                aId.No_8B = dId.No_8B.ToString();
                aId.No_9A = dId.No_9A.ToString();
                aId.No_9B = dId.No_9B.ToString();
                aId.Yes_7A = dId.Yes_7A.ToString();
                aId.Yes_7B = dId.Yes_7B.ToString();
                aId.Yes_7C = dId.Yes_7C.ToString();
                aId.Yes_8A = dId.Yes_8A.ToString();
                aId.Yes_8B = dId.Yes_8B.ToString();
                aId.Yes_9A = dId.Yes_9A.ToString();
                aId.Yes_9B = dId.Yes_9B.ToString();

                ExportBuyer(aId.Buyer_segment, dId.xC71_Buyer_segment);
                ExportDeclarant(aId.Declarant_segment, dId.xC71_Declarant_segment);
                ExportSeller(aId.Seller_segment, dId.xC71_Seller_segment);


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void ExportSeller(Value_declaration_formIdentification_segmentSeller_segment aseller, xC71_Seller_segment dseller)
        {
            try
            {
                aseller.Address.Text.Add(dseller.Address);
                aseller.Name.Text.Add(dseller.Name);
                

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void ExportDeclarant(Value_declaration_formIdentification_segmentDeclarant_segment aDecl, xC71_Declarant_segment dDecl)
        {
            try
            {
                aDecl.Address.Text.Add(dDecl.Address);
                aDecl.Code.Text.Add(dDecl.Code);
                aDecl.Name.Text.Add(dDecl.Name);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void ExportBuyer(Value_declaration_formIdentification_segmentBuyer_segment abuyer, xC71_Buyer_segment dbuyer)
        {
            try
            {
                abuyer.Address.Text.Add(dbuyer.Address);
                abuyer.Code.Text.Add(dbuyer.Code);
                abuyer.Name.Text.Add(dbuyer.Name);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

    }
}
