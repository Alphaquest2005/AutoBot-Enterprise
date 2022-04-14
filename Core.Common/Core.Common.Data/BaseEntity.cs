using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Core.Common.Data.Contracts;

namespace Core.Common.Data
{
    public abstract class BaseEntity<T> : IIdentifiableEntity, ICreateEntityFromString<T> where T : IIdentifiableEntity
    {
        public static bool DoStaticEvents = true;


        public virtual string EntityId
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public virtual string EntityName
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public virtual T CreateEntityFromString(string value)
        {
            throw new NotImplementedException();
        }

        public static event PropertyChangedEventHandler staticPropertyChanged;

        public static void OnStaticPropertyChanged(string info)
        {
            if (staticPropertyChanged != null && DoStaticEvents)
                staticPropertyChanged(null, new PropertyChangedEventArgs(info));
        }

        public static void OnStaticPropertyChanged<T>(Expression<Func<T>> selectorExpression)
        {
            if (selectorExpression == null)
                throw new ArgumentNullException(nameof(selectorExpression));
            if (!(selectorExpression.Body is MemberExpression body))
                throw new ArgumentException("The body must be a member expression");
            OnStaticPropertyChanged(body.Member.Name);
        }
    }
}