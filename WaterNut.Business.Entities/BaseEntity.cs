using System.ComponentModel.DataAnnotations.Schema;
using Core.Common.Data.Contracts;
//using Newtonsoft.Json;
using System.Runtime.Serialization;
using Omu.ValueInjecter;
using TrackableEntities.Client;


namespace WaterNut.Business.Entities
{
   // [JsonObject(IsReference = true)]
    [DataContract(IsReference = true)]
    public abstract partial class BaseEntity<T> : EntityBase, IIdentifiableEntity where T : IIdentifiableEntity
    {
        //[NotMapped]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //[IgnoreDataMember]
        [DataMember]
        public virtual string EntityId { get; set; }   
        
        //[NotMapped]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //[IgnoreDataMember]
        [DataMember]
        public virtual string EntityName { get; set; }       

    }
}
