//using System.ComponentModel.DataAnnotations.Schema;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Core.Common.Data.Contracts;
using TrackableEntities.Client;
//using Newtonsoft.Json;

namespace Core.Common.Client.DTO
{
    // [JsonObject(IsReference = true)]
    [DataContract(IsReference = true)]
    public abstract class BaseEntity<T> : EntityBase, IIdentifiableEntity, ICreateEntityFromString<T>
        where T : IIdentifiableEntity
    {
        private readonly Guid _entityGuid = Guid.NewGuid();


        //[NotMapped]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //[IgnoreDataMember]
        [DataMember] public virtual string EntityId { get; set; }

        //[NotMapped]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //[IgnoreDataMember]
        [DataMember]
        public virtual string EntityName
        {
            get => EntityId;
            set
            {
                // throw new NotImplementedException();
            }
        }

        public virtual void UpdateEntityName(object sender, PropertyChangedEventArgs e)
        {
        }

        public virtual T CreateEntityFromString(string value)
        {
            throw new NotImplementedException();
        }


        //event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add { _propertyChanged += value; } remove { _propertyChanged -= value; } }
        private event PropertyChangedEventHandler _propertyChanged;

        public new void NotifyPropertyChanged(string PropertyName)
        {
            _propertyChanged(null, new PropertyChangedEventArgs(PropertyName));
        }

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

        public override int GetHashCode()
        {
            // ReSharper disable once cuz of nhibernate
            return _entityGuid.ToString().GetHashCode();
        }
    }
}