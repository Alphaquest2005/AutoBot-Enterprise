using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks.Schedulers;
using System.Windows;
using AllocationQS.Client.Entities;
using Core.Common.Converters;
using PreviousDocumentQS.Client.Repositories;
using PreviousDocumentQS.Client.Entities;
using SimpleMvvmToolkit;
using WaterNut.Views;

namespace WaterNut.QuerySpace.PreviousDocumentQS.ViewModels
{
    public partial class PreviousItemsViewModel : ViewModelBase<PreviousItemsViewModel>
    {
         private static readonly PreviousItemsViewModel instance;
         static PreviousItemsViewModel()
        {
            instance = new PreviousItemsViewModel();
        }

         public static PreviousItemsViewModel Instance
        {
            get { return instance; }
        }
        private PreviousItemsViewModel()
		{
            RegisterToReceiveMessages<AsycudaSalesAndAdjustmentAllocationsEx>(AllocationQS.MessageToken.CurrentAsycudaSalesAndAdjustmentAllocationsExChanged,
                                                                    OnCurrentAsycudaSalesAllocationsExChanged);
            RegisterToReceiveMessages<PreviousDocumentItem>(MessageToken.CurrentPreviousDocumentItemChanged, OnCurrentPreviousDocumentItemChanged);
            RegisterToReceiveMessages<global::CoreEntities.Client.Entities.AsycudaDocumentItem>(CoreEntities.MessageToken.CurrentAsycudaDocumentItemChanged, OnCurrentAsycudaDocumentItemChanged);
		}

        private async void OnCurrentAsycudaSalesAllocationsExChanged(object sender, NotificationEventArgs<AsycudaSalesAndAdjustmentAllocationsEx> e)
        {
            await GetPreviousItems(e.Data.PreviousItem_Id.GetValueOrDefault()).ConfigureAwait(false);
        }

        private async void OnCurrentAsycudaDocumentItemChanged(object sender, NotificationEventArgs<global::CoreEntities.Client.Entities.AsycudaDocumentItem> e)
        {
            if (e.Data == null)
            {
                PreviousItems = new List<PreviousItemsEx>();
            }
            else
            {
                using (var ctx = new PreviousItemsExRepository())
                {
                    PreviousItems =
                        (await ctx.GetPreviousItemsExesByExpression($"AsycudaDocumentItemId == {e.Data.Item_Id}",//Item_Id
                                                            new List<string>()
                                                            {
                                                                "PreviousDocumentItem.PreviousDocument",//
                                                               // "AsycudaDocumentItem"
                                                            }).ConfigureAwait(false)).ToList();
                }
            }
        }

        private async void OnCurrentPreviousDocumentItemChanged(object sender, NotificationEventArgs<PreviousDocumentItem> e)
        {
            if (e.Data == null)
            {
                PreviousItems = new List<PreviousItemsEx>();
            }
            else
            {
                using (var ctx = new PreviousItemsExRepository())
                {
                    PreviousItems =
                        (await ctx.GetPreviousItemsExesByExpression($"PreviousDocumentItemId == {e.Data.Item_Id}",//Item_Id
                                                            new List<string>()
                                                            {
                                                                "PreviousDocumentItem.PreviousDocument",//
                                                               // "AsycudaDocumentItem"
                                                            }).ConfigureAwait(false)).ToList();
                }
            }

        }

        List<PreviousItemsEx> _previousItems = new List<PreviousItemsEx>();

        public List<PreviousItemsEx> PreviousItems
        {
            get { return _previousItems; }
            set
            {
                _previousItems = value;
                NotifyPropertyChanged(x => x.PreviousItems);
            }
        }

        private async void OnCurrentAsycudaSalesAllocationsExChanged(object sender, NotificationEventArgs<AsycudaSalesAllocationsEx> e)
        {
           
               
                    await GetPreviousItems(e.Data.PreviousItem_Id).ConfigureAwait(false);
              
            
        }

        private async Task GetPreviousItems(int previousItem_Id)
        {
            using (var ctx = new PreviousItemsExRepository())
            {
                PreviousItems =
                    (await ctx.GetPreviousItemsExesByExpression(
                        $"PreviousDocumentItemId == {previousItem_Id}").ConfigureAwait(false))
                    .ToList();
            }
        }

        private string _prev_reg_nbrFilter;
        public string Prev_reg_nbrFilter
        {
            get
            {
                return _prev_reg_nbrFilter;
            }
            set
            {
                _prev_reg_nbrFilter = value;
                NotifyPropertyChanged(x => Prev_reg_nbrFilter);
                FilterData();

            }
        }

        private string _previous_item_numberFilter;
        public string Previous_item_numberFilter
        {
            get
            {
                return _previous_item_numberFilter;
            }
            set
            {
                _previous_item_numberFilter = value;
                NotifyPropertyChanged(x => Previous_item_numberFilter);
                FilterData();

            }
        }	

        private  void FilterData()
        {
            var res = new StringBuilder();
            if (!string.IsNullOrEmpty(Prev_reg_nbrFilter))
            {
                res.Append($"&& Prev_reg_nbr == \"{Prev_reg_nbrFilter}\"");
            }

            if (!string.IsNullOrEmpty(Previous_item_numberFilter))
            {
                res.Append($"&& Previous_item_number == \"{Previous_item_numberFilter}\"");
            }

            if (res.Length > 3)
            {
                using (var ctx = new PreviousItemsExRepository())
                {
                    PreviousItems =
                        (ctx.GetPreviousItemsExesByExpression(res.ToString().Substring(3), //Item_Id
                            new List<string>()
                            {
                                "PreviousDocumentItem.PreviousDocument", //
                                // "AsycudaDocumentItem"
                            })).Result.ToList();
                }
            }
        }

        public async Task Send2Excel()
        {
            IEnumerable<PreviousItemsEx> lst = PreviousItems;
            if (lst == null || !lst.Any())
            {
                MessageBox.Show("No Data to Send to Excel");
                return;
            }
            var s = new ExportToCSV<PreviousItemsExViewModel_AutoGen.PreviousItemsExExcelLine, List<PreviousItemsExViewModel_AutoGen.PreviousItemsExExcelLine>>
            {
                dataToPrint = lst.Select(x => new PreviousItemsExViewModel_AutoGen.PreviousItemsExExcelLine
                {

                    Packages_number = x.Packages_number,


                    Previous_Packages_number = x.Previous_Packages_number,


                    Hs_code = x.Hs_code,


                    Commodity_code = x.Commodity_code,


                    Previous_item_number = x.Previous_item_number,


                    Goods_origin = x.Goods_origin,


                    Net_weight = x.Net_weight,


                    Prev_net_weight = x.Prev_net_weight,


                    Prev_reg_ser = x.Prev_reg_ser,


                    Prev_reg_nbr = x.Prev_reg_nbr,


                    Prev_reg_dat = x.Prev_reg_dat,


                    Prev_reg_cuo = x.Prev_reg_cuo,


                    Suplementary_Quantity = x.Suplementary_Quantity,


                    Preveious_suplementary_quantity = x.Preveious_suplementary_quantity,


                    Current_value = x.Current_value,


                    Previous_value = x.Previous_value,


                    Current_item_number = x.Current_item_number,


                    QtyAllocated = x.QtyAllocated,


                    RndCurrent_Value = x.RndCurrent_Value,


                    CNumber = x.CNumber,


                    RegistrationDate = x.RegistrationDate,

                    ReferenceNumber = x.ReferenceNumber,

                    AssessmentDate = x.AssessmentDate

                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
                await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

    }
}