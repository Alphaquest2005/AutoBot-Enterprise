using System;
using Core.Common.Data.Contracts;
using TrackableEntities.Client;

namespace Core.Common.Client.Entities
{
    public abstract class BaseEntity<T> : EntityBase, IIdentifiableEntity, ICreateEntityFromString<T>
        where T : IIdentifiableEntity
    {
        private readonly Guid _entityGuid = Guid.NewGuid();


        // public DateTime EntryDateTime { get; set; }


        public virtual string EntityId
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public virtual string EntityName
        {
            get => EntityId;
            set
            {
                //throw new NotImplementedException();
            }
        }

        public virtual T CreateEntityFromString(string value)
        {
            throw new NotImplementedException();
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