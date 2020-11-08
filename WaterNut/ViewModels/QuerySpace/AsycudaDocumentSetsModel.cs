using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Core.Common.UI;
using CoreEntities.Client.Repositories;
using Microsoft.Win32;
using SimpleMvvmToolkit;
using CoreEntities.Client.Entities;
using WaterNut.QuerySpace.AllocationQS.ViewModels;
using System.ServiceModel.ComIntegration;

namespace WaterNut.QuerySpace.CoreEntities.ViewModels
{
    public class AsycudaDocumentSetsModel : AsycudaDocumentSetExViewModel_AutoGen 
    {
       private static readonly AsycudaDocumentSetsModel instance;
       static AsycudaDocumentSetsModel()
        {
            instance = new AsycudaDocumentSetsModel()
            {
                ImportOnlyRegisteredDocuments = true,ImportTariffCodes = true,NoMessages = false, OverwriteExisting = true, LinkPi = true,
                ViewCurrentApplicationSettings = true
            };
        }

       public static AsycudaDocumentSetsModel Instance
        {
            get { return instance; }
        }

        private AsycudaDocumentSetsModel()
        {
            RegisterToReceiveMessages<AsycudaDocumentSetEx>(MessageToken.CurrentAsycudaDocumentSetExChanged, OnCurrentAsycudaDocumentSetExChanged2);
            RegisterToReceiveMessages(MessageToken.AsycudaDocumentSetExsChanged, OnAsycudaDocumentSetExsChanged3);
            RegisterToReceiveMessages<AsycudaDocumentSetEx>(MessageToken.AsycudaDocumentSetExsChanged, OnAsycudaDocumentSetExsChanged2);

            
            
        }


        //res.Append(" && " + string.Format("(ApplicationSettingsId == \"{0}\")", CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId));
        private async void OnAsycudaDocumentSetExsChanged3(object sender, NotificationEventArgs e)
        {
            await RefreshAsycudaDocuments().ConfigureAwait(false);
        }

        private async void OnAsycudaDocumentSetExsChanged2(object sender, NotificationEventArgs<AsycudaDocumentSetEx> e)
        {
            await RefreshAsycudaDocuments().ConfigureAwait(false);
        }

        private async Task RefreshAsycudaDocuments()
        {
            if (BaseViewModel.Instance.CurrentAsycudaDocumentSetEx != null)
            {
                AsycudaDocuments =
                    await
                        AsycudaDocumentRepository.Instance.GetAsycudaDocumentByAsycudaDocumentSetId(
                                BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId.ToString())
                            .ConfigureAwait(false);
            }
        }

        private async void OnCurrentAsycudaDocumentSetExChanged2(object sender, NotificationEventArgs<AsycudaDocumentSetEx> e)
        {
            if (e.Data != null)
            {
                AsycudaDocuments =
                    await
                        AsycudaDocumentRepository.Instance.GetAsycudaDocumentByAsycudaDocumentSetId(
                            e.Data.AsycudaDocumentSetId.ToString()).ConfigureAwait(false);
            }
        }



        public bool ImportOnlyRegisteredDocuments { get; set; }
        public bool NoMessages { get; set; }
        public bool ImportTariffCodes { get; set; }

        public bool OverwriteExisting { get; set; }

        public bool LinkPi { get; set; }


        public async Task ImportDocuments()
        {
            await ImportDocuments(ImportOnlyRegisteredDocuments, ImportTariffCodes, NoMessages, OverwriteExisting, LinkPi)
                .ConfigureAwait(false);
        }

        internal async Task ImportDocuments(bool onlyRegisteredDocuments, bool importTariffCodes, bool noMessages,
            bool overwriteExisting, bool linkPi)
        {
            StatusModel.Timer("Importing Documents");
            //import asycuda xml id and details
            if (BaseViewModel.Instance.CurrentAsycudaDocumentSetEx == null)
            {
                MessageBox.Show("Please Select Asycuda Document Set");
                return;
            }


            var od = new OpenFileDialog();
            od.DefaultExt = ".xml";
            od.Filter = "Xml Documents (.xml)|*.xml";
            od.Multiselect = true;
            var result = od.ShowDialog();
            if (result == true)
            {
                if (!noMessages)
                {
                    var res =
                        MessageBox.Show("Do you want to Update Database Inventory TariffCodes with Imported codes?",
                            "Update TariffCodes", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes)
                        importTariffCodes = true;

                    var ires = MessageBox.Show("Import only Registered Documents?", "Import Documents",
                        MessageBoxButton.YesNo);
                    if (ires == MessageBoxResult.No)
                        ImportOnlyRegisteredDocuments = false;
                }
                StatusModel.Timer($"Importing {od.FileNames.Count()} files");
                StatusModel.StartStatusUpdate("Importing files", od.FileNames.Count());
                StatusModel.RefreshNow();
                // foreach (var f in od.FileNames)
                await
                    AsycudaDocumentSetExRepository.Instance.ImportDocuments(BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId, onlyRegisteredDocuments, importTariffCodes,
                        noMessages, overwriteExisting,linkPi, od.FileNames.ToList()).ConfigureAwait(false);

                MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentsChanged, null,
                           new NotificationEventArgs(MessageToken.AsycudaDocumentsChanged));

                MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentSetExsChanged, null,
                                new NotificationEventArgs(MessageToken.AsycudaDocumentSetExsChanged));

                StatusModel.StopStatusUpdate();

            }
            StatusModel.StopStatusUpdate();
            MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            
        }

        internal async Task ExportDocuments()
        {

             if (BaseViewModel.Instance.CurrentAsycudaDocument == null)
            {
                MessageBox.Show("Please select Asycuda Document to Export");
                return;
            }

             StatusModel.Timer("Exporting Documents");
            var od = new SaveFileDialog();
            od.FileName = BaseViewModel.Instance.CurrentAsycudaDocument.ReferenceNumber + ".xml";
            var result = od.ShowDialog();
            if (result == true)
            {
                foreach (var f in od.FileNames)
                {
                    try
                    {
                        await AsycudaDocumentSetExRepository.Instance.ExportDocument(f, BaseViewModel.Instance.CurrentAsycudaDocument.ASYCUDA_Id).ConfigureAwait(false);
                        await SalesReportModel.Instance.Send2Excel(f, BaseViewModel.Instance.CurrentAsycudaDocument).ConfigureAwait(false);
                    }
                    catch (Exception)
                    {
                        
                        //throw;
                    }
                    
                }
            }
            StatusModel.StopStatusUpdate();
            MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        internal async Task DeleteDocuments(int docSetId)
        {
            StatusModel.Timer("Deleting Documents");
             var res = MessageBox.Show("Do you want to Delete ALL Documents in this Document Set?", "Clear Document Set", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {
                await AsycudaDocumentSetExRepository.Instance.DeleteDocuments(docSetId).ConfigureAwait(false);
                MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentsChanged, null,
                    new NotificationEventArgs(MessageToken.AsycudaDocumentsChanged));
                MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentSetExsChanged, null,
                    new NotificationEventArgs(MessageToken.AsycudaDocumentSetExsChanged));
                StatusModel.StopStatusUpdate();
                MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }


        }

        internal async Task ExportDocSet(AsycudaDocumentSetEx docSet)
        {
            var res = MessageBox.Show("Do you want to Export ALL Documents in this Document Set?", "Export Document Set", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {
                StatusModel.Timer("Exporting DocumentSet");
                var od = new SaveFileDialog();
                od.FileName = docSet.Declarant_Reference_Number + ".xml";
                var result = od.ShowDialog();
                if (result == true)
                {
                    string first = null;
                    foreach (var name in od.FileNames)
                    {
                        first = name;
                        break;
                    }
                    if (first != null)
                    {
                        var directoryInfo = new DirectoryInfo(first).Parent;
                        if (directoryInfo != null)
                        {
                            var dir = directoryInfo.FullName;
                            // WAS SLOWER I FIND CONFLICTING I FEEL
                            //var t1 =  AsycudaDocumentSetExRepository.Instance.ExportDocSet(docSet.AsycudaDocumentSetId, dir);
                            //var t2 = SalesReportModel.Instance.ExportDocSetSalesReport(docSet.AsycudaDocumentSetId, dir);
                            //await Task.WhenAll(t1, t2).ConfigureAwait(false);

                            await AsycudaDocumentSetExRepository.Instance.ExportDocSet(docSet.AsycudaDocumentSetId, dir).ConfigureAwait(false);
                            await SalesReportModel.Instance.ExportDocSetSalesReport(docSet.AsycudaDocumentSetId, dir).ConfigureAwait(false);




                        }
                    }
                }
                StatusModel.StopStatusUpdate();
                MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            }
        }

        internal async Task SaveDocument(AsycudaDocument asycudaDocument)
        {
            StatusModel.Timer("Saving Document");
            await AsycudaDocumentRepository.Instance.SaveDocument(asycudaDocument).ConfigureAwait(false);
            StatusModel.StopStatusUpdate();
        }

        internal async Task CleanBond(bool perIM7)
        {
            if (BaseViewModel.Instance.CurrentAsycudaDocumentSetEx == null)
            {
                MessageBox.Show("Please Select DocumentSet before proceding.");
                return;
            }

            StatusModel.Timer("Saving Document");
            await AsycudaDocumentSetExRepository.Instance.CleanBond(BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId, perIM7).ConfigureAwait(false);
            StatusModel.StopStatusUpdate();
            MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        internal async Task CleanSelectedBond(bool perIM7)
        {
            if (BaseViewModel.Instance.CurrentAsycudaDocumentSetEx == null)
            {
                MessageBox.Show("Please Select DocumentSet before proceding.");
                return;
            }

            var lst =
                AsycudaDocumentsModel.Instance.SelectedAsycudaDocuments.Select(x => x.ASYCUDA_Id);
            if (lst.Any())
            {
                StatusModel.Timer("Saving Document");
                await
                    AsycudaDocumentSetExRepository.Instance.CleanEntries(
                        BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId, lst, perIM7)
                        .ConfigureAwait(false);
                StatusModel.StopStatusUpdate();
                MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        internal async Task CleanSelectedLines(bool perIM7)
        {
            if (BaseViewModel.Instance.CurrentAsycudaDocumentSetEx == null)
            {
                MessageBox.Show("Please Select DocumentSet before proceding.");
                return;
            }

            var lst =
                AsycudaDocumentItemsModel.Instance.SelectedAsycudaDocumentItems.Select(x => x.Item_Id);
            if (lst.Any())
            {
                StatusModel.Timer("Saving Document");
                await
                    AsycudaDocumentSetExRepository.Instance.CleanLines(
                        BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId, lst, perIM7)
                        .ConfigureAwait(false);
                StatusModel.StopStatusUpdate();
                MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private IEnumerable<AsycudaDocument> _asycudaDocuments = null;
        public IEnumerable<AsycudaDocument> AsycudaDocuments {
            get { return _asycudaDocuments; }
            set
            {
                _asycudaDocuments = value;
                NotifyPropertyChanged(x => this.AsycudaDocuments);
            } }

        public async Task AttachDocuments()
        {
            StatusModel.Timer("Attaching Documents");
            //import asycuda xml id and details
            if (BaseViewModel.Instance.CurrentAsycudaDocumentSetEx == null)
            {
                MessageBox.Show("Please Select Asycuda Document Set");
                return;
            }


            var od = new OpenFileDialog();
            od.DefaultExt = ".*";
            od.Filter = "C71, LIC Documents (.xml)|*.xml|PDF Documents (.pdf)|*.pdf";
            od.Multiselect = true;
            var result = od.ShowDialog();
            if (result == true)
            {
                
                StatusModel.Timer($"Attaching {od.FileNames.Count()} files");
                StatusModel.StartStatusUpdate("Attaching files", od.FileNames.Count());
                StatusModel.RefreshNow();
                // foreach (var f in od.FileNames)
                await
                    AsycudaDocumentSetExRepository.Instance.AttachDocuments(BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId, od.FileNames.ToList()).ConfigureAwait(false);

                MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentsChanged, null,
                           new NotificationEventArgs(MessageToken.AsycudaDocumentsChanged));

                MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentSetExsChanged, null,
                                new NotificationEventArgs(MessageToken.AsycudaDocumentSetExsChanged));

                StatusModel.StopStatusUpdate();

            }
            StatusModel.StopStatusUpdate();
            MessageBox.Show("Complete", "Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);

        }
    }
}