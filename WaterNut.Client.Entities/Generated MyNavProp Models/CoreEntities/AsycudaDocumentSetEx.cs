﻿// <autogenerated>
//   This file was generated by T4 code generator AllClientModels.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

using CoreEntities;
using CoreEntities.Client.Entities;
//using WaterNut.Client.Services;
using CoreEntities.Client.Services;
using System.Linq;

namespace CoreEntities.Client.Entities
{
    public partial class AsycudaDocumentSetEx
    {
        
            partial void MyNavPropStartUp()
            {

              PropertyChanged += UpdateMyNavProp;

            }


      
       #region MyNavProp Entities
      
      

        void UpdateMyNavProp(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
           if (e.PropertyName == "Customs_ProcedureId")
            {
                UpdateCustoms_Procedure();
            }
           if (e.PropertyName == "Document_TypeId")
            {
                UpdateDocument_Type();
            }
        }

        private void UpdateCustoms_Procedure()
        {
            using (var ctx = new Customs_ProcedureClient())
            {
                var dto = ctx.GetCustoms_Procedure().Result.FirstOrDefault(x => x.Customs_ProcedureId == this.Customs_ProcedureId);
                if(dto != null)Customs_Procedure = new Customs_Procedure(dto);
            }
        }        

        Customs_Procedure _customs_Procedure = null;

        public Customs_Procedure Customs_Procedure
        {
            get
            {
                if(_customs_Procedure != null) return _customs_Procedure;
                UpdateCustoms_Procedure();
                return _customs_Procedure;
            }
            set
            {
                if (value != null)
                {
                    _customs_Procedure = value;

                    Customs_ProcedureId = _customs_Procedure.Customs_ProcedureId;

                    NotifyPropertyChanged("Customs_Procedure");
                }
            }

        }
 
        private void UpdateDocument_Type()
        {
            using (var ctx = new Document_TypeClient())
            {
                var dto = ctx.GetDocument_Type().Result.FirstOrDefault(x => x.Document_TypeId == this.Document_TypeId);
                if(dto != null)Document_Type = new Document_Type(dto);
            }
        }        

        Document_Type _document_Type = null;

        public Document_Type Document_Type
        {
            get
            {
                if(_document_Type != null) return _document_Type;
                UpdateDocument_Type();
                return _document_Type;
            }
            set
            {
                if (value != null)
                {
                    _document_Type = value;

                    Document_TypeId = _document_Type.Document_TypeId;

                    NotifyPropertyChanged("Document_Type");
                }
            }

        }
        

         #endregion
 
    }
   
}
		