using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AdjustmentQS.Client.Entities;
using AdjustmentQS.Client.Repositories;
using Core.Common.UI;
using CoreEntities.Business.Enums;
using SimpleMvvmToolkit;
using WaterNut.Views;


namespace WaterNut.QuerySpace.AdjustmentQS.ViewModels
{

    public partial class AdjustmentShortViewModel_AutoGen
    {
        partial void OnCreated()
        {
            EffectiveDateFilter = DateTime.MinValue;
            _startEffectiveDateFilter = DateTime.MinValue;
            _endEffectiveDateFilter = DateTime.MinValue;
            EmailDateFilter = DateTime.MinValue;
            _startEmailDateFilter = DateTime.MinValue;
            _endEmailDateFilter = DateTime.MinValue;

            InvoiceDateFilter = DateTime.MinValue;
            _startInvoiceDateFilter = DateTime.MinValue;
            _endInvoiceDateFilter = DateTime.MinValue;

        }


    }

    public class AdjustmentShortDetailsModel : AdjustmentShortViewModel_AutoGen
    {
        private static readonly AdjustmentShortDetailsModel instance;
        static AdjustmentShortDetailsModel()
        {
            instance = new AdjustmentShortDetailsModel() { ViewCurrentAdjustmentEx = true };
        }


        internal override void OnCurrentAdjustmentExChanged(object sender,
            SimpleMvvmToolkit.NotificationEventArgs<AdjustmentEx> e)
        {
            if (ViewCurrentAdjustmentEx == false) return;
            if (e.Data == null || e.Data.InvoiceNo == null)
            {
                vloader.FilterExpression = "None";
            }
            else
            {

                vloader.FilterExpression = $"EntryDataId == \"{e.Data.InvoiceNo.ToString()}\" " +
                                           $"&& AsycudaDocumentSetId == \"{CoreEntities.ViewModels.BaseViewModel.Instance?.CurrentAsycudaDocumentSetEx?.AsycudaDocumentSetId}\"";
            }

            AdjustmentShorts.Refresh();
            NotifyPropertyChanged(x => this.AdjustmentShorts);
        }



        public static AdjustmentShortDetailsModel Instance
        {
            get { return instance; }
        }

        bool _perInvoice = false;
        public bool PerInvoice
        {
            get
            {
                return _perInvoice;
            }
            set
            {
                _perInvoice = value;

                NotifyPropertyChanged(x => this.PerInvoice);
            }
        }

        public bool ViewDocSetData
        {
            get => _viewDocSetData;
            set
            {
                _viewDocSetData = value;
                FilterData();
                NotifyPropertyChanged(x => this.ViewDocSetData);
            }
        }

        bool _viewErrors = false;
        public bool ViewErrors
        {
            get
            {
                return _viewErrors;
            }
            set
            {
                _viewErrors = value;
                FilterData();
                NotifyPropertyChanged(x => this.ViewErrors);
            }
        }



        bool _viewMatches = false;
        public bool ViewMatches
        {
            get
            {
                return _viewMatches;
            }
            set
            {
                _viewMatches = value;
                FilterData();
                NotifyPropertyChanged(x => this.ViewMatches);
            }
        }



        bool _viewSelected = false;
        public bool ViewSelected
        {
            get
            {
                return _viewSelected;
            }
            set
            {
                _viewSelected = value;
                FilterData();
                NotifyPropertyChanged(x => this.ViewSelected);
            }
        }




        bool _viewNoMatches = false;
        private bool _viewDocSetData;

        public bool ViewNoMatches
        {
            get
            {
                return _viewNoMatches;
            }
            set
            {
                _viewNoMatches = value;
                FilterData();
                NotifyPropertyChanged(x => this.ViewNoMatches);
            }
        }

        public async Task AutoMatch()
        {
            StatusModel.Timer("AutoMatching");
            using (var ctx = new AdjustmentShortRepository())
            {
                await ctx.AutoMatch(CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, true).ConfigureAwait(false);
            }

            

            MessageBus.Default.BeginNotify(MessageToken.AdjustmentShortsChanged, this,
                new NotificationEventArgs(MessageToken.AdjustmentShortsChanged));
            StatusModel.StopStatusUpdate();
            MessageBox.Show("AutoMatch Complete", "Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        
        internal void ViewSuggestions(AdjustmentShort osd)
        {
            QuerySpace.CoreEntities.ViewModels.AsycudaDocumentItemsModel.Instance.vloader.FilterExpression =
                $"ItemNumber == \"{osd.ItemNumber}\" && (CustomsOperationId == { (int)CustomsOperations.Warehouse})";//ToDO: check this im7 string

            QuerySpace.CoreEntities.ViewModels.AsycudaDocumentItemsModel.Instance.AsycudaDocumentItems.OrderBy(
                x => x.Data.AsycudaDocument.RegistrationDate - osd.AdjustmentEx.InvoiceDate);

            // OnStaticPropertyChanged("OSSuggestedAsycudaItemEntry");
        }

        internal async Task RemoveOsMatch(AdjustmentShort osd)
        {
            //osd.EntryDataDetailAllocations.Clear();
            //osd.Status = "";
            //using (var ctx = new EntryDataDetailRepository())
            //{
            //    await ctx.UpdateEntryDataDetail(osd).ConfigureAwait(false);
            //}
            //EntryDataDetails.Refresh();
        }



        public override void FilterData()
        {
            var res = GetAutoPropertyFilterString();
            res.Append($" && ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}");
            if (ViewDocSetData && CoreEntities.ViewModels.BaseViewModel.Instance?.CurrentAsycudaDocumentSetEx != null)
            {
                res.Append($@" && AsycudaDocumentSetId == ""{CoreEntities.ViewModels.BaseViewModel.Instance?.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId}""");
            }

            if (ViewCurrentAdjustmentEx && BaseViewModel.Instance?.CurrentAdjustmentEx != null)
            {
                res.Append($@" && EntryDataId == ""{BaseViewModel.Instance.CurrentAdjustmentEx.EntityId}""");
            }
            
            
            if (_viewErrors)
            {
                res.Append(@" && Status != null");
            }
            if (_viewMatches)
            {
                res.Append(@" && Status == null");
            }

            if (_viewSelected)
            {
                var lst = AdjustmentExModel.Instance.SelectedAdjustmentExes.ToList();
                if (lst.Any())
                {
                    var slst = BuildOSLst(lst);
                    //remove comma

                    res.Append($@" && ({slst})");
                }
            }

            if (_viewNoMatches)
            {
                //vloader.SetNavigationExpression("EX");
                res.Append(@" && EX.Duration > 15 && EX.InvoiceMonth != EX.AsycudaMonth");

            }

            FilterData(res);

        }

        

        private string BuildOSLst(List<AdjustmentEx> lst)
        {
            // res.Append($@" && EntryDataId == ""{BaseViewModel.Instance.CurrentAdjustmentEx.EntityId}""")
            return lst.Where(x => x?.InvoiceNo != null).Select(x => x.InvoiceNo.ToString()).Aggregate(new StringBuilder(), (o, n) =>
            {
                //if (o.Length > 0) o.Append($"EntryDataId =={o}");
                o.Append($" EntryDataId == \"{n}\" ||");
                return o;
                
            }).ToString().TrimEnd('|',' ');
        }




        internal async Task MatchToCurrentItem(AdjustmentShort entryDataDetailsEx)
        {
            if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentItem == null)
            {
                MessageBox.Show("Please Select Asycuda Item");
                return;
            }

            using (var ctx = new AdjustmentShortRepository())
            {
                await ctx.MatchToAsycudaItem(entryDataDetailsEx.EntryDataDetailsId,CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentItem.Item_Id).ConfigureAwait(false);
            }


            MessageBus.Default.BeginNotify(MessageToken.CurrentAdjustmentShortChanged, this,
              new NotificationEventArgs<AdjustmentShort>(
                  QuerySpace.AdjustmentQS.MessageToken.CurrentAdjustmentShortChanged, entryDataDetailsEx));
            MessageBox.Show("Manual Match Complete");
        }

        internal async Task RemoveEntryDataDetail(AdjustmentShort EntryDataDetailsEX)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure you want to delete this Adjustment Short Detail?",
               "Delete selected Items", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {
                await EntryDataDetailRepository.Instance.DeleteEntryDataDetail(EntryDataDetailsEX.EntityId).ConfigureAwait(false);

                MessageBus.Default.BeginNotify(MessageToken.AdjustmentShortsChanged, this,
                    new NotificationEventArgs(MessageToken.AdjustmentShortsChanged));
                BaseViewModel.Instance.CurrentAdjustmentShort = null;

            }

            MessageBox.Show("Short Deleted");
        }


        public async Task CreateIM9()
        {
            StatusModel.Timer("Creating IM9");
            using (var ctx = new AdjustmentShortRepository())
            {
                await ctx.CreateIM9(vloader.FilterExpression, PerInvoice, Process7100, CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId, "DIS", "Duty Free").ConfigureAwait(false);
            }

            MessageBus.Default.BeginNotify(MessageToken.AdjustmentShortsChanged, this,
                new NotificationEventArgs(MessageToken.AdjustmentShortsChanged));
            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentsChanged, null,
                new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentsChanged));
            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged, null,
                new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged));
            StatusModel.StopStatusUpdate();
            MessageBox.Show("IM9 Entries Complete", "Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        public bool Process7100 { get; set; }

        public async Task CreateIM4()
        {
            StatusModel.Timer("Creating IM4");
            using (var ctx = new AdjustmentShortRepository())
            {
                await ctx.CreateIM9(vloader.FilterExpression, PerInvoice, Process7100, CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId, "DIS", "Duty Free").ConfigureAwait(false);// the code 801 makes it duty free
            }

            MessageBus.Default.BeginNotify(MessageToken.AdjustmentShortsChanged, this,
                new NotificationEventArgs(MessageToken.AdjustmentShortsChanged));
            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentsChanged, null,
                new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentsChanged));
            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged, null,
                new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged));
            StatusModel.StopStatusUpdate();
            MessageBox.Show("IM4 Entries Complete", "Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }
}