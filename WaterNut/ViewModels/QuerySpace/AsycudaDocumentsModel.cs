using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CoreEntities.Client.Entities;
using CoreEntities.Client.Enums;
using CoreEntities.Client.Repositories;
using SimpleMvvmToolkit;
using CustomsOperations = CoreEntities.Client.Enums.CustomsOperations;

namespace WaterNut.QuerySpace.CoreEntities.ViewModels
{
    public class AsycudaDocumentsModel : AsycudaDocumentViewModel_AutoGen
    {
        private static readonly AsycudaDocumentsModel instance;
        static AsycudaDocumentsModel()
        {
            instance = new AsycudaDocumentsModel
            {
                ViewCurrentAsycudaDocumentSetEx = true,
                ViewCurrentApplicationSettings = true,
                
                EffectiveRegistrationDateFilter = DateTime.MinValue,
                StartEffectiveRegistrationDateFilter = DateTime.MinValue,
                EndEffectiveRegistrationDateFilter = DateTime.MinValue,
                RegistrationDateFilter = DateTime.MinValue,
                StartRegistrationDateFilter = DateTime.MinValue,
                EndRegistrationDateFilter = DateTime.MinValue,
                StartAssessmentDateFilter = DateTime.MinValue,
                EndAssessmentDateFilter = DateTime.MinValue,
                StartExpiryDateFilter = DateTime.MinValue,
                EndExpiryDateFilter = DateTime.MinValue,
                AssessmentDateFilter = DateTime.MinValue,
                ExpiryDateFilter = DateTime.MinValue,
                ImportCompleteFilter = true
            };
        }

        public static AsycudaDocumentsModel Instance
        {
            get { return instance; }
        }
        private AsycudaDocumentsModel()
        {

            RegisterToReceiveMessages<AsycudaDocumentSetEx>(
                MessageToken.CurrentAsycudaDocumentSetExChanged, OnCurrentAsycudaDocumentSetChanged);
            RegisterToReceiveMessages<AsycudaDocumentItem>(MessageToken.CurrentAsycudaDocumentItemChanged,
                OnCurrentAsycudaDocumentItemChanged);
            RegisterToReceiveMessages<ApplicationSettings>(MessageToken.CurrentApplicationSettingsChanged, OnCurrentApplicationSettingsChanged);
            vloader.FilterExpression = $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";
        }

        private void OnCurrentAsycudaDocumentItemChanged(object sender,
            NotificationEventArgs<AsycudaDocumentItem> e)
        {
           
            if (e.Data != null)
            {
                if (e.Data == null || e.Data.AsycudaDocumentId == null)
                {
                    vloader.FilterExpression = null;
                }
                else
                {

                    vloader.FilterExpression = $"ASYCUDA_Id == \"{e.Data.AsycudaDocumentId.ToString()}\"";
                }

				AsycudaDocuments.Refresh();
				NotifyPropertyChanged(x => this.AsycudaDocuments);
			}
                //if (e.Data.AsycudaDocument != null && e.Data.AsycudaDocument.AsycudaDocumentSetEx != null) BaseViewModel.Instance.CurrentAsycudaDocumentSetEx = e.Data.AsycudaDocument.AsycudaDocumentSetEx;
                //(e.Data.AsycudaDocument != null)BaseViewModel.Instance.CurrentAsycudaDocument = e.Data.AsycudaDocument;
            
        }

        private void OnCurrentAsycudaDocumentSetChanged(object sender,
            NotificationEventArgs<AsycudaDocumentSetEx> e)
        {
            if (e.Data != null
                && e.Data.Declarant_Reference_Number != "Search Results")
                //|| e.PropertyName == "CurrentAsycudaDocument"
            {
                //base.OnCurrentAsycudaDocumentSetExChanged(sender, e);
            }
        }



       
        private bool _viewIm4 = true;
        public bool ViewIm4
        {
            get { return _viewIm4; }
            set
            {
                _viewIm4 = value;

                FilterData();
                NotifyPropertyChanged(x => ViewIm4);
            }
        }

        private bool _viewIm7 = true;

        public bool ViewIm7
        {
            get { return _viewIm7; }
            set
            {
                _viewIm7 = value;

                FilterData();
                NotifyPropertyChanged(x => ViewIm7);

            }
        }

        private bool _viewEx9 = true;

        public bool ViewEx9
        {
            get { return _viewEx9; }
            set
            {
                _viewEx9 = value;
                FilterData();
                NotifyPropertyChanged(x => ViewEx9);
            }
        }

        private bool _viewIm9 = true;
        public bool ViewIm9
        {
            get { return _viewIm9; }
            set
            {
                _viewIm9 = value;
                FilterData();
                NotifyPropertyChanged(x => ViewIm9);
            }
        }


        public override void FilterData()
        {

            var res = GetAutoPropertyFilterString();
            res.Append($" && ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}");
            
            var viewres = FilterViewStr();
            if (viewres.Length > 0) res.Append(" && (" + viewres + ")");

            FilterData(res);
        }

        private StringBuilder FilterViewStr()
        {
            var viewres = new StringBuilder();
            
            if (ViewIm7 == true)
            {
                viewres.Append($"|| CustomsOperationId == {(int)CustomsOperations.Warehouse}");
            }
            if (ViewEx9 == true)
            {
                viewres.Append($"|| CustomsOperationId == {(int)CustomsOperations.Exwarehouse}");
            }
            if (ViewIm9 == true)
            {
                viewres.Append($"|| CustomsOperationId == {(int)CustomsOperations.Import}");
            }
            if (ViewIm4 == true)
            {
                viewres.Append($"|| CustomsOperationId == {(int)CustomsOperations.Import}");
            }
            if (viewres.Length > 0)
            {
                viewres = new StringBuilder(viewres.ToString().Trim().Substring(2).Trim());
            }
            return viewres;
        }

        internal async Task ExportDocument(string fileName)
        {
            await AsycudaDocumentSetsModel.Instance.ExportDocuments().ConfigureAwait(false);
        }

        internal async Task ImportDocuments()
        {
            await AsycudaDocumentSetsModel.Instance.ImportDocuments().ConfigureAwait(false);
        }

        internal async Task SaveDocument(AsycudaDocument asycudaDocument)
        {
            await AsycudaDocumentSetsModel.Instance.SaveDocument(asycudaDocument).ConfigureAwait(false);
        }

        internal async Task IM72Ex9Document(string f)
        {
            await AsycudaDocumentRepository.Instance.IM72Ex9Document(f).ConfigureAwait(false);
        }
    }
}