using Core.Common.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SimpleMvvmToolkit;

namespace WaterNut.QuerySpace.CoreEntities.ViewModels
{
    public class MainWindowViewModel : ViewModelBase<MainWindowViewModel>
    {
        

        public MainWindowViewModel()
        {
			this.navAllocationQS = new RelayCommand(OnNavAllocationQS);
				this.navCounterPointQS = new RelayCommand(OnNavCounterPointQS);
				this.navCoreEntities = new RelayCommand(OnNavCoreEntities);
				this.navEntryDataQS = new RelayCommand(OnNavEntryDataQS);
				this.navInventoryQS = new RelayCommand(OnNavInventoryQS);
				this.navPreviousDocumentQS = new RelayCommand(OnNavPreviousDocumentQS);
				this.navSalesDataQS = new RelayCommand(OnNavSalesDataQS);
				this.navAdjustmentQS = new RelayCommand(OnNavAdjustmentQS);
	            
        }

		private RelayCommand navAllocationQS;
        private void OnNavAllocationQS()
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("AllocationQSEXP"); 
        }

		public ICommand NavAllocationQS
        {
            get { return this.navAllocationQS; }
        }

			private RelayCommand navCounterPointQS;
        private void OnNavCounterPointQS()
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("CounterPointQSEXP"); 
        }

		public ICommand NavCounterPointQS
        {
            get { return this.navCounterPointQS; }
        }

			private RelayCommand navCoreEntities;
        private void OnNavCoreEntities()
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("CoreEntitiesEXP"); 
        }

		public ICommand NavCoreEntities
        {
            get { return this.navCoreEntities; }
        }

			private RelayCommand navEntryDataQS;
        private void OnNavEntryDataQS()
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("EntryDataQSEXP"); 
        }

		public ICommand NavEntryDataQS
        {
            get { return this.navEntryDataQS; }
        }

			private RelayCommand navInventoryQS;
        private void OnNavInventoryQS()
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("InventoryQSEXP"); 
        }

		public ICommand NavInventoryQS
        {
            get { return this.navInventoryQS; }
        }

			private RelayCommand navPreviousDocumentQS;
        private void OnNavPreviousDocumentQS()
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("PreviousDocumentQSEXP"); 
        }

		public ICommand NavPreviousDocumentQS
        {
            get { return this.navPreviousDocumentQS; }
        }

			private RelayCommand navSalesDataQS;
        private void OnNavSalesDataQS()
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("SalesDataQSEXP"); 
        }

		public ICommand NavSalesDataQS
        {
            get { return this.navSalesDataQS; }
        }

			private RelayCommand navAdjustmentQS;
        private void OnNavAdjustmentQS()
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("AdjustmentQSEXP"); 
        }

		public ICommand NavAdjustmentQS
        {
            get { return this.navAdjustmentQS; }
        }

	 
        
    }
}

		
