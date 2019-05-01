using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AdjustmentQS.Client.Entities;
using AdjustmentQS.Client.Repositories;
using Core.Common.UI;
using SimpleMvvmToolkit;


namespace WaterNut.QuerySpace.AdjustmentQS.ViewModels
{
    public class AdjustmentOverDetailsModel : AdjustmentOverViewModel_AutoGen
    {
        private static readonly AdjustmentOverDetailsModel instance;
        static AdjustmentOverDetailsModel()
        {
            instance = new AdjustmentOverDetailsModel() { ViewCurrentAdjustmentEx = true };
        }

        

        public static AdjustmentOverDetailsModel Instance
        {
            get { return instance; }
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






        bool _viewBadMatches = false;
        private bool _viewDocSetData;

        public bool ViewBadMatches
        {
            get
            {
                return _viewBadMatches;
            }
            set
            {
                _viewBadMatches = value;
                FilterData();
                NotifyPropertyChanged(x => this.ViewBadMatches);
            }
        }



        private async Task<IEnumerable<AdjustmentOver>> GetBadMatchLst()
        {
            using (var ctx = new AdjustmentOverRepository())
            {
                var lst = await ctx.GetAdjustmentOversByExpressionNav("All",
                    new Dictionary<string, string>()
                    {
                        {
                            "EX", "(Duration > 15) || (AsycudaMonth != InvoiceMonth)"
                        }
                    }).ConfigureAwait(false);

                return lst;
            }
        }



        internal void ViewSuggestions(AdjustmentOver osd)
        {
            QuerySpace.CoreEntities.ViewModels.AsycudaDocumentItemsModel.Instance.vloader.FilterExpression =
                $"ItemNumber == \"{osd.ItemNumber}\" && (DocumentType == \"IM7\"  || DocumentType == \"OS7\")";

            QuerySpace.CoreEntities.ViewModels.AsycudaDocumentItemsModel.Instance.AsycudaDocumentItems.OrderBy(
                x => x.Data.AsycudaDocument.RegistrationDate - osd.AdjustmentEx.InvoiceDate);

            // OnStaticPropertyChanged("OSSuggestedAsycudaItemEntry");
        }

        internal async Task RemoveOsMatch(AdjustmentOver osd)
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
            //res.Append(@" && InvoiceQty < ReceivedQty");
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
                res.Append(@" && EntryDataDetailAllocations.Any()");
            }

            if (_viewSelected)
            {
                var lst = AdjustmentExModel.Instance.SelectedAdjustmentExes.ToList();
                if (lst.Any())
                {
                    var slst = BuildOSLst(lst);
                    //remove comma

                    res.Append(slst);
                }
            }

            if (_viewBadMatches)
            {
                //vloader.SetNavigationExpression("EX");
                res.Append(@" && EX.Duration > 15 && EX.InvoiceMonth != EX.AsycudaMonth");

            }

            FilterData(res);

        }

        private async Task BuildOSLst(List<AdjustmentEx> lst)
        {
           // await AdjustmentExRepository.Instance.BuildOSLst(lst.Select(x => x.AdjustmentsId).ToList()).ConfigureAwait(false);
        }




        internal async Task MatchToCurrentItem(AdjustmentOver entryDataDetailsEx)
        {
            if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentItem == null)
            {
                MessageBox.Show("Please Select Asycuda Item");
                return;
            }

            //await EntryDataDetailsExRepository.Instance.MatchToCurrentItem(
            //    CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentItem.Item_Id, entryDataDetailsEx)
            //    .ConfigureAwait(false);

            MessageBus.Default.BeginNotify(MessageToken.CurrentAdjustmentOverChanged, this,
              new NotificationEventArgs<AdjustmentOver>(
                  QuerySpace.AdjustmentQS.MessageToken.CurrentAdjustmentOverChanged, entryDataDetailsEx));
        }

        internal async Task RemoveEntryDataDetail(AdjustmentOver EntryDataDetailsEX)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure you want to delete this EntryData Detail?",
               "Delete selected Items", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {
                await EntryDataDetailRepository.Instance.DeleteEntryDataDetail(EntryDataDetailsEX.EntityId).ConfigureAwait(false);

                MessageBus.Default.BeginNotify(MessageToken.AdjustmentOversChanged, this,
                    new NotificationEventArgs(MessageToken.AdjustmentOversChanged));
                BaseViewModel.Instance.CurrentAdjustmentOver = null;

            }
        }



        public async Task CreateOPS()
        {
            StatusModel.Timer("Creating IM9");
            using (var ctx = new AdjustmentOverRepository())
            {
                await ctx.CreateOPS(vloader.FilterExpression, PerInvoice, CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId).ConfigureAwait(false);
            }

            MessageBus.Default.BeginNotify(MessageToken.AdjustmentOversChanged, this,
                new NotificationEventArgs(MessageToken.AdjustmentOversChanged));
            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentsChanged, null,
                new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentsChanged));
            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged, null,
                new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged));
            StatusModel.StopStatusUpdate();
            MessageBox.Show("ADJ OPS Entries Complete", "Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        public bool PerInvoice { get; set; }

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
    }
}