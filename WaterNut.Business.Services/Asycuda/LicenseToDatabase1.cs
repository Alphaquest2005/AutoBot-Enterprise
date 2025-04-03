using Asycuda421;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using LicenseDS.Business.Entities;
//using WaterNut.DataLayer;
using TrackableEntities;
using TrackableEntities.EF6;
using ValuationDS.Business.Entities;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using Registered = LicenseDS.Business.Entities.Registered;


namespace WaterNut.DataSpace.Asycuda
{
    public partial class LicenseToDataBase
    {
        private static readonly LicenseToDataBase instance;
        static LicenseToDataBase()
        {
            instance = new LicenseToDataBase();
           
       
        }

         public static LicenseToDataBase Instance
        {
            get { return instance; }
        }



       
        
        public async Task SaveToDatabase(Licence adoc, AsycudaDocumentSet docSet, FileInfo file)
        {

            try
            {

                var mat = Regex.Match(file.FullName,
                    @"[A-Z\\ -.]*(?<RegNumber>[0-9]+)-LIC.*.xml",
                    RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (!mat.Success) return;
                var regNumber = mat.Groups[1].Captures[0].Value;

                var da =  CreateLicense(docSet, file,regNumber);
                
                SaveGeneralInfo(adoc.General_segment, da.xLIC_General_segment);

                SaveDocumentRef(da);
                SaveItems(adoc.Lic_item_segment, da);

                using (var ctx = new LicenseDSContext())
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
            var match = Regex.Match(da.xLIC_General_segment.Exporter_address,
                @"((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))", RegexOptions.IgnoreCase);
            if (match.Success)
                da.DocumentReference = match.Groups["Value"].Value.Trim().Replace(",","");
        }

        public static bool GetLicenceRegNumber(FileInfo file, out string regNumber)
        {
            using (var ctx = new LicenseDSContext())
            {
                var lic = ctx.xLIC_License.OfType<Registered>()
                    .Include(x => x.xLIC_General_segment)
                    .FirstOrDefault(x => x.SourceFile == file.FullName);
                if (lic != null)
                {
                    regNumber = DateTime.Parse(lic.xLIC_General_segment.Application_date).Year.ToString() + " " +
                       lic.RegistrationNumber;
                    return true;
                }
                else
                {
                    regNumber = null;
                    return false;
                }
                
            }
            
        }

        private void SaveItems(ObservableCollection<LicenceLic_item_segment> aItems, xLIC_License dl)
        {
            try
            {
                foreach (var aitem in aItems)
                {
                    var ditem = dl.xLIC_Lic_item_segment.FirstOrDefault(x =>
                        x.Commodity_code == aitem.Commodity_code && x.Description == aitem.Description);
                    if (ditem == null)
                    {
                        ditem = new xLIC_Lic_item_segment(true)
                        {
                            xLIC_License = dl,
                            TrackingState = TrackingState.Added
                        };
                        dl.xLIC_Lic_item_segment.Add(ditem);
                    }

                    ditem.Description = aitem.Description;
                    ditem.Commodity_code = aitem.Commodity_code;
                    ditem.Quantity_requested = Convert.ToInt32(aitem.Quantity_requested);
                    ditem.Origin = aitem.Origin;
                    ditem.Unit_of_measurement = aitem.Unit_of_measurement;
                    ditem.Quantity_to_approve = Convert.ToInt32(aitem.Quantity_to_approve);



                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        private void SaveGeneralInfo(LicenceGeneral_segment agen, xLIC_General_segment dgen)
        {
            try
            {
                dgen.Arrival_date = agen.Arrival_date;
                dgen.Application_date = agen.Application_date;
                dgen.Expiry_date = agen.Expiry_date.Text.FirstOrDefault();
                dgen.Importation_date = agen.Importation_date;
                dgen.Importer_cellphone = agen.Importer_cellphone.Text.FirstOrDefault();
                dgen.Exporter_address = agen.Exporter_address.Text.FirstOrDefault();
                dgen.Exporter_country_code = agen.Exporter_country_code.Text.FirstOrDefault();
                dgen.Importer_code = agen.Importer_code.Text.FirstOrDefault();
                dgen.Owner_code = agen.Owner_code.Text.FirstOrDefault();
                dgen.Exporter_email = agen.Exporter_email.Text.FirstOrDefault();
                dgen.Importer_email = agen.Importer_email.Text.FirstOrDefault();
                dgen.Importer_name = agen.Importer_name.Text.FirstOrDefault();
                dgen.Importer_contact = agen.Importer_contact.Text.FirstOrDefault();
                dgen.Exporter_name = agen.Exporter_name.Text.FirstOrDefault();
                dgen.Exporter_telephone = agen.Exporter_telephone.Text.FirstOrDefault();
                dgen.Importer_telephone = agen.Importer_telephone.Text.FirstOrDefault();
                dgen.Exporter_country_name = agen.Exporter_country_name.Text.FirstOrDefault();
                dgen.Exporter_cellphone = agen.Exporter_cellphone.Text.FirstOrDefault();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private LicenseDS.Business.Entities.Registered CreateLicense(AsycudaDocumentSet docSet, FileInfo file, string regNumber)
        {
            LicenseDS.Business.Entities.Registered ndoc;
            using (var ctx = new LicenseDSContext())
            {
                ndoc = ctx.xLIC_License.OfType<LicenseDS.Business.Entities.Registered>()
                    .Include(x => x.xLIC_General_segment)
                    .Include(x => x.xLIC_Lic_item_segment)
                    .FirstOrDefault(x => x.RegistrationNumber == regNumber);
                if (ndoc == null)
                {
                    ndoc = CreateRegisteredLicense(regNumber,file.FullName);
                    
                    AttachLicenseToDocSet(docSet, file);
                    
                }
            }

            //ndoc.SetupProperties();


            return ndoc;
        }

        public xLIC_License CreateLicense(List<TODO_LicenseToXML> lst, Contacts contact, Suppliers supplier,
            string docRef)
        {
            var lic = CreateLicense();
            lic.xLIC_General_segment.Application_date = DateTime.Now.Date.ToShortDateString();
            lic.xLIC_General_segment.Importation_date = DateTime.Now.Date.AddMonths(3).ToShortDateString();
            lic.xLIC_General_segment.Arrival_date = DateTime.Now.Date.ToShortDateString();
            lic.xLIC_General_segment.Importer_contact = contact.Name;
            lic.xLIC_General_segment.Importer_cellphone = contact.CellPhone;
            lic.xLIC_General_segment.Importer_email = contact.EmailAddress;
            lic.xLIC_General_segment.Exporter_address =
                $"Ref:{docRef}\r\n{supplier?.Street}\r\n{supplier?.City},{supplier?.CountryCode}\r\n{supplier?.Country}";
            lic.xLIC_General_segment.Exporter_name = supplier?.SupplierName?? supplier?.SupplierCode;
            lic.xLIC_General_segment.Exporter_country_code = supplier?.CountryCode;

            lic.xLIC_General_segment.Importer_code = BaseDataModel.Instance.CurrentApplicationSettings.DeclarantCode;

            foreach (var item in lst.GroupBy(x => new{ x.TariffCode, x.LicenseDescription}))
            {
                lic.xLIC_Lic_item_segment.Add(new xLIC_Lic_item_segment(true)
                {
                    Description = item.Key.LicenseDescription,
                    Commodity_code = item.Key.TariffCode,
                    Quantity_requested = Convert.ToInt32(item.Sum(x => x.Quantity)),
                    Quantity_to_approve = Convert.ToInt32(item.Sum(x => x.Quantity)),
                    Origin = item.First().Country_of_origin_code,
                    Unit_of_measurement = item.First().UOM,
                    TrackingState = TrackingState.Added
                });
            }

            return lic;
        }

        private static xLIC_License CreateLicense()
        {
            xLIC_License ndoc;
            ndoc = new xLIC_License(true)
            {
                xLIC_General_segment = new xLIC_General_segment(true) {TrackingState = TrackingState.Added},
                xLIC_Lic_item_segment = new List<xLIC_Lic_item_segment>(),
                TrackingState = TrackingState.Added
            };
            
            return ndoc;
        }

        private static Registered CreateRegisteredLicense(string regNumber, string file)
        {
            Registered ndoc;
            ndoc = new Registered(true)
            {
                RegistrationNumber = regNumber,
                SourceFile = file,
                xLIC_General_segment = new xLIC_General_segment(true) { TrackingState = TrackingState.Added },
                xLIC_Lic_item_segment = new List<xLIC_Lic_item_segment>(),
                TrackingState = TrackingState.Added
            };

            return ndoc;
        }

        private static void AttachLicenseToDocSet(AsycudaDocumentSet docSet, FileInfo file)
        {
            using (var cctx = new CoreEntitiesContext())
            {
                var fileType = cctx.FileTypes.First(x =>
                    x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                    x.Type == "LIC");
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
                        Reference = "LIC",
                        TrackingState = TrackingState.Added,
                    }
                };
                cctx.AsycudaDocumentSet_Attachments.Add(res);
                cctx.SaveChanges();
            }
        }

        public async Task<bool> ExportLicense(int docSetId, xLIC_License lic, string fileName, List<string> invoices)
        {
            try
            {
                var docSet = await BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).ConfigureAwait(false);
                var adoc = DatabaseToLicence(lic);
                adoc.SaveToFile(fileName);
                var fileInfo = new FileInfo(fileName);
                AttachLicenseToDocSet(docSet, fileInfo);
                //var emailres = new FileInfo(Path.Combine(fileInfo.DirectoryName, "LICResults.txt"));
                var results = new FileInfo(Path.Combine(fileInfo.DirectoryName, "LIC-Results.txt"));
                if(File.Exists(results.FullName)) File.Delete(results.FullName);
                var instructions = Path.Combine(fileInfo.DirectoryName, "LIC-Instructions.txt");
                File.AppendAllText(instructions, $"File\t{fileInfo.FullName}\r\n");
                foreach (var itm in invoices)
                {
                    File.AppendAllText(instructions, $"Attach\t{Path.Combine(fileInfo.DirectoryName, $"{itm}.pdf")}\t{itm}\t{"IV05"}\r\n");
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }

        private Licence DatabaseToLicence(xLIC_License lic)
        {
            var aLic = new Licence();
            ExportGeneralInfo(aLic.General_segment, lic.xLIC_General_segment);
            ExportItems(aLic.Lic_item_segment, lic);

            return aLic;
        }

        private void ExportItems(ObservableCollection<LicenceLic_item_segment> aLic, xLIC_License lic)
        {
            try
            {
                foreach (var ditem in lic.xLIC_Lic_item_segment)
                {

                   var aitem = new LicenceLic_item_segment();
                        aLic.Add(aitem);
                   

                    aitem.Description = ditem.Description;
                    aitem.Commodity_code = ditem.Commodity_code;
                    aitem.Quantity_requested = Convert.ToInt32(ditem.Quantity_requested).ToString();
                    aitem.Origin = ditem.Origin;
                    aitem.Unit_of_measurement = ditem.Unit_of_measurement;
                    aitem.Quantity_to_approve = Convert.ToInt32(ditem.Quantity_to_approve).ToString();



                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void ExportGeneralInfo(LicenceGeneral_segment aItem, xLIC_General_segment dItem)
        {
            try
            {
                aItem.Arrival_date = dItem.Arrival_date;
                aItem.Application_date = dItem.Application_date;
                
                //if (dItem.Expiry_date == null) aItem.Expiry_date = new LicenceGeneral_segmentExpiry_date() { @null = new object() }; else aItem.Expiry_date.Text.Add(dItem.Expiry_date);
                aItem.Importation_date = dItem.Importation_date;
                aItem.Importer_cellphone.Text.Add(dItem.Importer_cellphone);
                aItem.Exporter_address.Text.Add(dItem.Exporter_address);
                aItem.Exporter_country_code.Text.Add(dItem.Exporter_country_code);
                aItem.Importer_code.Text.Add(dItem.Importer_code);
                aItem.Owner_code.Text.Add(dItem.Owner_code);
                aItem.Exporter_email.Text.Add(dItem.Exporter_email);
                aItem.Importer_email.Text.Add(dItem.Importer_email);
                aItem.Importer_name.Text.Add(dItem.Importer_name);
                aItem.Importer_contact.Text.Add(dItem.Importer_contact);
                aItem.Exporter_name.Text.Add(dItem.Exporter_name);
                aItem.Exporter_telephone.Text.Add(dItem.Exporter_telephone);
                aItem.Importer_telephone.Text.Add(dItem.Importer_telephone);
                aItem.Exporter_country_name.Text.Add(dItem.Exporter_country_name);
                aItem.Exporter_cellphone.Text.Add(dItem.Exporter_cellphone);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
