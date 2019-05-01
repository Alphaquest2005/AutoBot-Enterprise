using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Common.Data.Contracts;
//using Newtonsoft.Json;
using System.Runtime.Serialization;
using TrackableEntities.Client;


namespace Core.Common.Business.Entities
{
   // [JsonObject(IsReference = true)]
    [DataContract(IsReference = true)]
    public abstract class BaseEntity<T> : EntityBase, IIdentifiableEntity where T : IIdentifiableEntity
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

        public override bool Equals(object obj)
        {

            var other = obj as BaseEntity<T>;
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;
            if (EntityId == "0" || other.EntityId == "0") return false;
            return EntityId == other.EntityId;
        }

        public static bool operator ==(BaseEntity<T> a, BaseEntity<T> b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
            return a.Equals(b);
        }

        public static bool operator !=(BaseEntity<T> a, BaseEntity<T> b)
        {
            return !(a == b);
        }

        private readonly Guid _entityGuid = Guid.NewGuid();

        public override int GetHashCode()
        {
            // ReSharper disable once cuz of nhibernate
            return (_entityGuid.ToString()).GetHashCode();
        }
    }
}
