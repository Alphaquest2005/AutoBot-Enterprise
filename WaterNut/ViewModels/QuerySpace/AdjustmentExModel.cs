using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AdjustmentQS.Client.Entities;
using Core.Common.UI;
using CoreEntities.Client.Entities;
using EntryDataQS.Client.Repositories;
using SimpleMvvmToolkit;

namespace WaterNut.QuerySpace.AdjustmentQS.ViewModels
{
    using Serilog;

    public class AdjustmentExModel : AdjustmentExViewModel_AutoGen
    {
        private string _entryDataDateFilter;

        static AdjustmentExModel()
        {
            Instance = new AdjustmentExModel();
        }


        private AdjustmentExModel()
        {
            RegisterToReceiveMessages<AsycudaDocumentSetEx>(
                CoreEntities.MessageToken.CurrentAsycudaDocumentSetExChanged, OnCurrentAsycudaDocumentSetExChanged);
            RegisterToReceiveMessages<AsycudaDocument>(CoreEntities.MessageToken.CurrentAsycudaDocumentChanged,
                OnCurrentAsycudaDocumentChanged);
            RegisterToReceiveMessages<AdjustmentShort>(MessageToken.CurrentAdjustmentShortChanged,
                OnCurrentAdjustmentShortChanged);
            RegisterToReceiveMessages<AdjustmentOver>(MessageToken.CurrentAdjustmentOverChanged,
                OnCurrentAdjustmentOverChanged);
        }

        public static AdjustmentExModel Instance { get; }


        public string EntryDataDateFilter
        {
            get => _entryDataDateFilter;
            set
            {
                _entryDataDateFilter = value;
                NotifyPropertyChanged(x => EntryDataDateFilter);
                FilterData();
            }
        }

        private void OnCurrentAdjustmentOverChanged(object sender, NotificationEventArgs<AdjustmentOver> e)
        {
            if (e.Data == null) return;
            vloader.FilterExpression =
                $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId} " +
                $"&& InvoiceNo == \"{e.Data.EntryDataId}\"";
            AdjustmentExes.Refresh();
        }

        private void OnCurrentAdjustmentShortChanged(object sender, NotificationEventArgs<AdjustmentShort> e)
        {
            if (e.Data == null) return;
            vloader.FilterExpression =
                $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId} " +
                $"&& InvoiceNo == \"{e.Data.EntryDataId}\"";
            AdjustmentExes.Refresh();
        }

        private void OnCurrentAsycudaDocumentChanged(object sender, NotificationEventArgs<AsycudaDocument> e)
        {
            if (e.Data != null)
            {
                vloader.FilterExpression =
                    $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";
                // vloader.NavExpression = string.Format("AsycudaDocumentSetId = {0}", e.Data.AsycudaDocumentSetId);
                vloader.SetNavigationExpression("AsycudaDocuments", $"AsycudaDocumentId == {e.Data.ASYCUDA_Id}");
                AdjustmentExes.Refresh();
            }
        }


        private void OnCurrentAsycudaDocumentSetExChanged(object sender, NotificationEventArgs<AsycudaDocumentSetEx> e)
        {
            if (e.Data != null)
            {
                vloader.FilterExpression =
                    $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";
                vloader.SetNavigationExpression("AsycudaDocumentSets",
                    $"AsycudaDocumentSetId == {e.Data.AsycudaDocumentSetId}");
                AdjustmentExes.Refresh();
            }
        }


        internal async Task RemoveAdjustment(AdjustmentEx adjustment)
        {
            await EntryDataRepository.Instance.DeleteEntryData(adjustment.EntityId).ConfigureAwait(false);

            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentsChanged, null,
                new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentsChanged));

            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged, null,
                new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged));

            MessageBus.Default.BeginNotify(CounterPointQS.MessageToken.CounterPointPOsChanged, null,
                new NotificationEventArgs(CounterPointQS.MessageToken.CounterPointPOsChanged));

            MessageBus.Default.BeginNotify(MessageToken.AdjustmentExesChanged, null,
                new NotificationEventArgs(MessageToken.AdjustmentExesChanged));
        }


        public async Task Import(string fileType, ILogger log)
        {
            StatusModel.Timer("Importing Adjustments");
            if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx == null)
            {
                MessageBox.Show("Please Select Adjustment DocumentSet.");
                return;
            }

            await SaveCSV.Instance.SaveCSVFile(fileType,
                    CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId, log)
                .ConfigureAwait(false);
            // only for DIS don't bother for ADJ
           // await AdjustmentShortDetailsModel.Instance.AutoMatch().ConfigureAwait(false); 

            MessageBus.Default.BeginNotify(MessageToken.AdjustmentExesChanged, null,
                new NotificationEventArgs(MessageToken.AdjustmentExesChanged));


            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentsChanged, null,
                new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentsChanged));
            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged, null,
                new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged));
            StatusModel.StopStatusUpdate();
            MessageBox.Show("Complete", "Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        internal async Task RemoveSelectedAdjustment(
            List<AdjustmentEx> list)
        {
            var res = MessageBox.Show("Are you sure you want to delete all Selected Items?", "Delete selected Items",
                MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {
                await EntryDataRepository.Instance
                    .RemoveSelectedEntryData(SelectedAdjustmentExes.Select(x => x.EntityId)).ConfigureAwait(false);

                MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentsChanged, null,
                    new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentsChanged));

                MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged, null,
                    new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged));

                MessageBus.Default.BeginNotify(CounterPointQS.MessageToken.CounterPointPOsChanged, null,
                    new NotificationEventArgs(CounterPointQS.MessageToken.CounterPointPOsChanged));

                MessageBus.Default.BeginNotify(MessageToken.AdjustmentExesChanged, null,
                    new NotificationEventArgs(MessageToken.AdjustmentExesChanged));

                MessageBus.Default.BeginNotify(MessageToken.AdjustmentExesFilterExpressionChanged, null,
                    new NotificationEventArgs(MessageToken.AdjustmentExesFilterExpressionChanged));

                MessageBox.Show("Complete", "Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                //adjustment.Refresh();
            }
        }
    }
}

//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Threading.Tasks;
//using System.ComponentModel;
//using Core.Common.UI;
//using OversShortQS.Client.Entities;
//using OversShortQS.Client.Repositories;
//using SimpleMvvmToolkit;

//using WaterNut.QuerySpace.OversShortQS.ViewModels;
//using System.Windows;


//namespace WaterNut.QuerySpace.OversShortQS.ViewModels
//{
//    public class AdjustmentExModel : OversShortEXViewModel_AutoGen
//    {
//          private static readonly AdjustmentExModel instance;
//          static AdjustmentExModel()
//        {
//            instance = new AdjustmentExModel();

//        }

//          public static AdjustmentExModel Instance
//        {
//            get { return instance; }
//        }

//        private bool _viewDocData = false;

//        public bool ViewDocData
//        {
//            get { return _viewDocData; }
//            set
//            {
//                _viewDocData = value;
//                base.FilterData();

//            }
//        }
//        private bool _applyEX9Bucket = false;
//        public bool ApplyEX9Bucket
//        {
//            get { return _applyEX9Bucket; }

//            set
//            {
//                _applyEX9Bucket = value;
//                if (_applyEX9Bucket == true)
//                {
//                   BreakOnMonthYear = false;
//                   NotifyPropertyChanged(x => this.BreakOnMonthYear);
//                }
//            }
//        }

//        public bool OverWriteExisting { get; set; }

//        public  bool BreakOnMonthYear { get; set; }

//        private string referenceNumberFilter;
//        public string ReferenceNumberFilter
//        {
//            get { return referenceNumberFilter;  }
//            set
//            {
//                referenceNumberFilter = value;
//                FilterData();
//                NotifyPropertyChanged(x => this.ReferenceNumberFilter);
//            }
//        }

//        private string cNumberFilter;
//        public string CNumberFilter
//        {
//            get { return cNumberFilter; }
//            set
//            {
//                cNumberFilter = value;
//                FilterData();
//                NotifyPropertyChanged(x => this.CNumberFilter);
//            }
//        }

//        public override void FilterData()
//        {
//            var res = GetAutoPropertyFilterString();
//            if (!string.IsNullOrEmpty(ReferenceNumberFilter))
//            {
//                res.Append(string.Format(" && OverShortSuggestedDocuments.ReferenceNumber.Contains(\"{0}\")",ReferenceNumberFilter));
//            }

//            if (!string.IsNullOrEmpty(ReferenceNumberFilter))
//            {
//                res.Append(string.Format(" && OverShortSuggestedDocuments.CNumber.Contains(\"{0}\")", CNumberFilter));
//            }
//            FilterData(res);
//        }

//        internal async Task Import()
//        {
//            if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx == null)
//            {
//                MessageBox.Show("Please Select a Document Set before Importing.");
//                return;
//            }
//            Microsoft.Win32.OpenFileDialog od = new Microsoft.Win32.OpenFileDialog();
//            od.Title = "Import Sales";
//            od.DefaultExt = ".csv";
//            od.Filter = "CSV Files (.csv)|*.csv";
//            od.Multiselect = true;
//            Nullable<bool> result = od.ShowDialog();
//            if (result == true)
//            {
//                foreach (string f in od.FileNames)
//                {
//                    await OversShortEXRepository.Instance.Import(f, "OversShort", CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId, OverWriteExisting).ConfigureAwait(false);
//                }
//            }
//            MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
//            //ViewAll();
//        }

//        internal async Task CreateOSEntries()
//        {

//            if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx == null)
//            {
//                MessageBox.Show("Please Select a Asycuda Document Set before proceding");
//                return;
//            }

//            var selOS = SelectedOversShorts.ToList();

//            if (selOS.Any() == false)
//            {
//                MessageBox.Show("Please Select Overs/Shorts before proceding");
//                return;
//            }

//            await OversShortEXRepository.Instance.CreateOversOps(SelectedOversShorts.Select(x => x.OversShortsId),
//                CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId).ConfigureAwait(false);

//            await
//                OversShortEXRepository.Instance.CreateShortsEx9(SelectedOversShorts.Select(x => x.OversShortsId),
//                    CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId, BreakOnMonthYear,
//                    ApplyEX9Bucket).ConfigureAwait(false);

//            StatusModel.StopStatusUpdate();

//            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentsChanged, this, new NotificationEventArgs(QuerySpace.CoreEntities.MessageToken.AsycudaDocumentsChanged));
//            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged, this, new NotificationEventArgs(QuerySpace.CoreEntities.MessageToken.AsycudaDocumentSetExsChanged));


//            MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
//        }

//        internal async Task SaveCNumber(string p)
//        {
//            await OversShortEXRepository.Instance.SaveCNumber(SelectedOversShorts.Select(x => x.OversShortsId), p).ConfigureAwait(false);

//            MessageBus.Default.BeginNotify(MessageToken.OversShortsChanged, this,
//             new NotificationEventArgs(MessageToken.OversShortsChanged));
//        }

//        internal async Task AutoMatch()
//        {
//            await OversShortEXRepository.Instance.AutoMatch(SelectedOversShorts.Select(x => x.OversShortsId)).ConfigureAwait(false);
//        }

//        internal async Task RemoveSelectedOverShorts()
//        {
//            MessageBoxResult res = MessageBox.Show("Are you sure you want to delete all Selected Items?",
//              "Delete selected Items", MessageBoxButton.YesNo);
//            List<OversShortEX> lst = null;
//            if (res == MessageBoxResult.Yes)

//                lst = Instance.SelectedOversShorts.ToList();
//            if (lst == null) return;

//            await OversShortEXRepository.Instance.RemoveSelectedOverShorts(SelectedOversShorts.Select(x => x.OversShortsId)).ConfigureAwait(false);

//            MessageBus.Default.BeginNotify(MessageToken.OversShortsChanged, this,
//              new NotificationEventArgs(MessageToken.OversShortsChanged));
//            MessageBus.Default.BeginNotify(MessageToken.SelectedOversShortsChanged, this,
//                new NotificationEventArgs(MessageToken.OversShortsChanged));
//        }

//        internal async Task SaveReferenceNumber(string p)
//        {
//            await OversShortEXRepository.Instance.SaveReferenceNumber(SelectedOversShorts.Select(x => x.OversShortsId), p).ConfigureAwait(false);

//            MessageBus.Default.BeginNotify(MessageToken.OversShortsChanged, this,
//                 new NotificationEventArgs(MessageToken.OversShortsChanged));
//        }

//        internal async Task MatchEntries()
//        {
//            await OversShortEXRepository.Instance.MatchEntries(SelectedOversShorts.Select(x => x.OversShortsId)).ConfigureAwait(false);
//        }
//    }
//}