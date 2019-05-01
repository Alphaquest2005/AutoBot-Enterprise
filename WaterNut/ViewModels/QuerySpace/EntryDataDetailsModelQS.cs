

using System;
using System.Threading.Tasks;
using System.Windows;
using CoreEntities.Client.Entities;
using EntryDataQS.Client.Entities;
using EntryDataQS.Client.Repositories;
using SimpleMvvmToolkit;
using TrackableEntities;

namespace WaterNut.QuerySpace.EntryDataQS.ViewModels
{
    public class EntryDataDetailsModel : EntryDataDetailsExViewModel_AutoGen
	{
         private static readonly EntryDataDetailsModel instance;
         static EntryDataDetailsModel()
         {
             instance = new EntryDataDetailsModel() {ViewCurrentEntryDataEx = true};
         }

         public static EntryDataDetailsModel Instance
        {
            get { return instance; }
        }

        private EntryDataDetailsModel()
        {
            RegisterToReceiveMessages<AsycudaDocumentSetEx>(CoreEntities.MessageToken.CurrentAsycudaDocumentSetExChanged, OnCurrentAsycudaDocumentSetExChanged);

        }

        private void OnCurrentAsycudaDocumentSetExChanged(object sender, NotificationEventArgs<AsycudaDocumentSetEx> e)
        {
            if (e.Data != null)
            {
                Task.Run(() => FilterData());
            }
        }

        bool _zeroPriceItems = false;
        public bool ZeroPriceItems
        {
            get
            {
                return _zeroPriceItems;
            }
            set
            {
                _zeroPriceItems = value;
                
                if (_zeroPriceItems)
                {
                    FilterData();
                    ViewAll = false;
                }
                NotifyPropertyChanged(x => ZeroPriceItems);
                
            }
        }

        bool _viewDocData = false;
        public bool ViewDocData
        {
            get
            {
                return _viewDocData;
            }
            set
            {
                _viewDocData = value;

                if (_viewDocData)
                {

                    FilterData();
                    ViewAll = false;
                }
                NotifyPropertyChanged(x => ViewDocData);
    
            }
        }

        bool _viewAll = false;
        public bool ViewAll
        {
            get
            {
                return _viewAll;
            }
            set
            {
                _viewAll = value;
                if (_viewAll == true)
                {
                    ViewAll();
                    ViewDocData = false;
                    ZeroPriceItems = false;
                    NullItems = false;
                }
                NotifyPropertyChanged(x => ViewAll);
            }
        }

        
        

        bool _nullItems = false;
         public bool NullItems
        {
            get
            {
                return _nullItems;
            }
            set
            {
                _nullItems = value;
                
                if (_nullItems)
                {
                    FilterData();
                    ViewAll = false;
                }
                NotifyPropertyChanged(x => NullItems);
            }
        }

         public override void FilterData()
         {
             var res = GetAutoPropertyFilterString();

              if (ViewDocData == true)
              {
                  if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx != null)
                  {
                     
                      vloader.SetNavigationExpression("AsycudaDocumentSets",
                          $@"AsycudaDocumentSetId == {
                              CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx
                                  .AsycudaDocumentSetId
                          }");
                      //res.Append(string.Format(@" && AsycudaDocumentSetId == {0}", CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId));
                  }
              }
             
             if (ZeroPriceItems == true)
             {
                 res.Append(@" && Cost == 0");
             }
            
             if (NullItems == true)
             {
                 res.Append(@" && TariffCode == null");
             }
             if (res.Length == 0 && vloader.NavigationExpression.Count != 0) res.Append("&& All");
             FilterData(res);

         }
        
         internal async Task AddToEntry(System.Collections.Generic.List<global::EntryDataQS.Client.Entities.EntryDataDetailsEx> list)
         {
             throw new NotImplementedException();
         }

         internal async Task SaveEntryDataDetailsEx(EntryDataDetailsEx entryDataDetailsEx)
         {
             await
                 EntryDataDetailsExRepository.Instance.SaveEntryDataDetailsEX(entryDataDetailsEx).ConfigureAwait(false);
             MessageBus.Default.BeginNotify(MessageToken.EntryDataDetailsExesChanged, null,
                          new NotificationEventArgs(MessageToken.EntryDataDetailsExesChanged));


             MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
         }

         internal async Task NewEntryDataDetailEx()
         {
             BaseViewModel.Instance.CurrentEntryDataDetailsEx = new EntryDataDetailsEx() { TrackingState = TrackingState.Added };
         }
    }
}