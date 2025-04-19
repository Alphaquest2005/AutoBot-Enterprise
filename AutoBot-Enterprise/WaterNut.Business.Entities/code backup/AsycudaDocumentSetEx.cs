


//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity;
//using System.Data.Entity.Core.Objects;
//using System.Data.Entity.Core.Objects.DataClasses;
//using System.Linq;
//using System.Runtime.Serialization;
//using Newtonsoft.Json;
//using TrackableEntities;
//using WaterNut.Business.Entities;
//using Core.Common.Data;
//using System.Data.Entity.Core.Metadata.Edm;


//namespace CoreEntities.Business.Entities
//{
    
//    public partial class AsycudaDocumentSetExDTO :IEntityWithRelationships
//    {
//        //private static ObjectQuery<AsycudaDocumentDTO> t = null;
//        //[IgnoreDataMember]
//        //public static IQueryable<AsycudaDocumentDTO> IAsycudaDocuments
//        //{
            
//        //    get { return t; }
//        //    set { var t = value; }
//        //}
//        private IQueryable<AsycudaDocumentDTO> iqAsycudaDocumentDTO = null; 
//        //[NotMapped]
//        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
//        //[IgnoreDataMember]
//        [DataMember]
//        public virtual IQueryable<AsycudaDocumentDTO> IAsycudaDocuments 
//        {
//            get
//            {
//                var relatedEnd = RelationshipManager.GetRelatedEnd("CoreEntities.Business.Entities.AsycudaDocumentDTO_AsycudaDocumentSetEx", "AsycudaDocumentDTO_AsycudaDocumentSetEx_Source");
//                return ((dynamic)relatedEnd).CreateSourceQuery();
//            }
//            set { iqAsycudaDocumentDTO = value; }
//        }

//        private RelationshipManager rm = null;
//        public RelationshipManager RelationshipManager
//        {
//            get
//            {
//                if (rm == null)
//                {
//                    rm = RelationshipManager.Create(this);
//                }
//                return rm;
//            }
//        }
//    }
//}


