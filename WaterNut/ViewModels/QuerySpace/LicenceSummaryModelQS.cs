using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Core.Common.Converters;
using CoreEntities.Business.Entities;
using CoreEntities.Client.Entities;
using CoreEntities.Client.Repositories;
using SimpleMvvmToolkit;
using WaterNut.Converters;

namespace WaterNut.QuerySpace.CoreEntities.ViewModels
{
	public class LicenceSummaryModel : ViewModelBase<LicenceSummaryModel>
	{
         private static readonly LicenceSummaryModel instance;
         static LicenceSummaryModel()
        {
            instance = new LicenceSummaryModel();
        }

         public static LicenceSummaryModel Instance
        {
            get { return instance; }
        }
		private LicenceSummaryModel()
		{
		    
            RegisterToReceiveMessages<AsycudaDocumentSetEx>(MessageToken.CurrentAsycudaDocumentSetExChanged, OnCurrentAsycudaDocumentSetChanged);
		}

        private async void OnCurrentAsycudaDocumentSetChanged(object sender, NotificationEventArgs<AsycudaDocumentSetEx> e)
        {
            await RefreshData().ConfigureAwait(false);
        }

	    public async Task RefreshData()
	    {
	       // NotifyPropertyChanged(x => LicenceSummaryData);
	        await GetLicenceSummaryLines().ConfigureAwait(false);
	        NotifyPropertyChanged(x => SelectedTotal);
	    }


	    public double SelectedTotal
        {
            get
            {
                if(LicenceSummaryData != null)
                return LicenceSummaryData.Sum(x => x.Total);

                return 0;
            }
        }

         ObservableCollection<LicenceSummaryLine> _licenceSummaryData = new ObservableCollection<LicenceSummaryLine>();
        public ObservableCollection<LicenceSummaryLine> LicenceSummaryData
        {
            get { return _licenceSummaryData; }
        }

        private async Task GetLicenceSummaryLines()
	    {
            try
            {
                if (BaseViewModel.Instance.CurrentAsycudaDocumentSetEx != null)
                {
                    //ObservableCollection<LicenceSummaryLine> licenceSummaryLines = null;
                    using (var ctx = new LicenceSummaryRepository())
                    {
                        var clst =
                            ctx.GetLicenceSummaryByAsycudaDocumentSetId(
                                BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId.ToString())
                                .Result;
                        if (clst.ToList().Any() == false)
                        {
                            // return licenceSummaryLines;
                            //return true;
                            return;
                        }
                        var lst = from t in clst
                            select new LicenceSummaryLine
                            {
                                TariffCode = t.TariffCode,
                                Description = t.TariffCodeDescription,
                                Quantity = Convert.ToDouble(t.Quantity),
                                Total = Convert.ToDouble(t.Total)
                            };
                        if (lst.Any())
                        {
                            _licenceSummaryData = new ObservableCollection<LicenceSummaryLine>(lst);
                            NotifyPropertyChanged(x => this.LicenceSummaryData);
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                throw;
            }


	    }

	    public class LicenceSummaryLine
        {
            public string TariffCode { get; set; }
            public string Description { get; set; }
            public double Quantity { get; set; }
            public double Total { get; set; }

        }

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}
		#endregion

        
        public void Send2Excel()
        {
            var s = new ExportToCSV<LicenceSummaryLine, List<LicenceSummaryLine>>();
            s.dataToPrint = _licenceSummaryData.ToList();
            s.GenerateReport();
        }
    }
}