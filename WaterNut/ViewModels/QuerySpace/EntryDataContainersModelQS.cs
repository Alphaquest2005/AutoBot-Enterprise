

using System;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CoreEntities.Client.Entities;
using EntryDataQS.Client.Entities;
using EntryDataQS.Client.Repositories;
using SimpleMvvmToolkit;
using TrackableEntities;

namespace WaterNut.QuerySpace.EntryDataQS.ViewModels
{
    public class EntryDataContainersModel : ContainerExViewModel_AutoGen
	{
         private static readonly EntryDataContainersModel instance;
         public Task Initialization { get; private set; }
         static EntryDataContainersModel()
         {
             instance = new EntryDataContainersModel()
             {
                 ShipDateFilter = DateTime.MinValue,
                 DeliveryDateFilter =  DateTime.MinValue,
                 StartDeliveryDateFilter = DateTime.MinValue,
                 EndDeliveryDateFilter = DateTime.MinValue,
                 StartShipDateFilter = DateTime.MinValue,
                 EndShipDateFilter = DateTime.MinValue
                 
             };//ViewCurrentContainer = true
         }

         public static EntryDataContainersModel Instance
        {
            get { return instance; }
        }

        // private EntryDataContainersModel()
        //{
        //    RegisterToReceiveMessages<AsycudaDocumentSetEx>(CoreEntities.MessageToken.CurrentAsycudaDocumentSetExChanged, OnCurrentAsycudaDocumentSetExChanged);
        //    RegisterToReceiveMessages<EntryDataEx>(MessageToken.CurrentEntryDataExChanged, OnCurrentEntryDataExChanged);
        //    //Initialization = InitializationAsync();
        //    LoadContainerTypes().Wait();
        //    LoadPackageTypes().Wait();
        //     LoadEmptyFullCodes().Wait();

        //}

         private void OnCurrentEntryDataExChanged(object sender, NotificationEventArgs<EntryDataEx> e)
         {
             if (e.Data != null)
             {
                 Task.Run(() => FilterData());
             }
         }

         //private async Task InitializationAsync()
         //{
         //    await LoadContainerTypes().ConfigureAwait(false);
         //    await LoadPackageTypes().ConfigureAwait(false);
         //    await LoadEmptyFullCodes().ConfigureAwait(false);
         //}

         private async Task LoadEmptyFullCodes()
         {
             using (var ctx = new EmptyFullCodeRepository())
             {
                 var res = await ctx.EmptyFullCodes().ConfigureAwait(false);
                 if (res != null)
                 {
                     emptyFullCodes = new ObservableCollection<EmptyFullCode>(res);
                 }
             }
         }
         private async Task LoadPackageTypes()
         {
             using (var ctx = new PackageTypeRepository())
             {
                 var res = await ctx.PackageTypes().ConfigureAwait(false);
                 if (res != null)
                 {
                     packageTypes = new ObservableCollection<PackageType>(res);
                 }
             }
         }

        // private async Task LoadContainerTypes()
        // {
        //     using (var ctx = new ContainerTypeRepository())
        //     {
        //         var res = await ctx.ContainerTypes().ConfigureAwait(false);
        //         if (res != null)
        //         {
        //             containerTypes = new ObservableCollection<ContainerType>(res);
        //         }
        //     }
        // }

        //private void OnCurrentAsycudaDocumentSetExChanged(object sender, NotificationEventArgs<AsycudaDocumentSetEx> e)
        //{
        //    if (e.Data != null)
        //    {
        //        Task.Run(() => FilterData());
        //    }
        //}


        //private ObservableCollection<ContainerType> containerTypes = new ObservableCollection<ContainerType>();
        //public ObservableCollection<ContainerType> ContainerTypes
        //{
        //    get { return containerTypes; }
        //}


        private ObservableCollection<PackageType> packageTypes = new ObservableCollection<PackageType>();
        public ObservableCollection<PackageType> PackageTypes
        {
            get { return packageTypes; }
        }

        private ObservableCollection<EmptyFullCode> emptyFullCodes;

        public ObservableCollection<EmptyFullCode> EmptyFullCodes
        {
            get { return emptyFullCodes; }
        }


        private DateTime startDateFilter;

        public DateTime StartDateFilter
        {
            get { return startDateFilter; }
            set
            {
                startDateFilter = value;
                FilterData();
                NotifyPropertyChanged(x => this.StartDateFilter);
            }
        }

        private DateTime endDateFilter;
       

        public DateTime EndDateFilter
        {
            get { return endDateFilter; }
            set
            {
                endDateFilter = value;
                FilterData();
                NotifyPropertyChanged(x => this.EndDateFilter);
            }
        }
       
         public override void FilterData()
         {
             var res = GetAutoPropertyFilterString();

             if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx != null)
             {
                 res.Append(
                     $"&& AsycudaDocumentSetId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId}");
             }

             if (BaseViewModel.Instance.CurrentEntryDataEx != null)
             {
                 res.Append($"&& EntryDataId == \"{BaseViewModel.Instance.CurrentEntryDataEx.InvoiceNo}\"");
             }

             if (StartDateFilter != DateTime.MinValue )
             {
                 res.Append(string.Format("&& DeliveryDate >= \"{0}\" || ShipDate >= \"{0}\"", StartDateFilter.ToShortDateString()));
             }

             if (EndDateFilter != DateTime.MinValue)
             {
                 res.Append(string.Format("&& DeliveryDate <= \"{0}\" || ShipDate <= \"{0}\"", EndDateFilter.ToShortDateString()));
             }
              
             //if (res.Length == 0 && vloader.NavigationExpression.Count != 0) res.Append("&& All");
             FilterData(res);

         }
        
        
         internal async Task SaveContainer(global::EntryDataQS.Client.Entities.ContainerEx container)
         {
             if (container == null) return;
             if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx != null)
             {
                 container.AsycudaDocumentSetId =
                     CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId;
                 await ContainerExRepository.Instance.SaveContainer(container).ConfigureAwait(false);

                 MessageBus.Default.BeginNotify(MessageToken.ContainerExesChanged, null,
                           new NotificationEventArgs(MessageToken.ContainerExesChanged));


                 MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
             }
             else
             {
                 MessageBox.Show("Please Select Document Set then try again.");
                 
             }

         }

         //internal async Task AssignInvoies()
         //{
         //    if (BaseViewModel.Instance.CurrentContainerEx == null)
         //    {
         //        MessageBox.Show("Please Select a Container to Assign");
         //        return;
         //    }
         //    if (BaseViewModel.Instance.CurrentEntryDataEx != null)
         //    {
         //        var c =
         //            BaseViewModel.Instance.CurrentContainerEx.ContainerEntryDatas.FirstOrDefault(
         //                x => x.EntryDataId == BaseViewModel.Instance.CurrentEntryDataEx.InvoiceNo);
         //        if (c == null)
         //        {
         //            c = new ContainerEntryData()
         //            {
         //                Container_Id = BaseViewModel.Instance.CurrentContainerEx.Container_Id,
         //                EntryDataId = BaseViewModel.Instance.CurrentEntryDataEx.InvoiceNo,
         //                TrackingState = TrackingState.Added
         //            };

         //            await ContainerEntryDataRepository.Instance.UpdateContainerEntryData(c).ConfigureAwait(false);

         //            MessageBus.Default.BeginNotify(MessageToken.ContainerExesChanged, null,
         //                new NotificationEventArgs(MessageToken.ContainerExesChanged));


         //            MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
         //        }
         //    }
         //    else
         //    {
         //        MessageBox.Show("Please Select an Invoice to Assign, then try again.");

         //    }
         //}

         internal async  Task NewContainer(global::EntryDataQS.Client.Entities.ContainerEx containerEx)
         {
             BaseViewModel.Instance.CurrentContainerEx = new ContainerEx(){TrackingState = TrackingState.Added};

         }
    }
}