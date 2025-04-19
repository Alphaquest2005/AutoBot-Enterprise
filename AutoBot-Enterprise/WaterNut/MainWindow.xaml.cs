
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using AutoBot;
using CoreEntities.Client.Entities;
using Core.Common.UI;
using Microsoft.Win32;
using WaterNut.QuerySpace.AllocationQS.ViewModels;
using WaterNut.QuerySpace.InventoryQS.ViewModels;
using WaterNut.QuerySpace.CoreEntities.ViewModels;
using BaseViewModel = WaterNut.QuerySpace.CoreEntities.ViewModels.BaseViewModel;


namespace WaterNut
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();

            if (BaseViewModel.Instance.CurrentApplicationSettings.AllowCounterPoint == "Hidden")
            {
                downloadCounterPountTxt = null;
                CPSalesTxt = null;
            }
            else
            {
                downloadCounterPountTxt.Visibility = (Visibility)Enum.Parse(Visibility.GetType(), BaseViewModel.Instance.CurrentApplicationSettings.AllowCounterPoint);
                CPSalesTxt.Visibility = (Visibility)Enum.Parse(Visibility.GetType(), BaseViewModel.Instance.CurrentApplicationSettings.AllowCounterPoint);
            }
            

            homeExpand.Visibility = (Visibility)Enum.Parse(Visibility.GetType(), BaseViewModel.Instance.CurrentApplicationSettings.AllowWareHouse);
            Ex9.Visibility = (Visibility)Enum.Parse(Visibility.GetType(), BaseViewModel.Instance.CurrentApplicationSettings.AllowXBond);
            AsycudaDoc.Visibility = (Visibility)Enum.Parse(Visibility.GetType(), BaseViewModel.Instance.CurrentApplicationSettings.AllowAsycudaManager);
            TariffCodes.Visibility = (Visibility)Enum.Parse(Visibility.GetType(), BaseViewModel.Instance.CurrentApplicationSettings.AllowTariffCodes);
            viewContainersTxt.Visibility = (Visibility)Enum.Parse(Visibility.GetType(), BaseViewModel.Instance.CurrentApplicationSettings.AllowContainers);
        }

        void BaseViewModel_staticPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();

        }

        private void SwitchWindowState(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
              WindowState = WindowState.Normal;
              return;
            }
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;

            }


        }
        public static ApplicationSettings _currentApplicationSettings;
        public ApplicationSettings CurrentApplicationSettings
        {
            get
            {
                return BaseViewModel.Instance.CurrentApplicationSettings;
            }
           
        }

        private void MinimizeWindow(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseWindow(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
        bool collapseHome = false;
        private void Expander_Expanded_1(object sender, RoutedEventArgs e)
        {
            FrameworkElement p = FooterBar;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(p); i++)
            {
                var child = VisualTreeHelper.GetChild(p, i);
                if (typeof(Expander).IsInstanceOfType(child) && child != sender)
                {
                    if (child == homeExpand && collapseHome == false)
                    {
                        collapseHome = true;
                    }

                    (child as Expander).IsExpanded = false;

                }

            }
            if (((Expander)sender).Name == "homeExpand")
            {
                collapseHome = false;
            }
            else
            {
                collapseHome = true;
                homeExpand.IsExpanded = false;
            }


        }
        private void HomeExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            if (collapseHome == false)
            {
                ((Expander)sender).UpdateLayout();
                ((Expander)sender).IsExpanded = true;
                // collapseHome = true;
            }

        }

        private void AsycudaDocBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("AsycudaDocumentSummaryExP");
        }

        private void TariffCodesBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("TariffInventoryItemsExP");
        }

        private void HomeBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("AsycudaDocumentSummaryIntroExP");
        }

        private void CreateDocument(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("CreateAsycudaDocumentExP");
        }

        private void DownloadCPO(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("CPPurchaseOrdersExP");
        }

        private void ViewNullTariffCode(object sender, MouseButtonEventArgs e)
        {

                InventoryItemsModel.Instance.ViewPOItems = false;
                InventoryItemsModel.Instance.ViewUnknownTariff = false;
                InventoryItemsModel.Instance.ViewDocSetItems = true;
                InventoryItemsModel.Instance.ViewNullTariff = true;
            

            Core.Common.UI.BaseViewModel.Slider.MoveTo("TariffInventoryItemsExP");
        }
        
        private void ViewUnknowTariff(object sender, MouseButtonEventArgs e)
        {
          
                InventoryItemsModel.Instance.ViewPOItems = false;
                InventoryItemsModel.Instance.ViewUnknownTariff = true;
                InventoryItemsModel.Instance.ViewNullTariff = false;
         
            Core.Common.UI.BaseViewModel.Slider.MoveTo("TariffInventoryItemsExP");
        }

        private void ViewTarifCategory(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("TariffCodesExP");
        }

        private void ReviewDocuments(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("AsycudaDocumentSummaryExP");
        }

        private async void ExportDocuments(object sender, MouseButtonEventArgs e)
        {
            await AsycudaDocumentSetsModel.Instance.ExportDocSet(
                BaseViewModel.Instance.CurrentAsycudaDocumentSetEx).ConfigureAwait(false);
          
        }

        private void ViewEntryData(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("EntryDataExP");
        }

        private void ViewEx9(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("Ex9IntroExP");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            App.splash.Close(TimeSpan.FromSeconds(1));
            Core.Common.UI.BaseViewModel.Slider.MoveTo("AsycudaDocumentSummaryIntroExP");
        }

        private void DownloadCPSales(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("CPSalesExP");
        }

        private async void AllocateSales(object sender, MouseButtonEventArgs e)
        {
            await AllocationsModel.Instance.AllocateSales(false, false).ConfigureAwait(false);
        }

        private void ViewSalesData(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("SalesDataExP");
        }

        private void ViewAllocations(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("AllocationsExP");
        }

        private void ViewPreviousEntries(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("PreviousEntriesExP");
        }

        private async void CreatOPSEntries(object sender, MouseButtonEventArgs e)
        {
            await AllocationsModel.Instance.CreateOPS().ConfigureAwait(false);
        }

        private async void CreateErrOPS(object sender, MouseButtonEventArgs e)
        {
            await AllocationsModel.Instance.CreateErrorOPS().ConfigureAwait(false);
        }

        private async void CreateEx9(object sender, MouseButtonEventArgs e)
        {
            await AllocationsModel.Instance.CreateEx9().ConfigureAwait(false);
        }

        private void CreateDocument2(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("CreateAsycudaDocument2ExP");
        }

        private void ViewLicenceSummary(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("LicenceSummaryExP");
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
                //OnStaticPropertyChanged(info);
            }
        }

        private async void ClearAllocations(object sender, MouseButtonEventArgs e)
        {
            await AllocationsModel.Instance.ClearAllocations(true).ConfigureAwait(false);
        }

        private void ViewDocuments(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.BringIntoView("AsycudaDocumentsExP");
        }

        private void ViewDocumentDetails(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.BringIntoView("AsycudaEntrySummaryListExP");
        }

        private void GotoTariffCategories(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.BringIntoView("TariffCategoryExP");
        }

        private void GotoInventory(object sender, MouseButtonEventArgs e)
        {
            if (((sender as FrameworkElement).DataContext) is InventoryItemsModel im)
            {
                im.ViewPOItems = false;
                im.ViewUnknownTariff = false;
                im.ViewDocSetItems = false;
                im.ViewNullTariff = false;
            }
            Core.Common.UI.BaseViewModel.Slider.MoveTo("TariffInventoryItemsExP");
        }

        private void GotoTariffCodes(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.BringIntoView("TariffCodesExP");
        }

        private async void IM72Ex9(object sender, MouseButtonEventArgs e)
        {
            var od = new OpenFileDialog
            {
                DefaultExt = ".xml",
                Filter = "Xml Documents (.xml)|*.xml",
                Multiselect = true
            };
            var result = od.ShowDialog();
            if (result == true)
            {
                StatusModel.StartStatusUpdate("Converting Files files", od.FileNames.Count());
                foreach (var f in od.FileNames)
                {
                    StatusModel.StatusUpdate();
                    try
                    {
                        await AsycudaDocumentsModel.Instance.IM72Ex9Document(f).ConfigureAwait(false);
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show($"Could not Convert file - '{f}. Error:{Ex.Message} Stacktrace:{Ex.StackTrace}");
                    }

                }
                MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private async void ClearSomeAllocations(object sender, MouseButtonEventArgs e)
        {
            await AllocationsModel.Instance.ClearAllocations(false).ConfigureAwait(false);
        }


        private void GotoOSAllocations(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.BringIntoView("OverShortDetailAllocationsExP");
        }

        private void GotoOSDetails(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.BringIntoView("OverShortDetailsExP");
        }

        private void GotoOversShort(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("OversShortExP");
        }

        private void OversShortBtn_OnMouseLeftButtonDownBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("OversShortExP");
        }

        private void ViewContainers(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("ContainersExP");
        }

        private async void CleanBond(object sender, MouseButtonEventArgs e)
        {
            await AsycudaDocumentSetsModel.Instance.CleanBond(PerIM7Chk.IsChecked.GetValueOrDefault()).ConfigureAwait(false);
        }

        private async void CleanSelectedBond(object sender, MouseButtonEventArgs e)
        {
            await AsycudaDocumentSetsModel.Instance.CleanSelectedBond(PerIM7Chk.IsChecked.GetValueOrDefault()).ConfigureAwait(false);
        }

        private async void CleanSelectedLines(object sender, MouseButtonEventArgs e)
        {
            await AsycudaDocumentSetsModel.Instance.CleanSelectedLines(PerIM7Chk.IsChecked.GetValueOrDefault()).ConfigureAwait(false);
        }

        private async void EX9AllSales(object sender, MouseButtonEventArgs e)
        {
            await AllocationsModel.Instance.EX9AllSales(true).ConfigureAwait(false);
        }

        private void ImportExpiredEntries(object sender, MouseButtonEventArgs e)
        {
            var od = new OpenFileDialog
            {
                DefaultExt = ".csv",
                Filter = "CSV Documents (.csv)|*.csv"
            };

            var result = od.ShowDialog();
            if (result == true)
            {
                StatusModel.StartStatusUpdate("Importing Expired Files files", od.FileNames.Count());
                foreach (var f in od.FileNames)
                {
                    StatusModel.StatusUpdate();
                    try
                    {
                        EntryDocSetUtils.ImportExpiredEntires(f);
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show($"Could not Import file - '{f}. Error:{Ex.Message} Stacktrace:{Ex.StackTrace}");
                    }

                }
                MessageBox.Show("Complete", "Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            
        }


        private void ImportXSales(object sender, MouseButtonEventArgs e)
        {
            var od = new OpenFileDialog
            {
                DefaultExt = ".csv",
                Filter = "CSV Documents (.csv)|*.csv",
                Multiselect = true

            };

            var result = od.ShowDialog();
            if (result == true)
            {
                StatusModel.StartStatusUpdate("Importing XSales files", od.FileNames.Count());
                foreach (var f in od.FileNames)
                {
                    StatusModel.StatusUpdate();
                    try
                    {
                        EX9Utils.ImportXSalesFiles(f);
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show($"Could not Import file - '{f}. Error:{Ex.Message} Stacktrace:{Ex.StackTrace}");
                    }

                }
                MessageBox.Show("Complete", "Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }

        private async void EX9AllAllocations(object sender, MouseButtonEventArgs e)
        {
            await AllocationsModel.Instance.EX9AllAllocations(true).ConfigureAwait(false);
        }

     
    }
}
