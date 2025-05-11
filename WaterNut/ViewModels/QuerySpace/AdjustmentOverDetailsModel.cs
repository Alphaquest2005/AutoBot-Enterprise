using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AdjustmentQS.Client.Entities;
using AdjustmentQS.Client.Repositories;
using Core.Common.UI;
using CoreEntities.Client.Enums;
using SimpleMvvmToolkit;
using WaterNut.QuerySpace.CoreEntities.ViewModels;

namespace WaterNut.QuerySpace.AdjustmentQS.ViewModels
{
    public partial class AdjustmentOverViewModel_AutoGen
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

    public class AdjustmentOverDetailsModel : AdjustmentOverViewModel_AutoGen
    {
        private bool _viewBadMatches;
        private bool _viewDocSetData;

        private bool _viewErrors;


        private bool _viewMatches;


        private bool _viewSelected;

        static AdjustmentOverDetailsModel()
        {
            Instance = new AdjustmentOverDetailsModel {ViewCurrentAdjustmentEx = true};
        }


        public static AdjustmentOverDetailsModel Instance { get; }

        public bool ViewErrors
        {
            get => _viewErrors;
            set
            {
                _viewErrors = value;
                FilterData();
                NotifyPropertyChanged(x => ViewErrors);
            }
        }

        public bool ViewMatches
        {
            get => _viewMatches;
            set
            {
                _viewMatches = value;
                FilterData();
                NotifyPropertyChanged(x => ViewMatches);
            }
        }

        public bool ViewSelected
        {
            get => _viewSelected;
            set
            {
                _viewSelected = value;
                FilterData();
                NotifyPropertyChanged(x => ViewSelected);
            }
        }

        public bool ViewBadMatches
        {
            get => _viewBadMatches;
            set
            {
                _viewBadMatches = value;
                FilterData();
                NotifyPropertyChanged(x => ViewBadMatches);
            }
        }

        public bool PerInvoice { get; set; }

        public bool ViewDocSetData
        {
            get => _viewDocSetData;
            set
            {
                _viewDocSetData = value;
                FilterData();
                NotifyPropertyChanged(x => ViewDocSetData);
            }
        }


        private async Task<IEnumerable<AdjustmentOver>> GetBadMatchLst()
        {
            using (var ctx = new AdjustmentOverRepository())
            {
                var lst = await ctx.GetAdjustmentOversByExpressionNav("All",
                    new Dictionary<string, string>
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
            AsycudaDocumentItemsModel.Instance.vloader.FilterExpression =
                $"ItemNumber == \"{osd.ItemNumber}\" && (CustomsOperationId == {(int) CustomsOperations.Warehouse})"; // i don't know what going on so left it

            AsycudaDocumentItemsModel.Instance.AsycudaDocumentItems.OrderBy(
                x => x.Data.AsycudaDocument.RegistrationDate - osd.AdjustmentEx.InvoiceDate);

            // OnStaticPropertyChanged("OSSuggestedAsycudaItemEntry");
        }

        internal Task RemoveOsMatch(AdjustmentOver osd)
        {
            return Task.CompletedTask;
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
            res.Append(
                $" && ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}");
            if (ViewDocSetData && CoreEntities.ViewModels.BaseViewModel.Instance?.CurrentAsycudaDocumentSetEx != null)
                res.Append(
                    $@" && AsycudaDocumentSetId == ""{CoreEntities.ViewModels.BaseViewModel.Instance?.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId}""");

            if (ViewCurrentAdjustmentEx && BaseViewModel.Instance?.CurrentAdjustmentEx != null)
                res.Append($@" && EntryDataId == ""{BaseViewModel.Instance.CurrentAdjustmentEx.EntityId}""");

            if (_viewErrors) res.Append(@" && Status != null");
            if (_viewMatches) res.Append(@" && Status == null");

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

            if (_viewBadMatches)
                //vloader.SetNavigationExpression("EX");
                res.Append(@" && EX.Duration > 15 && EX.InvoiceMonth != EX.AsycudaMonth");

            FilterData(res);
        }

        private string BuildOSLst(List<AdjustmentEx> lst)
        {
            // res.Append($@" && EntryDataId == ""{BaseViewModel.Instance.CurrentAdjustmentEx.EntityId}""")
            return lst.Where(x => x?.EntryData_Id != 0).Select(x => x.EntryData_Id.ToString()).Aggregate(
                new StringBuilder(), (o, n) =>
                {
                    //if (o.Length > 0) o.Append($"EntryDataId =={o}");
                    o.Append($" EntryData_Id == {n} ||");
                    return o;
                }).ToString().TrimEnd('|', ' ');
        }


        internal async Task MatchToCurrentItem(AdjustmentOver entryDataDetailsEx)
        {
            if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentItem == null)
            {
                MessageBox.Show("Please Select Asycuda Item");
                return;
            }

            await AdjustmentShortRepository.Instance.MatchToAsycudaItem(entryDataDetailsEx.EntryDataDetailsId,
                    CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentItem.Item_Id)
                .ConfigureAwait(false);

            MessageBus.Default.BeginNotify(MessageToken.CurrentAdjustmentOverChanged, this,
                new NotificationEventArgs<AdjustmentOver>(
                    MessageToken.CurrentAdjustmentOverChanged, entryDataDetailsEx));
            MessageBox.Show("Manual Match Complete");
        }

        internal async Task RemoveEntryDataDetail(AdjustmentOver EntryDataDetailsEX)
        {
            var res = MessageBox.Show("Are you sure you want to delete this EntryData Detail?",
                "Delete selected Items", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {
                await EntryDataDetailRepository.Instance.DeleteEntryDataDetail(EntryDataDetailsEX.EntityId)
                    .ConfigureAwait(false);

                MessageBus.Default.BeginNotify(MessageToken.AdjustmentOversChanged, this,
                    new NotificationEventArgs(MessageToken.AdjustmentOversChanged));
                BaseViewModel.Instance.CurrentAdjustmentOver = null;
            }
        }


        public async Task CreateOPS()
        {
            if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx == null)
            {
                MessageBox.Show("Please Select Asycuda Document Set!");
                return;
            }

            StatusModel.Timer("Creating IM9");
            using (var ctx = new AdjustmentOverRepository())
            {
                await ctx.CreateOPS(vloader.FilterExpression, PerInvoice, "DIS",
                        CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId)
                    .ConfigureAwait(false);
            }

            MessageBus.Default.BeginNotify(MessageToken.AdjustmentOversChanged, this,
                new NotificationEventArgs(MessageToken.AdjustmentOversChanged));
            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentsChanged, null,
                new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentsChanged));
            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged, null,
                new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged));
            StatusModel.StopStatusUpdate();
            MessageBox.Show("ADJ OPS Entries Complete", "Asycuda Toolkit", MessageBoxButton.OK,
                MessageBoxImage.Exclamation);
        }
    }
}