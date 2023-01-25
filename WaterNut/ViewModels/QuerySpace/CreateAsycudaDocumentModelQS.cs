using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Core.Common;
using CoreEntities.Client.Entities;
using CoreEntities.Client.Repositories;
using SimpleMvvmToolkit;
using Document_Type = CoreEntities.Client.Entities.Document_Type;


namespace WaterNut.QuerySpace.CoreEntities.ViewModels
{
	public class CreateAsycudaDocumentModel : ViewModelBase<CreateAsycudaDocumentModel>, IAsyncInitialization
	{
	   private static readonly CreateAsycudaDocumentModel instance; 
	   public Task Initialization { get; private set; }
	   static CreateAsycudaDocumentModel()
	   {
		   instance = new CreateAsycudaDocumentModel();
	   }

	   public static CreateAsycudaDocumentModel Instance
		{
			get { return instance; }
		}
		private CreateAsycudaDocumentModel()
		{
			
			RegisterToReceiveMessages<AsycudaDocumentSetEx>(MessageToken.CurrentAsycudaDocumentSetExChanged, OnCurrentAsycudaDocumentSetChanged);
			RegisterToReceiveMessages<AsycudaDocument>(MessageToken.CurrentAsycudaDocumentChanged, OnCurrentAsycudaDocumentChanged);
		   // LoadAsycudaDocumentSet();
			Initialization = InitializationAsync();

		}

		private async Task InitializationAsync()
		{
			await LoadCustomsProcedures().ConfigureAwait(false);
			await LoadDocumentTypes().ConfigureAwait(false);
		}

		private async Task LoadCustomsProcedures()
		{
			using (var ctx = new Customs_ProcedureRepository())
			{
				_customs_Procedures = new ObservableCollection<Customs_Procedure>(await ctx.GetCustoms_ProcedureByExpression($"BondTypeId == {BaseViewModel.Instance.CurrentApplicationSettings.BondTypeId} && IsObsolete != true",new List<string>() { "Document_Type" }).ConfigureAwait(false));
				NotifyPropertyChanged(x => this.Customs_Procedures);
			}
		}

		private async Task LoadDocumentTypes()
		{
			using (var ctx = new Document_TypeRepository())
			{
				_document_types = new ObservableCollection<Document_Type>(await ctx.Document_Type().ConfigureAwait(false));
				NotifyPropertyChanged(x => this.Document_Types);
			}
		}

		private new void OnCurrentAsycudaDocumentSetChanged(object sender, NotificationEventArgs<AsycudaDocumentSetEx> e)
		{

			Task.Run(() => { LoadDataFromAdoc(e.Data, QuerySpace.CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocument).Wait(); });
		}

		private new void OnCurrentAsycudaDocumentChanged(object sender, NotificationEventArgs<AsycudaDocument> e)
		{

			Task.Run(() => { LoadDataFromAdoc(QuerySpace.CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx, e.Data).Wait(); });
		}

		private async Task LoadDataFromAdoc(AsycudaDocumentSetEx docSet, AsycudaDocument doc)
		{
			if (docSet == null && ((doc != null) && (docSet != doc.AsycudaDocumentSetEx)))
			{
				docSet = doc.AsycudaDocumentSetEx;
			}

			if (docSet != null && doc == null )//(docSet != null && (doc == null || (doc != null && doc.AutoUpdate == true)))
			{
				Document_TypeId = Convert.ToInt32(docSet.Document_TypeId);
				Description = docSet.Description;
				Customs_ProcedureId = Convert.ToInt32(docSet.Customs_ProcedureId);
				Country_of_origin_code = docSet.Country_of_origin_code;
				Currency_Code = docSet.Currency_Code;
				Exchange_Rate = Convert.ToSingle(docSet.Exchange_Rate);
				Declarant_Reference_Number = docSet.Declarant_Reference_Number;
				Manifest_Number = docSet.Manifest_Number;
				BLNumber = docSet.BLNumber;
				TotalWeight = docSet.TotalWeight.GetValueOrDefault();
				TotalFreight = docSet.TotalFreight.GetValueOrDefault();
			    ApportionMethod = docSet.ApportionMethod;

			}
			else if (doc != null)//else if (doc != null && doc.AutoUpdate == false)
			{
				Document_TypeId = Convert.ToInt32(doc.Document_TypeId);
				Description = doc.Description;
				Customs_ProcedureId = Convert.ToInt32(doc.Customs_ProcedureId);

			  //  if (doc.Country_first_destination != null)
					Country_of_origin_code = doc.Country_first_destination;


				Currency_Code = doc.Currency_code;
				Exchange_Rate = Convert.ToSingle(doc.Currency_rate);


				Declarant_Reference_Number = doc.ReferenceNumber; //docSet.Declarant_Reference_Number;

				//if (doc.Manifest_reference_number != null)
					Manifest_Number = doc.Manifest_reference_number; // docSet.Manifest_Number;

				BLNumber = doc.BLNumber;// docSet.BLNumber;
				TotalWeight = doc.TotalGrossWeight.GetValueOrDefault();
				TotalFreight = doc.TotalFreight.GetValueOrDefault();
			}
			else
			{
				Description = "";
				Customs_ProcedureId = 0;
				Country_of_origin_code = null;
				Currency_Code = "";
				Exchange_Rate = 0;
				Declarant_Reference_Number = "";
				Manifest_Number = "";
				BLNumber = "";
				
			    ApportionMethod = "Equal";
			}

			//if (CurrentAsycudaDocument != null)
			//{
			//    IsManuallyAssessed = CurrentAsycudaDocument.IsManuallyAssessed;
			//    NotifyPropertyChanged(x => this.IsManuallyAssessed");
			//}

			NotifyPropertyChanged(x => Document_TypeId);
			NotifyPropertyChanged(x => Description);
			NotifyPropertyChanged(x => Customs_ProcedureId);
			NotifyPropertyChanged(x => Declarant_Reference_Number);
			NotifyPropertyChanged(x => Currency_Code);
			NotifyPropertyChanged(x => Country_of_origin_code);
			NotifyPropertyChanged(x => Manifest_Number);
			NotifyPropertyChanged(x => BLNumber);
			NotifyPropertyChanged(x => Exchange_Rate);
			NotifyPropertyChanged(x => TotalFreight);
			NotifyPropertyChanged(x => TotalWeight);
		    NotifyPropertyChanged(x => ApportionMethod);

        }

		private ObservableCollection<Document_Type> _document_types = new ObservableCollection<Document_Type>();
		public ObservableCollection<Document_Type> Document_Types
		{
			get { return _document_types; }
		}

		private ObservableCollection<Customs_Procedure> _customs_Procedures = new ObservableCollection<Customs_Procedure>();
		public ObservableCollection<Customs_Procedure> Customs_Procedures
		{
			get { return _customs_Procedures; }
		}

		public  string _Description;
		public string Description
		{
			get
			{ return _Description; }
			set
			{   _Description = value;
			   
			   
			}
		}

		public  string _ManifestNumber;
		public string Manifest_Number
		{
			get
			{ return _ManifestNumber; }
			set
			{
				_ManifestNumber = value;


			}
		}

		public  string _BlNumber;
		public string BLNumber
		{
			get
			{ return _BlNumber; }
			set
			{
				_BlNumber = value;


			}
		}

	    public string _apportionMethod = "Equal";
	    public string ApportionMethod
        {
	        get
	        { return _apportionMethod; }
	        set
	        {
                _apportionMethod = value;


	        }
	    }

        public double _TotalWeight;
		public double TotalWeight
		{
			get
			{ return _TotalWeight; }
			set
			{
			    _TotalWeight = value;
				NotifyPropertyChanged(x => TotalWeight);
			}
		}

		public double _TotalFreight;
		public double TotalFreight
		{
			get
			{ return _TotalFreight; }
			set
			{
				_TotalFreight = value;
				NotifyPropertyChanged(x => TotalFreight);
			}
		}

		public  string _DeclarantReferenceNumber;
		public string Declarant_Reference_Number
		{
			get
			{
				return _DeclarantReferenceNumber;
			}
			set
			{
				_DeclarantReferenceNumber = value;
				NotifyPropertyChanged(x => this.Declarant_Reference_Number);
			}
		}
		public  int _Document_TypeId;
		public int Document_TypeId
		{
			get
			{
				return _Document_TypeId;
			}
			set
			{
				_Document_TypeId = value;
				NotifyPropertyChanged(x => Document_TypeId);
			}
		}
		public  int _Customs_ProcedureId;
		public  int Customs_ProcedureId
		{
			get
			{
				return _Customs_ProcedureId;
			}
			set
			{
				_Customs_ProcedureId = value;
				NotifyPropertyChanged(x => Customs_ProcedureId);
			}
		}
		public  string _Country_of_origin_code;
		public  string Country_of_origin_code
		{
			get
			{
				return _Country_of_origin_code;
			}
			set
			{
				_Country_of_origin_code = value;
				NotifyPropertyChanged(x => Country_of_origin_code);
			}
		}
		public  string _Currency_Code;
		public  string Currency_Code
		{
			get
			{
				return _Currency_Code;
			}
			set
			{
				_Currency_Code = value;
				NotifyPropertyChanged(x => Currency_Code);
			}
		}
		public  float _Exchange_Rate;
		
		public  double Exchange_Rate
		{
			get
			{
				return _Exchange_Rate;
			}
			set
			{
				_Exchange_Rate = (float) value;
				NotifyPropertyChanged(x => Exchange_Rate);
			}
		}

		internal async Task NewDocument()
		{
			if (BaseViewModel.Instance.CurrentAsycudaDocumentSetEx != null)
			{
				var doc =
					await AsycudaDocumentRepository.Instance.NewDocument(BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId).ConfigureAwait(false);

				MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentsChanged, null,
													new NotificationEventArgs(MessageToken.AsycudaDocumentsChanged));

				MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentSetExsChanged, null,
														new NotificationEventArgs(MessageToken.AsycudaDocumentSetExsChanged));

				BaseViewModel.Instance.CurrentAsycudaDocument = doc;
			}
			else
			{
				MessageBox.Show("Please Select DocumentSet");
			}
		}

		internal async Task SaveDocumentSetEx()
		{
			if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocument != null)
			{
				var doc = CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocument;

				doc.BLNumber = this.BLNumber;
				doc.Country_first_destination = this.Country_of_origin_code;
				doc.Currency_code = this.Currency_Code;
				doc.Description = this.Description;
				doc.Currency_rate = this.Exchange_Rate;
				doc.Document_TypeId = this.Document_TypeId;
				doc.Customs_ProcedureId = this.Customs_ProcedureId;
				doc.Manifest_reference_number = this.Manifest_Number;
				doc.ReferenceNumber = this.Declarant_Reference_Number;

				await AsycudaDocumentRepository.Instance.SaveDocumentCT(doc).ConfigureAwait(false);
			}
			if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx != null)
			{
			   var docSet = await AsycudaDocumentSetExRepository.Instance.GetAsycudaDocumentSetEx(CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId.ToString(), new List<string>()
				{
					"AsycudaDocuments"
				}).ConfigureAwait(false);
               if (docSet == null)
               {
                   MessageBox.Show("Please Select DocumentSet");
                   return;
               }
                //var docSet = CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx;
                docSet.BLNumber = this.BLNumber;
				docSet.Country_of_origin_code = this.Country_of_origin_code;
				docSet.Currency_Code = this.Currency_Code;
				docSet.Description = this.Description;
				docSet.Exchange_Rate = this.Exchange_Rate;
				docSet.Document_TypeId = this.Document_TypeId;
				docSet.Customs_ProcedureId = this.Customs_ProcedureId;
				docSet.Manifest_Number = this.Manifest_Number;
				docSet.Declarant_Reference_Number = this.Declarant_Reference_Number;
				docSet.TotalFreight = this.TotalFreight;
			    docSet.TotalWeight = this.TotalWeight;
			    docSet.ApportionMethod = this.ApportionMethod;

				await  AsycudaDocumentSetExRepository.Instance.SaveAsycudaDocumentSetEx(docSet).ConfigureAwait(false);

				foreach (var doc in docSet.AsycudaDocuments.Where(x => x.AutoUpdate == true))
				{
				   
					doc.BLNumber = docSet.BLNumber;
					doc.Country_first_destination = docSet.Country_of_origin_code;
					doc.Currency_code = docSet.Currency_Code;
					doc.Description = docSet.Description;
					doc.Currency_rate = docSet.Exchange_Rate;
					doc.Document_TypeId = docSet.Document_TypeId;
					doc.Customs_ProcedureId = docSet.Customs_ProcedureId;
					doc.Manifest_reference_number = docSet.Manifest_Number;
					doc.ReferenceNumber = docSet.Declarant_Reference_Number;

					await AsycudaDocumentRepository.Instance.SaveDocumentCT(doc).ConfigureAwait(false);
				}
			}
			else
			{
				MessageBox.Show("Please Create or Select a DocumentSet before Saving");
				return;
			}

			MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentsChanged, null,
													 new NotificationEventArgs(MessageToken.AsycudaDocumentsChanged));

			MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentSetExsChanged, null,
													new NotificationEventArgs(MessageToken.AsycudaDocumentSetExsChanged));

		    MessageBox.Show("Complete", "Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);

        }

		internal async Task DeleteDocument()
		{
		   var doc = BaseViewModel.Instance.CurrentAsycudaDocument;
			var docSet = BaseViewModel.Instance.CurrentAsycudaDocumentSetEx;
			if (doc == null && docSet != null)
			{
				//if (docSet.AsycudaDocuments.Any())
				//{
					var res = MessageBox.Show("Do you want to delete this Document Set?", "Delete?",
						MessageBoxButton.YesNo);
					if (res == MessageBoxResult.Yes)
					{

					await AsycudaDocumentSetExRepository.Instance.DeleteDocumentSet(docSet.AsycudaDocumentSetId).ConfigureAwait(false);



						// SwitchToDB(mydb, BaseDataModel.Instance.CurrentAsycudaDocumentSet);

						BaseViewModel.Instance.CurrentAsycudaDocumentSetEx = null;
						CoreEntities.ViewModels.AsycudaDocumentSetsModel.Instance.ViewAll();

						MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentsChanged, null,
							new NotificationEventArgs(MessageToken.AsycudaDocumentsChanged));

						MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentSetExsChanged, null,
							new NotificationEventArgs(MessageToken.AsycudaDocumentSetExsChanged));


						return;
					}
					else
					{
						return;
					}
				//}
				//else
				//{
				//    MessageBox.Show("Please select an Asycuda Document to delete");
				//    return;
				//}
			}
			else
			{

				await
					AsycudaDocumentRepository.Instance.DeleteDocument(doc.ASYCUDA_Id)
						.ConfigureAwait(false);
			}

			MessageBus.Default.BeginNotify<AsycudaDocumentSetEx>(MessageToken.CurrentAsycudaDocumentSetExChanged, null,
					new NotificationEventArgs<AsycudaDocumentSetEx>(MessageToken.CurrentAsycudaDocumentSetExChanged, null));

			MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentsChanged, null,
					new NotificationEventArgs(MessageToken.AsycudaDocumentsChanged));

			MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentSetExsChanged, null,
					new NotificationEventArgs(MessageToken.AsycudaDocumentSetExsChanged));

			MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentsDeleted, null,
			   new NotificationEventArgs<AsycudaDocument>(MessageToken.AsycudaDocumentsDeleted, doc));
			
			MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
		}

		internal async Task AssessAllinSet()
		{
			throw new NotImplementedException();
		}

		internal async Task NewDocumentSet(int applicationSettingsId)
		{
		   var docSet = await AsycudaDocumentSetExRepository.Instance.NewDocumentSet(applicationSettingsId).ConfigureAwait(false);
		   
		   MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentSetExsChanged, null,
												   new NotificationEventArgs(MessageToken.AsycudaDocumentSetExsChanged));

			BaseViewModel.Instance.CurrentAsycudaDocumentSetEx = docSet;

		}
	}
}