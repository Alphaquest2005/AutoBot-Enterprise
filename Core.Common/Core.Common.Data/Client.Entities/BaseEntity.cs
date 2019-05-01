using Core.Common;
using Core.Common.Data.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TrackableEntities.Client;


namespace Core.Common.Client.Entities
{
    public abstract class BaseEntity<T> : EntityBase, IIdentifiableEntity, ICreateEntityFromString<T> where T : IIdentifiableEntity
    {
        
        
       // public DateTime EntryDateTime { get; set; }


        public virtual string EntityId
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual string EntityName
        {
            get
            {
                return EntityId.ToString();
            }
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

        private readonly Guid _entityGuid = Guid.NewGuid();

        public override int GetHashCode()
        {
            // ReSharper disable once cuz of nhibernate
            return (_entityGuid.ToString()).GetHashCode();
        }

    }
}
